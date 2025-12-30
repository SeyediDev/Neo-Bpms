/**
 * Neo BPMS Admin Panel Core
 * 
 * Reusable infrastructure for Neo Admin Panel SPA
 */

// ==================== Components ====================
export * from './components';

// Layout components (re-exported for convenience)
export { 
  Sidebar, 
  Header, 
  AdminLayout 
} from './components/layout';

export type { 
  SidebarProps, 
  MenuItem,
  HeaderProps,
  Breadcrumb,
  Notification,
  UserInfo,
  AdminLayoutProps
} from './components/layout';

// ==================== Pages ====================
export * from './pages';

// ==================== Router ====================
export * from './router';

// ==================== Auth ====================
export { AuthProvider, useAuthContext } from './auth/AuthProvider';
export { useAuth } from './auth/useAuth';
export { ProtectedRoute } from './auth/ProtectedRoute';

// ==================== API ====================
export { apiClient, ApiClient } from './api/apiClient';

// ==================== State ====================
export * from './state';

// ==================== Hooks ====================
export * from './hooks';

// ==================== Types ====================
export * from './types';

// ==================== Config ====================
export * from './config';

// ==================== Features ====================
export * from './features';
