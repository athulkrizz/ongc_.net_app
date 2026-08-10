# Quick Setup Guide

## Prerequisites Check

Before starting, ensure you have:
- ✅ Node.js 18+ installed
- ✅ PostgreSQL 12+ running
- ✅ Apache Kafka running
- ✅ .NET ONGC.MilestoneAPI running (producer)

## Quick Start (5 minutes)

### 1. Install Dependencies

```bash
npm install
```

### 2. Configure Environment

```bash
cp .env.example .env
```

Edit `.env` if your settings differ from defaults:
- Kafka broker: `localhost:9092`
- PostgreSQL: `localhost:5432`
- Database: `milestone_consumer_db`

### 3. Create Database

```bash
# Connect to PostgreSQL
psql -U postgres

# Create database
CREATE DATABASE milestone_consumer_db;
\q
```

### 4. Run Migrations

```bash
npm run migrate
```

You should see:
```
✓ Database migration completed successfully
✓ Tables created: milestone_events, processing_errors
```

### 5. Start the Consumer

**Development mode (recommended for first run):**
```bash
npm run dev
```

**Production mode:**
```bash
npm start
```

You should see:
```
✓ Database connection established
✓ Database tables initialized successfully
✓ Health check server listening on port 3000
✓ Kafka consumer connected successfully
✓ Subscribed to Kafka topic: wellbore-milestone-events
✓ Application started successfully
```

### 6. Verify It's Working

Open a new terminal and check health:

```bash
curl http://localhost:3000/health
```

You should get a JSON response with `"status": "healthy"`

### 7. Test with the .NET API

1. Make sure the .NET ONGC.MilestoneAPI is running
2. Create a milestone via the API (POST /api/milestones)
3. Watch the consumer logs - you should see:
   ```
   Received milestone event: eventId=abc-123, designId=1, type=DesignInitiated
   Event processed successfully: eventId=abc-123
   ```

### 8. View Processed Events

```bash
npm run test
```

This shows:
- Total events processed
- Events by milestone type
- Last 10 processed events
- Any processing errors

## Common Issues

### "Cannot connect to Kafka"
- Ensure Kafka is running on `localhost:9092`
- Check `KAFKA_BROKER` in `.env`

### "Database connection failed"
- Ensure PostgreSQL is running
- Verify database `milestone_consumer_db` exists
- Check credentials in `.env`

### "No events being processed"
- Verify .NET API is publishing to Kafka
- Check topic name matches: `wellbore-milestone-events`
- Review logs: `tail -f logs/app.log`

## Next Steps

- Monitor with: `curl http://localhost:3000/stats`
- View logs: `tail -f logs/app.log`
- Reset database: `npm run reset` (if needed)
- Stop consumer: `Ctrl+C`

## Project Structure

```
milestone-event-consumer/
├── src/
│   ├── config/          # Configuration (database, kafka, logger)
│   ├── models/          # Database schema and migrations
│   ├── services/        # Business logic (consumer, processor)
│   ├── validators/      # Event validation
│   ├── utils/           # Utilities (retry logic)
│   └── index.js         # Main entry point
├── scripts/             # Utility scripts
├── logs/                # Application logs
├── .env                 # Configuration
└── README.md            # Full documentation
```

For complete documentation, see [README.md](README.md)
