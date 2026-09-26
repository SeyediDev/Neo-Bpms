/**
 * App Router Component
 * Neo BPMS Admin Panel Core
 * 
 * Main router setup with authentication and layouts.
 */

import { Suspense, lazy, type ReactNode } from 'react';
import { BrowserRouter, Routes, Route, Outlet } from 'react-router-dom';
import { AuthProvider } from '../auth/AuthProvider';
import { ProtectedRoute } from '../auth/ProtectedRoute';
import { AdminLayout } from '../components/layout/AdminLayout';
import type { MenuItem, AuthConfig } from '../types';

// ==================== Types ====================

export interface RouteConfig {
  path: string;
  element: ReactNode;
  children?: RouteConfig[];
  permission?: string | string[];
  role?: string | string[];
  index?: boolean;
}

export interface AppRouterProps {
  /** Menu items for sidebar */
  menuItems?: MenuItem[];
  /** Custom routes */
  routes?: RouteConfig[];
  /** Login page component (defaults to built-in) */
  loginPage?: ReactNode;
  /** Dashboard page component */
  dashboardPage?: ReactNode;
  /** 404 page component (defaults to built-in) */
  notFoundPage?: ReactNode;
  /** Auth configuration */
  authConfig?: Partial<AuthConfig>;
  /** Base path for the app */
  basePath?: string;
  /** Logo component or URL */
  logo?: ReactNode | string;
  /** App title */
  appTitle?: string;
  /** Loading fallback */
  loadingFallback?: ReactNode;
  /** Callback when auth state changes */
  onAuthStateChange?: (isAuthenticated: boolean, user: unknown) => void;
  /** Callback when session expires */
  onSessionExpired?: () => void;
  /** Custom header actions */
  headerActions?: ReactNode;
}

// ==================== Lazy Loaded Pages ====================

const LazyLoginPage = lazy(() => import('../pages/LoginPage'));
const LazyNotFoundPage = lazy(() => import('../pages/NotFoundPage'));
const LazyDashboardPage = lazy(() => import('../pages/DashboardPage'));

// ==================== Loading Fallback ====================

function DefaultLoadingFallback() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-canvas dark:bg-surface">
      <div className="text-center">
        <div className="w-12 h-12 border-4 border-purple-500 border-t-transparent rounded-full animate-spin mx-auto mb-4" />
        <p className="text-muted dark:text-muted">در حال بارگذاری...</p>
      </div>
    </div>
  );
}

// ==================== Layout Wrapper ====================

interface LayoutWrapperProps {
  menuItems: MenuItem[];
  logo?: ReactNode | string;
  appTitle?: string;
  headerActions?: ReactNode;
}

function LayoutWrapper({ menuItems, logo, appTitle, headerActions }: LayoutWrapperProps) {
  return (
    <AdminLayout
      menuItems={menuItems}
      logo={logo}
      logoText={appTitle}
      headerActions={headerActions}
    >
      <Suspense fallback={<DefaultLoadingFallback />}>
        <Outlet />
      </Suspense>
    </AdminLayout>
  );
}

// ==================== Route Renderer ====================

function renderRoutes(routes: RouteConfig[]): ReactNode[] {
  return routes.map((route, index) => {
    const element = route.permission || route.role ? (
      <ProtectedRoute 
        requiredPermissions={route.permission ? (Array.isArray(route.permission) ? route.permission : [route.permission]) : undefined}
        requiredRoles={route.role ? (Array.isArray(route.role) ? route.role : [route.role]) : undefined}
      >
        {route.element}
      </ProtectedRoute>
    ) : (
      route.element
    );

    if (route.children) {
      return (
        <Route key={index} path={route.path} element={element}>
          {renderRoutes(route.children)}
        </Route>
      );
    }

    return route.index ? (
      <Route key={index} index element={element} />
    ) : (
      <Route key={index} path={route.path} element={element} />
    );
  });
}

// ==================== Main Component ====================

/**
 * App Router Component
 * 
 * Sets up routing with authentication, layouts, and protected routes.
 */
export function AppRouter({
  menuItems = [],
  routes = [],
  loginPage,
  dashboardPage,
  notFoundPage,
  authConfig,
  basePath = '/',
  logo,
  appTitle = 'پنل مدیریت',
  loadingFallback,
  onAuthStateChange,
  onSessionExpired,
  headerActions,
}: AppRouterProps) {
  const LoadingComponent = loadingFallback || <DefaultLoadingFallback />;

  return (
    <BrowserRouter basename={basePath}>
      <AuthProvider
        config={authConfig}
        onAuthStateChange={onAuthStateChange}
        onSessionExpired={onSessionExpired}
      >
        <Suspense fallback={LoadingComponent}>
          <Routes>
            {/* Public Routes */}
            <Route
              path="/login"
              element={loginPage || <LazyLoginPage logo={logo} title={appTitle} />}
            />

            {/* Protected Routes with Layout */}
            <Route
              element={
                <ProtectedRoute redirectTo="/login">
                  <LayoutWrapper
                    menuItems={menuItems}
                    logo={logo}
                    appTitle={appTitle}
                    headerActions={headerActions}
                  />
                </ProtectedRoute>
              }
            >
              {/* Dashboard (index) */}
              <Route
                index
                element={dashboardPage || <LazyDashboardPage />}
              />

              {/* Custom Routes */}
              {renderRoutes(routes)}
            </Route>

            {/* 404 Not Found */}
            <Route
              path="*"
              element={notFoundPage || <LazyNotFoundPage />}
            />
          </Routes>
        </Suspense>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default AppRouter;

