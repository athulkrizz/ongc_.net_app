import dotenv from 'dotenv';
import database from '../config/database.js';
import logger from '../config/logger.js';
import { ALL_MIGRATIONS } from './schema.js';

// Load environment variables
dotenv.config();

/**
 * Run database migrations to create tables and indexes
 */
async function migrate() {
  try {
    logger.info('Starting database migration...');

    // Connect to database
    await database.connect();

    // Run all migrations
    for (let i = 0; i < ALL_MIGRATIONS.length; i++) {
      const migration = ALL_MIGRATIONS[i];
      logger.info(`Running migration ${i + 1}/${ALL_MIGRATIONS.length}`);
      await database.query(migration);
    }

    logger.info('Database migration completed successfully');

    // Verify tables exist
    const tableCheck = await database.query(`
      SELECT table_name 
      FROM information_schema.tables 
      WHERE table_schema = 'public' 
      AND table_name IN ('milestone_events', 'processing_errors')
    `);

    logger.info('Tables created:', {
      tables: tableCheck.rows.map(row => row.table_name)
    });

    await database.close();
    process.exit(0);
  } catch (error) {
    logger.error('Migration failed', {
      error: error.message,
      stack: error.stack
    });
    await database.close();
    process.exit(1);
  }
}

// Run migration if called directly
if (import.meta.url === `file://${process.argv[1]}`) {
  migrate();
}

export default migrate;
