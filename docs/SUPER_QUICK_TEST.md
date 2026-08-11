# 🚀 SUPER QUICK TEST GUIDE (One Page!)

## Start Everything (4 Terminals):

### Terminal 1 - Zookeeper:
```powershell
cd C:\kafka
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties
```
Wait for: `"binding to port"`

### Terminal 2 - Kafka (NEW window):
```powershell
cd C:\kafka
.\bin\windows\kafka-server-start.bat .\config\server.properties
```
Wait for: `"[KafkaServer id=0] started"`

### Terminal 3 - .NET API:
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\ONGC.MilestoneAPI
dotnet run
```
Wait for: `"Now listening on: http://localhost:5275"` ✅

### Terminal 4 - Node.js Consumer:
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\milestone-event-consumer
npm start
```
Wait for: `"Waiting for messages..."` ✅

---

## Test in Postman:

### 1. Login:
```
POST http://localhost:5275/api/Auth/login
Body (raw JSON):
{
  "email": "testuser@example.com",
  "password": "Test@1234"
}
```
→ **COPY THE TOKEN!**

### 2. Send Milestone:
```
POST http://localhost:5275/api/Milestone
Authorization: Bearer {PASTE_TOKEN_HERE}
Body (raw JSON):
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
→ Click **Send**

---

## Watch It Flow!

**Terminal 3 (.NET):**
```
[INFO] Publishing milestone event to Kafka...
[INFO] Event published successfully ✓
```

**Terminal 4 (Node.js):**
```
[INFO] Received milestone event
[INFO] Milestone inserted successfully ✓
```

**Postman:**
```
202 Accepted ✓
```

---

## Verify Database:

```powershell
psql -U postgres -d ongc_insight
SELECT * FROM milestones;
```

**Should show your milestone!** ✅

---

## Success!

```
You → .NET → Kafka → Node.js → PostgreSQL
	   ✓      ✓       ✓         ✓
```

**Total time: ~250ms from click to database!**

**For detailed guide, see: COMPLETE_BEGINNERS_TEST_GUIDE.md**
