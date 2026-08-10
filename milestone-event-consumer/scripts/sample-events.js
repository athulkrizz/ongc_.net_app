/**
 * Sample test events that match the .NET API event format
 * These can be used to test the consumer without the .NET API running
 */

export const sampleEvents = [
  {
    eventId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T08:30:00.000Z",
    data: {
      designId: 1,
      milestoneType: "GnGDataReceived",
      workCentre: "Mumbai Office",
      userId: 1,
      recordedAt: "2026-08-10T08:30:00.000Z"
    }
  },
  {
    eventId: "8b2c1d3e-4f5a-6b7c-8d9e-0f1a2b3c4d5e",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T09:15:00.000Z",
    data: {
      designId: 1,
      milestoneType: "MDTConducted",
      workCentre: "Mumbai Office",
      userId: 2,
      recordedAt: "2026-08-10T09:15:00.000Z"
    }
  },
  {
    eventId: "1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T10:00:00.000Z",
    data: {
      designId: 1,
      milestoneType: "DesignInitiated",
      workCentre: "Mumbai Office",
      userId: 3,
      recordedAt: "2026-08-10T10:00:00.000Z"
    }
  },
  {
    eventId: "7f8e9d0c-1b2a-3f4e-5d6c-7b8a9f0e1d2c",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T11:30:00.000Z",
    data: {
      designId: 2,
      milestoneType: "ApprovalInitiated",
      workCentre: "Delhi Office",
      userId: 4,
      recordedAt: "2026-08-10T11:30:00.000Z"
    }
  },
  {
    eventId: "9e8d7c6b-5a4f-3e2d-1c0b-9a8f7e6d5c4b",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T12:00:00.000Z",
    data: {
      designId: 2,
      milestoneType: "Level1Approval",
      workCentre: "Delhi Office",
      userId: 5,
      recordedAt: "2026-08-10T12:00:00.000Z"
    }
  },
  {
    eventId: "6d5c4b3a-2f1e-0d9c-8b7a-6f5e4d3c2b1a",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T13:00:00.000Z",
    data: {
      designId: 3,
      milestoneType: "DWPSentDFS",
      workCentre: "Bangalore Office",
      userId: 6,
      recordedAt: "2026-08-10T13:00:00.000Z"
    }
  },
  {
    eventId: "5c4b3a2f-1e0d-9c8b-7a6f-5e4d3c2b1a0f",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T14:00:00.000Z",
    data: {
      designId: 3,
      milestoneType: "DWPReceivedDFS",
      workCentre: "Bangalore Office",
      userId: 7,
      recordedAt: "2026-08-10T14:00:00.000Z"
    }
  },
  {
    eventId: "4b3a2f1e-0d9c-8b7a-6f5e-4d3c2b1a0f9e",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T15:00:00.000Z",
    data: {
      designId: 4,
      milestoneType: "Level2Approval",
      workCentre: "Kolkata Office",
      userId: 8,
      recordedAt: "2026-08-10T15:00:00.000Z"
    }
  },
  {
    eventId: "3a2f1e0d-9c8b-7a6f-5e4d-3c2b1a0f9e8d",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T16:00:00.000Z",
    data: {
      designId: 5,
      milestoneType: "Level3Approval",
      workCentre: "Chennai Office",
      userId: 9,
      recordedAt: "2026-08-10T16:00:00.000Z"
    }
  },
  {
    eventId: "2f1e0d9c-8b7a-6f5e-4d3c-2b1a0f9e8d7c",
    eventType: "MilestoneUpdated",
    timestamp: "2026-08-10T17:00:00.000Z",
    data: {
      designId: 1,
      milestoneType: "DWPSentCementing",
      workCentre: "Mumbai Office",
      userId: 10,
      recordedAt: "2026-08-10T17:00:00.000Z"
    }
  }
];

export default sampleEvents;
