/**
 * Protected Route Component
 * Neo BPMS Admin Panel Core
 * 
 * Guards routes that require authentication or specific permissions/roles.
 */

import React, { type ReactNode, useMemo } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './useAuth';
import type { ProtectedRouteProps } from '../types';

/**
 * Default loading component
 */
function DefaultLoadingFallback() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-100 dark:bg-slate-900">
      <div className="flex flex-col items-center gap-4">
        <div className="w-12 h-12 border-4 border-primary-500 border-t-transparent rounded-full animate-spin" />
        <p className="text-slate-600 dark:text-slate-400">در حال بارگذاری...</p>
      </div>
    </div>
  );
}

/**
 * Default unauthorized component
 */
function DefaultUnauthorizedFallback() {
  return (
    <div className="flex items-center justify-center min-h-screen bg-slate-100 dark:bg-slate-900">
      <div className="text-center p-8 bg-white dark:bg-slate-800 rounded-xl shadow-lg max-w-md">
        <div className="w-16 h-16 mx-auto mb-4 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center">
          <svg className="w-8 h-8 text-red-600 dark:text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <h2 className="text-xl font-bold text-slate-800 dark:text-white mb-2">
          دسترسی غیرمجاز
        </h2>
        <p className="text-slate-600 dark:text-slate-400 mb-4">
          شما مجوز دسترسی به این صفحه را ندارید.
        </p>
        <a
          href="/"
          className="inline-block px-4 py-2 bg-primary-500 text-white rounded-lg hover:bg-primary-600 transition-colors"
        >
          بازگشت به صفحه اصلی
        </a>
      </div>
    </div>
  );
}

/**
 * Protected Route Component
 */
export function ProtectedRoute({
  children,
  requiredPermissions = [],
  requiredRoles = [],
  requireAll = false,
  fallback,
  redirectTo = '/login',
}: ProtectedRouteProps) {
  const { 
    isAuthenticated, 
    isLoading, 
    hasAllPermissions,
    hasAnyPermission,
    hasAllRoles,
    hasAnyRole,
  } = useAuth();
  const location = useLocation();

  /**
   * Check if user has required authorization
   */
  const hasAuthorization = useMemo(() => {
    // No specific requirements - just need authentication
    if (requiredPermissions.length === 0 && requiredRoles.length === 0) {
      return true;
    }

    // Check permissions
    const hasRequiredPermissions = requiredPermissions.length === 0 ||
      (requireAll 
        ? hasAllPermissions(requiredPermissions) 
        : hasAnyPermission(requiredPermissions));

    // Check roles
    const hasRequiredRoles = requiredRoles.length === 0 ||
      (requireAll 
        ? hasAllRoles(requiredRoles) 
        : hasAnyRole(requiredRoles));

    // Both must pass if both are specified
    if (requiredPermissions.length > 0 && requiredRoles.length > 0) {
      return requireAll 
        ? hasRequiredPermissions && hasRequiredRoles
        : hasRequiredPermissions || hasRequiredRoles;
    }

    return hasRequiredPermissions && hasRequiredRoles;
  }, [
    requiredPermissions, 
    requiredRoles, 
    requireAll, 
    hasAllPermissions, 
    hasAnyPermission,
    hasAllRoles,
    hasAnyRole,
  ]);

  // Show loading while checking authentication
  if (isLoading) {
    return fallback ? <>{fallback}</> : <DefaultLoadingFallback />;
  }

  // Redirect to login if not authenticated
  if (!isAuthenticated) {
    return (
      <Navigate 
        to={redirectTo} 
        state={{ from: location.pathname + location.search }} 
        replace 
      />
    );
  }

  // Show unauthorized if doesn't have required permissions/roles
  if (!hasAuthorization) {
    return fallback ? <>{fallback}</> : <DefaultUnauthorizedFallback />;
  }

  // Render children if all checks pass
  return <>{children}</>;
}

/**
 * HOC version of ProtectedRoute
 */
export function withProtectedRoute<P extends object>(
  WrappedComponent: React.ComponentType<P>,
  options: Omit<ProtectedRouteProps, 'children'> = {}
) {
  return function ProtectedComponent(props: P) {
    return (
      <ProtectedRoute {...options}>
        <WrappedComponent {...props} />
      </ProtectedRoute>
    );
  };
}

/**
 * Component to show content only if user has permission
 */
export function RequirePermission({
  permission,
  children,
  fallback = null,
}: {
  permission: string | string[];
  children: ReactNode;
  fallback?: ReactNode;
}) {
  const { hasPermission, isAuthenticated } = useAuth();

  if (!isAuthenticated) return <>{fallback}</>;
  
  const permissions = Array.isArray(permission) ? permission : [permission];
  const hasAny = permissions.some(p => hasPermission(p));

  return hasAny ? <>{children}</> : <>{fallback}</>;
}

/**
 * Component to show content only if user has role
 */
export function RequireRole({
  role,
  children,
  fallback = null,
}: {
  role: string | string[];
  children: ReactNode;
  fallback?: ReactNode;
}) {
  const { hasRole, isAuthenticated } = useAuth();

  if (!isAuthenticated) return <>{fallback}</>;
  
  const roles = Array.isArray(role) ? role : [role];
  const hasAny = roles.some(r => hasRole(r));

  return hasAny ? <>{children}</> : <>{fallback}</>;
}

/**
 * Component to show content only if user is admin
 */
export function RequireAdmin({
  children,
  fallback = null,
}: {
  children: ReactNode;
  fallback?: ReactNode;
}) {
  const { isAdmin, isAuthenticated } = useAuth();

  if (!isAuthenticated || !isAdmin) return <>{fallback}</>;

  return <>{children}</>;
}

export default ProtectedRoute;

