import React, { useState, useRef, useEffect } from 'react';
import clsx from 'clsx';

export interface Breadcrumb {
  label: string;
  path?: string;
}

export interface Notification {
  id: string;
  title: string;
  message: string;
  time: string;
  read?: boolean;
  type?: 'info' | 'success' | 'warning' | 'error';
}

export interface UserInfo {
  name: string;
  email?: string;
  avatar?: string;
  role?: string;
}

export interface HeaderProps {
  /** Current user info */
  user?: UserInfo;
  /** Breadcrumb navigation */
  breadcrumbs?: Breadcrumb[];
  /** Notifications list */
  notifications?: Notification[];
  /** Unread notifications count */
  unreadCount?: number;
  /** Current theme */
  theme?: 'light' | 'dark';
  /** Callback when theme changes */
  onThemeChange?: (theme: 'light' | 'dark') => void;
  /** Callback when user clicks logout */
  onLogout?: () => void;
  /** Callback when sidebar toggle is clicked */
  onMenuToggle?: () => void;
  /** Callback when notification is clicked */
  onNotificationClick?: (notification: Notification) => void;
  /** Callback when "View All Notifications" is clicked */
  onViewAllNotifications?: () => void;
  /** Custom actions to render in header */
  actions?: React.ReactNode;
  /** Custom class name */
  className?: string;
  /** Show menu toggle button */
  showMenuToggle?: boolean;
}

/**
 * Header component with user menu, notifications, and theme toggle
 */
export const Header: React.FC<HeaderProps> = ({
  user,
  breadcrumbs = [],
  notifications = [],
  unreadCount = 0,
  theme = 'light',
  onThemeChange,
  onLogout,
  onMenuToggle,
  onNotificationClick,
  onViewAllNotifications,
  actions,
  className,
  showMenuToggle = true,
}) => {
  const [showUserMenu, setShowUserMenu] = useState(false);
  const [showNotifications, setShowNotifications] = useState(false);
  const userMenuRef = useRef<HTMLDivElement>(null);
  const notificationsRef = useRef<HTMLDivElement>(null);

  // Close dropdowns when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (userMenuRef.current && !userMenuRef.current.contains(event.target as Node)) {
        setShowUserMenu(false);
      }
      if (notificationsRef.current && !notificationsRef.current.contains(event.target as Node)) {
        setShowNotifications(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const getNotificationIcon = (type?: string) => {
    switch (type) {
      case 'success':
        return (
          <span className="w-8 h-8 rounded-full bg-green-100 flex items-center justify-center">
            <svg className="w-4 h-4 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </span>
        );
      case 'warning':
        return (
          <span className="w-8 h-8 rounded-full bg-yellow-100 flex items-center justify-center">
            <svg className="w-4 h-4 text-yellow-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          </span>
        );
      case 'error':
        return (
          <span className="w-8 h-8 rounded-full bg-red-100 flex items-center justify-center">
            <svg className="w-4 h-4 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </span>
        );
      default:
        return (
          <span className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center">
            <svg className="w-4 h-4 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </span>
        );
    }
  };

  return (
    <header
      className={clsx(
        'h-16 bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-700',
        'flex items-center justify-between px-4',
        className
      )}
    >
      {/* Left Section - Menu Toggle & Breadcrumbs */}
      <div className="flex items-center gap-4">
        {showMenuToggle && (
          <button
            onClick={onMenuToggle}
            className="p-2 rounded-lg bg-white dark:bg-gray-800 hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-500 dark:text-gray-400 transition-colors border border-gray-200 dark:border-gray-700"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
        )}

        {/* Breadcrumbs */}
        {breadcrumbs.length > 0 && (
          <nav className="flex items-center gap-2 text-sm">
            {breadcrumbs.map((crumb, index) => (
              <React.Fragment key={index}>
                {index > 0 && (
                  <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
                  </svg>
                )}
                {crumb.path ? (
                  <a
                    href={crumb.path}
                    className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
                  >
                    {crumb.label}
                  </a>
                ) : (
                  <span className="text-gray-900 dark:text-white font-medium">
                    {crumb.label}
                  </span>
                )}
              </React.Fragment>
            ))}
          </nav>
        )}
      </div>

      {/* Right Section - Actions, Notifications, Theme, User */}
      <div className="flex items-center gap-3">
        {/* Custom Actions */}
        {actions}

        {/* Theme Toggle */}
        {onThemeChange && (
          <button
            onClick={() => onThemeChange(theme === 'light' ? 'dark' : 'light')}
            className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400"
            title={theme === 'light' ? 'حالت تاریک' : 'حالت روشن'}
          >
            {theme === 'light' ? (
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
              </svg>
            ) : (
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            )}
          </button>
        )}

        {/* Notifications */}
        <div ref={notificationsRef} className="relative">
          <button
            onClick={() => setShowNotifications(!showNotifications)}
            className="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 text-gray-500 dark:text-gray-400 relative"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
            </svg>
            {unreadCount > 0 && (
              <span className="absolute top-1 right-1 w-4 h-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">
                {unreadCount > 9 ? '9+' : unreadCount}
              </span>
            )}
          </button>

          {/* Notifications Dropdown */}
          {showNotifications && (
            <div className="absolute left-0 mt-2 w-80 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-50">
              <div className="p-3 border-b border-gray-200 dark:border-gray-700">
                <h3 className="font-semibold text-gray-900 dark:text-white">اعلان‌ها</h3>
              </div>
              <div className="max-h-80 overflow-y-auto">
                {notifications.length === 0 ? (
                  <div className="p-4 text-center text-gray-500 dark:text-gray-400">
                    اعلانی وجود ندارد
                  </div>
                ) : (
                  notifications.map(notification => (
                    <button
                      key={notification.id}
                      onClick={() => onNotificationClick?.(notification)}
                      className={clsx(
                        'w-full p-3 flex items-start gap-3 hover:bg-gray-50 dark:hover:bg-gray-700 text-right',
                        !notification.read && 'bg-blue-50 dark:bg-blue-900/20'
                      )}
                    >
                      {getNotificationIcon(notification.type)}
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-gray-900 dark:text-white truncate">
                          {notification.title}
                        </p>
                        <p className="text-xs text-gray-500 dark:text-gray-400 line-clamp-2">
                          {notification.message}
                        </p>
                        <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">
                          {notification.time}
                        </p>
                      </div>
                    </button>
                  ))
                )}
              </div>
              {notifications.length > 0 && (
                <div className="p-2 border-t border-gray-200 dark:border-gray-700">
                  <button
                    onClick={onViewAllNotifications}
                    className="w-full py-2 text-sm text-blue-600 hover:text-blue-700 dark:text-blue-400"
                  >
                    مشاهده همه اعلان‌ها
                  </button>
                </div>
              )}
            </div>
          )}
        </div>

        {/* User Menu */}
        {user && (
          <div ref={userMenuRef} className="relative">
            <button
              onClick={() => setShowUserMenu(!showUserMenu)}
              className="flex items-center gap-2 p-1.5 rounded-lg bg-white dark:bg-gray-800 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors border border-gray-200 dark:border-gray-700"
            >
              {user.avatar ? (
                <img src={user.avatar} alt={user.name} className="w-8 h-8 rounded-full" />
              ) : (
                <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white font-medium">
                  {user.name.charAt(0)}
                </div>
              )}
              <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
              </svg>
            </button>

            {/* User Dropdown */}
            {showUserMenu && (
              <div className="absolute left-0 mt-2 w-56 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 z-50">
                <div className="p-3 border-b border-gray-200 dark:border-gray-700">
                  <p className="font-medium text-gray-900 dark:text-white">{user.name}</p>
                  {user.email && (
                    <p className="text-sm text-gray-500 dark:text-gray-400">{user.email}</p>
                  )}
                  {user.role && (
                    <span className="inline-block mt-1 px-2 py-0.5 bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300 text-xs rounded">
                      {user.role}
                    </span>
                  )}
                </div>
                <div className="p-2">
                  <button
                    onClick={onLogout}
                    className="w-full flex items-center gap-2 px-3 py-2 text-sm text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg"
                  >
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                    </svg>
                    خروج از حساب
                  </button>
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </header>
  );
};

export default Header;

