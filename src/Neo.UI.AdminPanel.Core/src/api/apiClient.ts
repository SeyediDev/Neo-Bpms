/**
 * API Client
 * Neo BPMS Admin Panel Core
 * 
 * Centralized HTTP client with interceptors for authentication,
 * error handling, request/response transformation.
 */

import axios, { 
  AxiosInstance, 
  AxiosRequestConfig, 
  AxiosResponse, 
  AxiosError,
  InternalAxiosRequestConfig,
} from 'axios';
import type { ApiResponse, ApiClientConfig, ApiError } from '../types';
import { 
  DEFAULT_API_CONFIG, 
  HTTP_STATUS, 
  ERROR_CODES, 
  STORAGE_KEYS,
  EVENTS,
} from '../config/defaults';

/**
 * Create configured Axios instance
 */
function createAxiosInstance(config: Partial<ApiClientConfig> = {}): AxiosInstance {
  const mergedConfig = { ...DEFAULT_API_CONFIG, ...config };
  
  return axios.create({
    baseURL: mergedConfig.baseURL,
    timeout: mergedConfig.timeout,
    withCredentials: mergedConfig.withCredentials,
    headers: mergedConfig.headers,
  });
}

/**
 * API Client class with advanced features
 */
class ApiClient {
  private instance: AxiosInstance;
  private config: ApiClientConfig;
  private isRefreshing = false;
  private refreshSubscribers: Array<(token: string) => void> = [];

  constructor(config: Partial<ApiClientConfig> = {}) {
    this.config = { ...DEFAULT_API_CONFIG, ...config };
    this.instance = createAxiosInstance(config);
    this.setupInterceptors();
  }

  /**
   * Get the underlying Axios instance
   */
  getAxiosInstance(): AxiosInstance {
    return this.instance;
  }

  /**
   * Setup request and response interceptors
   */
  private setupInterceptors(): void {
    // Request interceptor
    this.instance.interceptors.request.use(
      this.handleRequest.bind(this),
      this.handleRequestError.bind(this)
    );

    // Response interceptor
    this.instance.interceptors.response.use(
      this.handleResponse.bind(this),
      this.handleResponseError.bind(this)
    );
  }

  /**
   * Handle outgoing requests
   */
  private handleRequest(config: InternalAxiosRequestConfig): InternalAxiosRequestConfig {
    // Add auth token if available
    const token = this.getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    // Add request ID for tracking
    config.headers['X-Request-ID'] = this.generateRequestId();

    // Add timestamp
    config.headers['X-Request-Time'] = new Date().toISOString();

    // Log in development
    if (process.env.NODE_ENV === 'development') {
      console.log(`[API] ${config.method?.toUpperCase()} ${config.url}`, {
        params: config.params,
        data: config.data,
      });
    }

    return config;
  }

  /**
   * Handle request errors
   */
  private handleRequestError(error: Error): Promise<never> {
    console.error('[API] Request error:', error);
    return Promise.reject(error);
  }

  /**
   * Handle incoming responses
   */
  private handleResponse(response: AxiosResponse): AxiosResponse {
    // Log in development
    if (process.env.NODE_ENV === 'development') {
      console.log(`[API] Response ${response.status}:`, response.data);
    }

    return response;
  }

  /**
   * Handle response errors
   */
  private async handleResponseError(error: AxiosError): Promise<never> {
    const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

    // Handle network errors
    if (!error.response) {
      this.config.onNetworkError?.(error);
      this.dispatchEvent(EVENTS.NETWORK_ERROR, error);
      return Promise.reject(this.createApiError(ERROR_CODES.NETWORK_ERROR, 'Network error'));
    }

    const { status, data } = error.response;

    // Handle 401 Unauthorized - try to refresh token
    if (status === HTTP_STATUS.UNAUTHORIZED && !originalRequest._retry) {
      if (this.isRefreshing) {
        // Wait for refresh to complete
        return new Promise((resolve) => {
          this.refreshSubscribers.push((token: string) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            resolve(this.instance(originalRequest) as never);
          });
        });
      }

      originalRequest._retry = true;
      this.isRefreshing = true;

      try {
        const newToken = await this.refreshToken();
        if (newToken) {
          this.notifyRefreshSubscribers(newToken);
          originalRequest.headers.Authorization = `Bearer ${newToken}`;
          return this.instance(originalRequest) as never;
        }
      } catch (refreshError) {
        // Refresh failed - clear auth and redirect
        this.clearAuth();
        this.config.onUnauthorized?.();
        this.dispatchEvent(EVENTS.UNAUTHORIZED);
        return Promise.reject(refreshError);
      } finally {
        this.isRefreshing = false;
        this.refreshSubscribers = [];
      }
    }

    // Handle 403 Forbidden
    if (status === HTTP_STATUS.FORBIDDEN) {
      this.dispatchEvent(EVENTS.PERMISSION_DENIED);
    }

    // Handle 5xx Server errors
    if (status >= 500) {
      const serverError = this.extractApiError(data);
      this.config.onServerError?.(serverError);
      this.dispatchEvent(EVENTS.SERVER_ERROR, serverError);
    }

    // Log error in development
    if (process.env.NODE_ENV === 'development') {
      console.error(`[API] Error ${status}:`, data);
    }

    return Promise.reject(this.extractApiError(data, status));
  }

  /**
   * Refresh the access token
   */
  private async refreshToken(): Promise<string | null> {
    const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
    if (!refreshToken) return null;

    try {
      const response = await axios.post(
        `${this.config.baseURL}/api/auth/refresh`,
        { refreshToken },
        { withCredentials: true }
      );

      const { accessToken, refreshToken: newRefreshToken } = response.data;
      
      localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, accessToken);
      if (newRefreshToken) {
        localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, newRefreshToken);
      }

      this.dispatchEvent(EVENTS.TOKEN_REFRESHED, { accessToken });
      
      return accessToken;
    } catch {
      return null;
    }
  }

  /**
   * Notify all subscribers waiting for token refresh
   */
  private notifyRefreshSubscribers(token: string): void {
    this.refreshSubscribers.forEach(callback => callback(token));
  }

  /**
   * Get access token from storage
   */
  private getAccessToken(): string | null {
    return localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
  }

  /**
   * Clear authentication data
   */
  private clearAuth(): void {
    localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.USER);
  }

  /**
   * Generate unique request ID
   */
  private generateRequestId(): string {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Create standardized API error
   */
  private createApiError(code: string, message: string, field?: string): ApiError {
    return { code, message, field };
  }

  /**
   * Extract API error from response data
   */
  private extractApiError(data: unknown, status?: number): ApiError {
    if (typeof data === 'object' && data !== null) {
      const errorData = data as Record<string, unknown>;
      return {
        code: (errorData.code as string) || String(status) || ERROR_CODES.UNKNOWN_ERROR,
        message: (errorData.message as string) || 'An error occurred',
        field: errorData.field as string | undefined,
        details: errorData.details as Record<string, unknown> | undefined,
      };
    }
    return this.createApiError(ERROR_CODES.UNKNOWN_ERROR, String(data));
  }

  /**
   * Dispatch custom event
   */
  private dispatchEvent(eventName: string, detail?: unknown): void {
    window.dispatchEvent(new CustomEvent(eventName, { detail }));
  }

  // ==================== Public API Methods ====================

  /**
   * GET request
   */
  async get<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.instance.get<ApiResponse<T>>(url, config);
    return response.data;
  }

  /**
   * POST request
   */
  async post<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.instance.post<ApiResponse<T>>(url, data, config);
    return response.data;
  }

  /**
   * PUT request
   */
  async put<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.instance.put<ApiResponse<T>>(url, data, config);
    return response.data;
  }

  /**
   * PATCH request
   */
  async patch<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.instance.patch<ApiResponse<T>>(url, data, config);
    return response.data;
  }

  /**
   * DELETE request
   */
  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    const response = await this.instance.delete<ApiResponse<T>>(url, config);
    return response.data;
  }

  /**
   * Upload file
   */
  async uploadFile<T>(
    url: string, 
    file: File, 
    fieldName = 'file',
    additionalData?: Record<string, unknown>,
    onProgress?: (percent: number) => void
  ): Promise<ApiResponse<T>> {
    const formData = new FormData();
    formData.append(fieldName, file);
    
    if (additionalData) {
      Object.entries(additionalData).forEach(([key, value]) => {
        formData.append(key, String(value));
      });
    }

    const response = await this.instance.post<ApiResponse<T>>(url, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
      onUploadProgress: (progressEvent) => {
        if (progressEvent.total && onProgress) {
          const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          onProgress(percent);
        }
      },
    });

    return response.data;
  }

  /**
   * Download file
   */
  async downloadFile(url: string, filename?: string): Promise<void> {
    const response = await this.instance.get(url, {
      responseType: 'blob',
    });

    const blob = new Blob([response.data]);
    const downloadUrl = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = downloadUrl;
    link.download = filename || this.extractFilename(response) || 'download';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(downloadUrl);
  }

  /**
   * Extract filename from response headers
   */
  private extractFilename(response: AxiosResponse): string | null {
    const contentDisposition = response.headers['content-disposition'];
    if (contentDisposition) {
      const matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(contentDisposition);
      if (matches?.[1]) {
        return matches[1].replace(/['"]/g, '');
      }
    }
    return null;
  }

  /**
   * Set authorization token manually
   */
  setAuthToken(token: string): void {
    localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, token);
    this.instance.defaults.headers.common.Authorization = `Bearer ${token}`;
  }

  /**
   * Remove authorization token
   */
  removeAuthToken(): void {
    this.clearAuth();
    delete this.instance.defaults.headers.common.Authorization;
  }

  /**
   * Update base URL
   */
  setBaseURL(baseURL: string): void {
    this.config.baseURL = baseURL;
    this.instance.defaults.baseURL = baseURL;
  }
}

// Export singleton instance
export const apiClient = new ApiClient();

// Export class for custom instances
export { ApiClient };

export default apiClient;

