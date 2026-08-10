import Joi from 'joi';

/**
 * Valid milestone types from the .NET API
 */
const VALID_MILESTONE_TYPES = [
  'GnGDataReceived',
  'MDTConducted',
  'DWPSentDFS',
  'DWPReceivedDFS',
  'DWPSentCementing',
  'DWPReceivedCementing',
  'DesignInitiated',
  'ApprovalInitiated',
  'Level1Approval',
  'Level2Approval',
  'Level3Approval'
];

/**
 * Joi schema for milestone event validation
 */
export const milestoneEventSchema = Joi.object({
  eventId: Joi.string()
    .guid({ version: 'uuidv4' })
    .required()
    .description('Unique event identifier'),

  eventType: Joi.string()
    .valid('MilestoneUpdated')
    .required()
    .description('Type of event'),

  timestamp: Joi.string()
    .isoDate()
    .required()
    .description('ISO 8601 timestamp when event occurred'),

  data: Joi.object({
    designId: Joi.number()
      .integer()
      .positive()
      .required()
      .description('Design ID'),

    milestoneType: Joi.string()
      .valid(...VALID_MILESTONE_TYPES)
      .required()
      .description('Type of milestone'),

    workCentre: Joi.string()
      .max(200)
      .allow(null, '')
      .description('Work centre location'),

    userId: Joi.number()
      .integer()
      .positive()
      .required()
      .description('User ID who recorded the milestone'),

    recordedAt: Joi.string()
      .isoDate()
      .required()
      .description('ISO 8601 timestamp when milestone was recorded')
  }).required()
}).required();

/**
 * Validate a milestone event against the schema
 * @param {Object} event - The event to validate
 * @returns {Object} Validation result { error, value }
 */
export function validateMilestoneEvent(event) {
  return milestoneEventSchema.validate(event, {
    abortEarly: false, // Return all errors, not just the first one
    stripUnknown: true // Remove unknown properties
  });
}

/**
 * Check if validation error is critical (structural issues)
 * vs non-critical (could retry)
 * @param {Joi.ValidationError} error
 * @returns {boolean} True if error is critical
 */
export function isCriticalValidationError(error) {
  if (!error || !error.details) {
    return false;
  }

  // Critical errors that should not be retried
  const criticalTypes = ['object.base', 'any.required', 'string.guid'];
  
  return error.details.some(detail => 
    criticalTypes.includes(detail.type)
  );
}

export default {
  milestoneEventSchema,
  validateMilestoneEvent,
  isCriticalValidationError,
  VALID_MILESTONE_TYPES
};
