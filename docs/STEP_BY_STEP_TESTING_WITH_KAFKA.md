# 🧪 Step-by-Step Testing Guide with Kafka Monitoring

This guide walks you through testing the complete system and monitoring Kafka.

---

## 📋 Prerequisites Checklist

Before starting, ensure you have:

```
□ PostgreSQL running on port 5432
□ Kafka running on port 9092
□ Zookeeper running on port 2181
□ .NET API running on port 5275
□ Node.js Consumer running
□ Offset Explorer installed (optional but recommended)
□ Postman installed
```

---

## 🚀 Part 1: Complete System Test

### Step 1: Start All Services

**Terminal 1 - Zookeeper:**
```powershell
cd C:\kafka
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
```
✅ Wait for: `binding to port 0.0.0.0/0.0.0.0:2181`

**Terminal 2 - Kafka:**
```powershell
cd C:\kafka
.\bin\windows\kafka-server-start.bat .\config\server.properties
```
✅ Wait for: `[KafkaServer id=0] started`

**Terminal 3 - .NET API:**
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\ONGC.MilestoneAPI
dotnet run
```
✅ Wait for: `Now listening on: http://localhost:5275`

**Terminal 4 - Node.js Consumer:**
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\milestone-event-consumer
npm start
```
✅ Wait for: `Kafka consumer started successfully`

---

### Step 2: Verify Services Are Running

**Check Kafka:**
```powershell
Test-NetConnection localhost -Port 9092
```
Expected: `TcpTestSucceeded : True`

**Check API (in browser):**
```
http://localhost:5275/swagger
```
Expected: Swagger UI loads

**Check Consumer (in Terminal 4):**
Expected to see:
```
[INFO] Kafka consumer connected
[INFO] Subscribed to topic: milestone-events
```

---

### Step 3: Login to Get JWT Token

**Open Postman**

**Request:**
```
POST http://localhost:5275/api/Auth/login
```

**Headers:**
```
Content-Type: application/json
```

**Body (raw JSON):**
```json
{
  "email": "testuser@example.com",
  "password": "Test@1234"
}
```

**Click "Send"**

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlckBleGFtcGxlLmNvbSIsImVtYWlsIjoidGVzdHVzZXJAZXhhbXBsZS5jb20iLCJqdGkiOiI4OGI3ZjJhNi0zNGI5LTRhNjMtOTRhYS1jZjkyZTMxYzllMGEiLCJleHAiOjE3MDMyNTUwNzEsImlzcyI6Ik9OR0MuTWlsZXN0b25lQVBJIiwiYXVkIjoiT05HQy5DbGllbnQifQ.dK8fH3jQm9pY7xNvB2sL4mZaGcWqR1tPnO6vU8iEjKw",
  "email": "testuser@example.com",
  "expiresAt": "2024-12-20T10:30:00Z"
}
```

**Copy the token value** (you'll need it!)

---

### Step 4: Create a Milestone Event

**In Postman, create new request:**

**Request:**
```
POST http://localhost:5275/api/Milestone
```

**Headers:**
```
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9... (paste your token)
```

**Body (raw JSON):**
```json
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

**Click "Send"**

**Expected Response: 202 Accepted**
```json
{
  "message": "Milestone event published successfully",
  "eventId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2024-12-20T08:15:30Z"
}
```

**Copy the eventId** - you'll verify this!

---

### Step 5: Monitor the Flow

**Watch Terminal 3 (.NET API) - You should see:**
```
info: ONGC.MilestoneAPI.Services.KafkaProducerService[0]
	  Event 550e8400-e29b-41d4-a716-446655440000 published to Kafka topic: milestone-events
```

**Watch Terminal 4 (Consumer) - You should see:**
```
[2024-12-20 08:15:31] INFO: Event received: 550e8400-e29b-41d4-a716-446655440000
[2024-12-20 08:15:31] INFO: Event validation passed
[2024-12-20 08:15:31] INFO: Checking for duplicate event...
[2024-12-20 08:15:31] INFO: Saving milestone to database...
[2024-12-20 08:15:31] INFO: Milestone saved successfully: 550e8400-e29b-41d4-a716-446655440000
```

**This confirms the complete flow worked!** ✅

---

### Step 6: Verify in Database

**Option 1: Using pgAdmin**

1. Open pgAdmin
2. Connect to your database
3. Run query:

```sql
SELECT 
	event_id,
	asset,
	well,
	wellbore,
	current_milestone,
	status,
	days,
	percent_completed,
	user_email,
	event_timestamp,
	processed_at
FROM milestones
WHERE event_id = '550e8400-e29b-41d4-a716-446655440000';
```

**Option 2: Using psql command line**

```powershell
psql -h localhost -U ongc_user -d ongc_insight
```

Then:
```sql
SELECT * FROM milestones ORDER BY processed_at DESC LIMIT 1;
```

**Expected Result:**
```
event_id                              | asset        | well   | current_milestone | status      | percent_completed
--------------------------------------|--------------|--------|-------------------|-------------|------------------
550e8400-e29b-41d4-a716-446655440000 | Mumbai High  | MH-001 | Drilling Started | In-progress | 25.50
```

**If you see the row → SUCCESS!** 🎉

---

## 🔍 Part 2: Monitor Kafka with Offset Explorer

### Step 1: Install Offset Explorer

1. Download from: https://www.kafkatool.com/download.html
2. Install and launch

### Step 2: Add Connection

**In Offset Explorer:**

1. Click **File → Add New Connection**

2. **Properties Tab:**
   ```
   Connection Name: ONGC Kafka Local
   Kafka Cluster Version: 2.0 or higher
   Bootstrap servers: localhost:9092
   ```

3. **Advanced Tab:**
   ```
   (Leave defaults)
   ```

4. Click **Test** button
   - Expected: "Connection successful"

5. Click **Add**

### Step 3: Connect and View Topics

1. In left panel, **double-click "ONGC Kafka Local"**
2. Connection establishes (indicator turns green)
3. **Expand "Topics"**
4. You should see: **milestone-events**

### Step 4: View Messages in Topic

1. **Click on "milestone-events"**
2. **Expand "milestone-events"** to see partitions:
   ```
   └── milestone-events
	   ├── 0
	   ├── 1
	   └── 2
   ```

3. **Click on partition "0"**
4. At the bottom, click the **"Data"** tab
5. In toolbar, set **"Message Format"** to **"JSON"**
6. Click **"Retrieve Messages"** button (or press F5)

**You should see messages:**

```
Offset | Partition | Key                                  | Timestamp           | Value
-------|-----------|--------------------------------------|---------------------|--------
0      | 0         | 550e8400-e29b-41d4-a716-446655440000 | 2024-12-20 08:15:30 | {...}
1      | 0         | 661f9510-f3ac-52e5-b827-557766551111 | 2024-12-20 08:16:45 | {...}
```

### Step 5: View Message Details

**Click on any row** to see the full JSON in the bottom panel:

```json
{
  "eventId": "550e8400-e29b-41d4-a716-446655440000",
  "asset": "Mumbai High",
  "well": "MH-001",
  "wellbore": "MH-001-A1",
  "userEmail": "testuser@example.com",
  "currentMilestone": "Drilling Started",
  "approvalLevel": "Level-1",
  "status": "In-progress",
  "days": 15,
  "percentCompleted": 25.5,
  "eventTimestamp": "2024-12-20T08:15:30Z"
}
```

**This is the exact event in Kafka!** 📨

### Step 6: Monitor Consumer Groups

1. In left panel, **expand "Consumers"**
2. You should see: **milestone-consumer-group**
3. **Click on "milestone-consumer-group"**

**You'll see consumer information:**

```
Topic: milestone-events

Partition | Consumer   | Current Offset | Log End Offset | Lag
----------|------------|----------------|----------------|-----
0         | consumer-1 | 15             | 15             | 0
1         | consumer-1 | 12             | 12             | 0
2         | consumer-1 | 18             | 18             | 0
```

**Understanding the columns:**
- **Current Offset**: How many messages consumer has processed
- **Log End Offset**: Total messages in partition
- **Lag**: Difference (0 = caught up, >0 = behind)

**Lag = 0 means consumer is processing in real-time!** ✅

---

## 🧪 Part 3: Testing Different Scenarios

### Test 1: Send Multiple Events Quickly

**In Postman:**

Send the same request **5 times rapidly** (change asset each time):

```json
{"asset": "Mumbai High", "well": "MH-001", ...}
{"asset": "Bombay Offshore", "well": "BO-002", ...}
{"asset": "Krishna Godavari", "well": "KG-003", ...}
{"asset": "Assam Shelf", "well": "AS-004", ...}
{"asset": "Cambay Basin", "well": "CB-005", ...}
```

**Watch Offset Explorer:**
- Messages appear across different partitions
- Offset numbers increase

**Watch Consumer logs:**
- All 5 events processed
- Database has 5 new rows

**Verify:**
```sql
SELECT asset, well, processed_at 
FROM milestones 
ORDER BY processed_at DESC 
LIMIT 5;
```

### Test 2: Stop Consumer and Test Kafka Storage

**Stop the consumer:**
- In Terminal 4, press **Ctrl+C**

**Send 3 more milestone events via Postman**

**Check Offset Explorer:**
- Messages are in Kafka ✅ (in "Data" tab)
- Lag increases (in "Consumers" tab)

**Example:**
```
Partition | Current Offset | Log End Offset | Lag
----------|----------------|----------------|-----
0         | 15             | 17             | 2  ← Behind!
```

**Now restart consumer:**
```powershell
npm start
```

**Watch consumer logs:**
```
[INFO] Event received: <eventId1>
[INFO] Milestone saved: <eventId1>
[INFO] Event received: <eventId2>
[INFO] Milestone saved: <eventId2>
[INFO] Event received: <eventId3>
[INFO] Milestone saved: <eventId3>
```

**Check Offset Explorer again:**
```
Partition | Current Offset | Log End Offset | Lag
----------|----------------|----------------|-----
0         | 17             | 17             | 0  ← Caught up!
```

**This proves Kafka stores messages even when consumer is down!** 🛡️

### Test 3: Duplicate Detection (Idempotency)

**Scenario:** Consumer processes same event twice

**How to test:**

1. Send a milestone and note the `eventId`
2. View it in database
3. Manually insert same `eventId` again:

```sql
-- This will FAIL due to UNIQUE constraint
INSERT INTO milestones (event_id, asset, well, ...)
VALUES ('550e8400-e29b-41d4-a716-446655440000', ...);
```

Expected: `ERROR: duplicate key value violates unique constraint`

**Consumer also checks before inserting:**

If you republish same event to Kafka, consumer logs:
```
[WARN] Event 550e8400-e29b-41d4-a716-446655440000 already processed. Skipping.
```

**This proves the system prevents duplicates!** 🔒

### Test 4: Invalid Data Handling

**Send invalid milestone (missing required field):**

```json
{
  "asset": "Mumbai High",
  "well": "MH-001",
  "wellbore": "MH-001-A1"
  // Missing currentMilestone, status, etc.
}
```

**Expected Response: 400 Bad Request**
```json
{
  "errors": {
	"currentMilestone": ["The currentMilestone field is required."],
	"status": ["The status field is required."]
  }
}
```

Event is **not sent to Kafka** - validation happens at API level!

### Test 5: Consumer Error Handling

**Simulate validation failure:**

1. Manually publish malformed JSON to Kafka:

```powershell
cd C:\kafka
.\bin\windows\kafka-console-producer.bat --bootstrap-server localhost:9092 --topic milestone-events
```

Type:
```json
{"eventId": "test", "asset": 123}
```

Press Enter, then Ctrl+C

**Watch Consumer logs:**
```
[ERROR] Validation failed: "asset" must be a string
[ERROR] Event logged to processing_errors table
```

**Check error table:**
```sql
SELECT * FROM processing_errors ORDER BY created_at DESC LIMIT 1;
```

You'll see the error logged!

---

## 📊 Part 4: Monitoring Dashboard (Using Offset Explorer)

### Key Metrics to Monitor

#### 1. Topic Metrics

**In Offset Explorer:**
- Click on "milestone-events" topic
- View "Details" tab

**Key Info:**
```
Partitions: 3
Replication Factor: 1
Total Messages: 45
Size: 12.5 KB
```

#### 2. Partition Distribution

**Check each partition:**
```
Partition 0: 15 messages
Partition 1: 12 messages
Partition 2: 18 messages
```

Should be roughly balanced.

#### 3. Consumer Lag (Most Important!)

**Consumers → milestone-consumer-group:**

```
Lag = 0   ✅ Healthy (keeping up)
Lag < 10  ⚠️  Slight delay
Lag > 100 🔴 Consumer falling behind
```

**If lag is increasing:**
- Consumer is too slow
- Consider adding more consumers
- Or optimize processing logic

#### 4. Message Rate

**Send 10 messages and monitor:**
- Offset increases by 10
- Consumer processes all 10
- Lag returns to 0

**Calculation:**
```
Messages per second = (End Offset - Start Offset) / Time Elapsed
```

---

## ✅ Success Criteria Checklist

After testing, verify:

```
□ API returns 202 Accepted when creating milestone
□ Event appears in Kafka (verified in Offset Explorer)
□ Consumer processes event (logs show success)
□ Milestone saved in database (SQL query returns row)
□ Consumer lag is 0 (in Offset Explorer)
□ Duplicate events are rejected
□ Invalid data is rejected by API
□ Consumer handles errors gracefully
□ Events survive consumer restarts
```

If all boxes are checked → **Your system is working perfectly!** 🎉

---

## 🐛 Troubleshooting Common Issues

### Issue 1: No messages in Kafka

**Symptoms:**
- Offset Explorer shows 0 messages
- API returns 202 but nothing in Kafka

**Check:**
```powershell
# Check if topic exists
cd C:\kafka
.\bin\windows\kafka-topics.bat --list --bootstrap-server localhost:9092
```

**Should show:** `milestone-events`

**If not, create it:**
```powershell
.\bin\windows\kafka-topics.bat --create --topic milestone-events --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

### Issue 2: Consumer not processing

**Symptoms:**
- Messages in Kafka but not in database
- Consumer lag increasing

**Check consumer logs:**
```
[ERROR] ...
```

**Common causes:**
- Database connection failed
- Validation errors
- Consumer crashed

**Restart consumer:**
```powershell
npm start
```

### Issue 3: Offset Explorer can't connect

**Symptoms:**
- "Connection failed" error

**Check:**
1. Kafka is running:
   ```powershell
   Test-NetConnection localhost -Port 9092
   ```

2. Bootstrap servers correct:
   ```
   localhost:9092  (not localhost:2181)
   ```

3. Firewall not blocking

### Issue 4: Database errors

**Symptoms:**
```
[ERROR] Database error: connection refused
```

**Check:**
```powershell
# Test PostgreSQL connection
psql -h localhost -U ongc_user -d ongc_insight
```

**Check .env file:**
```env
DATABASE_HOST=localhost
DATABASE_PORT=5432
DATABASE_NAME=ongc_insight
DATABASE_USER=ongc_user
DATABASE_PASSWORD=your_password
```

---

## 📝 Quick Testing Script

Save this as `quick-verify.ps1`:

```powershell
# Quick Verification Script

Write-Host "=== ONGC System Verification ===" -ForegroundColor Green

# 1. Check Kafka
Write-Host "`n1. Checking Kafka..." -ForegroundColor Yellow
Test-NetConnection localhost -Port 9092 | Select-Object TcpTestSucceeded

# 2. Check API
Write-Host "`n2. Checking API..." -ForegroundColor Yellow
try {
	$response = Invoke-WebRequest -Uri "http://localhost:5275/swagger" -UseBasicParsing
	Write-Host "API is running ✓" -ForegroundColor Green
} catch {
	Write-Host "API is NOT running ✗" -ForegroundColor Red
}

# 3. Check topics
Write-Host "`n3. Listing Kafka topics..." -ForegroundColor Yellow
cd C:\kafka
.\bin\windows\kafka-topics.bat --list --bootstrap-server localhost:9092

# 4. Check database
Write-Host "`n4. Checking database..." -ForegroundColor Yellow
$env:PGPASSWORD = "your_password"
$count = psql -h localhost -U ongc_user -d ongc_insight -t -c "SELECT COUNT(*) FROM milestones;"
Write-Host "Total milestones: $count" -ForegroundColor Cyan

Write-Host "`n=== Verification Complete ===" -ForegroundColor Green
```

Run it:
```powershell
.\quick-verify.ps1
```

---

## 🎯 Summary

You now know how to:

1. ✅ **Test the complete flow** from API to database
2. ✅ **Use Offset Explorer** to view Kafka messages
3. ✅ **Monitor consumer groups** and check lag
4. ✅ **Verify each component** is working
5. ✅ **Test edge cases** (duplicates, errors, downtime)
6. ✅ **Troubleshoot issues** when they occur

### Key Tools:
- **Postman**: Send API requests
- **Offset Explorer**: View Kafka messages
- **pgAdmin/psql**: Verify database
- **Terminal logs**: Monitor processing

### The Testing Flow:
```
Postman → API logs → Offset Explorer → Consumer logs → Database query
```

**Happy Testing!** 🚀
