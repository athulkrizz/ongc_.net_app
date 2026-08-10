# ✅ QUICK TEST CHECKLIST - Print This!

## Before You Start
- [ ] .NET API running: `dotnet run` in ONGC.MilestoneAPI folder
- [ ] PostgreSQL running: Check with `Get-Process postgres`
- [ ] Kafka running: `Test-NetConnection localhost -Port 9092`
- [ ] Postman installed and open

---

## Test Sequence (Do in Order)

### 1. Test API is Alive
```
GET http://localhost:5275/
```
- [ ] Got ANY response (not "Could not send request")

---

### 2. Register User
```
POST http://localhost:5275/api/Auth/register
Body:
{
  "email": "testuser@example.com",
  "password": "Test@1234",
  "role": "User"
}
```
- [ ] Status: 201 Created
- [ ] See "User registered successfully"

---

### 3. Login
```
POST http://localhost:5275/api/Auth/login
Body:
{
  "email": "testuser@example.com",
  "password": "Test@1234"
}
```
- [ ] Status: 200 OK
- [ ] Got a token
- [ ] **COPY THE TOKEN!**

---

### 4. Create Milestone (Main Test!)
```
POST http://localhost:5275/api/Milestone
Authorization: Bearer Token (paste your token here!)
Body:
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
- [ ] Status: 201 Created
- [ ] Got an ID (like "id": 1)
- [ ] Message says "published to Kafka successfully"

---

### 5. Get All Milestones
```
GET http://localhost:5275/api/Milestone
Authorization: Bearer Token
```
- [ ] Status: 200 OK
- [ ] See your milestone in the list

---

### 6. Check Database
```powershell
psql -U postgres -d ongc_insight
SELECT * FROM "Milestones";
\q
```
- [ ] See your milestone in database

---

### 7. Check Kafka
```powershell
cd C:\kafka
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic milestone-events --from-beginning
```
- [ ] See JSON message with your milestone
- [ ] Press Ctrl+C to stop

---

## Success! ✅

If all checkboxes are marked:
- ✅ Complete flow works!
- ✅ Postman → API → Database → Kafka
- ✅ You're ready to build more!

---

## If Something Failed

### Got 401 Unauthorized?
→ Make sure token is pasted in Authorization → Bearer Token

### Got "Could not send request"?
→ API not running. Run: `dotnet run` in ONGC.MilestoneAPI

### Database error?
→ Check PostgreSQL is running: `Get-Process postgres`

### Kafka not showing messages?
→ Check Kafka is running. Data still saved to database!

---

## Quick Commands

**Start API:**
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\ONGC.MilestoneAPI
dotnet run
```

**Check Database:**
```powershell
psql -U postgres -d ongc_insight -c "SELECT COUNT(*) FROM \"Milestones\";"
```

**Read Kafka:**
```powershell
cd C:\kafka
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic milestone-events --from-beginning
```

---

## Test Data Examples

### Milestone 1 - Mumbai
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

### Milestone 2 - Gujarat
```json
{
  "asset": "Gujarat",
  "well": "GJ-456",
  "wellbore": "GJ-456-B2",
  "currentMilestone": "Reach Target Depth",
  "approvalLevel": "Level-2",
  "status": "Completed",
  "days": 45,
  "percentCompleted": 100.0
}
```

### Milestone 3 - Rajasthan
```json
{
  "asset": "Rajasthan",
  "well": "RJ-789",
  "wellbore": "RJ-789-C1",
  "currentMilestone": "Install Casing",
  "approvalLevel": "Level-1",
  "status": "In-progress",
  "days": 30,
  "percentCompleted": 60.0
}
```

---

**Keep this checklist handy while testing! 📋**
