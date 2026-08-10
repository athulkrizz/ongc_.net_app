import dotenv from 'dotenv';
import database from '../src/config/database.js';
import logger from '../src/config/logger.js';
import databaseService from '../src/services/databaseService.js';

// Load environment variables
dotenv.config();

/**
 * Test script to verify consumer is working and display processed events
 */
async function testConsumer(): Promise<void> {
  try {
    logger.info('Starting consumer test...');

    // Connect to database
    await database.connect();

    // Get statistics
    console.log('\n=== DATABASE STATISTICS ===');
    const stats = await databaseService.getStatistics();
    console.log(`Total Events Processed: ${stats.totalEvents}`);
    console.log(`Unresolved Errors: ${stats.unresolvedErrors}`);
    
    console.log('\nEvents by Milestone Type:');
    stats.eventsByType.forEach(row => {
      console.log(`  ${row.milestone_type}: ${row.count}`);
    });

    // Get recent events
    console.log('\n=== LAST 10 PROCESSED EVENTS ===');
    const recentEvents = await databaseService.getRecentEvents(10);
    
    if (recentEvents.length === 0) {
      console.log('No events found in database');
    } else {
      recentEvents.forEach((event, index) => {
        console.log(`\n${index + 1}. Event ID: ${event.event_id}`);
        console.log(`   Design ID: ${event.design_id}`);
        console.log(`   Milestone Type: ${event.milestone_type}`);
        console.log(`   Work Centre: ${event.work_centre || 'N/A'}`);
        console.log(`   User ID: ${event.user_id}`);
        console.log(`   Milestone Timestamp: ${event.milestone_timestamp}`);
        console.log(`   Processed At: ${event.processed_at}`);
      });
    }

    // Get processing errors
    console.log('\n=== RECENT PROCESSING ERRORS ===');
    const errors = await database.query(
      'SELECT * FROM processing_errors WHERE resolved = false ORDER BY created_at DESC LIMIT 5'
    );
    
    if (errors.rows.length === 0) {
      console.log('No unresolved errors');
    } else {
      errors.rows.forEach((error: any, index: number) => {
        console.log(`\n${index + 1}. Error ID: ${error.id}`);
        console.log(`   Event ID: ${error.event_id || 'N/A'}`);
        console.log(`   Error: ${error.error_message}`);
        console.log(`   Retry Count: ${error.retry_count}`);
        console.log(`   Created At: ${error.created_at}`);
      });
    }

    console.log('\n=== TEST COMPLETED ===\n');

    await database.close();
    process.exit(0);
  } catch (error) {
    const err = error as Error;
    logger.error('Test failed', {
      error: err.message,
      stack: err.stack
    });
    await database.close();
    process.exit(1);
  }
}

// Run test
testConsumer();
