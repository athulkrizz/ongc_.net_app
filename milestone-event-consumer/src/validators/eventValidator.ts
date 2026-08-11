import Joi from 'joi';

/**
 * Milestone event data structure (from .NET API)
 */
export interface MilestoneEventData {
  Asset: string;
  Well: string;
  Wellbore: string;
  User: string;
  CurrentMilestone: string;
  ApprovalLevel: string;
  Status: string;
  Days: number;
  PercentCompleted: number;
}

/**
 * Complete milestone event structure (from .NET API)
 */
export interface MilestoneEvent {
  EventId: string;
  EventType: string;
  Timestamp: string;
  Data: MilestoneEventData;
}

/**
 * Joi schema for milestone event validation
 */
export const milestoneEventSchema = Joi.object<MilestoneEvent>({
  EventId: Joi.string()
    .guid({ version: 'uuidv4' })
    .required()
    .description('Unique event identifier'),

  EventType: Joi.string()
    .valid('MilestoneCreated')
    .required()
    .description('Type of event'),

  Timestamp: Joi.string()
    .isoDate()
    .required()
    .description('ISO 8601 timestamp when event occurred'),

  Data: Joi.object({
    Asset: Joi.string()
      .max(200)
      .required()
      .description('Asset name'),

    Well: Joi.string()
      .max(200)
      .required()
      .description('Well name'),

    Wellbore: Joi.string()
      .max(200)
      .required()
      .description('Wellbore name'),

    User: Joi.string()
      .email()
      .max(200)
      .required()
      .description('User email'),

    CurrentMilestone: Joi.string()
      .max(500)
      .required()
      .description('Current milestone'),

    ApprovalLevel: Joi.string()
      .max(100)
      .required()
      .description('Approval level'),

    Status: Joi.string()
      .max(100)
      .required()
      .description('Status'),

    Days: Joi.number()
      .integer()
      .min(0)
      .required()
      .description('Days'),

    PercentCompleted: Joi.number()
      .min(0)
      .max(100)
      .required()
      .description('Percent completed')
  }).required()
});

/**
 * Validate a milestone event
 */
export function validateMilestoneEvent(event: unknown): Joi.ValidationResult<MilestoneEvent> {
  return milestoneEventSchema.validate(event, { abortEarly: false });
}

/**
 * Check if an error is a critical validation error that should not be retried
 */
export function isCriticalValidationError(error: Joi.ValidationError): boolean {
  return error.details.some(detail => 
    detail.type === 'any.required' ||
    detail.type === 'string.guid' ||
    detail.type === 'string.email'
  );
}
