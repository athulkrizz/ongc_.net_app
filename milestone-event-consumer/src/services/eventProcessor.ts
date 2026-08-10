import { EachMessagePayload } from 'kafkajs';
import { validateMilestoneEvent, isCriticalValidationError, MilestoneEvent } from '../validators/eventValidator.js';
import databaseService from './databaseService.js';
import retryHelper from '../utils/retry.js';
import logger from '../config/logger.js';

interface ProcessingStats {
  processedCount: number;
  errorCount: number;
  duplicateCount: number;
  uptimeMs: number;
  uptimeFormatted: string;
}

/**
 * Service to process milestone events from Kafka
 */
class EventProcessor {
  private processedCount = 0;
  private errorCount = 0;
  private duplicateCount = 0;
  private startTime = Date.now();

  /**
   * Process a single milestone event
   * @param payload - Kafka message payload
   * @returns True if processed successfully
   */
  async processEvent(payload: EachMessagePayload): Promise<boolean> {
    let event: MilestoneEvent | null = null;
    let eventId: string | null = null;

    try {
      // Parse JSON message
      const messageValue = payload.message.value?.toString();
      if (!messageValue) {
        throw new Error('Empty message value');
      }

      event = JSON.parse(messageValue);
      eventId = event?.eventId || null;

      logger.info('Received milestone event', {
        eventId,
        designId: event?.data?.designId,
        milestoneType: event?.data?.milestoneType,
        partition: payload.partition,
        offset: payload.message.offset
      });

      // Validate event structure
      const validation = validateMilestoneEvent(event);
      if (validation.error) {
        logger.error('Event validation failed', {
          eventId,
          errors: validation.error.details.map(d => ({
            message: d.message,
            path: d.path.join('.'),
            type: d.type
          }))
        });

        // Log to processing_errors table
        await databaseService.logProcessingError(
          eventId,
          new Error(`Validation failed: ${validation.error.message}`),
          event,
          0
        );

        this.errorCount++;

        // Don't retry critical validation errors
        if (isCriticalValidationError(validation.error)) {
          logger.warn('Critical validation error - skipping event', { eventId });
          return true; // Return true to commit offset
        }

        return false; // Return false to retry
      }

      // Use validated event
      event = validation.value!;

      // Check for duplicate (idempotency)
      const exists = await databaseService.eventExists(eventId!);
      if (exists) {
        logger.info('Duplicate event detected - skipping', {
          eventId,
          designId: event.data.designId
        });
        this.duplicateCount++;
        return true; // Already processed, commit offset
      }

      // Insert into database with retry logic
      await retryHelper.executeWithRetry(
        async () => await databaseService.insertMilestoneEvent(event!),
        {
          maxRetries: 3,
          delayMs: 2000,
          operation: `insert event ${eventId}`
        }
      );

      logger.info('Event processed successfully', {
        eventId,
        designId: event.data.designId,
        milestoneType: event.data.milestoneType
      });

      this.processedCount++;
      return true;

    } catch (error) {
      const err = error as Error;
      logger.error('Failed to process event', {
        eventId,
        error: err.message,
        stack: err.stack,
        partition: payload.partition,
        offset: payload.message.offset
      });

      // Log error to database
      try {
        await databaseService.logProcessingError(
          eventId,
          err,
          event || { raw: payload.message.value?.toString() },
          0
        );
      } catch (dbError) {
        const dbErr = dbError as Error;
        logger.error('Failed to log processing error', {
          eventId,
          dbError: dbErr.message
        });
      }

      this.errorCount++;
      
      // Return false to NOT commit offset (will retry on restart)
      return false;
    }
  }

  /**
   * Get processing statistics
   * @returns Statistics
   */
  getStats(): ProcessingStats {
    const uptime = Date.now() - this.startTime;
    return {
      processedCount: this.processedCount,
      errorCount: this.errorCount,
      duplicateCount: this.duplicateCount,
      uptimeMs: uptime,
      uptimeFormatted: this.formatUptime(uptime)
    };
  }

  /**
   * Format uptime in human-readable format
   * @param ms - Milliseconds
   * @returns Formatted uptime
   */
  private formatUptime(ms: number): string {
    const seconds = Math.floor(ms / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (days > 0) {
      return `${days}d ${hours % 24}h ${minutes % 60}m`;
    } else if (hours > 0) {
      return `${hours}h ${minutes % 60}m`;
    } else if (minutes > 0) {
      return `${minutes}m ${seconds % 60}s`;
    } else {
      return `${seconds}s`;
    }
  }

  /**
   * Reset statistics
   */
  resetStats(): void {
    this.processedCount = 0;
    this.errorCount = 0;
    this.duplicateCount = 0;
    this.startTime = Date.now();
    logger.info('Processing statistics reset');
  }
}

// Export singleton instance
const eventProcessor = new EventProcessor();
export default eventProcessor;
