/**
 * API Client with Retry Logic and Circuit Breaker
 */

import axios, { AxiosInstance, AxiosError } from 'axios';
import { CircuitBreaker } from './circuit-breaker';
import { QueuedChange } from './queue-manager';

export interface GridData {
  columns: GridColumn[];
  rows: GridRow[];
  totalCount?: number;
}

export interface GridColumn {
  id: string;
  name: string;
  type: 'text' | 'number' | 'date' | 'boolean' | 'select' | 'multiselect';
  editable?: boolean;
  required?: boolean;
  options?: { value: any; label: string }[]; // For select/multiselect
  format?: string; // For date/number formatting
  width?: number;
}

export interface GridRow {
  id: string | number;
  [key: string]: any;
}

export interface BatchUpdateRequest {
  changes: Array<{
    rowId: string | number;
    columnId: string;
    value: any;
  }>;
}

export interface BatchUpdateResponse {
  success: boolean;
  updated: number;
  errors?: Array<{
    rowId: string | number;
    columnId: string;
    message: string;
  }>;
}

export class GridApiClient {
  private client: AxiosInstance;
  private circuitBreaker: CircuitBreaker;
  private baseUrl: string;

  constructor(baseUrl: string = '/api/grid') {
    this.baseUrl = baseUrl;
    this.client = axios.create({
      baseURL: baseUrl,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.circuitBreaker = new CircuitBreaker({
      failureThreshold: 5,
      resetTimeout: 30000, // 30 seconds
      monitoringWindow: 60000, // 1 minute
    });

    // Add request interceptor for auth token
    this.client.interceptors.request.use((config) => {
      const token = this.getAuthToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
  }

  /**
   * Get grid data
   */
  async getData(endpoint: string, params?: any): Promise<GridData> {
    return this.circuitBreaker.execute(async () => {
      const response = await this.client.get(endpoint, { params });
      return response.data;
    });
  }

  /**
   * Batch update cells
   */
  async batchUpdate(
    endpoint: string,
    changes: QueuedChange[]
  ): Promise<BatchUpdateResponse> {
    return this.circuitBreaker.execute(async () => {
      const request: BatchUpdateRequest = {
        changes: changes.map(c => ({
          rowId: c.rowId,
          columnId: c.columnId,
          value: c.value,
        })),
      };

      const response = await this.client.post(`${endpoint}/batch-update`, request);
      return response.data;
    });
  }

  /**
   * Get auth token from storage
   */
  private getAuthToken(): string | null {
    if (typeof window === 'undefined') return null;
    return localStorage.getItem('auth_token') || sessionStorage.getItem('auth_token');
  }

  /**
   * Get circuit breaker state
   */
  getCircuitState() {
    return this.circuitBreaker.getState();
  }
}

