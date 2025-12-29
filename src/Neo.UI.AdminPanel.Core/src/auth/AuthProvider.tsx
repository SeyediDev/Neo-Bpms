/**
 * Authentication Provider
 * Neo BPMS Admin Panel Core
 * 
 * Provides authentication context to the entire application.
 * Handles automatic token refresh, session management, and auth state.
 */

import { 
  createContext, 
  useContext, 
  useCallback, 
  useEffect, 
  useRef,
  useMemo,
  type ReactNode 
} from 'react';
import { Provider as ReduxProvider } from 'react-redux';
import { 
  store, 
  useAppDispatch, 
  useAppSelector,
  login as loginAction,
  logout as logoutAction,
  refreshAccessToken,
  selectUser,
  selectIsAuthenticated,
  selectAuthIsLoading,
  selectAuthError,
  selectAccessToken,
  selectExpiresAt,
  updateUser as updateUserAction,
  updateLastActivity,
  clearAuth,
} from '../state';
import type { 
  AuthContextValue, 
  LoginRequest, 
  LoginResponse, 
  User,
  AuthConfig 
} from '../types';
import { DEFAULT_AUTH_CONFIG, EVENTS } from '../config/defaults';
import { apiClient } from '../api/apiClient';

/**
 * Auth Context
 */
const AuthContext = createContext<AuthContextValue | null>(null);

/**
 * Auth Provider Props
 */
interface AuthProviderProps {
  children: ReactNode;
  config?: Partial<AuthConfig>;
  onAuthStateChange?: (isAuthenticated: boolean, user: User | null) => void;
  onSessionExpired?: () => void;
  onUnauthorized?: () => void;
}

/**
 * Internal Auth Provider Component
 */
function AuthProviderInternal({ 
  children, 
  config,
  onAuthStateChange,
  onSessionExpired,
  onUnauthorized,
}: AuthProviderProps) {
  const dispatch = useAppDispatch();
  const user = useAppSelector(selectUser);
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const isLoading = useAppSelector(selectAuthIsLoading);
  const error = useAppSelector(selectAuthError);
  const accessToken = useAppSelector(selectAccessToken);
  const expiresAt = useAppSelector(selectExpiresAt);

  const authConfig = useMemo(() => ({ ...DEFAULT_AUTH_CONFIG, ...config }), [config]);
  const refreshTimerRef = useRef<NodeJS.Timeout | null>(null);
  const activityTimerRef = useRef<NodeJS.Timeout | null>(null);
  const prevIsAuthenticatedRef = useRef(isAuthenticated);

  /**
   * Login function
   */
  const login = useCallback(async (credentials: LoginRequest): Promise<LoginResponse> => {
    const result = await dispatch(loginAction({ credentials, apiClient }));
    if (loginAction.fulfilled.match(result)) {
      return result.payload;
    }
    throw new Error(result.payload || 'Login failed');
  }, [dispatch]);

  /**
   * Logout function
   */
  const logout = useCallback(async (): Promise<void> => {
    // Clear refresh timer
    if (refreshTimerRef.current) {
      clearTimeout(refreshTimerRef.current);
      refreshTimerRef.current = null;
    }
    
    await dispatch(logoutAction(undefined));
  }, [dispatch]);

  /**
   * Refresh token function
   */
  const refreshToken = useCallback(async (): Promise<void> => {
    const result = await dispatch(refreshAccessToken({ apiClient }));
    if (refreshAccessToken.rejected.match(result)) {
      // Token refresh failed, handle session expiry
      onSessionExpired?.();
    }
  }, [dispatch, onSessionExpired]);

  /**
   * Check if user has permission
   */
  const hasPermission = useCallback((permission: string | string[]): boolean => {
    if (!user) return false;
    if (user.isAdmin) return true;
    
    const permissions = Array.isArray(permission) ? permission : [permission];
    return permissions.some(p => 
      user.permissions.some(up => up.id === p || up.name === p)
    );
  }, [user]);

  /**
   * Check if user has role
   */
  const hasRole = useCallback((role: string | string[]): boolean => {
    if (!user) return false;
    if (user.isAdmin) return true;
    
    const roles = Array.isArray(role) ? role : [role];
    return roles.some(r => user.roles.includes(r));
  }, [user]);

  /**
   * Update user data
   */
  const updateUser = useCallback((userData: Partial<User>): void => {
    dispatch(updateUserAction(userData));
  }, [dispatch]);

  /**
   * Schedule token refresh
   */
  const scheduleTokenRefresh = useCallback(() => {
    if (!expiresAt || !accessToken) return;

    // Clear existing timer
    if (refreshTimerRef.current) {
      clearTimeout(refreshTimerRef.current);
    }

    // Calculate when to refresh (5 minutes before expiry)
    const refreshTime = expiresAt - authConfig.autoRefreshBuffer;
    const delay = Math.max(0, refreshTime - Date.now());

    if (delay > 0) {
      refreshTimerRef.current = setTimeout(() => {
        refreshToken();
      }, delay);
    } else {
      // Token already needs refresh
      refreshToken();
    }
  }, [expiresAt, accessToken, authConfig.autoRefreshBuffer, refreshToken]);

  /**
   * Track user activity for session timeout
   */
  const trackActivity = useCallback(() => {
    dispatch(updateLastActivity());

    // Reset activity timer
    if (activityTimerRef.current) {
      clearTimeout(activityTimerRef.current);
    }

    if (isAuthenticated && authConfig.sessionTimeout > 0) {
      activityTimerRef.current = setTimeout(() => {
        // Session timeout - logout user
        dispatch(clearAuth());
        onSessionExpired?.();
      }, authConfig.sessionTimeout);
    }
  }, [dispatch, isAuthenticated, authConfig.sessionTimeout, onSessionExpired]);

  /**
   * Setup token refresh scheduling
   */
  useEffect(() => {
    if (isAuthenticated && accessToken) {
      scheduleTokenRefresh();
    }

    return () => {
      if (refreshTimerRef.current) {
        clearTimeout(refreshTimerRef.current);
      }
    };
  }, [isAuthenticated, accessToken, scheduleTokenRefresh]);

  /**
   * Setup activity tracking
   */
  useEffect(() => {
    if (isAuthenticated) {
      // Track various user activities
      const events = ['mousedown', 'keydown', 'scroll', 'touchstart'];
      
      events.forEach(event => {
        window.addEventListener(event, trackActivity, { passive: true });
      });

      // Initial activity tracking
      trackActivity();

      return () => {
        events.forEach(event => {
          window.removeEventListener(event, trackActivity);
        });
        
        if (activityTimerRef.current) {
          clearTimeout(activityTimerRef.current);
        }
      };
    }
  }, [isAuthenticated, trackActivity]);

  /**
   * Notify on auth state change
   */
  useEffect(() => {
    if (prevIsAuthenticatedRef.current !== isAuthenticated) {
      prevIsAuthenticatedRef.current = isAuthenticated;
      onAuthStateChange?.(isAuthenticated, user);
    }
  }, [isAuthenticated, user, onAuthStateChange]);

  /**
   * Listen for custom auth events
   */
  useEffect(() => {
    const handleUnauthorized = () => {
      onUnauthorized?.();
    };

    const handleSessionExpired = () => {
      onSessionExpired?.();
    };

    window.addEventListener(EVENTS.UNAUTHORIZED, handleUnauthorized);
    window.addEventListener(EVENTS.SESSION_EXPIRED, handleSessionExpired);

    return () => {
      window.removeEventListener(EVENTS.UNAUTHORIZED, handleUnauthorized);
      window.removeEventListener(EVENTS.SESSION_EXPIRED, handleSessionExpired);
    };
  }, [onUnauthorized, onSessionExpired]);

  /**
   * Context value
   */
  const contextValue = useMemo<AuthContextValue>(() => ({
    user,
    isAuthenticated,
    isLoading,
    error,
    login,
    logout,
    refreshToken,
    hasPermission,
    hasRole,
    updateUser,
  }), [
    user,
    isAuthenticated,
    isLoading,
    error,
    login,
    logout,
    refreshToken,
    hasPermission,
    hasRole,
    updateUser,
  ]);

  return (
    <AuthContext.Provider value={contextValue}>
      {children}
    </AuthContext.Provider>
  );
}

/**
 * Auth Provider with Redux Provider
 */
export function AuthProvider(props: AuthProviderProps) {
  return (
    <ReduxProvider store={store}>
      <AuthProviderInternal {...props} />
    </ReduxProvider>
  );
}

/**
 * Hook to access auth context
 */
export function useAuthContext(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuthContext must be used within an AuthProvider');
  }
  return context;
}

export default AuthProvider;

