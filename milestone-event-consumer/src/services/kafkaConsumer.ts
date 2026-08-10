import { Consumer, EachMessagePayload } from 'kafkajs';
import kafkaConfig from '../config/kafka.js';
import eventProcessor from './eventProcessor.js';
import logger from '../config/logger.js';

interface ConsumerStatus {
  isRunning: boolean;
  topic: string;
  groupId: string;
}

/**
 * Kafka consumer service for milestone events
 */
class KafkaConsumerService {
  private consumer: Consumer | null = null;
  private isRunning = false;
  private topic = process.env.KAFKA_TOPIC || 'wellbore-milestone-events';

  /**
   * Initialize and start the Kafka consumer
   */
  async start(): Promise<void> {
    try {
      // Initialize Kafka consumer
      this.consumer = await kafkaConfig.initialize();

      // Connect to Kafka
      await this.consumer.connect();
      logger.info('Kafka consumer connected successfully');

      // Subscribe to topic
      await this.consumer.subscribe({
        topic: this.topic,
        fromBeginning: true // Read from beginning on first run
      });

      logger.info('Subscribed to Kafka topic', { topic: this.topic });

      this.isRunning = true;

      // Start consuming messages
      await this.consumer.run({
        // Process each message
        eachMessage: async (payload: EachMessagePayload) => {
          try {
            logger.debug('Processing message', {
              topic: payload.topic,
              partition: payload.partition,
              offset: payload.message.offset,
              timestamp: payload.message.timestamp
            });

            // Process the event
            const success = await eventProcessor.processEvent(payload);

            if (!success) {
              logger.warn('Event processing failed - offset will not be committed', {
                partition: payload.partition,
                offset: payload.message.offset
              });
              // Throwing an error will prevent offset commit and trigger retry
              throw new Error('Event processing failed');
            }

            // Success - offset will be auto-committed
          } catch (error) {
            const err = error as Error;
            logger.error('Error in message handler', {
              error: err.message,
              partition: payload.partition,
              offset: payload.message.offset
            });
            // Re-throw to prevent offset commit
            throw error;
          }
        },

        // Auto-commit configuration
        autoCommit: true,
        autoCommitInterval: 5000,
        autoCommitThreshold: 100
      });

      logger.info('Kafka consumer is now running and processing messages');
    } catch (error) {
      const err = error as Error;
      logger.error('Failed to start Kafka consumer', {
        error: err.message,
        stack: err.stack
      });
      throw error;
    }
  }

  /**
   * Stop the Kafka consumer gracefully
   */
  async stop(): Promise<void> {
    try {
      if (this.consumer && this.isRunning) {
        logger.info('Stopping Kafka consumer...');
        this.isRunning = false;
        await this.consumer.disconnect();
        logger.info('Kafka consumer stopped successfully');
      }
    } catch (error) {
      const err = error as Error;
      logger.error('Error stopping Kafka consumer', {
        error: err.message,
        stack: err.stack
      });
      throw error;
    }
  }

  /**
   * Get consumer status
   * @returns Status information
   */
  getStatus(): ConsumerStatus {
    return {
      isRunning: this.isRunning,
      topic: this.topic,
      groupId: process.env.KAFKA_GROUP_ID || 'milestone-consumer-group'
    };
  }

  /**
   * Handle consumer errors
   */
  setupErrorHandlers(): void {
    if (!this.consumer) {
      return;
    }

    // Handle consumer errors
    this.consumer.on('consumer.crash', async (event: any) => {
      logger.error('Consumer crashed', {
        error: event.payload.error.message,
        restart: event.payload.restart
      });

      // If restart is false, we need to handle reconnection
      if (!event.payload.restart) {
        logger.info('Attempting to restart consumer...');
        try {
          await this.stop();
          await this.start();
        } catch (error) {
          const err = error as Error;
          logger.error('Failed to restart consumer', {
            error: err.message
          });
        }
      }
    });

    this.consumer.on('consumer.disconnect', () => {
      logger.warn('Consumer disconnected');
    });

    this.consumer.on('consumer.connect', () => {
      logger.info('Consumer connected');
    });

    this.consumer.on('consumer.network.request_timeout', (payload: any) => {
      logger.warn('Consumer request timeout', {
        broker: payload.payload.broker,
        clientId: payload.payload.clientId
      });
    });
  }
}

// Export singleton instance
const kafkaConsumerService = new KafkaConsumerService();
export default kafkaConsumerService;
