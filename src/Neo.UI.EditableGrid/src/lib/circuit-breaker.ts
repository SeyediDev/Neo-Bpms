/**
 * Circuit Breaker Pattern for Resilient API Calls
 * Prevents cascading failures by opening circuit after threshold failures
 */

export interface CircuitBreakerConfig {
  failureThreshold: number; // Number of failures before opening
  resetTimeout: number; // Milliseconds before attempting to close
  monitoringWindow: number; // Time window for tracking failures
}

export type CircuitState = 'closed' | 'open' | 'half-open';

export class CircuitBreaker {
  private state: CircuitState = 'closed';
  private failures: number[] = [];
  private config: CircuitBreakerConfig;
  private lastFailureTime: number = 0;

  constructor(config: CircuitBreakerConfig) {
    this.config = config;
  }

  /**
   * Execute a function with circuit breaker protection
   */
  async execute<T>(fn: () => Promise<T>): Promise<T> {
    // Check circuit state
    if (this.state === 'open') {
      const timeSinceLastFailure = Date.now() - this.lastFailureTime;
      if (timeSinceLastFailure >= this.config.resetTimeout) {
        // Try to close circuit (half-open state)
        this.state = 'half-open';
      } else {
        throw new Error('Circuit breaker is open');
      }
    }

    try {
      const result = await fn();
      this.onSuccess();
      return result;
    } catch (error) {
      this.onFailure();
      throw error;
    }
  }

  /**
   * Handle successful execution
   */
  private onSuccess() {
    if (this.state === 'half-open') {
      // Close circuit on success
      this.state = 'closed';
      this.failures = [];
    }
  }

  /**
   * Handle failed execution
   */
  private onFailure() {
    this.lastFailureTime = Date.now();
    this.failures.push(Date.now());

    // Remove old failures outside monitoring window
    const cutoff = Date.now() - this.config.monitoringWindow;
    this.failures = this.failures.filter(time => time > cutoff);

    // Check if threshold exceeded
    if (this.failures.length >= this.config.failureThreshold) {
      this.state = 'open';
    }
  }

  /**
   * Get current state
   */
  getState(): CircuitState {
    return this.state;
  }

  /**
   * Reset circuit breaker
   */
  reset() {
    this.state = 'closed';
    this.failures = [];
    this.lastFailureTime = 0;
  }
}

