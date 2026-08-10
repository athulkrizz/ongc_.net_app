/**
 * SQL schema for milestone event consumer database
 */

/**
 * Create milestone_events table
 * Stores all processed milestone events from Kafka
 */
export const CREATE_MILESTONE_EVENTS_TABLE = `
CREATE TABLE IF NOT EXISTS milestone_events (
  id SERIAL PRIMARY KEY,
  event_id VARCHAR(255) UNIQUE NOT NULL,
  event_type VARCHAR(50) NOT NULL,
  design_id INTEGER NOT NULL,
  milestone_type VARCHAR(100) NOT NULL,
  work_centre VARCHAR(200),
  user_id INTEGER,
  milestone_timestamp TIMESTAMP NOT NULL,
  recorded_at TIMESTAMP NOT NULL,
  processed_at TIMESTAMP DEFAULT NOW(),
  created_at TIMESTAMP DEFAULT NOW()
);
`;

/**
 * Create processing_errors table
 * Stores events that failed to process for troubleshooting
 */
export const CREATE_PROCESSING_ERRORS_TABLE = `
CREATE TABLE IF NOT EXISTS processing_errors (
  id SERIAL PRIMARY KEY,
  event_id VARCHAR(255),
  error_message TEXT NOT NULL,
  error_stack TEXT,
  raw_event JSONB,
  retry_count INTEGER DEFAULT 0,
  created_at TIMESTAMP DEFAULT NOW(),
  resolved BOOLEAN DEFAULT FALSE
);
`;

/**
 * Create indexes for better query performance
 */
export const CREATE_INDEXES = `
CREATE INDEX IF NOT EXISTS idx_milestone_events_design_id 
  ON milestone_events(design_id);

CREATE INDEX IF NOT EXISTS idx_milestone_events_milestone_type 
  ON milestone_events(milestone_type);

CREATE INDEX IF NOT EXISTS idx_milestone_events_timestamp 
  ON milestone_events(milestone_timestamp);

CREATE INDEX IF NOT EXISTS idx_milestone_events_created_at 
  ON milestone_events(created_at);

CREATE INDEX IF NOT EXISTS idx_processing_errors_event_id 
  ON processing_errors(event_id);

CREATE INDEX IF NOT EXISTS idx_processing_errors_resolved 
  ON processing_errors(resolved);
`;

/**
 * All migration queries in order
 */
export const ALL_MIGRATIONS = [
  CREATE_MILESTONE_EVENTS_TABLE,
  CREATE_PROCESSING_ERRORS_TABLE,
  CREATE_INDEXES
];
