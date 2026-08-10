import pg from 'pg';
import logger from './logger.js';

const { Pool } = pg;

interface PoolStats {
  totalCount: number;
  idleCount: number;
  waitingCount: number;
}

/**
 * Database configuration and connection pool
 */
class Database {
  private pool: pg.Pool | null = null;

  /**
   * Initialize database connection pool
   */
  async connect(): Promise<boolean> {
    try {
      const config: pg.PoolConfig = {
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
      this.pool.on('error', (err: Error) => {
        logger.error('Unexpected database pool error', { error: err.message, stack: err.stack });
      });

      return true;
    } catch (error) {
      const err = error as Error;
      logger.error('Failed to connect to database', {
        error: err.message,
        stack: err.stack,
        host: process.env.DB_HOST,
        database: process.env.DB_NAME
      });
      throw error;
    }
  }

  /**
   * Execute a query
   * @param text - SQL query
   * @param params - Query parameters
   * @returns Query result
   */
  async query(text: string, params?: any[]): Promise<pg.QueryResult> {
    const start = Date.now();
    try {
      if (!this.pool) {
        throw new Error('Database pool not initialized');
      }
      const result = await this.pool.query(text, params);
      const duration = Date.now() - start;
      
      logger.debug('Executed query', {
        query: text,
        duration: `${duration}ms`,
        rows: result.rowCount
      });
      
      return result;
    } catch (error) {
      const err = error as Error;
      logger.error('Query execution failed', {
        error: err.message,
        query: text,
        params
      });
      throw error;
    }
  }

  /**
   * Get a client from the pool for transactions
   * @returns Database client
   */
  async getClient(): Promise<pg.PoolClient> {
    if (!this.pool) {
      throw new Error('Database pool not initialized');
    }
    return await this.pool.connect();
  }

  /**
   * Check if database is healthy
   * @returns Health status
   */
  async healthCheck(): Promise<boolean> {
    try {
      const result = await this.query('SELECT 1 as health');
      return result.rows.length > 0;
    } catch (error) {
      const err = error as Error;
      logger.error('Database health check failed', { error: err.message });
      return false;
    }
  }

  /**
   * Get connection pool stats
   * @returns Pool statistics
   */
  getStats(): PoolStats | null {
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
  async close(): Promise<void> {
    try {
      if (this.pool) {
        await this.pool.end();
        logger.info('Database connection pool closed');
      }
    } catch (error) {
      const err = error as Error;
      logger.error('Error closing database pool', { error: err.message });
      throw error;
    }
  }
}

// Export singleton instance
const database = new Database();
export default database;
