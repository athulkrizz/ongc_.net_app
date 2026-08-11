import database from '../config/database.js';
import logger from '../config/logger.js';
import { MilestoneEvent } from '../validators/eventValidator.js';

/**
 * Database service for milestone events and error tracking
 */
class DatabaseService {
  /**
   * Insert a milestone event into the database
   */
  async insertMilestoneEvent(event: MilestoneEvent): Promise<any> {
    const query = `
      INSERT INTO milestones (
        event_id,
        asset,
        well,
        wellbore,
        user_email,
        current_milestone,
        approval_level,
        status,
        days,
        percent_completed,
        event_timestamp
      ) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11)
      RETURNING *
    `;

    const values = [
      event.EventId,
      event.Data.Asset,
      event.Data.Well,
      event.Data.Wellbore,
      event.Data.User,
      event.Data.CurrentMilestone,
      event.Data.ApprovalLevel,
      event.Data.Status,
      event.Data.Days,
      event.Data.PercentCompleted,
      new Date(event.Timestamp)
    ];

    try {
      const result = await database.query(query, values);
      logger.info('Milestone event inserted successfully', {
        eventId: event.EventId,
        asset: event.Data.Asset,
        well: event.Data.Well,
        milestone: event.Data.CurrentMilestone,
        dbId: result.rows[0].id
      });
      return result.rows[0];
    } catch (error) {
      logger.error('Failed to insert milestone event', {
        error: error.message,
        eventId: event.EventId,
        code: error.code
      });
      throw error;
    }
  }

  /**
   * Check if an event already exists (for idempotency)
   * @param {string} eventId - The event ID to check
   * @returns {Promise<boolean>} True if event exists
   */
  async eventExists(eventId) {
    const query = 'SELECT COUNT(*) as count FROM milestones WHERE event_id = $1';

    try {
      const result = await database.query(query, [eventId]);
      const exists = parseInt(result.rows[0].count, 10) > 0;

      if (exists) {
        logger.debug('Duplicate event detected', { eventId });
      }

      return exists;
    } catch (error) {
      logger.error('Failed to check event existence', {
        error: error.message,
        eventId
      });
      throw error;
    }
  }

  /**
   * Log a processing error to the database
   * @param {string} eventId - Event ID (if available)
   * @param {Error} error - The error object
   * @param {Object} rawEvent - The raw event data
   * @param {number} retryCount - Number of retries attempted
   * @returns {Promise<Object>} Inserted error record
   */
  async logProcessingError(eventId, error, rawEvent, retryCount = 0) {
    const query = `
      INSERT INTO processing_errors (
        event_id,
        error_message,
        error_stack,
        raw_event,
        retry_count
      ) VALUES ($1, $2, $3, $4, $5)
      RETURNING *
    `;

    const values = [
      eventId || null,
      error.message,
      error.stack || null,
      JSON.stringify(rawEvent),
      retryCount
    ];

    try {
      const result = await database.query(query, values);
      logger.warn('Processing error logged to database', {
        errorId: result.rows[0].id,
        eventId,
        retryCount
      });
      return result.rows[0];
    } catch (dbError) {
      // Even logging the error failed - this is critical
      logger.error('CRITICAL: Failed to log processing error to database', {
        originalError: error.message,
        dbError: dbError.message,
        eventId
      });
      // Don't throw - we don't want to crash the consumer
      return null;
    }
  }

  /**
   * Get statistics about processed events
   * @returns {Promise<Object>} Statistics object
   */
  async getStatistics() {
    try {
      const [totalResult, typeResult, errorResult] = await Promise.all([
        // Total events
        database.query('SELECT COUNT(*) as total FROM milestone_events'),
        // Events by milestone type
        database.query(`
          SELECT milestone_type, COUNT(*) as count 
          FROM milestone_events 
          GROUP BY milestone_type 
          ORDER BY count DESC
        `),
        // Error count
        database.query('SELECT COUNT(*) as total FROM processing_errors WHERE resolved = false')
      ]);

      return {
        totalEvents: parseInt(totalResult.rows[0].total, 10),
        eventsByType: typeResult.rows,
        unresolvedErrors: parseInt(errorResult.rows[0].total, 10)
      };
    } catch (error) {
      logger.error('Failed to get statistics', { error: error.message });
      throw error;
    }
  }

  /**
   * Get the last processed event
   * @returns {Promise<Object|null>} Last event or null
   */
  async getLastProcessedEvent() {
    const query = `
      SELECT * FROM milestone_events 
      ORDER BY processed_at DESC 
      LIMIT 1
    `;

    try {
      const result = await database.query(query);
      return result.rows.length > 0 ? result.rows[0] : null;
    } catch (error) {
      logger.error('Failed to get last processed event', { error: error.message });
      throw error;
    }
  }

  /**
   * Get recent events for testing/debugging
   * @param {number} limit - Number of events to retrieve
   * @returns {Promise<Array>} Array of events
   */
  async getRecentEvents(limit = 10) {
    const query = `
      SELECT * FROM milestone_events 
      ORDER BY created_at DESC 
      LIMIT $1
    `;

    try {
      const result = await database.query(query, [limit]);
      return result.rows;
    } catch (error) {
      logger.error('Failed to get recent events', { error: error.message });
      throw error;
    }
  }

  /**
   * Mark a processing error as resolved
   * @param {number} errorId - The error ID
   * @returns {Promise<boolean>} Success status
   */
  async resolveError(errorId) {
    const query = 'UPDATE processing_errors SET resolved = true WHERE id = $1';
    
    try {
      await database.query(query, [errorId]);
      logger.info('Error marked as resolved', { errorId });
      return true;
    } catch (error) {
      logger.error('Failed to resolve error', { errorId, error: error.message });
      return false;
    }
  }
}

// Export singleton instance
const databaseService = new DatabaseService();
export default databaseService;
