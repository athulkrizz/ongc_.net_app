# Milestone Event Consumer

A production-ready Node.js application that consumes milestone events from Apache Kafka and stores them in PostgreSQL. This consumer works in conjunction with the ONGC.MilestoneAPI .NET application.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Schema](#database-schema)
- [Event Format](#event-format)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Scripts](#scripts)
- [Monitoring & Logs](#monitoring--logs)
- [Docker Support](#docker-support)
- [Troubleshooting](#troubleshooting)

## Overview

This Node.js application consumes milestone events published by the .NET ONGC.MilestoneAPI to a Kafka topic and persists them in a PostgreSQL database. It provides:

- Reliable event processing with automatic retries
- Idempotent event handling (prevents duplicate processing)
- Comprehensive error tracking
- Health check endpoints for monitoring
- Production-ready logging with Winston

## Features

- ✅ **Kafka Consumer** - Consumes from `wellbore-milestone-events` topic
- ✅ **PostgreSQL Storage** - Persists events with full audit trail
- ✅ **Idempotency** - Prevents duplicate event processing
- ✅ **Retry Logic** - Exponential backoff for failed operations
- ✅ **Error Tracking** - Failed events logged to `processing_errors` table
- ✅ **Health Checks** - HTTP endpoints for monitoring
- ✅ **Graceful Shutdown** - Proper cleanup on SIGTERM/SIGINT
- ✅ **Schema Validation** - Joi-based event validation
- ✅ **Comprehensive Logging** - Winston logger with file rotation

## Prerequisites

- **Node.js** 18.x or higher
- **PostgreSQL** 12.x or higher
- **Apache Kafka** 2.8.x or higher
- **npm** or **yarn**

## Installation

1. **Clone or navigate to the project directory:**

```bash
cd milestone-event-consumer
```

2. **Install dependencies:**

```bash
npm install
```

3. **Set up environment variables:**

```bash
cp .env.example .env
# Edit .env with your configuration
```

4. **Create the PostgreSQL database:**

```sql
CREATE DATABASE milestone_consumer_db;
```

5. **Run database migrations:**

```bash
npm run migrate
```

## Configuration

Edit the `.env` file with your settings:

### Kafka Configuration

```env
KAFKA_BROKER=localhost:9092
KAFKA_GROUP_ID=milestone-consumer-group
KAFKA_TOPIC=wellbore-milestone-events
KAFKA_CLIENT_ID=milestone-consumer
```

### Database Configuration

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=milestone_consumer_db
DB_USER=postgres
DB_PASSWORD=postgres
DB_MAX_CONNECTIONS=20
```

### Application Configuration

```env
NODE_ENV=development
LOG_LEVEL=info
HEALTH_CHECK_PORT=3000
```

### Retry Configuration

```env
MAX_RETRIES=3
RETRY_DELAY_MS=2000
```

## Database Schema

### `milestone_events` Table

Stores all successfully processed milestone events.

| Column               | Type          | Description                              |
|---------------------|---------------|------------------------------------------|
| id                  | SERIAL        | Primary key                              |
| event_id            | VARCHAR(255)  | Unique event identifier from Kafka       |
| event_type          | VARCHAR(50)   | Type of event (e.g., "MilestoneUpdated") |
| design_id           | INTEGER       | Design identifier                        |
| milestone_type      | VARCHAR(100)  | Type of milestone                        |
| work_centre         | VARCHAR(200)  | Work centre location                     |
| user_id             | INTEGER       | User who recorded the milestone          |
| milestone_timestamp | TIMESTAMP     | When the milestone occurred              |
| recorded_at         | TIMESTAMP     | When recorded in .NET API                |
| processed_at        | TIMESTAMP     | When processed by this consumer          |
| created_at          | TIMESTAMP     | Record creation time                     |

### `processing_errors` Table

Stores events that failed to process for debugging.

| Column        | Type          | Description                    |
|--------------|---------------|--------------------------------|
| id           | SERIAL        | Primary key                    |
| event_id     | VARCHAR(255)  | Event identifier (if available)|
| error_message| TEXT          | Error description              |
| error_stack  | TEXT          | Stack trace                    |
| raw_event    | JSONB         | Original event data            |
| retry_count  | INTEGER       | Number of retry attempts       |
| created_at   | TIMESTAMP     | When error occurred            |
| resolved     | BOOLEAN       | Whether error has been fixed   |

## Event Format

The .NET API publishes events in this JSON format:

```json
{
  "eventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "eventType": "MilestoneUpdated",
  "timestamp": "2026-08-10T12:30:00.000Z",
  "data": {
    "designId": 1,
    "milestoneType": "DesignInitiated",
    "workCentre": "Mumbai Office",
    "userId": 5,
    "recordedAt": "2026-08-10T12:30:00.000Z"
  }
}
```

### Valid Milestone Types

- `GnGDataReceived`
- `MDTConducted`
- `DWPSentDFS`
- `DWPReceivedDFS`
- `DWPSentCementing`
- `DWPReceivedCementing`
- `DesignInitiated`
- `ApprovalInitiated`
- `Level1Approval`
- `Level2Approval`
- `Level3Approval`

## Running the Application

### Development Mode (with auto-restart)

```bash
npm run dev
```

### Production Mode

```bash
npm start
```

### Run Database Migration

```bash
npm run migrate
```

### Test Consumer (View Processed Events)

```bash
npm run test
```

### Reset Database (Clear All Data)

```bash
npm run reset
```

## API Endpoints

The application exposes HTTP endpoints on port 3000 (configurable via `HEALTH_CHECK_PORT`).

### Health Check

```bash
GET http://localhost:3000/health
```

Response:
```json
{
  "status": "healthy",
  "timestamp": "2026-08-10T12:30:00.000Z",
  "database": {
    "connected": true,
    "stats": {
      "totalCount": 10,
      "idleCount": 9,
      "waitingCount": 0
    }
  },
  "kafka": {
    "connected": true,
    "topic": "wellbore-milestone-events",
    "groupId": "milestone-consumer-group"
  },
  "lastProcessedEvent": {
    "eventId": "abc-123",
    "designId": 1,
    "milestoneType": "DesignInitiated",
    "processedAt": "2026-08-10T12:25:00.000Z"
  }
}
```

### Statistics

```bash
GET http://localhost:3000/stats
```

Response:
```json
{
  "timestamp": "2026-08-10T12:30:00.000Z",
  "uptime": {
    "ms": 3600000,
    "formatted": "1h 0m"
  },
  "processor": {
    "processedCount": 150,
    "errorCount": 2,
    "duplicateCount": 5,
    "uptimeMs": 3600000,
    "uptimeFormatted": "1h 0m"
  },
  "database": {
    "totalEvents": 150,
    "eventsByType": [
      { "milestone_type": "DesignInitiated", "count": 50 },
      { "milestone_type": "Level1Approval", "count": 40 }
    ],
    "unresolvedErrors": 2
  }
}
```

### Readiness Probe

```bash
GET http://localhost:3000/ready
```

Response:
```json
{
  "ready": true
}
```

## Scripts

### `npm start`
Runs the application in production mode.

### `npm run dev`
Runs the application with nodemon for automatic restarts on file changes.

### `npm run migrate`
Creates database tables and indexes.

### `npm run test`
Displays statistics and recent processed events.

### `npm run reset`
**WARNING**: Deletes all data from the database tables. Requires confirmation.

## Monitoring & Logs

### Log Levels

- `error` - Critical errors
- `warn` - Warnings and non-critical issues
- `info` - General information (default)
- `debug` - Detailed debugging information

### Log Locations

**Development:**
- Console output with colors

**Production:**
- `logs/app.log` - All logs
- `logs/error.log` - Error logs only

### What Gets Logged

- ✅ Consumer connection/disconnection
- ✅ Each event received (eventId, designId, milestoneType)
- ✅ Event processed successfully
- ✅ Duplicate events detected
- ✅ Validation errors
- ✅ Database errors with retry attempts
- ✅ All errors with full stack traces

## Docker Support

### Build Docker Image

```bash
npm run docker:build
```

### Run with Docker

```bash
npm run docker:run
```

Or manually:

```bash
docker build -t milestone-event-consumer .
docker run -p 3000:3000 --env-file .env milestone-event-consumer
```

### Docker Compose Example

```yaml
version: '3.8'
services:
  milestone-consumer:
    build: .
    ports:
      - "3000:3000"
    environment:
      KAFKA_BROKER: kafka:9092
      DB_HOST: postgres
      DB_NAME: milestone_consumer_db
      DB_USER: postgres
      DB_PASSWORD: postgres
    depends_on:
      - postgres
      - kafka
```

## Troubleshooting

### Consumer Not Receiving Messages

1. **Check Kafka connection:**
   ```bash
   # Verify Kafka is running
   curl http://localhost:9092
   ```

2. **Verify topic exists:**
   ```bash
   kafka-topics.sh --list --bootstrap-server localhost:9092
   ```

3. **Check consumer group:**
   ```bash
   kafka-consumer-groups.sh --bootstrap-server localhost:9092 --group milestone-consumer-group --describe
   ```

### Database Connection Errors

1. **Verify PostgreSQL is running:**
   ```bash
   psql -h localhost -U postgres -d milestone_consumer_db
   ```

2. **Check connection settings in `.env`**

3. **Ensure database exists:**
   ```sql
   CREATE DATABASE milestone_consumer_db;
   ```

### Events Not Being Processed

1. **Check logs** for validation errors:
   ```bash
   tail -f logs/app.log
   ```

2. **Query processing_errors table:**
   ```sql
   SELECT * FROM processing_errors WHERE resolved = false ORDER BY created_at DESC;
   ```

3. **Verify event format** matches the expected schema

### High Memory Usage

1. **Reduce max connections** in `.env`:
   ```env
   DB_MAX_CONNECTIONS=10
   ```

2. **Adjust Kafka consumer settings** in `src/config/kafka.js`

### Duplicate Events

This is normal behavior - the consumer detects duplicates and skips them. Check logs:
```
Duplicate event detected - skipping: eventId=abc-123
```

## Architecture

```
┌─────────────────┐
│  .NET API       │
│ (Producer)      │
└────────┬────────┘
         │ Publishes events
         ▼
┌─────────────────┐
│  Kafka Topic    │
│ milestone-events│
└────────┬────────┘
         │ Consumes
         ▼
┌─────────────────┐
│ Node.js Consumer│
│ (This App)      │
└────────┬────────┘
         │ Stores
         ▼
┌─────────────────┐
│  PostgreSQL     │
│    Database     │
└─────────────────┘
```

## Support

For issues or questions:
1. Check the logs in `logs/` directory
2. Query the `processing_errors` table
3. Review the `.env` configuration
4. Check the health endpoint: `http://localhost:3000/health`

## License

MIT
