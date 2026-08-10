import logger from '../config/logger.js';

interface RetryOptions {
  maxRetries?: number;
  delayMs?: number;
  backoffMultiplier?: number;
  operation?: string;
}

/**
 * Retry utility with exponential backoff
 */
class RetryHelper {
  /**
   * Execute a function with retry logic
   * @param fn - Async function to execute
   * @param options - Retry options
   * @returns Result of the function
   */
  async executeWithRetry<T>(
    fn: () => Promise<T>,
    options: RetryOptions = {}
  ): Promise<T> {
    const {
      maxRetries = parseInt(process.env.MAX_RETRIES || '3', 10),
      delayMs = parseInt(process.env.RETRY_DELAY_MS || '2000', 10),
      backoffMultiplier = 2,
      operation = 'operation'
    } = options;

    let lastError: Error = new Error('Unknown error');
    
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
        lastError = error as Error;
        
        if (attempt < maxRetries) {
          const delay = delayMs * Math.pow(backoffMultiplier, attempt);
          logger.warn(`${operation} failed, will retry`, {
            attempt: `${attempt + 1}/${maxRetries}`,
            error: lastError.message,
            nextRetryIn: `${delay}ms`
          });
          
          await this.sleep(delay);
        } else {
          logger.error(`${operation} failed after ${maxRetries} retries`, {
            error: lastError.message,
            stack: lastError.stack
          });
        }
      }
    }

    throw lastError;
  }

  /**
   * Sleep for a specified duration
   * @param ms - Milliseconds to sleep
   * @returns Promise that resolves after the specified time
   */
  sleep(ms: number): Promise<void> {
    return new Promise(resolve => setTimeout(resolve, ms));
  }

  /**
   * Execute with timeout
   * @param fn - Function to execute
   * @param timeoutMs - Timeout in milliseconds
   * @param operation - Operation name
   * @returns Promise with the result or timeout error
   */
  async executeWithTimeout<T>(
    fn: () => Promise<T>,
    timeoutMs: number,
    operation = 'operation'
  ): Promise<T> {
    return Promise.race([
      fn(),
      new Promise<T>((_, reject) =>
        setTimeout(() => reject(new Error(`${operation} timed out after ${timeoutMs}ms`)), timeoutMs)
      )
    ]);
  }
}

// Export singleton instance
const retryHelper = new RetryHelper();
export default retryHelper;
