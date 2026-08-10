/**
 * SQL schema for ONGC milestone event consumer database
 * Updated to match .NET API event format
 */

/**
 * Create milestones table (matches .NET API structure)
 * Stores all processed milestone events from Kafka
 */
export const CREATE_MILESTONE_EVENTS_TABLE = `
CREATE TABLE IF NOT EXISTS milestones (
  id SERIAL PRIMARY KEY,
  event_id VARCHAR(255) UNIQUE NOT NULL,
  asset VARCHAR(200) NOT NULL,
  well VARCHAR(200) NOT NULL,
  wellbore VARCHAR(200) NOT NULL,
  user_email VARCHAR(200) NOT NULL,
  current_milestone VARCHAR(500) NOT NULL,
  approval_level VARCHAR(100) NOT NULL,
  status VARCHAR(100) NOT NULL,
  days INTEGER NOT NULL,
  percent_completed DECIMAL(5,2) NOT NULL,
  event_timestamp TIMESTAMP NOT NULL,
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
CREATE INDEX IF NOT EXISTS idx_milestones_asset 
  ON milestones(asset);

CREATE INDEX IF NOT EXISTS idx_milestones_well 
  ON milestones(well);

CREATE INDEX IF NOT EXISTS idx_milestones_wellbore 
  ON milestones(wellbore);

CREATE INDEX IF NOT EXISTS idx_milestones_status 
  ON milestones(status);

CREATE INDEX IF NOT EXISTS idx_milestones_event_timestamp 
  ON milestones(event_timestamp);

CREATE INDEX IF NOT EXISTS idx_milestones_created_at 
  ON milestones(created_at);

CREATE INDEX IF NOT EXISTS idx_processing_errors_event_id 
  ON processing_errors(event_id);

CREATE INDEX IF NOT EXISTS idx_processing_errors_resolved 
  ON processing_errors(resolved);
`;

/**
 * All migration queries in order
 */
export const ALL_MIGRATIONS: string[] = [
  CREATE_MILESTONE_EVENTS_TABLE,
  CREATE_PROCESSING_ERRORS_TABLE,
  CREATE_INDEXES
];
