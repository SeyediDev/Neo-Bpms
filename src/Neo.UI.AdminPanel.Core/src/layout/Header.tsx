/**
 * Header Component
 * Neo BPMS Admin Panel Core
 * 
 * Top navigation bar with breadcrumbs, search, notifications, and user menu.
 */

import React, { type ReactNode, useState } from 'react';
import { Link } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import clsx from 'clsx';
import { 
  useAppDispatch, 
  useAppSelector,
  toggleMobileSidebar,
  toggleThemeMode,
  selectTheme,
  selectBreadcrumbs,
  selectPageTitle,
  selectNotifications,
  removeNotification,
} from '../state';
import { useAuth } from '../auth/useAuth';

/**
 * Header Props
 */
interface HeaderProps {
  children?: ReactNode;
  showSearch?: boolean;
  showNotifications?: boolean;
  showThemeToggle?: boolean;
  customActions?: ReactNode;
}

/**
 * Breadcrumb Component
 */
function Breadcrumbs() {
  const breadcrumbs = useAppSelector(selectBreadcrumbs);

  if (!breadcrumbs.length) return null;

  return (
    <nav className="flex items-center gap-2 text-sm">
      <Link to="/" className="text-slate-400 hover:text-primary-500 transition-colors">
        <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
        </svg>
      </Link>
      {breadcrumbs.map((crumb, index) => (
        <React.Fragment key={index}>
          <svg className="w-4 h-4 text-slate-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
          </svg>
          {crumb.path ? (
            <Link
              to={crumb.path}
              className="text-slate-400 hover:text-primary-500 transition-colors"
            >
              {crumb.label}
            </Link>
          ) : (
            <span className="text-slate-300 font-medium">{crumb.label}</span>
          )}
        </React.Fragment>
      ))}
    </nav>
  );
}

/**
 * Notifications Dropdown
 */
function NotificationsDropdown() {
  const [isOpen, setIsOpen] = useState(false);
  const dispatch = useAppDispatch();
  const notifications = useAppSelector(selectNotifications);

  const unreadCount = notifications.length;

  return (
    <div className="relative">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className={clsx(
          'relative p-2 rounded-lg transition-colors',
          'text-slate-400 hover:text-white hover:bg-slate-800/50'
        )}
      >
        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
        </svg>
        {unreadCount > 0 && (
          <span className="absolute top-1 right-1 w-4 h-4 bg-red-500 rounded-full text-xs text-white flex items-center justify-center">
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        )}
      </button>

      <AnimatePresence>
        {isOpen && (
          <>
            {/* Backdrop */}
            <div
              className="fixed inset-0 z-40"
              onClick={() => setIsOpen(false)}
            />

            {/* Dropdown */}
            <motion.div
              initial={{ opacity: 0, y: -10, scale: 0.95 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={{ opacity: 0, y: -10, scale: 0.95 }}
              className="absolute left-0 mt-2 w-80 bg-slate-800 rounded-xl shadow-xl border border-slate-700 z-50 overflow-hidden"
            >
              <div className="p-4 border-b border-slate-700">
                <h3 className="font-semibold text-white">اعلان‌ها</h3>
              </div>

              <div className="max-h-80 overflow-y-auto">
                {notifications.length === 0 ? (
                  <div className="p-4 text-center text-slate-400">
                    اعلان جدیدی وجود ندارد
                  </div>
                ) : (
                  notifications.map((notification) => (
                    <div
                      key={notification.id}
                      className={clsx(
                        'p-4 border-b border-slate-700/50 hover:bg-slate-700/50 transition-colors',
                        'flex items-start gap-3'
                      )}
                    >
                      <div className={clsx(
                        'w-8 h-8 rounded-full flex items-center justify-center',
                        notification.type === 'success' && 'bg-green-500/20 text-green-400',
                        notification.type === 'error' && 'bg-red-500/20 text-red-400',
                        notification.type === 'warning' && 'bg-yellow-500/20 text-yellow-400',
                        notification.type === 'info' && 'bg-blue-500/20 text-blue-400',
                      )}>
                        {notification.type === 'success' && '✓'}
                        {notification.type === 'error' && '✕'}
                        {notification.type === 'warning' && '!'}
                        {notification.type === 'info' && 'i'}
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="text-sm font-medium text-white">{notification.title}</p>
                        {notification.message && (
                          <p className="text-xs text-slate-400 mt-1">{notification.message}</p>
                        )}
                      </div>
                      <button
                        onClick={() => dispatch(removeNotification(notification.id))}
                        className="text-slate-500 hover:text-white"
                      >
                        <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                      </button>
                    </div>
                  ))
                )}
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  );
}

/**
 * User Menu Dropdown
 */
function UserMenu() {
  const [isOpen, setIsOpen] = useState(false);
  const { user, logout, isAdmin } = useAuth();

  if (!user) return null;

  return (
    <div className="relative">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center gap-2 p-1 rounded-lg hover:bg-slate-800/50 transition-colors"
      >
        <div className="w-8 h-8 rounded-full bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white text-sm font-bold">
          {user.displayName?.charAt(0) || user.username?.charAt(0) || 'U'}
        </div>
        <span className="hidden md:block text-sm text-slate-300">
          {user.displayName || user.username}
        </span>
        <svg className="w-4 h-4 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
        </svg>
      </button>

      <AnimatePresence>
        {isOpen && (
          <>
            <div className="fixed inset-0 z-40" onClick={() => setIsOpen(false)} />
            
            <motion.div
              initial={{ opacity: 0, y: -10, scale: 0.95 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={{ opacity: 0, y: -10, scale: 0.95 }}
              className="absolute left-0 mt-2 w-56 bg-slate-800 rounded-xl shadow-xl border border-slate-700 z-50 overflow-hidden"
            >
              <div className="p-4 border-b border-slate-700">
                <p className="font-medium text-white">{user.displayName || user.username}</p>
                <p className="text-sm text-slate-400">{user.email}</p>
                {isAdmin && (
                  <span className="inline-block mt-2 px-2 py-0.5 bg-primary-500/20 text-primary-400 text-xs rounded-full">
                    مدیر سیستم
                  </span>
                )}
              </div>

              <div className="py-2">
                <Link
                  to="/profile"
                  className="flex items-center gap-3 px-4 py-2 text-sm text-slate-300 hover:bg-slate-700/50 transition-colors"
                  onClick={() => setIsOpen(false)}
                >
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  پروفایل
                </Link>
                <Link
                  to="/settings"
                  className="flex items-center gap-3 px-4 py-2 text-sm text-slate-300 hover:bg-slate-700/50 transition-colors"
                  onClick={() => setIsOpen(false)}
                >
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                  تنظیمات
                </Link>
              </div>

              <div className="py-2 border-t border-slate-700">
                <button
                  onClick={() => {
                    setIsOpen(false);
                    logout();
                  }}
                  className="flex items-center gap-3 w-full px-4 py-2 text-sm text-red-400 hover:bg-slate-700/50 transition-colors"
                >
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                  خروج از حساب
                </button>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  );
}

/**
 * Header Component
 */
export function Header({
  children,
  showSearch = true,
  showNotifications = true,
  showThemeToggle = true,
  customActions,
}: HeaderProps) {
  const dispatch = useAppDispatch();
  const theme = useAppSelector(selectTheme);
  const pageTitle = useAppSelector(selectPageTitle);

  const handleToggleMobile = () => {
    dispatch(toggleMobileSidebar());
  };

  const handleToggleTheme = () => {
    dispatch(toggleThemeMode());
  };

  return (
    <header className="sticky top-0 z-30 bg-slate-900/80 backdrop-blur-lg border-b border-slate-800">
      <div className="flex items-center justify-between h-16 px-4 md:px-6">
        {/* Left Section */}
        <div className="flex items-center gap-4">
          {/* Mobile Menu Toggle */}
          <button
            onClick={handleToggleMobile}
            className="lg:hidden p-2 text-slate-400 hover:text-white hover:bg-slate-800/50 rounded-lg transition-colors"
          >
            <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>

          {/* Page Title */}
          {pageTitle && (
            <h1 className="text-lg font-semibold text-white hidden md:block">
              {pageTitle}
            </h1>
          )}

          {/* Breadcrumbs */}
          <Breadcrumbs />
        </div>

        {/* Right Section */}
        <div className="flex items-center gap-2">
          {/* Search */}
          {showSearch && (
            <button className="p-2 text-slate-400 hover:text-white hover:bg-slate-800/50 rounded-lg transition-colors">
              <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </button>
          )}

          {/* Theme Toggle */}
          {showThemeToggle && (
            <button
              onClick={handleToggleTheme}
              className="p-2 text-slate-400 hover:text-white hover:bg-slate-800/50 rounded-lg transition-colors"
            >
              {theme.mode === 'dark' ? (
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
                </svg>
              ) : (
                <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
                </svg>
              )}
            </button>
          )}

          {/* Notifications */}
          {showNotifications && <NotificationsDropdown />}

          {/* Custom Actions */}
          {customActions}

          {/* User Menu */}
          <UserMenu />

          {/* Additional Content */}
          {children}
        </div>
      </div>
    </header>
  );
}

export default Header;

