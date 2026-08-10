import dotenv from 'dotenv';
import database from '../src/config/database.js';
import logger from '../src/config/logger.js';
import readline from 'readline';

// Load environment variables
dotenv.config();

/**
 * Create readline interface for user input
 */
const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

/**
 * Prompt user for confirmation
 */
function askConfirmation(question) {
  return new Promise((resolve) => {
    rl.question(question, (answer) => {
      resolve(answer.toLowerCase() === 'y' || answer.toLowerCase() === 'yes');
    });
  });
}

/**
 * Reset database by truncating all tables
 */
async function resetDatabase() {
  try {
    logger.info('Starting database reset...');

    // Connect to database
    await database.connect();

    // Show current counts
    const eventCount = await database.query('SELECT COUNT(*) as count FROM milestone_events');
    const errorCount = await database.query('SELECT COUNT(*) as count FROM processing_errors');

    console.log('\n=== CURRENT DATABASE STATE ===');
    console.log(`Milestone Events: ${eventCount.rows[0].count}`);
    console.log(`Processing Errors: ${errorCount.rows[0].count}`);
    console.log('');

    // Ask for confirmation
    const confirmed = await askConfirmation(
      'Are you sure you want to DELETE ALL DATA from these tables? (y/n): '
    );

    if (!confirmed) {
      console.log('Reset cancelled.');
      rl.close();
      await database.close();
      process.exit(0);
      return;
    }

    // Truncate tables
    logger.info('Truncating tables...');
    
    await database.query('TRUNCATE TABLE milestone_events RESTART IDENTITY CASCADE');
    await database.query('TRUNCATE TABLE processing_errors RESTART IDENTITY CASCADE');

    logger.info('Database reset completed successfully');
    console.log('\n✓ All data has been cleared from the database');
    console.log('✓ ID sequences have been reset\n');

    rl.close();
    await database.close();
    process.exit(0);
  } catch (error) {
    logger.error('Database reset failed', {
      error: error.message,
      stack: error.stack
    });
    rl.close();
    await database.close();
    process.exit(1);
  }
}

// Run reset
resetDatabase();
