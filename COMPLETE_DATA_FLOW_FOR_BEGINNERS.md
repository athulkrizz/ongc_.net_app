# 🎓 Complete Data Flow Explained - Beginner's Guide

This guide explains **exactly** what happens when you create a milestone in the ONGC system, from start to finish.

---

## 📚 Table of Contents

1. [Overview - The Big Picture](#overview---the-big-picture)
2. [Step-by-Step Data Flow](#step-by-step-data-flow)
3. [What is Kafka and Why Do We Use It?](#what-is-kafka-and-why-do-we-use-it)
4. [Complete Flow with Code Examples](#complete-flow-with-code-examples)
5. [Using Offset Explorer to View Kafka Messages](#using-offset-explorer-to-view-kafka-messages)
6. [Troubleshooting and Verification](#troubleshooting-and-verification)
7. [Common Questions](#common-questions)

---

## 🌍 Overview - The Big Picture

### What Does This Application Do?

Imagine you're tracking drilling milestones for oil wells. Traditional approach:
- You send data to an API
- API saves it to database
- Done

**Problem**: What if the database is down? You lose data!

### Our Event-Driven Approach:

```
You → .NET API → Kafka (Message Queue) → Node.js Consumer → Database
```

**Benefits**:
- ✅ Data is **never lost** (Kafka stores it)
- ✅ API responds **fast** (doesn't wait for database)
- ✅ **Scalable** (multiple consumers can process events)
- ✅ **Reliable** (if consumer fails, it can retry)

---

## 🔄 Step-by-Step Data Flow

Let me explain what happens when you create a milestone:

### Step 1: User Sends Request (Postman/Frontend → .NET API)

**What You Do:**
```http
POST http://localhost:5275/api/Milestone
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
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

**What Happens:**
1. Your request hits the .NET API running on port 5275
2. The API checks your JWT token (authentication)
3. If valid, request goes to `MilestoneController`

---

### Step 2: .NET API Validates Request

**File**: `ONGC.MilestoneAPI/Controllers/MilestoneController.cs`

**What Happens:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateMilestone([FromBody] MilestoneCreateDto dto)
{
	// 1. VALIDATION: Check if data is valid
	if (!ModelState.IsValid)
		return BadRequest(ModelState);

	// 2. Get user email from JWT token
	var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

	// 3. Create event object
	var milestoneEvent = new MilestoneEvent
	{
		EventId = Guid.NewGuid().ToString(),
		Asset = dto.Asset,
		Well = dto.Well,
		// ... other properties
		UserEmail = userEmail,
		EventTimestamp = DateTime.UtcNow
	};

	// 4. Publish to Kafka (Next step!)
	await _kafkaProducer.PublishMilestoneEventAsync(milestoneEvent);

	// 5. Return response immediately (don't wait for database)
	return Accepted(new { 
		message = "Milestone event published",
		eventId = milestoneEvent.EventId 
	});
}
```

**Key Points:**
- ✅ API validates your data
- ✅ Creates a unique Event ID (like a tracking number)
- ✅ Adds timestamp and user info
- ✅ Returns **202 Accepted** (not 200 OK - because it's async!)

---

### Step 3: Kafka Producer Sends Event to Kafka

**File**: `ONGC.MilestoneAPI/Services/KafkaProducerService.cs`

**What Happens:**
```csharp
public async Task PublishMilestoneEventAsync(MilestoneEvent milestoneEvent)
{
	try
	{
		// 1. Convert object to JSON
		var eventJson = JsonSerializer.Serialize(milestoneEvent);

		// 2. Create Kafka message
		var message = new Message<string, string>
		{
			Key = milestoneEvent.EventId,  // Unique identifier
			Value = eventJson              // The actual data
		};

		// 3. Send to Kafka topic "milestone-events"
		var deliveryResult = await _producer.ProduceAsync(
			"milestone-events",  // Topic name
			message
		);

		_logger.LogInformation(
			$"Event {milestoneEvent.EventId} published to Kafka"
		);
	}
	catch (Exception ex)
	{
		_logger.LogError($"Failed to publish: {ex.Message}");
		throw;
	}
}
```

**What This Means:**
- Your milestone data is converted to JSON
- Sent to Kafka topic called "milestone-events"
- Kafka stores it (even if consumer is offline!)

**Example JSON in Kafka:**
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

---

### Step 4: Kafka Stores the Event

**What is Kafka?**

Think of Kafka as a **super-reliable post office**:
- When you mail a letter (event), it's stored safely
- Even if the recipient (consumer) is not home, letter is safe
- Recipient can pick it up anytime
- Post office keeps records of all letters

**Kafka Components:**

```
┌─────────────────────────────────────┐
│         KAFKA BROKER                │
│                                     │
│  Topic: milestone-events            │
│  ┌───────────────────────────────┐ │
│  │ Partition 0                   │ │
│  │ [Event1][Event2][Event3]...   │ │
│  └───────────────────────────────┘ │
│  ┌───────────────────────────────┐ │
│  │ Partition 1                   │ │
│  │ [Event4][Event5][Event6]...   │ │
│  └───────────────────────────────┘ │
│  ┌───────────────────────────────┐ │
│  │ Partition 2                   │ │
│  │ [Event7][Event8][Event9]...   │ │
│  └───────────────────────────────┘ │
└─────────────────────────────────────┘
```

**Key Concepts:**

1. **Topic**: Category of messages (like "milestone-events")
2. **Partition**: Sub-division of topic (for parallel processing)
3. **Offset**: Position of message in partition (like page number)
4. **Message**: Your actual event data

---

### Step 5: Node.js Consumer Receives Event

**File**: `milestone-event-consumer/src/services/kafkaConsumer.ts`

**What Happens:**
```typescript
export async function startConsumer() {
  // 1. Create Kafka consumer
  const consumer = kafka.consumer({
	groupId: 'milestone-consumer-group',  // Consumer group name
  });

  // 2. Connect to Kafka
  await consumer.connect();
  logger.info('Kafka consumer connected');

  // 3. Subscribe to topic
  await consumer.subscribe({ 
	topic: 'milestone-events',
	fromBeginning: false  // Only new messages
  });

  // 4. Start listening for messages
  await consumer.run({
	eachMessage: async ({ topic, partition, message }) => {
	  try {
		// 5. Get the JSON data
		const eventData = message.value?.toString();

		logger.info(`Received event from partition ${partition}`);

		// 6. Process the event (Next step!)
		await processEvent(JSON.parse(eventData));

	  } catch (error) {
		logger.error(`Error processing message: ${error}`);
	  }
	},
  });
}
```

**What This Means:**
- Consumer is always listening to Kafka
- When new message arrives, it processes it
- Like a mailman checking mailbox constantly

---

### Step 6: Event Validation

**File**: `milestone-event-consumer/src/services/eventProcessor.ts`

**What Happens:**
```typescript
export async function processEvent(event: any) {
  try {
	// 1. VALIDATE: Check if event structure is correct
	const { error, value } = milestoneEventSchema.validate(event);

	if (error) {
	  logger.error(`Validation failed: ${error.message}`);
	  await logError(event, error.message);
	  return;  // Stop processing bad data
	}

	logger.info(`Validation passed for event ${value.eventId}`);

	// 2. CHECK FOR DUPLICATES
	const exists = await checkIfEventExists(value.eventId);

	if (exists) {
	  logger.warn(`Event ${value.eventId} already processed. Skipping.`);
	  return;  // Idempotency - don't process twice!
	}

	// 3. SAVE TO DATABASE (Next step!)
	await saveMilestone(value);

	logger.info(`Event ${value.eventId} processed successfully`);

  } catch (error) {
	logger.error(`Processing error: ${error}`);
	await logError(event, error.message);
  }
}
```

**Validation Schema (Joi):**
```typescript
const milestoneEventSchema = Joi.object({
  eventId: Joi.string().required(),
  asset: Joi.string().max(200).required(),
  well: Joi.string().max(200).required(),
  wellbore: Joi.string().max(200).required(),
  userEmail: Joi.string().email().required(),
  currentMilestone: Joi.string().max(500).required(),
  approvalLevel: Joi.string().max(100).required(),
  status: Joi.string().max(100).required(),
  days: Joi.number().integer().min(0).required(),
  percentCompleted: Joi.number().min(0).max(100).required(),
  eventTimestamp: Joi.date().required(),
});
```

**What This Means:**
- ✅ Validates data structure
- ✅ Checks for duplicates (idempotency)
- ✅ Logs errors if validation fails
- ✅ Only processes valid, unique events

---

### Step 7: Save to PostgreSQL Database

**File**: `milestone-event-consumer/src/services/databaseService.ts`

**What Happens:**
```typescript
export async function saveMilestone(event: MilestoneEvent) {
  const query = `
	INSERT INTO milestones (
	  event_id,
	  asset,
	  well,
	  wellbore,
	  user_email,
	  current_milestone,
	  approval_level,
	  status,
	  days,
	  percent_completed,
	  event_timestamp,
	  processed_at,
	  created_at
	) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, NOW(), NOW())
	RETURNING *;
  `;

  const values = [
	event.eventId,
	event.asset,
	event.well,
	event.wellbore,
	event.userEmail,
	event.currentMilestone,
	event.approvalLevel,
	event.status,
	event.days,
	event.percentCompleted,
	event.eventTimestamp,
  ];

  try {
	const result = await pool.query(query, values);
	logger.info(`Milestone saved: ${event.eventId}`);
	return result.rows[0];

  } catch (error) {
	logger.error(`Database error: ${error.message}`);

	// Log to error table
	await logProcessingError(event, error.message);
	throw error;
  }
}
```

**What This Means:**
- ✅ Data is inserted into PostgreSQL
- ✅ Timestamps are added automatically
- ✅ Errors are logged in separate table

---

### Step 8: Complete - Data in Database!

**Verify in Database:**
```sql
SELECT 
  event_id,
  asset,
  well,
  current_milestone,
  status,
  percent_completed,
  processed_at
FROM milestones
WHERE event_id = '550e8400-e29b-41d4-a716-446655440000';
```

**Result:**
```
event_id                              | asset        | well   | current_milestone  | status      | percent_completed | processed_at
--------------------------------------|--------------|--------|-------------------|-------------|-------------------|---------------------
550e8400-e29b-41d4-a716-446655440000 | Mumbai High  | MH-001 | Drilling Started  | In-progress | 25.50             | 2024-12-20 08:15:31
```

---

## 🎯 Complete Flow Summary

```
┌──────────────┐
│   1. USER    │  Sends POST request with milestone data
│   (Postman)  │
└──────┬───────┘
	   │ HTTP Request
	   ▼
┌─────────────────────────────────────────────┐
│   2. .NET API (MilestoneController)         │
│   - Validates JWT token                     │
│   - Validates request data                  │
│   - Creates MilestoneEvent object           │
│   - Generates unique Event ID               │
└──────┬──────────────────────────────────────┘
	   │ Call KafkaProducerService
	   ▼
┌─────────────────────────────────────────────┐
│   3. KafkaProducerService                   │
│   - Converts event to JSON                  │
│   - Sends to Kafka topic                    │
│   - Returns 202 Accepted to user            │
└──────┬──────────────────────────────────────┘
	   │ Kafka Protocol
	   ▼
┌─────────────────────────────────────────────┐
│   4. KAFKA BROKER                           │
│   - Stores event in "milestone-events"      │
│   - Assigns to partition                    │
│   - Keeps event until consumed              │
│   - Guarantees delivery                     │
└──────┬──────────────────────────────────────┘
	   │ Consumer polls for messages
	   ▼
┌─────────────────────────────────────────────┐
│   5. Node.js Kafka Consumer                 │
│   - Listens to topic continuously           │
│   - Receives new event                      │
│   - Parses JSON                             │
└──────┬──────────────────────────────────────┘
	   │ Pass to Event Processor
	   ▼
┌─────────────────────────────────────────────┐
│   6. Event Processor                        │
│   - Validates event with Joi schema         │
│   - Checks for duplicate (eventId)          │
│   - If valid & unique, continue             │
└──────┬──────────────────────────────────────┘
	   │ Call Database Service
	   ▼
┌─────────────────────────────────────────────┐
│   7. Database Service                       │
│   - Inserts into PostgreSQL                 │
│   - Adds timestamps                         │
│   - Returns success                         │
└──────┬──────────────────────────────────────┘
	   │
	   ▼
┌─────────────────────────────────────────────┐
│   8. PostgreSQL Database                    │
│   ✅ Milestone stored permanently           │
│   ✅ Available for queries                  │
└─────────────────────────────────────────────┘
```

---

## 📊 Timeline of Operation

```
Time    | Component          | Action
--------|-------------------|------------------------------------------
00:00   | User              | Sends POST request
00:05   | .NET API          | Receives and validates
00:10   | .NET API          | Creates event object
00:15   | Kafka Producer    | Publishes to Kafka
00:20   | Kafka             | Stores event
00:20   | .NET API          | Returns 202 Accepted to user ✅
00:25   | Node.js Consumer  | Polls Kafka, receives event
00:30   | Event Processor   | Validates event
00:35   | Database Service  | Inserts to PostgreSQL
00:40   | PostgreSQL        | Data saved ✅
```

**Notice**: User gets response at 00:20ms, but data is saved at 00:40ms!
This is **asynchronous processing** - faster for user!

---

## 🔍 Using Offset Explorer to View Kafka Messages

### What is Offset Explorer?

**Offset Explorer** (formerly Kafka Tool) is a GUI application to view and manage Kafka data.

### Step 1: Download Offset Explorer

1. Visit: https://www.kafkatool.com/download.html
2. Download for Windows/Mac/Linux
3. Install the application

### Step 2: Configure Connection

**Launch Offset Explorer:**

1. Click **File → Add New Connection**

2. **Enter Connection Details:**

```
Connection Name: Local Kafka
Bootstrap servers: localhost:9092
```

**Cluster Details Tab:**
```
Kafka Cluster Version: 2.0 or higher
```

**Properties Tab:**
```
(Leave default)
```

**Security Tab:**
```
(Leave empty for local development)
```

3. Click **Test** to verify connection
4. Click **Add** to save

### Step 3: Connect to Kafka

1. In left panel, double-click your connection "Local Kafka"
2. You should see:
   ```
   └── Cluster: Local Kafka
	   ├── Brokers
	   ├── Topics
	   ├── Consumers
	   └── Configuration
   ```

### Step 4: View Topics

1. Expand **Topics**
2. You should see **milestone-events**
3. Expand **milestone-events**:
   ```
   └── milestone-events
	   ├── 0 (Partition 0)
	   ├── 1 (Partition 1)
	   └── 2 (Partition 2)
   ```

### Step 5: View Messages

**To see messages:**

1. Click on **Partition 0** (or any partition)
2. Click **Data** tab at the bottom
3. Set **Format** to **JSON**
4. Click **Retrieve Messages** button

**You'll see:**

```
Offset | Key                                  | Value
-------|--------------------------------------|------------------
0      | 550e8400-e29b-41d4-a716-446655440000 | { "eventId": ... }
1      | 661f9510-f3ac-52e5-b827-557766551111 | { "eventId": ... }
2      | 772fa620-04bd-63f6-c938-668877662222 | { "eventId": ... }
```

### Step 6: Inspect Message Details

**Click on any message** to see full JSON:

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

### Step 7: Monitor Consumer Groups

1. Expand **Consumers** in left panel
2. Find **milestone-consumer-group**
3. Click to see:
   - Which partitions are being consumed
   - Current offset (how many messages processed)
   - Lag (how many messages behind)

**Example:**
```
Consumer Group: milestone-consumer-group

Topic: milestone-events
Partition | Current Offset | Log End Offset | Lag
----------|---------------|----------------|-----
0         | 45            | 45             | 0
1         | 38            | 38             | 0
2         | 42            | 42             | 0
```

**Understanding Lag:**
- **Lag = 0**: Consumer is up-to-date ✅
- **Lag > 0**: Consumer is behind (processing old messages)

---

## 🛠️ Troubleshooting and Verification

### Verify Each Step Works

#### 1. Check Kafka is Running

**Windows PowerShell:**
```powershell
Test-NetConnection localhost -Port 9092
```

**Expected Output:**
```
ComputerName     : localhost
RemoteAddress    : ::1
RemotePort       : 9092
TcpTestSucceeded : True
```

#### 2. Check Topic Exists

```powershell
cd C:\kafka
.\bin\windows\kafka-topics.bat --list --bootstrap-server localhost:9092
```

**Expected Output:**
```
milestone-events
```

#### 3. Check .NET API is Running

**Browser:**
```
http://localhost:5275/swagger
```

You should see Swagger UI with API endpoints.

#### 4. Check Consumer is Running

**Look for logs:**
```
[INFO] Kafka consumer connected
[INFO] Subscribed to topic: milestone-events
[INFO] Kafka consumer started successfully
```

#### 5. Send Test Event

```http
POST http://localhost:5275/api/Milestone
```

**Check .NET API logs:**
```
info: KafkaProducerService[0]
	  Event 550e8400-e29b-41d4-a716-446655440000 published to Kafka
```

#### 6. Check Kafka Received Event

**Command line:**
```powershell
cd C:\kafka
.\bin\windows\kafka-console-consumer.bat --bootstrap-server localhost:9092 --topic milestone-events --from-beginning
```

**You should see JSON:**
```json
{"eventId":"550e8400-e29b-41d4-a716-446655440000","asset":"Mumbai High"...}
```

**Or use Offset Explorer** (easier!)

#### 7. Check Consumer Processed Event

**Consumer logs:**
```
[INFO] Event received: 550e8400-e29b-41d4-a716-446655440000
[INFO] Validation passed
[INFO] Milestone saved: 550e8400-e29b-41d4-a716-446655440000
```

#### 8. Verify in Database

```sql
SELECT * FROM milestones 
WHERE event_id = '550e8400-e29b-41d4-a716-446655440000';
```

**Should return 1 row.**

---

## ❓ Common Questions

### Q1: What happens if the consumer is offline?

**Answer**: Events are safe in Kafka!
- Kafka stores events for 7 days (default)
- When consumer comes back online, it processes all missed events
- No data is lost

### Q2: What if the same event is processed twice?

**Answer**: We prevent duplicates!
```typescript
// Consumer checks eventId before saving
const exists = await checkIfEventExists(value.eventId);
if (exists) {
  return; // Skip duplicate
}
```

This is called **Idempotency**.

### Q3: Why not save directly to database from .NET API?

**Answer**: Multiple reasons!

**Without Kafka:**
```
.NET API → PostgreSQL
```
❌ If database is down, data is lost  
❌ API is slow (waits for database)  
❌ Can't scale easily  
❌ Tight coupling  

**With Kafka:**
```
.NET API → Kafka → Consumer → PostgreSQL
```
✅ Data safe in Kafka  
✅ API responds fast  
✅ Can add more consumers  
✅ Loose coupling  

### Q4: How do I know consumer is processing events?

**Check logs:**
```bash
# Consumer logs show:
[INFO] Event received: <eventId>
[INFO] Validation passed
[INFO] Milestone saved: <eventId>
```

**Check consumer lag in Offset Explorer:**
- Lag = 0: Everything processed ✅
- Lag > 0: Consumer is behind

**Check database:**
```sql
SELECT COUNT(*) FROM milestones;
```

### Q5: What if validation fails?

**Answer**: Event goes to error table!

```typescript
// If validation fails:
await logProcessingError(event, errorMessage);
```

**Check errors:**
```sql
SELECT * FROM processing_errors 
ORDER BY created_at DESC 
LIMIT 10;
```

### Q6: Can I replay events?

**Yes!** Kafka keeps events for 7 days.

**Replay all events:**
```typescript
// Change consumer to read from beginning
await consumer.subscribe({ 
  topic: 'milestone-events',
  fromBeginning: true  // ← Change this
});
```

### Q7: How fast is this system?

**Typical Performance:**
- API Response: 50ms
- Kafka Write: 10ms
- Consumer Processing: 100ms
- Database Insert: 20ms

**Total**: ~180ms (but user only waits 50ms!)

---

## 🎓 Key Concepts Explained

### Event-Driven Architecture

**Traditional:**
```
Request → Process → Save → Response
(User waits for everything)
```

**Event-Driven:**
```
Request → Publish Event → Response
		  ↓
	   (Async) Process → Save
(User doesn't wait)
```

### Kafka Topics

Think of topics like TV channels:
- "milestone-events" channel broadcasts milestone updates
- Consumers "subscribe" to watch this channel
- Producers "broadcast" to this channel

### Partitions

Topics are divided into partitions for parallel processing:
```
Topic: milestone-events
├── Partition 0: [Event1, Event4, Event7, ...]
├── Partition 1: [Event2, Event5, Event8, ...]
└── Partition 2: [Event3, Event6, Event9, ...]
```

Multiple consumers can process different partitions simultaneously!

### Offsets

Offset = Position in partition

```
Partition 0: [Msg0][Msg1][Msg2][Msg3][Msg4]
			  ↑                        ↑
		   Offset 0               Offset 4
```

Consumer remembers: "I processed up to offset 4"

### Consumer Groups

Multiple consumers with same group ID share the work:

```
Consumer Group: milestone-consumer-group

Consumer 1 → Partition 0
Consumer 2 → Partition 1
Consumer 3 → Partition 2
```

---

## 📝 Summary

### What You Learned:

1. ✅ **Complete data flow** from .NET to PostgreSQL
2. ✅ **How Kafka works** as a message broker
3. ✅ **How to use Offset Explorer** to view messages
4. ✅ **How to verify** each step works
5. ✅ **Why event-driven** architecture is beneficial
6. ✅ **How to troubleshoot** issues

### The Flow in One Sentence:

> User sends milestone to .NET API → API publishes event to Kafka → Kafka stores it → Node.js consumer reads it → Validates it → Saves to PostgreSQL.

### Key Benefits:

- 🚀 **Fast Response**: API returns immediately
- 🛡️ **Reliable**: Data never lost (stored in Kafka)
- 📈 **Scalable**: Add more consumers for speed
- 🔄 **Resilient**: Can replay events if needed
- 🎯 **Decoupled**: Services work independently

---

## 🚀 Next Steps

1. **Practice**: Create milestones and watch the flow
2. **Monitor**: Use Offset Explorer to see messages
3. **Experiment**: Stop consumer and see events queue up
4. **Learn**: Try adding your own fields
5. **Scale**: Start multiple consumers

---

**Happy Learning! 🎉**

If you have questions, check the other documentation:
- [COMPLETE_TESTING_GUIDE_STEP_BY_STEP.md](COMPLETE_TESTING_GUIDE_STEP_BY_STEP.md)
- [EVENT_DRIVEN_SETUP_GUIDE.md](EVENT_DRIVEN_SETUP_GUIDE.md)
