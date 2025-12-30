import React, { useState, useCallback } from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import clsx from 'clsx';

export interface MenuItem {
  id: string;
  label: string;
  labelEn?: string;
  icon?: React.ReactNode | string;
  path?: string;
  children?: MenuItem[];
  badge?: string | number;
  badgeColor?: 'primary' | 'success' | 'warning' | 'danger' | 'info';
  permission?: string;
  permissions?: string[];
  roles?: string[];
  divider?: boolean;
  isExternal?: boolean;
  isHidden?: boolean;
  isDivider?: boolean;
}

export interface SidebarProps {
  /** Menu items to display */
  items: MenuItem[];
  /** Whether sidebar is collapsed */
  collapsed?: boolean;
  /** Callback when collapse state changes */
  onCollapsedChange?: (collapsed: boolean) => void;
  /** Logo component or image URL */
  logo?: React.ReactNode | string;
  /** Logo text when expanded */
  logoText?: string;
  /** Footer content */
  footer?: React.ReactNode;
  /** Custom class name */
  className?: string;
  /** Check if user has permission */
  hasPermission?: (permission: string) => boolean;
}

/**
 * Sidebar navigation component with collapsible menu
 */
export const Sidebar: React.FC<SidebarProps> = ({
  items,
  collapsed = false,
  onCollapsedChange,
  logo,
  logoText = 'Admin Panel',
  footer,
  className,
  hasPermission = () => true,
}) => {
  const location = useLocation();
  const [expandedItems, setExpandedItems] = useState<Set<string>>(new Set());

  const toggleExpanded = useCallback((id: string) => {
    setExpandedItems(prev => {
      const next = new Set(prev);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });
  }, []);

  const isActive = useCallback((path?: string) => {
    if (!path) return false;
    return location.pathname === path || location.pathname.startsWith(path + '/');
  }, [location.pathname]);

  const renderMenuItem = (item: MenuItem, level: number = 0) => {
    // Check permission
    if (item.permission && !hasPermission(item.permission)) {
      return null;
    }

    // Divider
    if (item.divider) {
      return (
        <div
          key={item.id}
          className="my-2 mx-4 border-t border-gray-200 dark:border-gray-700"
        />
      );
    }

    const hasChildren = item.children && item.children.length > 0;
    const isExpanded = expandedItems.has(item.id);
    const active = isActive(item.path);

    const content = (
      <>
        {/* Icon */}
        {item.icon && (
          <span className="flex-shrink-0 w-5 h-5 flex items-center justify-center">
            {item.icon}
          </span>
        )}

        {/* Label */}
        {!collapsed && (
          <span className="flex-1 text-sm font-medium truncate">
            {item.label}
          </span>
        )}

        {/* Badge */}
        {!collapsed && item.badge && (
          <span
            className={clsx(
              'px-2 py-0.5 text-xs font-semibold rounded-full',
              {
                'bg-purple-100 text-purple-700 dark:bg-purple-900 dark:text-purple-300': item.badgeColor === 'primary',
                'bg-green-100 text-green-700 dark:bg-green-900 dark:text-green-300': item.badgeColor === 'success',
                'bg-yellow-100 text-yellow-700 dark:bg-yellow-900 dark:text-yellow-300': item.badgeColor === 'warning',
                'bg-red-100 text-red-700 dark:bg-red-900 dark:text-red-300': item.badgeColor === 'danger',
                'bg-blue-100 text-blue-700 dark:bg-blue-900 dark:text-blue-300': item.badgeColor === 'info',
                'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300': !item.badgeColor,
              }
            )}
          >
            {item.badge}
          </span>
        )}

        {/* Expand arrow */}
        {!collapsed && hasChildren && (
          <svg
            className={clsx(
              'w-4 h-4 transition-transform duration-200',
              isExpanded && 'rotate-90'
            )}
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
          </svg>
        )}
      </>
    );

    const itemClasses = clsx(
      'flex items-center gap-3 px-4 py-2.5 rounded-lg transition-all duration-200',
      'hover:bg-gray-100 dark:hover:bg-gray-800',
      active && 'bg-blue-50 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400',
      !active && 'text-gray-700 dark:text-gray-300',
      collapsed && 'justify-center px-2',
      level > 0 && !collapsed && 'mr-4'
    );

    return (
      <div key={item.id}>
        {item.path && !hasChildren ? (
          <NavLink to={item.path} className={itemClasses} title={collapsed ? item.label : undefined}>
            {content}
          </NavLink>
        ) : (
          <button
            onClick={() => hasChildren && toggleExpanded(item.id)}
            className={clsx(itemClasses, 'w-full text-right')}
            title={collapsed ? item.label : undefined}
          >
            {content}
          </button>
        )}

        {/* Children */}
        {hasChildren && isExpanded && !collapsed && (
          <div className="mt-1 space-y-1">
            {item.children!.map(child => renderMenuItem(child, level + 1))}
          </div>
        )}
      </div>
    );
  };

  return (
    <aside
      className={clsx(
        'flex flex-col h-screen bg-white dark:bg-gray-900 border-l border-gray-200 dark:border-gray-700',
        'transition-all duration-300 ease-in-out',
        collapsed ? 'w-16' : 'w-64',
        className
      )}
    >
      {/* Header / Logo */}
      <div className="flex items-center justify-between h-16 px-4 border-b border-gray-200 dark:border-gray-700">
        {!collapsed && (
          <div className="flex items-center gap-3">
            {typeof logo === 'string' ? (
              <img src={logo} alt="Logo" className="w-8 h-8" />
            ) : (
              logo
            )}
            <span className="font-bold text-gray-900 dark:text-white">
              {logoText}
            </span>
          </div>
        )}
        
        <button
          onClick={() => onCollapsedChange?.(!collapsed)}
          className={clsx(
            'p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800',
            'text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200',
            collapsed && 'mx-auto'
          )}
          title={collapsed ? 'باز کردن منو' : 'بستن منو'}
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            {collapsed ? (
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 5l7 7-7 7M5 5l7 7-7 7" />
            ) : (
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 19l-7-7 7-7m8 14l-7-7 7-7" />
            )}
          </svg>
        </button>
      </div>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto p-2 space-y-1">
        {items.map(item => renderMenuItem(item))}
      </nav>

      {/* Footer */}
      {footer && (
        <div className="border-t border-gray-200 dark:border-gray-700 p-4">
          {footer}
        </div>
      )}
    </aside>
  );
};

export default Sidebar;

