/**
 * Queue Manager for Optimized Client-Server Communication
 * Manages change queue, batching, and retry logic
 */

export interface QueuedChange {
  id: string;
  rowId: string | number;
  columnId: string;
  value: any;
  timestamp: number;
  retryCount: number;
  status: 'pending' | 'saving' | 'saved' | 'error';
  isNew?: boolean; // Indicates if this is a new row
}

export interface QueueConfig {
  batchSize: number;
  batchDelay: number; // milliseconds
  maxRetries: number;
  retryDelay: number; // milliseconds
}

export class QueueManager {
  private queue: QueuedChange[] = [];
  private processing = false;
  private config: QueueConfig;
  private batchTimer: NodeJS.Timeout | null = null;
  private onBatchReady: (changes: QueuedChange[]) => Promise<void>;
  private onStatusChange: (change: QueuedChange) => void;

  constructor(
    config: QueueConfig,
    onBatchReady: (changes: QueuedChange[]) => Promise<void>,
    onStatusChange: (change: QueuedChange) => void
  ) {
    this.config = config;
    this.onBatchReady = onBatchReady;
    this.onStatusChange = onStatusChange;
  }

  /**
   * Add a change to the queue
   */
  enqueue(change: Omit<QueuedChange, 'id' | 'timestamp' | 'retryCount' | 'status'>): string {
    const id = `${change.rowId}_${change.columnId}_${Date.now()}`;
    
    // Remove any existing pending change for the same cell
    this.queue = this.queue.filter(
      c => !(c.rowId === change.rowId && c.columnId === change.columnId && c.status === 'pending')
    );

    const queuedChange: QueuedChange = {
      ...change,
      id,
      timestamp: Date.now(),
      retryCount: 0,
      status: 'pending',
    };

    this.queue.push(queuedChange);
    this.onStatusChange(queuedChange);
    this.scheduleBatch();

    return id;
  }

  /**
   * Schedule batch processing
   */
  private scheduleBatch() {
    if (this.processing) return;

    if (this.batchTimer) {
      clearTimeout(this.batchTimer);
    }

    // If queue is full, process immediately
    if (this.queue.filter(c => c.status === 'pending').length >= this.config.batchSize) {
      this.processBatch();
      return;
    }

    // Otherwise, wait for batch delay
    this.batchTimer = setTimeout(() => {
      this.processBatch();
    }, this.config.batchDelay);
  }

  /**
   * Process a batch of changes
   */
  private async processBatch() {
    if (this.processing) return;

    const pendingChanges = this.queue.filter(c => c.status === 'pending');
    if (pendingChanges.length === 0) return;

    this.processing = true;

    // Take up to batchSize changes
    const batch = pendingChanges.slice(0, this.config.batchSize);
    
    // Mark as saving
    batch.forEach(change => {
      change.status = 'saving';
      this.onStatusChange(change);
    });

    try {
      await this.onBatchReady(batch);
      
      // Mark as saved
      batch.forEach(change => {
        change.status = 'saved';
        this.onStatusChange(change);
        
        // Remove from queue after a delay (for visual feedback)
        setTimeout(() => {
          this.queue = this.queue.filter(c => c.id !== change.id);
        }, 1000);
      });
    } catch (error) {
      // Mark as error and retry
      batch.forEach(change => {
        change.retryCount++;
        if (change.retryCount <= this.config.maxRetries) {
          change.status = 'pending';
          // Exponential backoff
          setTimeout(() => {
            this.scheduleBatch();
          }, this.config.retryDelay * Math.pow(2, change.retryCount - 1));
        } else {
          change.status = 'error';
        }
        this.onStatusChange(change);
      });
    } finally {
      this.processing = false;
      
      // Process next batch if queue has more items
      if (this.queue.filter(c => c.status === 'pending').length > 0) {
        this.scheduleBatch();
      }
    }
  }

  /**
   * Get queue status
   */
  getStatus() {
    return {
      total: this.queue.length,
      pending: this.queue.filter(c => c.status === 'pending').length,
      saving: this.queue.filter(c => c.status === 'saving').length,
      saved: this.queue.filter(c => c.status === 'saved').length,
      error: this.queue.filter(c => c.status === 'error').length,
    };
  }

  /**
   * Clear queue
   */
  clear() {
    this.queue = [];
    if (this.batchTimer) {
      clearTimeout(this.batchTimer);
      this.batchTimer = null;
    }
  }
}

