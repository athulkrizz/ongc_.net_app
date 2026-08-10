import { Kafka } from 'kafkajs';

console.log('🚀 Starting Kafka Consumer...\n');

const kafka = new Kafka({
  clientId: 'test-consumer',
  brokers: ['localhost:9092']
});

const consumer = kafka.consumer({ groupId: 'test-group' });

const run = async () => {
  try {
    await consumer.connect();
    console.log('✅ Connected to Kafka broker\n');

    await consumer.subscribe({ 
      topic: 'wellbore-milestone-events', 
      fromBeginning: true 
    });
    console.log('✅ Subscribed to topic: wellbore-milestone-events\n');
    console.log('👂 Listening for milestone events...\n');
    console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n');

    await consumer.run({
      eachMessage: async ({ topic, partition, message }) => {
        const event = JSON.parse(message.value.toString());

        console.log('📨 NEW EVENT RECEIVED!');
        console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
        console.log(`🆔 Event ID:        ${event.eventId}`);
        console.log(`📝 Event Type:      ${event.eventType}`);
        console.log(`⏰ Timestamp:       ${event.timestamp}`);
        console.log('');
        console.log('📊 Data:');
        console.log(`   Design ID:       ${event.data.designId}`);
        console.log(`   Milestone Type:  ${event.data.milestoneType}`);
        console.log(`   Work Centre:     ${event.data.workCentre}`);
        console.log(`   User ID:         ${event.data.userId}`);
        console.log(`   Recorded At:     ${event.data.recordedAt}`);
        console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n');
      },
    });
  } catch (error) {
    console.error('❌ Error:', error);
    process.exit(1);
  }
};

run();

process.on('SIGINT', async () => {
  console.log('\n\n👋 Shutting down...');
  await consumer.disconnect();
  process.exit(0);
});
