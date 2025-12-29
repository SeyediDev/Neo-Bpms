/**
 * Authentication Module Index
 * Neo BPMS Admin Panel Core
 */

// Provider
export { AuthProvider, useAuthContext } from './AuthProvider';

// Hooks
export { 
  useAuth, 
  useIsAuthenticated,
  useCurrentUser,
  useAuthLoading,
  useAuthError,
  usePermission,
  useRole,
  useRequireAuth,
  type UseAuthReturn,
} from './useAuth';

// Components
export { 
  ProtectedRoute, 
  withProtectedRoute,
  RequirePermission,
  RequireRole,
  RequireAdmin,
} from './ProtectedRoute';

// Re-export types
export type {
  User,
  Permission,
  Role,
  LoginRequest,
  LoginResponse,
  AuthState,
  AuthConfig,
  AuthContextValue,
  ProtectedRouteProps,
  TokenPayload,
  SessionInfo,
  ChangePasswordRequest,
} from '../types';

