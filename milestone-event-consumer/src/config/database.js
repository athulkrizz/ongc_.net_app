import pg from 'pg';
import logger from './logger.js';

const { Pool } = pg;

/**
 * Database configuration and connection pool
 */
class Database {
  constructor() {
    this.pool = null;
  }

  /**
   * Initialize database connection pool
   */
  async connect() {
    try {
      const config = {
        host: process.env.DB_HOST || 'localhost',
        port: parseInt(process.env.DB_PORT || '5432', 10),
        database: process.env.DB_NAME || 'milestone_consumer_db',
        user: process.env.DB_USER || 'postgres',
        password: process.env.DB_PASSWORD || 'postgres',
        max: parseInt(process.env.DB_MAX_CONNECTIONS || '20', 10),
        idleTimeoutMillis: 30000,
        connectionTimeoutMillis: 5000,
      };

      this.pool = new Pool(config);

      // Test connection
      const client = await this.pool.connect();
      const result = await client.query('SELECT NOW()');
      client.release();

      logger.info('Database connection established', {
        host: config.host,
        port: config.port,
        database: config.database,
        serverTime: result.rows[0].now
      });

      // Handle pool errors
      this.pool.on('error', (err) => {
        logger.error('Unexpected database pool error', { error: err.message, stack: err.stack });
      });

      return true;
    } catch (error) {
      logger.error('Failed to connect to database', {
        error: error.message,
        stack: error.stack,
        host: process.env.DB_HOST,
        database: process.env.DB_NAME
      });
      throw error;
    }
  }

  /**
   * Execute a query
   * @param {string} text - SQL query
   * @param {Array} params - Query parameters
   * @returns {Promise<Object>} Query result
   */
  async query(text, params) {
    const start = Date.now();
    try {
      const result = await this.pool.query(text, params);
      const duration = Date.now() - start;
      
      logger.debug('Executed query', {
        query: text,
        duration: `${duration}ms`,
        rows: result.rowCount
      });
      
      return result;
    } catch (error) {
      logger.error('Query execution failed', {
        error: error.message,
        query: text,
        params
      });
      throw error;
    }
  }

  /**
   * Get a client from the pool for transactions
   * @returns {Promise<Object>} Database client
   */
  async getClient() {
    return await this.pool.connect();
  }

  /**
   * Check if database is healthy
   * @returns {Promise<boolean>}
   */
  async healthCheck() {
    try {
      const result = await this.query('SELECT 1 as health');
      return result.rows.length > 0;
    } catch (error) {
      logger.error('Database health check failed', { error: error.message });
      return false;
    }
  }

  /**
   * Get connection pool stats
   * @returns {Object} Pool statistics
   */
  getStats() {
    if (!this.pool) {
      return null;
    }
    return {
      totalCount: this.pool.totalCount,
      idleCount: this.pool.idleCount,
      waitingCount: this.pool.waitingCount
    };
  }

  /**
   * Close database connection pool
   */
  async close() {
    try {
      if (this.pool) {
        await this.pool.end();
        logger.info('Database connection pool closed');
      }
    } catch (error) {
      logger.error('Error closing database pool', { error: error.message });
      throw error;
    }
  }
}

// Export singleton instance
const database = new Database();
export default database;
