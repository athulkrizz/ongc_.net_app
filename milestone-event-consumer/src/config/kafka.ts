import { Kafka, Consumer, logLevel } from 'kafkajs';
import logger from './logger.js';

/**
 * Kafka configuration and client initialization
 */
class KafkaConfig {
  private kafka: Kafka | null = null;
  private consumer: Consumer | null = null;

  /**
   * Initialize Kafka client and consumer
   */
  async initialize(): Promise<Consumer> {
    try {
      const brokers = (process.env.KAFKA_BROKER || 'localhost:9092').split(',');
      const clientId = process.env.KAFKA_CLIENT_ID || 'milestone-consumer';
      const groupId = process.env.KAFKA_GROUP_ID || 'milestone-consumer-group';

      // Create Kafka instance
      this.kafka = new Kafka({
        clientId,
        brokers,
        logLevel: this.getKafkaLogLevel(),
        retry: {
          initialRetryTime: 100,
          retries: 8,
          maxRetryTime: 30000,
          multiplier: 2,
          factor: 0.2
        }
      });

      // Create consumer
      this.consumer = this.kafka.consumer({
        groupId,
        sessionTimeout: 30000,
        heartbeatInterval: 3000,
        maxWaitTimeInMs: 5000,
        retry: {
          initialRetryTime: 100,
          retries: 8
        }
      });

      logger.info('Kafka configuration initialized', {
        brokers,
        clientId,
        groupId
      });

      return this.consumer;
    } catch (error) {
      const err = error as Error;
      logger.error('Failed to initialize Kafka', {
        error: err.message,
        stack: err.stack
      });
      throw error;
    }
  }

  /**
   * Map Winston log level to Kafka log level
   */
  private getKafkaLogLevel(): logLevel {
    const level = process.env.LOG_LEVEL || 'info';
    const mapping: Record<string, logLevel> = {
      error: logLevel.ERROR,
      warn: logLevel.WARN,
      info: logLevel.INFO,
      debug: logLevel.DEBUG
    };
    return mapping[level] || logLevel.INFO;
  }

  /**
   * Get Kafka consumer instance
   */
  getConsumer(): Consumer {
    if (!this.consumer) {
      throw new Error('Kafka consumer not initialized. Call initialize() first.');
    }
    return this.consumer;
  }

  /**
   * Disconnect consumer
   */
  async disconnect(): Promise<void> {
    try {
      if (this.consumer) {
        await this.consumer.disconnect();
        logger.info('Kafka consumer disconnected');
      }
    } catch (error) {
      const err = error as Error;
      logger.error('Error disconnecting Kafka consumer', { error: err.message });
      throw error;
    }
  }
}

// Export singleton instance
const kafkaConfig = new KafkaConfig();
export default kafkaConfig;
