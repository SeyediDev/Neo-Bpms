/**
 * Authentication Hook
 * Neo BPMS Admin Panel Core
 * 
 * Custom hook for accessing authentication state and actions.
 */

import { useCallback, useMemo } from 'react';
import { useAuthContext } from './AuthProvider';
import { 
  useAppSelector, 
  selectUser, 
  selectIsAuthenticated,
  selectAuthIsLoading,
  selectAuthError,
  selectAccessToken,
  selectExpiresAt,
} from '../state';
import type { User, Permission } from '../types';

/**
 * Hook return type
 */
export interface UseAuthReturn {
  // State
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  accessToken: string | null;
  expiresAt: number | null;
  
  // Actions
  login: ReturnType<typeof useAuthContext>['login'];
  logout: ReturnType<typeof useAuthContext>['logout'];
  refreshToken: ReturnType<typeof useAuthContext>['refreshToken'];
  updateUser: ReturnType<typeof useAuthContext>['updateUser'];
  
  // Permission helpers
  hasPermission: (permission: string | string[]) => boolean;
  hasRole: (role: string | string[]) => boolean;
  hasAnyPermission: (permissions: string[]) => boolean;
  hasAllPermissions: (permissions: string[]) => boolean;
  hasAnyRole: (roles: string[]) => boolean;
  hasAllRoles: (roles: string[]) => boolean;
  
  // User helpers
  isAdmin: boolean;
  displayName: string;
  permissions: Permission[];
  roles: string[];
}

/**
 * Main authentication hook
 */
export function useAuth(): UseAuthReturn {
  const context = useAuthContext();
  
  // Additional selectors for direct Redux access
  const user = useAppSelector(selectUser);
  const accessToken = useAppSelector(selectAccessToken);
  const expiresAt = useAppSelector(selectExpiresAt);

  /**
   * Check if user has any of the given permissions
   */
  const hasAnyPermission = useCallback((permissions: string[]): boolean => {
    return permissions.some(p => context.hasPermission(p));
  }, [context]);

  /**
   * Check if user has all of the given permissions
   */
  const hasAllPermissions = useCallback((permissions: string[]): boolean => {
    return permissions.every(p => context.hasPermission(p));
  }, [context]);

  /**
   * Check if user has any of the given roles
   */
  const hasAnyRole = useCallback((roles: string[]): boolean => {
    return roles.some(r => context.hasRole(r));
  }, [context]);

  /**
   * Check if user has all of the given roles
   */
  const hasAllRoles = useCallback((roles: string[]): boolean => {
    return roles.every(r => context.hasRole(r));
  }, [context]);

  /**
   * Memoized values
   */
  const memoizedValues = useMemo(() => ({
    isAdmin: user?.isAdmin ?? false,
    displayName: user?.displayName ?? user?.username ?? '',
    permissions: user?.permissions ?? [],
    roles: user?.roles ?? [],
  }), [user]);

  return {
    // State from context
    user: context.user,
    isAuthenticated: context.isAuthenticated,
    isLoading: context.isLoading,
    error: context.error,
    accessToken,
    expiresAt,
    
    // Actions from context
    login: context.login,
    logout: context.logout,
    refreshToken: context.refreshToken,
    updateUser: context.updateUser,
    
    // Permission helpers
    hasPermission: context.hasPermission,
    hasRole: context.hasRole,
    hasAnyPermission,
    hasAllPermissions,
    hasAnyRole,
    hasAllRoles,
    
    // User helpers
    ...memoizedValues,
  };
}

/**
 * Hook to check authentication status only
 */
export function useIsAuthenticated(): boolean {
  return useAppSelector(selectIsAuthenticated);
}

/**
 * Hook to get current user only
 */
export function useCurrentUser(): User | null {
  return useAppSelector(selectUser);
}

/**
 * Hook to check loading state only
 */
export function useAuthLoading(): boolean {
  return useAppSelector(selectAuthIsLoading);
}

/**
 * Hook to get auth error only
 */
export function useAuthError(): string | null {
  return useAppSelector(selectAuthError);
}

/**
 * Hook to check specific permission
 */
export function usePermission(permission: string | string[]): boolean {
  const { hasPermission } = useAuth();
  return hasPermission(permission);
}

/**
 * Hook to check specific role
 */
export function useRole(role: string | string[]): boolean {
  const { hasRole } = useAuth();
  return hasRole(role);
}

/**
 * Hook to require authentication (throws if not authenticated)
 */
export function useRequireAuth(): UseAuthReturn {
  const auth = useAuth();
  
  if (!auth.isAuthenticated && !auth.isLoading) {
    throw new Error('User is not authenticated');
  }
  
  return auth;
}

export default useAuth;

