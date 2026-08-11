# 🧪 COMPLETE SYSTEM TEST - Postman to Kafka
## Step-by-Step Beginner's Guide to Test Everything is Working

---

## 📋 What We'll Test

We're going to verify the complete flow:
1. ✅ .NET API is running
2. ✅ PostgreSQL database is accessible
3. ✅ Can register and login
4. ✅ Can create milestone
5. ✅ Data is saved in database
6. ✅ Event is published to Kafka
7. ✅ Can retrieve the data

**Time needed:** 15-20 minutes

---

## 🎯 PHASE 1: Pre-Flight Checks (Make Sure Everything is Running)

### Step 1.1: Check if .NET API is Running

**Open PowerShell and run:**
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\ONGC.MilestoneAPI
dotnet run
```

**Expected Output:**
```
Building...
info: Microsoft.Hosting.Lifetime[14]
	  Now listening on: http://localhost:5275
info: Microsoft.Hosting.Lifetime[0]
	  Application started. Press Ctrl+C to shut down.
```

✅ **Success Check:** You see "Now listening on: http://localhost:5275"  
❌ **If it fails:** Check for error messages, ensure .NET 8 SDK is installed

**LEAVE THIS TERMINAL WINDOW OPEN!**

---

### Step 1.2: Check if PostgreSQL is Running

**Open a NEW PowerShell window and run:**
```powershell
Get-Process postgres
```

**Expected Output:**
```
Handles  NPM(K)    PM(K)      WS(K)     CPU(s)     Id  SI ProcessName
-------  ------    -----      -----     ------     --  -- -----------
	...     ...   xxxxxx     xxxxxx      xx.xx   xxxx   x postgres
```

✅ **Success Check:** You see postgres processes listed  
❌ **If nothing shows:** PostgreSQL is not running, start it!

**To check database connection:**
```powershell
psql -U postgres -d ongc_insight -c "SELECT 1;"
```

Expected: Should return `1`

---

### Step 1.3: Check if Kafka is Running (Optional but Recommended)

**In PowerShell:**
```powershell
# Check if Kafka process is running
Get-Process java | Where-Object {$_.Path -like "*kafka*"} 
```

**OR check Kafka port:**
```powershell
Test-NetConnection -ComputerName localhost -Port 9092
```

**Expected Output:**
```
TcpTestSucceeded : True
```

✅ **Success Check:** Port 9092 is accessible  
⚠️ **If Kafka is not running:** The API will still work, but events won't be published

**To start Kafka** (if needed - separate guide):
```powershell
# Start Zookeeper first
cd C:\kafka
.\bin\windows\zookeeper-server-start.bat .\config\zookeeper.properties

# Then in another terminal, start Kafka
.\bin\windows\kafka-server-start.bat .\config\server.properties
```

---

## 🎯 PHASE 2: Test API with Postman

### Step 2.1: Open Postman

1. Launch Postman application
2. If prompted to sign in, you can skip it
3. You should see a workspace

---

### Step 2.2: Test API is Alive

**Create a simple GET request:**

1. Click **"New"** → **"HTTP Request"**
2. Method: **GET** (default)
3. URL: `http://localhost:5275/`
4. Click **"Send"**

**Expected Response:**
- Status: Could be 200 or 404 (doesn't matter for now)
- The important thing: **You get SOME response** (not "Could not send request")

✅ **Success Check:** You got a response (any response)  
❌ **If "Could not send request":** API is not running, go back to Step 1.1

---

### Step 2.3: Register a New User

**Create POST request for registration:**

1. Click **"New"** → **"HTTP Request"** (or use existing tab)
2. Method: Change to **POST**
3. URL: `http://localhost:5275/api/Auth/register`
4. Click on **"Body"** tab (below URL)
5. Select **"raw"**
6. Select **"JSON"** from the dropdown (right side)
7. Paste this JSON:

```json
{
  "email": "testuser@example.com",
  "password": "Test@1234",
  "role": "User"
}
```

8. Click **"Send"** button (big blue button on right)

**Expected Response:**
```json
{
  "message": "User registered successfully",
  "userId": 1,
  "email": "testuser@example.com",
  "role": "User"
}
```

**Status Code:** `201 Created` (you'll see this in top right corner, in green)

✅ **Success Check:** Status is 201, you see "User registered successfully"

**If you get 400 "Email Already Exists":**
- This is actually GOOD! It means it worked before
- Just use a different email like "testuser2@example.com"

---

### Step 2.4: Login to Get Token

**Create POST request for login:**

1. Create new request (or change the previous one)
2. Method: **POST**
3. URL: `http://localhost:5275/api/Auth/login`
4. Body → raw → JSON
5. Paste this:

```json
{
  "email": "testuser@example.com",
  "password": "Test@1234"
}
```

6. Click **"Send"**

**Expected Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0dXNlckBleGFtcGxlLmNvbSIsImVtYWlsIjoidGVzdHVzZXJAZXhhbXBsZS5jb20iLCJyb2xlIjoiVXNlciIsIm5iZiI6MTcwNjE4MDQwMCwiZXhwIjoxNzA2MjY2ODAwLCJpYXQiOjE3MDYxODA0MDB9.XYZ123...",
  "email": "testuser@example.com",
  "role": "User",
  "expiresAt": "2024-01-26T10:30:00Z"
}
```

**Status Code:** `200 OK` (in green)

✅ **Success Check:** You get a long token string

**IMPORTANT: COPY THE TOKEN!**
- Select the entire token value (the long string after "token": ")
- Right-click → Copy
- Save it in Notepad temporarily
- You'll need this for next steps!

**The token looks like:** `eyJhbGci...` (very long string)

---

## 🎯 PHASE 3: Create Milestone Data (THE MAIN TEST!)

### Step 3.1: Create Your First Milestone

**Create POST request for milestone:**

1. Create **NEW** request
2. Method: **POST**
3. URL: `http://localhost:5275/api/Milestone`

**NOW - Add Authentication!**

4. Click on **"Authorization"** tab (below URL bar)
5. Type: Select **"Bearer Token"** from the dropdown
6. Token: **PASTE YOUR TOKEN** (the one you copied from login)

**Add the data:**

7. Click on **"Body"** tab
8. Select **"raw"**
9. Select **"JSON"**
10. Paste this:

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

11. Click **"Send"**

**Expected Response:**
```json
{
  "id": 1,
  "asset": "Mumbai High",
  "well": "MH-001",
  "wellbore": "MH-001-A1",
  "user": "testuser@example.com",
  "currentMilestone": "Drilling Started",
  "approvalLevel": "Level-1",
  "status": "In-progress",
  "days": 15,
  "percentCompleted": 25.5,
  "createdAt": "2024-01-25T10:30:00Z",
  "message": "Milestone created and published to Kafka successfully"
}
```

**Status Code:** `201 Created` (green)

✅ **Success Check:** 
- Status is 201
- You see "message": "Milestone created and published to Kafka successfully"
- You got an "id" (like 1, 2, 3, etc.)

**IMPORTANT:** Note down the `id` value!

**If you get 401 Unauthorized:**
- Your token is missing or invalid
- Make sure you pasted the token in Authorization tab
- Make sure "Bearer Token" is selected
- Try logging in again to get a fresh token

---

### Step 3.2: Retrieve the Milestone You Just Created

**Create GET request:**

1. Create new request
2. Method: **GET**
3. URL: `http://localhost:5275/api/Milestone`
4. Authorization: **Bearer Token** (paste your token again)
5. Click **"Send"**

**Expected Response:**
```json
{
  "count": 1,
  "data": [
	{
	  "id": 1,
	  "asset": "Mumbai High",
	  "well": "MH-001",
	  "wellbore": "MH-001-A1",
	  "user": "testuser@example.com",
	  "currentMilestone": "Drilling Started",
	  "approvalLevel": "Level-1",
	  "status": "In-progress",
	  "days": 15,
	  "percentCompleted": 25.5,
	  "createdAt": "2024-01-25T10:30:00.123456"
	}
  ]
}
```

✅ **Success Check:** You see your milestone in the response!

---

### Step 3.3: Get Milestone by ID

**Test getting specific milestone:**

1. Method: **GET**
2. URL: `http://localhost:5275/api/Milestone/1` (use the ID from Step 3.1)
3. Authorization: **Bearer Token**
4. Click **"Send"**

**Expected:** You see the details of milestone with ID 1

---

### Step 3.4: Create More Test Milestones

**Let's create 2 more milestones with different data:**

**Milestone 2 - Gujarat:**
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

**Milestone 3 - Rajasthan:**
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

**For each:**
- Use same POST to /api/Milestone
- Same Authorization token
- Just change the Body
- Send
- Verify you get 201 Created

---

## 🎯 PHASE 4: Verify Data in PostgreSQL Database

### Step 4.1: Connect to PostgreSQL

**Open PowerShell:**
```powershell
psql -U postgres -d ongc_insight
```

**You should see:**
```
psql (15.x)
Type "help" for help.

ongc_insight=#
```

---

### Step 4.2: Query the Milestones Table

**Run this SQL:**
```sql
SELECT * FROM "Milestones";
```

**Expected Output:**
```
 Id |    Asset     |  Well  |  Wellbore  |         User          | CurrentMilestone  | ApprovalLevel |  Status     | Days | PercentCompleted |       CreatedAt
----+--------------+--------+------------+-----------------------+-------------------+---------------+-------------+------+------------------+---------------------
  1 | Mumbai High  | MH-001 | MH-001-A1  | testuser@example.com  | Drilling Started  | Level-1       | In-progress |   15 |             25.5 | 2024-01-25 10:30:00
  2 | Gujarat      | GJ-456 | GJ-456-B2  | testuser@example.com  | Reach Target Depth| Level-2       | Completed   |   45 |              100 | 2024-01-25 10:31:00
  3 | Rajasthan    | RJ-789 | RJ-789-C1  | testuser@example.com  | Install Casing    | Level-1       | In-progress |   30 |               60 | 2024-01-25 10:32:00
```

✅ **Success Check:** You see your milestones in the database!

**Count milestones:**
```sql
SELECT COUNT(*) FROM "Milestones";
```

Should show 3 (or however many you created)

**Exit psql:**
```sql
\q
```

---

## 🎯 PHASE 5: Verify Kafka Events

### Step 5.1: Check Kafka Topic Exists

**In PowerShell:**
```powershell
# Navigate to Kafka directory (adjust path as needed)
cd C:\kafka

# List topics
.\bin\windows\kafka-topics.bat --list --bootstrap-server localhost:9092
```

**Expected Output:**
```
milestone-events
__consumer_offsets
```

✅ **Success Check:** You see "milestone-events" topic

---

### Step 5.2: Read Messages from Kafka Topic

**This will show all messages published to Kafka:**

```powershell
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic milestone-events --from-beginning
```

**Expected Output:**
```json
{"Id":1,"Asset":"Mumbai High","Well":"MH-001","Wellbore":"MH-001-A1","User":"testuser@example.com","CurrentMilestone":"Drilling Started","ApprovalLevel":"Level-1","Timestamp":"2024-01-25T10:30:00Z","EventType":"MilestoneCreated"}
{"Id":2,"Asset":"Gujarat","Well":"GJ-456","Wellbore":"GJ-456-B2","User":"testuser@example.com","CurrentMilestone":"Reach Target Depth","ApprovalLevel":"Level-2","Timestamp":"2024-01-25T10:31:00Z","EventType":"MilestoneCreated"}
{"Id":3,"Asset":"Rajasthan","Well":"RJ-789","Wellbore":"RJ-789-C1","User":"testuser@example.com","CurrentMilestone":"Install Casing","ApprovalLevel":"Level-1","Timestamp":"2024-01-25T10:32:00Z","EventType":"MilestoneCreated"}
```

✅ **Success Check:** You see JSON messages for each milestone you created!

**To stop the consumer:** Press `Ctrl+C`

---

### Step 5.3: Verify Kafka Consumer Group (Optional)

**Check consumer groups:**
```powershell
.\bin\windows\kafka-consumer-groups.bat --bootstrap-server localhost:9092 --list
```

**Describe a consumer group:**
```powershell
.\bin\windows\kafka-consumer-groups.bat --bootstrap-server localhost:9092 --describe --group your-consumer-group-name
```

---

## 🎯 PHASE 6: Test Complete Flow (End-to-End)

### The Ultimate Test - Watch Everything Happen!

**Setup:**
1. Have Postman ready
2. Have PostgreSQL psql open
3. Have Kafka consumer running

**Now create a new milestone and watch it flow through:**

**1. In Kafka console, start consumer:**
```powershell
cd C:\kafka
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic milestone-events
```
(Leave this running - you'll see new messages appear here)

**2. In PostgreSQL, open another connection:**
```powershell
psql -U postgres -d ongc_insight
```

**3. In Postman, create a NEW milestone:**
```json
{
  "asset": "Assam",
  "well": "AS-321",
  "wellbore": "AS-321-A2",
  "currentMilestone": "Cementing",
  "approvalLevel": "Level-3",
  "status": "Waiting Approval",
  "days": 12,
  "percentCompleted": 40.0
}
```

Click **Send**

**4. WATCH WHAT HAPPENS:**

**In Postman:** You see 201 Created response ✅

**In Kafka Consumer Window:** New message appears immediately! ✅
```json
{"Id":4,"Asset":"Assam","Well":"AS-321"...}
```

**In PostgreSQL:**
```sql
SELECT * FROM "Milestones" WHERE "Well" = 'AS-321';
```
You see the new record! ✅

**🎉 CONGRATULATIONS! The complete flow works!**

---

## 🎯 PHASE 7: Advanced Tests

### Test 7.1: Query by Well

**In Postman:**
```
GET http://localhost:5275/api/Milestone/well/MH-001
Authorization: Bearer Token
```

**Expected:** Only milestones for well MH-001

---

### Test 7.2: Test Without Token (Should Fail)

**In Postman:**
1. Remove the Authorization header
2. Try to create milestone
3. Expected: **401 Unauthorized** ✅

This proves authentication is working!

---

### Test 7.3: Test Invalid Data (Should Fail)

**Send milestone with missing required field:**
```json
{
  "asset": "Mumbai High",
  "well": "MH-001"
}
```

**Expected:** **400 Bad Request** ✅

This proves validation is working!

---

## 📊 COMPLETE TEST CHECKLIST

Mark each item as you complete it:

### Prerequisites
- [ ] .NET API running on port 5275
- [ ] PostgreSQL database running
- [ ] Kafka running on port 9092
- [ ] Postman installed

### Authentication Tests
- [ ] Register new user → 201 Created
- [ ] Login → 200 OK + Token received
- [ ] Request without token → 401 Unauthorized ✅

### Milestone Creation
- [ ] Create milestone 1 (Mumbai) → 201 Created
- [ ] Create milestone 2 (Gujarat) → 201 Created
- [ ] Create milestone 3 (Rajasthan) → 201 Created

### Data Retrieval
- [ ] Get all milestones → See all 3
- [ ] Get milestone by ID → See specific one
- [ ] Get milestones by well → Filtered correctly

### Database Verification
- [ ] Connect to PostgreSQL ✅
- [ ] Query Milestones table → See all records
- [ ] Count matches number created

### Kafka Verification
- [ ] Topic "milestone-events" exists
- [ ] Read messages from topic → See all events
- [ ] Real-time: Create milestone & see Kafka message immediately

### Error Handling
- [ ] Test without token → 401
- [ ] Test with invalid data → 400
- [ ] Test with wrong password → 401

---

## 🎊 SUCCESS CRITERIA

**You've successfully tested everything if:**

✅ You can register and login  
✅ You can create milestones with token  
✅ You get 401 without token  
✅ Data appears in PostgreSQL database  
✅ Events appear in Kafka topic  
✅ You can retrieve the data via API  
✅ Complete flow works: Postman → API → Database → Kafka  

---

## 🐛 Troubleshooting

### Problem: "Could not send request" in Postman
**Solution:** API is not running
```powershell
cd ONGC.MilestoneAPI
dotnet run
```

### Problem: 401 Unauthorized
**Solution:** 
- Make sure you logged in first
- Copy the entire token
- Paste in Authorization → Bearer Token
- Make sure "Bearer Token" type is selected

### Problem: Can't connect to PostgreSQL
**Solution:**
```powershell
# Check if running
Get-Process postgres

# If not, start PostgreSQL service
net start postgresql-x64-15
```

### Problem: Kafka messages not appearing
**Solution:**
1. Check Kafka is running:
   ```powershell
   Test-NetConnection localhost -Port 9092
   ```
2. Check API logs for Kafka errors
3. Even if Kafka fails, data is saved to database!

### Problem: "Email Already Exists"
**Solution:** This is normal! Use a different email or login with existing one

---

## 📝 Quick Reference Commands

### Start API
```powershell
cd C:\Users\athul.krishnan\Desktop\nodejs_dotnet_kafka_project\ONGC.MilestoneAPI
dotnet run
```

### Check Database
```powershell
psql -U postgres -d ongc_insight -c "SELECT COUNT(*) FROM \"Milestones\";"
```

### Read Kafka Messages
```powershell
cd C:\kafka
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic milestone-events --from-beginning
```

### Check All Services
```powershell
# API
Test-NetConnection localhost -Port 5275

# PostgreSQL
Test-NetConnection localhost -Port 5432

# Kafka
Test-NetConnection localhost -Port 9092
```

---

## 🎯 Next Steps After Successful Test

1. **Save your Postman collection:**
   - Click "Save" on each request
   - Create a collection called "ONGC Milestone API"
   - Organize into folders

2. **Create environment variables in Postman:**
   - Create environment "Local Dev"
   - Add variable: `base_url` = `http://localhost:5275`
   - Add variable: `jwt_token` = (will be auto-saved)

3. **Export test data:**
   - Export some milestones from database
   - Save as backup

4. **Test the Node.js consumer** (if you have one)

5. **Create a simple dashboard** to visualize the data

---

**🎉 Congratulations! You've tested the complete system!**

**The data flows perfectly:**
Postman → .NET API → PostgreSQL → Kafka → Success! ✅

---

## 📞 Need Help?

If any test fails:
1. Check which phase failed
2. Look at API terminal for error logs
3. Verify that service is running
4. Check the troubleshooting section
5. Review error messages carefully

**Everything working? You're ready to build more features! 🚀**
