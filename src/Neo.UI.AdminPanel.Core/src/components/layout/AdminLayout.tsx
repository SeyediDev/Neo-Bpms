import React, { useState, useCallback } from 'react';
import clsx from 'clsx';
import { Sidebar, MenuItem } from './Sidebar';
import { Header, Breadcrumb, Notification, UserInfo } from './Header';

export interface AdminLayoutProps {
  /** Main content */
  children: React.ReactNode;
  /** Menu items for sidebar */
  menuItems: MenuItem[];
  /** Current user info */
  user?: UserInfo;
  /** Breadcrumb navigation */
  breadcrumbs?: Breadcrumb[];
  /** Notifications list */
  notifications?: Notification[];
  /** Unread notifications count */
  unreadCount?: number;
  /** Logo component or image URL */
  logo?: React.ReactNode | string;
  /** Logo text */
  logoText?: string;
  /** Sidebar footer content */
  sidebarFooter?: React.ReactNode;
  /** Header custom actions */
  headerActions?: React.ReactNode;
  /** Current theme */
  theme?: 'light' | 'dark';
  /** Callback when theme changes */
  onThemeChange?: (theme: 'light' | 'dark') => void;
  /** Callback when user clicks logout */
  onLogout?: () => void;
  /** Callback when notification is clicked */
  onNotificationClick?: (notification: Notification) => void;
  /** Callback when "View All Notifications" is clicked */
  onViewAllNotifications?: () => void;
  /** Check if user has permission */
  hasPermission?: (permission: string) => boolean;
  /** Initial sidebar collapsed state */
  defaultCollapsed?: boolean;
  /** Custom class names */
  className?: string;
  sidebarClassName?: string;
  headerClassName?: string;
  contentClassName?: string;
}

/**
 * Main admin panel layout with sidebar and header
 */
export const AdminLayout: React.FC<AdminLayoutProps> = ({
  children,
  menuItems,
  user,
  breadcrumbs,
  notifications,
  unreadCount,
  logo,
  logoText,
  sidebarFooter,
  headerActions,
  theme = 'light',
  onThemeChange,
  onLogout,
  onNotificationClick,
  onViewAllNotifications,
  hasPermission,
  defaultCollapsed = false,
  className,
  sidebarClassName,
  headerClassName,
  contentClassName,
}) => {
  const [sidebarCollapsed, setSidebarCollapsed] = useState(defaultCollapsed);
  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false);

  const handleSidebarCollapse = useCallback((collapsed: boolean) => {
    setSidebarCollapsed(collapsed);
  }, []);

  const handleMenuToggle = useCallback(() => {
    // On mobile, toggle mobile sidebar
    // On desktop, toggle collapsed state
    if (window.innerWidth < 1024) {
      setMobileSidebarOpen(prev => !prev);
    } else {
      setSidebarCollapsed(prev => !prev);
    }
  }, []);

  return (
    <div
      className={clsx(
        'flex h-screen bg-gray-100 dark:bg-gray-950',
        theme === 'dark' && 'dark',
        className
      )}
    >
      {/* Mobile Overlay */}
      {mobileSidebarOpen && (
        <div
          className={clsx(
            'fixed inset-0 z-40 lg:hidden',
            'bg-black/50 dark:bg-black/70',
            'transition-opacity duration-300'
          )}
          onClick={() => setMobileSidebarOpen(false)}
        />
      )}

      {/* Sidebar */}
      <div
        className={clsx(
          'fixed lg:static inset-y-0 right-0 z-50 lg:z-auto',
          'transform lg:transform-none transition-transform duration-300',
          mobileSidebarOpen ? 'translate-x-0' : 'translate-x-full lg:translate-x-0'
        )}
      >
        <Sidebar
          items={menuItems}
          collapsed={sidebarCollapsed}
          onCollapsedChange={handleSidebarCollapse}
          logo={logo}
          logoText={logoText}
          footer={sidebarFooter}
          hasPermission={hasPermission}
          className={sidebarClassName}
        />
      </div>

      {/* Main Content Area */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Header */}
        <Header
          user={user}
          breadcrumbs={breadcrumbs}
          notifications={notifications}
          unreadCount={unreadCount}
          theme={theme}
          onThemeChange={onThemeChange}
          onLogout={onLogout}
          onMenuToggle={handleMenuToggle}
          onNotificationClick={onNotificationClick}
          onViewAllNotifications={onViewAllNotifications}
          actions={headerActions}
          className={headerClassName}
        />

        {/* Page Content */}
        <main
          className={clsx(
            'flex-1 overflow-auto p-6',
            contentClassName
          )}
        >
          {children}
        </main>
      </div>
    </div>
  );
};

export default AdminLayout;

