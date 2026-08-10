# 🚀 Quick Setup Commands

This document contains all the commands you need to clone and run the ONGC Milestone System.

## Prerequisites

- .NET 8 SDK
- Node.js 18+
- PostgreSQL
- Kafka + Zookeeper
- Git

---

## Clone Repository

```bash
git clone https://github.com/athulkrizz/ongc_.net_app.git
cd ongc_.net_app
```

---

## Database Setup

```sql
-- In PostgreSQL
CREATE DATABASE ongc_insight;
CREATE USER ongc_user WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE ongc_insight TO ongc_user;
```

---

## Kafka Setup (Windows)

```powershell
# Terminal 1 - Zookeeper
cd C:\kafka
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties

# Terminal 2 - Kafka
cd C:\kafka
.\bin\windows\kafka-server-start.bat .\config\server.properties

# Terminal 3 - Create Topic
cd C:\kafka
.\bin\windows\kafka-topics.bat --create --topic milestone-events --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

---

## .NET API Setup

```bash
cd ONGC.MilestoneAPI

# Update appsettings.json with your database credentials

# Run migrations
dotnet ef database update

# Start API
dotnet run
```

**API URL**: http://localhost:5275  
**Swagger**: http://localhost:5275/swagger

---

## Node.js Consumer Setup

```bash
cd milestone-event-consumer

# Install dependencies
npm install

# Create .env file (copy from .env.example)
# Update with your database and Kafka settings

# Run migrations
npm run migrate

# Start consumer
npm start
```

---

## Test the System

### 1. Login

```http
POST http://localhost:5275/api/Auth/login
Content-Type: application/json

{
  "email": "testuser@example.com",
  "password": "Test@1234"
}
```

### 2. Create Milestone

```http
POST http://localhost:5275/api/Milestone
Authorization: Bearer <your_token>
Content-Type: application/json

{
  "asset": "Mumbai High",
  "well": "MH-001",
  "wellbore": "MH-001-A1",
  "currentMilestone": "Drilling Started",
  "approvalLevel": "Level-1",
  "status": "In-progress",
  "days": 15,
  "percentCompleted": 25.5
}
```

### 3. Verify in Database

```sql
SELECT * FROM milestones ORDER BY processed_at DESC LIMIT 10;
```

---

## Git Commands for Contributors

### Update Your Fork

```bash
# Add upstream remote (one time only)
git remote add upstream https://github.com/athulkrizz/ongc_.net_app.git

# Fetch latest changes
git fetch upstream

# Merge changes
git merge upstream/main
```

### Create Feature Branch

```bash
git checkout -b feature/your-feature-name
# Make your changes
git add .
git commit -m "feat: description of your feature"
git push origin feature/your-feature-name
```

### Update Your Code

```bash
# Pull latest changes
git pull origin main

# Or if you have conflicts
git fetch origin
git merge origin/main
```

---

## Docker Deployment

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api
docker-compose logs -f consumer

# Stop all services
docker-compose down
```

---

## Troubleshooting Commands

### Check Kafka

```bash
# Windows
Test-NetConnection localhost -Port 9092

# List topics
kafka-topics.bat --list --bootstrap-server localhost:9092

# Check consumer group
kafka-consumer-groups.bat --bootstrap-server localhost:9092 --describe --group milestone-consumer-group
```

### Check Database

```bash
# PostgreSQL connection test
psql -h localhost -U ongc_user -d ongc_insight
```

### Reset Everything

```bash
# Reset .NET Database
cd ONGC.MilestoneAPI
dotnet ef database drop --force
dotnet ef database update

# Reset Node.js Database
cd milestone-event-consumer
npm run reset
npm run migrate
```

---

## Quick Reference

| Service | Port | URL |
|---------|------|-----|
| API | 5275 | http://localhost:5275 |
| Swagger | 5275 | http://localhost:5275/swagger |
| PostgreSQL | 5432 | localhost:5432 |
| Kafka | 9092 | localhost:9092 |
| Zookeeper | 2181 | localhost:2181 |

---

## Support

For detailed documentation, see:
- [README.md](README.md) - Complete documentation
- [EVENT_DRIVEN_SETUP_GUIDE.md](EVENT_DRIVEN_SETUP_GUIDE.md) - Detailed setup
- [COMPLETE_TESTING_GUIDE_STEP_BY_STEP.md](COMPLETE_TESTING_GUIDE_STEP_BY_STEP.md) - Testing guide

---

**Repository**: https://github.com/athulkrizz/ongc_.net_app  
**Issues**: https://github.com/athulkrizz/ongc_.net_app/issues
