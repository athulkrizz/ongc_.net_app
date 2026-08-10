import dotenv from 'dotenv';
import http from 'http';
import database from './config/database.js';
import logger from './config/logger.js';
import kafkaConsumerService from './services/kafkaConsumer.js';
import eventProcessor from './services/eventProcessor.js';
import databaseService from './services/databaseService.js';
import { ALL_MIGRATIONS } from './models/schema.js';

// Load environment variables
dotenv.config();

interface AppState {
  isShuttingDown: boolean;
  startTime: number;
}

/**
 * Application state
 */
const appState: AppState = {
  isShuttingDown: false,
  startTime: Date.now()
};

/**
 * Create HTTP server for health checks
 */
function createHealthCheckServer(): http.Server {
  const port = parseInt(process.env.HEALTH_CHECK_PORT || '3000', 10);

  const server = http.createServer(async (req, res) => {
    // Set CORS headers
    res.setHeader('Access-Control-Allow-Origin', '*');
    res.setHeader('Content-Type', 'application/json');

    try {
      if (req.url === '/health' && req.method === 'GET') {
        // Health check endpoint
        const dbHealthy = await database.healthCheck();
        const consumerStatus = kafkaConsumerService.getStatus();
        const lastEvent = await databaseService.getLastProcessedEvent();

        const health = {
          status: dbHealthy && consumerStatus.isRunning ? 'healthy' : 'unhealthy',
          timestamp: new Date().toISOString(),
          database: {
            connected: dbHealthy,
            stats: database.getStats()
          },
          kafka: {
            connected: consumerStatus.isRunning,
            topic: consumerStatus.topic,
            groupId: consumerStatus.groupId
          },
          lastProcessedEvent: lastEvent ? {
            eventId: lastEvent.event_id,
            designId: lastEvent.design_id,
            milestoneType: lastEvent.milestone_type,
            processedAt: lastEvent.processed_at
          } : null
        };

        res.writeHead(dbHealthy && consumerStatus.isRunning ? 200 : 503);
        res.end(JSON.stringify(health, null, 2));

      } else if (req.url === '/stats' && req.method === 'GET') {
        // Statistics endpoint
        const processorStats = eventProcessor.getStats();
        const dbStats = await databaseService.getStatistics();
        const uptime = Date.now() - appState.startTime;

        const stats = {
          timestamp: new Date().toISOString(),
          uptime: {
            ms: uptime,
            formatted: formatUptime(uptime)
          },
          processor: processorStats,
          database: dbStats
        };

        res.writeHead(200);
        res.end(JSON.stringify(stats, null, 2));

      } else if (req.url === '/ready' && req.method === 'GET') {
        // Readiness probe
        const dbHealthy = await database.healthCheck();
        const consumerStatus = kafkaConsumerService.getStatus();

        if (dbHealthy && consumerStatus.isRunning) {
          res.writeHead(200);
          res.end(JSON.stringify({ ready: true }));
        } else {
          res.writeHead(503);
          res.end(JSON.stringify({ ready: false }));
        }

      } else {
        // Not found
        res.writeHead(404);
        res.end(JSON.stringify({ error: 'Not found' }));
      }
    } catch (error) {
      const err = error as Error;
      logger.error('Health check endpoint error', {
        error: err.message,
        url: req.url
      });
      res.writeHead(500);
      res.end(JSON.stringify({ error: 'Internal server error' }));
    }
  });

  server.listen(port, () => {
    logger.info(`Health check server listening on port ${port}`, {
      endpoints: [
        `http://localhost:${port}/health`,
        `http://localhost:${port}/stats`,
        `http://localhost:${port}/ready`
      ]
    });
  });

  return server;
}

/**
 * Format uptime in human-readable format
 */
function formatUptime(ms: number): string {
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
 * Initialize database tables
 */
async function initializeTables(): Promise<void> {
  try {
    logger.info('Initializing database tables...');
    
    for (let i = 0; i < ALL_MIGRATIONS.length; i++) {
      await database.query(ALL_MIGRATIONS[i]);
    }
    
    logger.info('Database tables initialized successfully');
  } catch (error) {
    const err = error as Error;
    logger.error('Failed to initialize tables', {
      error: err.message,
      stack: err.stack
    });
    throw error;
  }
}

/**
 * Graceful shutdown handler
 */
async function gracefulShutdown(signal: string): Promise<void> {
  if (appState.isShuttingDown) {
    logger.warn('Shutdown already in progress...');
    return;
  }

  appState.isShuttingDown = true;
  logger.info(`Received ${signal}, starting graceful shutdown...`);

  try {
    // Stop accepting new messages
    await kafkaConsumerService.stop();

    // Close database connections
    await database.close();

    logger.info('Graceful shutdown completed');
    process.exit(0);
  } catch (error) {
    const err = error as Error;
    logger.error('Error during shutdown', {
      error: err.message,
      stack: err.stack
    });
    process.exit(1);
  }
}

/**
 * Main application entry point
 */
async function main(): Promise<void> {
  try {
    logger.info('Starting Milestone Event Consumer', {
      nodeVersion: process.version,
      environment: process.env.NODE_ENV || 'development',
      pid: process.pid
    });

    // Connect to database
    logger.info('Connecting to database...');
    await database.connect();

    // Initialize tables
    await initializeTables();

    // Start health check server
    createHealthCheckServer();

    // Start Kafka consumer
    logger.info('Starting Kafka consumer...');
    kafkaConsumerService.setupErrorHandlers();
    await kafkaConsumerService.start();

    logger.info('Application started successfully');
    logger.info('Press Ctrl+C to stop');

    // Handle shutdown signals
    process.on('SIGTERM', () => gracefulShutdown('SIGTERM'));
    process.on('SIGINT', () => gracefulShutdown('SIGINT'));

    // Handle uncaught exceptions
    process.on('uncaughtException', (error: Error) => {
      logger.error('Uncaught exception', {
        error: error.message,
        stack: error.stack
      });
      gracefulShutdown('uncaughtException');
    });

    // Handle unhandled promise rejections
    process.on('unhandledRejection', (reason: any) => {
      logger.error('Unhandled promise rejection', {
        reason: reason instanceof Error ? reason.message : reason,
        stack: reason instanceof Error ? reason.stack : undefined
      });
      gracefulShutdown('unhandledRejection');
    });

  } catch (error) {
    const err = error as Error;
    logger.error('Failed to start application', {
      error: err.message,
      stack: err.stack
    });
    process.exit(1);
  }
}

// Start the application
main();
