import logger from '../config/logger.js';

/**
 * Retry utility with exponential backoff
 */
class RetryHelper {
  /**
   * Execute a function with retry logic
   * @param {Function} fn - Async function to execute
   * @param {Object} options - Retry options
   * @param {number} options.maxRetries - Maximum number of retries
   * @param {number} options.delayMs - Initial delay in milliseconds
   * @param {number} options.backoffMultiplier - Multiplier for exponential backoff
   * @param {string} options.operation - Operation name for logging
   * @returns {Promise<any>} Result of the function
   */
  async executeWithRetry(fn, options = {}) {
    const {
      maxRetries = parseInt(process.env.MAX_RETRIES || '3', 10),
      delayMs = parseInt(process.env.RETRY_DELAY_MS || '2000', 10),
      backoffMultiplier = 2,
      operation = 'operation'
    } = options;

    let lastError;
    
    for (let attempt = 0; attempt <= maxRetries; attempt++) {
      try {
        if (attempt > 0) {
          logger.info(`Retrying ${operation}`, {
            attempt: `${attempt}/${maxRetries}`,
            delay: `${delayMs * Math.pow(backoffMultiplier, attempt - 1)}ms`
          });
        }

        const result = await fn();
        
        if (attempt > 0) {
          logger.info(`${operation} succeeded after ${attempt} retries`);
        }
        
        return result;
      } catch (error) {
        lastError = error;
        
        if (attempt < maxRetries) {
          const delay = delayMs * Math.pow(backoffMultiplier, attempt);
          logger.warn(`${operation} failed, will retry`, {
            attempt: `${attempt + 1}/${maxRetries}`,
            error: error.message,
            nextRetryIn: `${delay}ms`
          });
          
          await this.sleep(delay);
        } else {
          logger.error(`${operation} failed after ${maxRetries} retries`, {
            error: error.message,
            stack: error.stack
          });
        }
      }
    }

    throw lastError;
  }

  /**
   * Sleep for a specified duration
   * @param {number} ms - Milliseconds to sleep
   * @returns {Promise<void>}
   */
  sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  /**
   * Execute with timeout
   * @param {Function} fn - Function to execute
   * @param {number} timeoutMs - Timeout in milliseconds
   * @param {string} operation - Operation name
   * @returns {Promise<any>}
   */
  async executeWithTimeout(fn, timeoutMs, operation = 'operation') {
    return Promise.race([
      fn(),
      new Promise((_, reject) =>
        setTimeout(() => reject(new Error(`${operation} timed out after ${timeoutMs}ms`)), timeoutMs)
      )
    ]);
  }
}

// Export singleton instance
const retryHelper = new RetryHelper();
export default retryHelper;
