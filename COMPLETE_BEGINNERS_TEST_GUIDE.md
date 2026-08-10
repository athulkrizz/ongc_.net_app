# 🧪 COMPLETE BEGINNER'S TESTING GUIDE
## Watch Your Data Flow from Start to Finish!

---

## 📋 What We'll Do

We're going to:
1. ✅ Start all the systems
2. ✅ Send a milestone
3. ✅ **WATCH** it flow through each component
4. ✅ Verify it's saved in the database

**Time needed:** 30 minutes (first time)

**What you'll see:** Your data traveling through the entire system in real-time!

---

## 🎯 PREPARATION (Do This First!)

### What You Need:

- [ ] Windows computer
- [ ] PostgreSQL installed and running
- [ ] Kafka installed (with Zookeeper)
- [ ] .NET 8 SDK installed
- [ ] Node.js installed
- [ ] Postman installed

### Open 4 Terminal Windows

**We'll use 4 separate PowerShell windows** so you can see everything happening at once!

```
┌─────────────┬─────────────┐
│  Terminal 1 │  Terminal 2 │  ← Top half of screen
│  Kafka      │  .NET API   │
├─────────────┼─────────────┤
│  Terminal 3 │  Terminal 4 │  ← Bottom half of screen
│  Node.js    │  Testing    │
└─────────────┴─────────────┘
```

**How to arrange:**
1. Open PowerShell (press Windows + X, select "PowerShell")
2. Press Windows + Arrow keys to snap to corners
3. Do this 4 times for 4 windows
4. Label them mentally: Kafka, .NET, Node.js, Testing

---

## 🚀 STEP-BY-STEP SETUP

### TERMINAL 1: Start Kafka

**Purpose:** This is the "mailbox" that holds messages

**What to do:**

1. In Terminal 1, navigate to Kafka folder:
```powershell
cd C:\kafka
```

2. Start Zookeeper first:
```powershell
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
```

**What you should see:**
```
[INFO] binding to port 0.0.0.0/0.0.0.0:2181
[INFO] Snapshotting: 0x0 to C:\kafka\zookeeper-data\version-2\snapshot.0
[INFO] Server environment:java.version=...
```

✅ **Success indicator:** "binding to port" message

**IMPORTANT:** Leave this window open and running!

---

3. **Open a NEW PowerShell window for Kafka broker:**

Navigate to Kafka again:
```powershell
cd C:\kafka
```

4. Start Kafka broker:
```powershell
.\bin\windows\kafka-server-start.bat .\config\server.properties
```

**What you should see:**
```
[INFO] Kafka version: 3.x.x
[INFO] starting (kafka.server.KafkaServer)
[INFO] [KafkaServer id=0] started
```

✅ **Success indicator:** "[KafkaServer id=0] started"

**IMPORTANT:** Leave this window open too!

---

### TERMINAL 2: Start .NET API

**Purpose:** This is the "post office" that validates and forwards messages

**What to do:**

1. Navigate to the .NET project:
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\ONGC.MilestoneAPI
```

2. Start the API:
```powershell
dotnet run
```

**What you should see:**
```
Building...
info: Microsoft.Hosting.Lifetime[14]
	  Now listening on: http://localhost:5275
info: Microsoft.Hosting.Lifetime[0]
	  Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
	  Hosting environment: Development
```

✅ **Success indicator:** "Now listening on: http://localhost:5275" in GREEN

**IMPORTANT:** Leave this running! You'll see logs appear here when you send requests.

---

### TERMINAL 3: Start Node.js Consumer

**Purpose:** This is the "mail carrier" that saves to database

**What to do:**

1. Navigate to Node.js project:
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\milestone-event-consumer
```

2. Install dependencies (first time only):
```powershell
npm install
```

Wait for installation to complete...

3. Create database tables (first time only):
```powershell
npm run migrate
```

**What you should see:**
```
Running database migrations...
Creating table: milestones
Creating table: processing_errors
Creating indexes...
Migrations completed successfully!
```

✅ **Success indicator:** "Migrations completed successfully!"

4. Start the consumer:
```powershell
npm start
```

**What you should see:**
```
[INFO] Kafka consumer initialized
[INFO] Connecting to database...
[INFO] Database connected successfully
[INFO] Subscribing to topic: milestone-events
[INFO] Consumer started successfully
[INFO] Waiting for messages...
```

✅ **Success indicator:** "Waiting for messages..."

**IMPORTANT:** Leave this running! This is where you'll see the data being processed!

---

### TERMINAL 4: Testing & Verification

**Purpose:** We'll use this to check the database

**Keep this open for later steps.**

---

## ✅ CHECKPOINT 1: All Systems Running

**Check each terminal window:**

```
Terminal 1 (Kafka):     "binding to port 0.0.0.0/0.0.0.0:2181" ✓
Terminal 2 (.NET API):  "Now listening on: http://localhost:5275" ✓
Terminal 3 (Node.js):   "Waiting for messages..." ✓
```

**All green?** ✅ **You're ready to test!**

**Something not working?** See troubleshooting at the bottom.

---

## 🎬 THE MAIN EVENT: Sending Your First Milestone

### Open Postman

1. Launch Postman application
2. Click **"New"** → **"HTTP Request"**

---

### PART 1: Login and Get Token

#### Step 1: Create Login Request

**In Postman:**

1. Method: Select **POST**
2. URL: `http://localhost:5275/api/Auth/login`
3. Click **"Body"** tab
4. Select **"raw"**
5. Select **"JSON"** from dropdown
6. Paste this:

```json
{
  "email": "testuser@example.com",
  "password": "Test@1234"
}
```

7. Click **"Send"** button

---

#### What Happens:

**In Postman (You'll see):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlckBl...",
  "email": "testuser@example.com",
  "role": "User",
  "expiresAt": "2024-01-26T10:30:00Z"
}
```
Status: **200 OK** (in green)

**In Terminal 2 (.NET API logs):**
```
[INFO] Request starting HTTP/1.1 POST http://localhost:5275/api/Auth/login
[INFO] Executing endpoint 'AuthController.Login'
[INFO] User logged in successfully: testuser@example.com
[INFO] Request finished - 200 OK
```

✅ **Success!** You're logged in!

---

#### Step 2: Copy the Token

**IMPORTANT:** We need this token for the next step!

1. In Postman response, find the `token` field
2. **Copy the ENTIRE token** (starts with "eyJ...")
3. Save it in Notepad temporarily

**The token looks like:**
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0...
```
(Very long string)

---

**If Login Fails with "User Not Found":**

You need to register first!

**Create a new request:**
- Method: POST
- URL: `http://localhost:5275/api/Auth/register`
- Body (raw JSON):
```json
{
  "email": "testuser@example.com",
  "password": "Test@1234",
  "role": "User"
}
```
- Click Send
- Then go back and login

---

### PART 2: Send Milestone Data (The Main Test!)

#### Step 1: Create Milestone Request

**In Postman (new request or modify existing):**

1. Method: **POST**
2. URL: `http://localhost:5275/api/Milestone`

3. **Click "Authorization" tab**
   - Type: Select **"Bearer Token"**
   - Token: **PASTE YOUR TOKEN HERE** (the one you copied)

4. **Click "Body" tab**
   - Select **"raw"**
   - Select **"JSON"**
   - Paste this:

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

5. **Click "Send"** and WATCH ALL YOUR TERMINALS!

---

#### 🎥 What Happens (Watch Each Window!)

This is where the magic happens! **Watch all 4 windows in order:**

---

### 📺 TERMINAL 2 (.NET API) - IMMEDIATELY

**You'll see this within milliseconds:**

```
[INFO] Request starting HTTP/1.1 POST http://localhost:5275/api/Milestone
[INFO] Executing endpoint 'MilestoneController.CreateMilestone'
[INFO] Publishing milestone event to Kafka: EventId=guid-123-456, Well=MH-001
[INFO] Milestone event published successfully to Kafka: EventId=guid-123-456
[INFO] Request finished - 202 Accepted
```

**This means:**
- ✅ API received your data
- ✅ Validated it
- ✅ Published to Kafka
- ✅ Responded to you

**Time: ~100 milliseconds**

---

### 💻 POSTMAN - YOU SEE

**Response in Postman:**
```json
{
  "eventId": "guid-123-456-789",
  "asset": "Mumbai High",
  "well": "MH-001",
  "wellbore": "MH-001-A1",
  "user": "testuser@example.com",
  "currentMilestone": "Drilling Started",
  "approvalLevel": "Level-1",
  "status": "In-progress",
  "days": 15,
  "percentCompleted": 25.5,
  "message": "Milestone event published to Kafka successfully. Node.js consumer will process and save it."
}
```

**Status:** `202 Accepted` (GREEN!)

✅ **This means your data is in Kafka and will be processed!**

---

### 📺 TERMINAL 3 (Node.js) - WITHIN 1 SECOND

**Now watch Terminal 3! You'll see:**

```
[INFO] Received milestone event {
  eventId: 'guid-123-456-789',
  well: 'MH-001',
  asset: 'Mumbai High',
  partition: 0,
  offset: 1
}
[INFO] Milestone event inserted successfully {
  eventId: 'guid-123-456-789',
  well: 'MH-001',
  asset: 'Mumbai High',
  dbId: 1
}
[INFO] Milestone event processed successfully {
  eventId: 'guid-123-456-789',
  processedCount: 1
}
```

**This means:**
- ✅ Node.js picked up the message from Kafka
- ✅ Validated it
- ✅ Saved to PostgreSQL database
- ✅ Database ID assigned: 1

**Time: Additional ~150 milliseconds**

---

## ✅ CHECKPOINT 2: Data Flow Complete!

**Total time from click to database: ~250 milliseconds!**

**Let's verify the data is really there...**

---

## 🔍 VERIFICATION: Check the Database

### TERMINAL 4: Query the Database

**In Terminal 4, connect to PostgreSQL:**

```powershell
psql -U postgres -d ongc_insight
```

**Enter your PostgreSQL password** (usually: postgres)

**You should see:**
```
psql (15.x)
Type "help" for help.

ongc_insight=#
```

✅ **You're in the database!**

---

### Query the Data

**Type this SQL command:**

```sql
SELECT * FROM milestones;
```

**Press Enter**

---

### What You Should See:

```
 id | event_id                              | asset       | well   | wellbore  | user_email           | current_milestone | approval_level | status      | days | percent_completed |      event_timestamp      |         processed_at          |         created_at
----+---------------------------------------+-------------+--------+-----------+----------------------+-------------------+----------------+-------------+------+-------------------+---------------------------+-------------------------------+---------------------------
  1 | guid-123-456-789                      | Mumbai High | MH-001 | MH-001-A1 | testuser@example.com | Drilling Started  | Level-1        | In-progress |   15 |             25.50 | 2024-01-25 10:30:00       | 2024-01-25 10:30:00.123       | 2024-01-25 10:30:00.123
(1 row)
```

✅ **YOUR DATA IS THERE!**

---

### Pretty View

For better viewing, try:

```sql
SELECT well, current_milestone, status, percent_completed, created_at 
FROM milestones;
```

**You'll see:**
```
  well  | current_milestone |   status    | percent_completed |       created_at
--------+-------------------+-------------+-------------------+------------------------
 MH-001 | Drilling Started  | In-progress |             25.50 | 2024-01-25 10:30:00
```

✅ **Perfect! Your milestone is saved!**

---

### Count Records

```sql
SELECT COUNT(*) FROM milestones;
```

**Shows:**
```
 count
-------
	 1
(1 row)
```

---

### Exit Database

```sql
\q
```

**Press Enter to exit psql**

---

## 🎊 SUCCESS! You've Tested the Complete Data Flow!

### What You Just Did:

```
1. You typed data in Postman ✓
   ↓
2. .NET API validated it ✓
   ↓
3. Published to Kafka ✓
   ↓
4. Node.js picked it up ✓
   ↓
5. Saved to PostgreSQL ✓
   ↓
6. You verified it's there ✓
```

**THE COMPLETE DATA FLOW!** 🎉

---

## 🔄 Test It Again (See it Multiple Times!)

Let's send another milestone to see the pattern!

### In Postman (same request, different data):

**Change the body to:**

```json
{
  "asset": "Gujarat",
  "well": "GJ-456",
  "wellbore": "GJ-456-B2",
  "currentMilestone": "Reached Target Depth",
  "approvalLevel": "Level-2",
  "status": "Completed",
  "days": 45,
  "percentCompleted": 100.0
}
```

**Click "Send"**

---

### Watch All Terminals Again!

**Terminal 2 (.NET):**
```
[INFO] Publishing milestone event to Kafka: EventId=guid-999, Well=GJ-456
[INFO] Milestone event published successfully
```

**Terminal 3 (Node.js):**
```
[INFO] Received milestone event { well: 'GJ-456' }
[INFO] Milestone event inserted successfully { dbId: 2 }
[INFO] Milestone event processed successfully { processedCount: 2 }
```

---

### Check Database Again:

```powershell
psql -U postgres -d ongc_insight
```

```sql
SELECT well, current_milestone, percent_completed FROM milestones;
```

**You should see:**
```
  well  |   current_milestone   | percent_completed
--------+-----------------------+-------------------
 MH-001 | Drilling Started      |             25.50
 GJ-456 | Reached Target Depth  |            100.00
(2 rows)
```

✅ **TWO milestones! The system is working perfectly!**

---

## 📊 Advanced Verification

### Check Kafka Messages

In Terminal 4:

```powershell
cd C:\kafka
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic milestone-events --from-beginning
```

**You'll see ALL the messages that went through Kafka:**

```json
{"EventId":"guid-123-456","EventType":"MilestoneCreated","Timestamp":"2024-01-25T10:30:00Z","Data":{"asset":"Mumbai High",...}}
{"EventId":"guid-999","EventType":"MilestoneCreated","Timestamp":"2024-01-25T10:31:00Z","Data":{"asset":"Gujarat",...}}
```

✅ **Proof that messages are in Kafka!**

**Press Ctrl+C to stop reading messages**

---

### Check Error Handling

In Terminal 4 (psql):

```sql
SELECT * FROM processing_errors;
```

**If everything worked:**
```
 id | event_id | error_message | error_stack | raw_event | retry_count | created_at | resolved
----+----------+---------------+-------------+-----------+-------------+------------+----------
(0 rows)
```

✅ **No errors! Perfect!**

---

## 🎯 Testing Different Scenarios

### Test 1: Without Token (Should Fail)

**In Postman:**
1. Remove the token from Authorization tab
2. Try to send milestone

**Expected Result:**
```json
{
  "status": 401,
  "title": "Unauthorized"
}
```

✅ **Good! Security is working!**

---

### Test 2: Invalid Data (Should Fail)

**In Postman:**
1. Add token back
2. Send incomplete data:

```json
{
  "asset": "Mumbai High",
  "well": "MH-001"
}
```

**Expected Result:**
```json
{
  "status": 400,
  "title": "Validation Failed"
}
```

✅ **Good! Validation is working!**

---

### Test 3: Duplicate Event ID (Should Skip)

This tests idempotency!

**Scenario:** What if the same event comes twice?

**The system will:**
- Check if event_id exists
- Skip if it's a duplicate
- Log it

✅ **This prevents duplicate data!**

---

## 📈 Monitor in Real-Time

### Watch Everything Happen

**Arrange your screen like this:**

```
┌──────────────────┬──────────────────┐
│   Postman        │  .NET Logs       │
│   (Click Send)   │  (Immediate)     │
├──────────────────┼──────────────────┤
│   Node.js Logs   │  Database Query  │
│   (Processes)    │  (Verify)        │
└──────────────────┴──────────────────┘
```

**Then:**
1. Click "Send" in Postman
2. Watch .NET logs light up
3. See Node.js process it
4. Query database to verify

**You're watching data flow in real-time!** 🔥

---

## 🐛 Troubleshooting

### Problem: "Could not send request" in Postman

**Cause:** .NET API not running

**Solution:**
```powershell
cd ONGC.MilestoneAPI
dotnet run
```

Look for: "Now listening on: http://localhost:5275"

---

### Problem: 401 Unauthorized

**Cause:** No token or expired token

**Solution:**
1. Login again to get fresh token
2. Copy the entire token
3. Paste in "Authorization → Bearer Token"

---

### Problem: Node.js not processing

**Cause:** Kafka not running or topic doesn't exist

**Check:**
```powershell
Test-NetConnection localhost -Port 9092
```

Should show: `TcpTestSucceeded : True`

**Create topic manually:**
```powershell
cd C:\kafka
.\bin\windows\kafka-topics.bat --create --topic milestone-events --bootstrap-server localhost:9092
```

---

### Problem: Database connection error

**Cause:** PostgreSQL not running

**Check:**
```powershell
Get-Process postgres
```

**Start PostgreSQL:**
```powershell
net start postgresql-x64-15
```

---

### Problem: "Table doesn't exist"

**Cause:** Migrations not run

**Solution:**
```powershell
cd milestone-event-consumer
npm run migrate
```

---

## 📋 Testing Checklist

Use this to verify everything:

- [ ] Kafka running (port 9092)
- [ ] .NET API running (port 5275)
- [ ] Node.js consumer running
- [ ] Can login and get token
- [ ] Can send milestone
- [ ] Get 202 Accepted response
- [ ] See logs in .NET terminal
- [ ] See logs in Node.js terminal
- [ ] Data appears in PostgreSQL
- [ ] Can query data successfully

**All checked?** ✅ **Your system is production-ready!**

---

## 🎓 What You Learned

You now know how to:
- ✅ Start all the services
- ✅ Send data through the system
- ✅ Watch the complete data flow
- ✅ Verify data in the database
- ✅ Check Kafka messages
- ✅ Troubleshoot issues
- ✅ Test different scenarios

**You're now an expert at testing your system!** 🎉

---

## 📝 Quick Reference

### Start Everything:

```powershell
# Terminal 1: Zookeeper
cd C:\kafka
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties

# Terminal 2: Kafka (new window)
cd C:\kafka
.\bin\windows\kafka-server-start.bat .\config\server.properties

# Terminal 3: .NET API
cd ONGC.MilestoneAPI
dotnet run

# Terminal 4: Node.js Consumer
cd milestone-event-consumer
npm start
```

### Test in Postman:

```
1. POST http://localhost:5275/api/Auth/login
   → Copy token

2. POST http://localhost:5275/api/Milestone
   → Authorization: Bearer {token}
   → Body: milestone JSON
   → Click Send

3. Watch terminals!
4. Check database
```

### Verify:

```sql
psql -U postgres -d ongc_insight
SELECT * FROM milestones;
\q
```

---

## 🎊 You're Done!

You've successfully:
- ✅ Set up the complete system
- ✅ Tested the full data flow
- ✅ Verified everything works
- ✅ Seen data flow in real-time!

**Your event-driven milestone system is fully functional!** 🚀

---

**Questions? Check:**
- Understanding how it works: BEGINNERS_ARCHITECTURE_GUIDE.md
- Visual diagrams: VISUAL_ARCHITECTURE_DIAGRAM.md
- Technical details: COMPLETE_DATA_FLOW_EXPLAINED.md

**Happy Testing!** 🎉
