import { Kafka } from 'kafkajs';

const kafka = new Kafka({
  clientId: 'milestone-consumer-test',
  brokers: ['localhost:9092']
});

const consumer = kafka.consumer({ groupId: 'test-group' });

const run = async () => {
  await consumer.connect();
  console.log('✅ Connected to Kafka');

  await consumer.subscribe({ topic: 'wellbore-milestone-events', fromBeginning: true });
  console.log('✅ Subscribed to: wellbore-milestone-events');
  console.log('👂 Listening for messages...\n');

  await consumer.run({
    eachMessage: async ({ topic, partition, message }) => {
      const event = JSON.parse(message.value.toString());

      console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
      console.log('📨 NEW MILESTONE EVENT RECEIVED!');
      console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━');
      console.log(`🆔 Event ID: ${event.eventId}`);
      console.log(`📝 Event Type: ${event.eventType}`);
      console.log(`⏰ Timestamp: ${event.timestamp}`);
      console.log('\n📊 Event Data:');
      console.log(`   Design ID: ${event.data.designId}`);
      console.log(`   Milestone Type: ${event.data.milestoneType}`);
      console.log(`   Work Centre: ${event.data.workCentre}`);
      console.log(`   User ID: ${event.data.userId}`);
      console.log(`   Recorded At: ${event.data.recordedAt}`);
      console.log('━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n');
    },
  });
};

run().catch(console.error);

process.on('SIGINT', async () => {
  console.log('\n👋 Disconnecting...');
  await consumer.disconnect();
  process.exit(0);
});
