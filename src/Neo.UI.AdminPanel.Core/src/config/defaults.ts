/**
 * Default Configuration
 * Neo BPMS Admin Panel Core
 */

import type { AuthConfig, ApiClientConfig } from '../types';

/**
 * Default authentication configuration
 */
export const DEFAULT_AUTH_CONFIG: AuthConfig = {
  loginEndpoint: '/api/auth/login',
  logoutEndpoint: '/api/auth/logout',
  refreshEndpoint: '/api/auth/refresh',
  userEndpoint: '/api/auth/me',
  tokenKey: 'neo_access_token',
  refreshTokenKey: 'neo_refresh_token',
  sessionTimeout: 30 * 60 * 1000, // 30 minutes
  autoRefreshBuffer: 5 * 60 * 1000, // 5 minutes before expiry
  persistSession: true,
  redirectAfterLogin: '/',
  redirectAfterLogout: '/login',
  unauthorizedRedirect: '/login',
};

/**
 * Default API client configuration
 */
export const DEFAULT_API_CONFIG: ApiClientConfig = {
  baseURL: '',
  timeout: 30000, // 30 seconds
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    'X-Requested-With': 'XMLHttpRequest',
  },
  retryAttempts: 3,
  retryDelay: 1000,
};

/**
 * HTTP Status codes
 */
export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  UNPROCESSABLE_ENTITY: 422,
  TOO_MANY_REQUESTS: 429,
  INTERNAL_SERVER_ERROR: 500,
  BAD_GATEWAY: 502,
  SERVICE_UNAVAILABLE: 503,
  GATEWAY_TIMEOUT: 504,
} as const;

/**
 * Error codes
 */
export const ERROR_CODES = {
  NETWORK_ERROR: 'NETWORK_ERROR',
  TIMEOUT: 'TIMEOUT',
  UNAUTHORIZED: 'UNAUTHORIZED',
  FORBIDDEN: 'FORBIDDEN',
  NOT_FOUND: 'NOT_FOUND',
  VALIDATION_ERROR: 'VALIDATION_ERROR',
  SERVER_ERROR: 'SERVER_ERROR',
  UNKNOWN_ERROR: 'UNKNOWN_ERROR',
  TOKEN_EXPIRED: 'TOKEN_EXPIRED',
  INVALID_TOKEN: 'INVALID_TOKEN',
  SESSION_EXPIRED: 'SESSION_EXPIRED',
} as const;

/**
 * Storage keys
 */
export const STORAGE_KEYS = {
  ACCESS_TOKEN: 'neo_access_token',
  REFRESH_TOKEN: 'neo_refresh_token',
  USER: 'neo_user',
  THEME: 'neo_theme',
  LANGUAGE: 'neo_language',
  SIDEBAR_COLLAPSED: 'neo_sidebar_collapsed',
  TABLE_SETTINGS: 'neo_table_settings',
} as const;

/**
 * Event names for custom events
 */
export const EVENTS = {
  AUTH_STATE_CHANGED: 'neo:auth:state_changed',
  TOKEN_REFRESHED: 'neo:auth:token_refreshed',
  SESSION_EXPIRED: 'neo:auth:session_expired',
  UNAUTHORIZED: 'neo:auth:unauthorized',
  PERMISSION_DENIED: 'neo:auth:permission_denied',
  NETWORK_ERROR: 'neo:network:error',
  SERVER_ERROR: 'neo:server:error',
} as const;

/**
 * Query keys for React Query
 */
export const QUERY_KEYS = {
  USER: ['user'],
  USERS: ['users'],
  ROLES: ['roles'],
  PERMISSIONS: ['permissions'],
  SESSIONS: ['sessions'],
} as const;

/**
 * Default pagination settings
 */
export const PAGINATION_DEFAULTS = {
  PAGE: 1,
  PAGE_SIZE: 20,
  PAGE_SIZE_OPTIONS: [10, 20, 50, 100],
  MAX_PAGE_SIZE: 100,
} as const;

