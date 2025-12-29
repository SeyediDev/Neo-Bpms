/**
 * Sidebar Component
 * Neo BPMS Admin Panel Core
 * 
 * Collapsible sidebar navigation with menu items and user info.
 */

import React, { type ReactNode } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import clsx from 'clsx';
import { 
  useAppDispatch, 
  useAppSelector,
  toggleSidebar,
  setMobileSidebarOpen,
  selectSidebarCollapsed,
  selectSidebarMobileOpen,
} from '../state';
import { useAuth } from '../auth/useAuth';
import type { MenuItem } from '../types';

/**
 * Sidebar Props
 */
interface SidebarProps {
  children?: ReactNode;
  logo?: ReactNode;
  menuItems?: MenuItem[];
  bottomContent?: ReactNode;
}

/**
 * Menu Item Component
 */
function SidebarMenuItem({ 
  item, 
  collapsed,
  depth = 0,
}: { 
  item: MenuItem; 
  collapsed: boolean;
  depth?: number;
}) {
  const location = useLocation();
  const { hasPermission, hasRole } = useAuth();
  const [isExpanded, setIsExpanded] = React.useState(false);

  // Check permissions
  if (item.permissions && !hasPermission(item.permissions)) return null;
  if (item.roles && !hasRole(item.roles)) return null;
  if (item.isHidden) return null;

  // Divider
  if (item.isDivider) {
    return <div className="my-2 border-t border-slate-700/50" />;
  }

  const isActive = item.path ? location.pathname.startsWith(item.path) : false;
  const hasChildren = item.children && item.children.length > 0;

  const handleClick = () => {
    if (hasChildren) {
      setIsExpanded(!isExpanded);
    }
  };

  const content = (
    <>
      {/* Icon */}
      {item.icon && (
        <span className={clsx(
          'w-5 h-5 flex items-center justify-center text-lg',
          isActive ? 'text-primary-400' : 'text-slate-400 group-hover:text-primary-400'
        )}>
          <i className={item.icon} />
        </span>
      )}

      {/* Label */}
      <AnimatePresence>
        {!collapsed && (
          <motion.span
            initial={{ opacity: 0, width: 0 }}
            animate={{ opacity: 1, width: 'auto' }}
            exit={{ opacity: 0, width: 0 }}
            className={clsx(
              'flex-1 truncate text-sm font-medium transition-colors',
              isActive ? 'text-white' : 'text-slate-300 group-hover:text-white'
            )}
          >
            {item.label}
          </motion.span>
        )}
      </AnimatePresence>

      {/* Badge */}
      {item.badge && !collapsed && (
        <span className={clsx(
          'px-2 py-0.5 text-xs font-medium rounded-full',
          item.badgeColor === 'danger' && 'bg-red-500/20 text-red-400',
          item.badgeColor === 'warning' && 'bg-yellow-500/20 text-yellow-400',
          item.badgeColor === 'success' && 'bg-green-500/20 text-green-400',
          item.badgeColor === 'info' && 'bg-blue-500/20 text-blue-400',
          (!item.badgeColor || item.badgeColor === 'primary') && 'bg-primary-500/20 text-primary-400',
        )}>
          {item.badge}
        </span>
      )}

      {/* Expand Icon */}
      {hasChildren && !collapsed && (
        <motion.span
          animate={{ rotate: isExpanded ? 90 : 0 }}
          className="text-slate-400"
        >
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
          </svg>
        </motion.span>
      )}
    </>
  );

  const itemClasses = clsx(
    'group flex items-center gap-3 px-3 py-2.5 rounded-lg transition-all duration-200 cursor-pointer',
    depth > 0 && 'mr-4',
    isActive
      ? 'bg-primary-500/10 text-white'
      : 'hover:bg-slate-800/50',
    collapsed && 'justify-center px-0'
  );

  // External link
  if (item.isExternal && item.path) {
    return (
      <a
        href={item.path}
        target="_blank"
        rel="noopener noreferrer"
        className={itemClasses}
      >
        {content}
      </a>
    );
  }

  // Internal link
  if (item.path && !hasChildren) {
    return (
      <Link to={item.path} className={itemClasses}>
        {content}
      </Link>
    );
  }

  // Expandable menu
  return (
    <div>
      <div onClick={handleClick} className={itemClasses}>
        {content}
      </div>
      
      {/* Children */}
      <AnimatePresence>
        {hasChildren && isExpanded && !collapsed && (
          <motion.div
            initial={{ height: 0, opacity: 0 }}
            animate={{ height: 'auto', opacity: 1 }}
            exit={{ height: 0, opacity: 0 }}
            className="overflow-hidden"
          >
            <div className="mt-1 space-y-1">
              {item.children!.map((child) => (
                <SidebarMenuItem
                  key={child.id}
                  item={child}
                  collapsed={collapsed}
                  depth={depth + 1}
                />
              ))}
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}

/**
 * Sidebar Component
 */
export function Sidebar({
  children,
  logo,
  menuItems = [],
  bottomContent,
}: SidebarProps) {
  const dispatch = useAppDispatch();
  const collapsed = useAppSelector(selectSidebarCollapsed);
  const mobileOpen = useAppSelector(selectSidebarMobileOpen);
  const { user, logout } = useAuth();

  const handleToggle = () => {
    dispatch(toggleSidebar());
  };

  const handleCloseMobile = () => {
    dispatch(setMobileSidebarOpen(false));
  };

  const sidebarClasses = clsx(
    'fixed top-0 right-0 h-full z-50 flex flex-col',
    'bg-slate-900 border-l border-slate-800',
    'transition-all duration-300 ease-in-out',
    collapsed ? 'w-20' : 'w-64',
    // Mobile
    'translate-x-full lg:translate-x-0',
    mobileOpen && 'translate-x-0'
  );

  return (
    <>
      <aside className={sidebarClasses}>
        {/* Logo Section */}
        <div className={clsx(
          'flex items-center h-16 px-4 border-b border-slate-800',
          collapsed ? 'justify-center' : 'justify-between'
        )}>
          {logo || (
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white font-bold">
                N
              </div>
              {!collapsed && (
                <motion.span
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  className="text-lg font-bold text-white"
                >
                  Neo BPMS
                </motion.span>
              )}
            </div>
          )}
          
          {/* Toggle Button (Desktop) */}
          <button
            onClick={handleToggle}
            className={clsx(
              'hidden lg:flex items-center justify-center w-8 h-8 rounded-lg',
              'text-slate-400 hover:text-white hover:bg-slate-800 transition-colors',
              collapsed && 'absolute -left-3 top-6 bg-slate-800 border border-slate-700'
            )}
          >
            <motion.svg
              animate={{ rotate: collapsed ? 180 : 0 }}
              className="w-4 h-4"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </motion.svg>
          </button>
          
          {/* Close Button (Mobile) */}
          <button
            onClick={handleCloseMobile}
            className="lg:hidden flex items-center justify-center w-8 h-8 rounded-lg text-slate-400 hover:text-white hover:bg-slate-800"
          >
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Menu Items */}
        <nav className="flex-1 px-3 py-4 overflow-y-auto custom-scrollbar">
          <div className="space-y-1">
            {menuItems.map((item) => (
              <SidebarMenuItem key={item.id} item={item} collapsed={collapsed} />
            ))}
          </div>
          
          {/* Additional Content */}
          {children}
        </nav>

        {/* User Section */}
        {user && (
          <div className={clsx(
            'p-4 border-t border-slate-800',
            collapsed && 'flex justify-center'
          )}>
            {collapsed ? (
              <div className="w-10 h-10 rounded-full bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white font-bold">
                {user.displayName?.charAt(0) || user.username?.charAt(0) || 'U'}
              </div>
            ) : (
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-full bg-gradient-to-br from-primary-500 to-neo-500 flex items-center justify-center text-white font-bold">
                  {user.displayName?.charAt(0) || user.username?.charAt(0) || 'U'}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-white truncate">
                    {user.displayName || user.username}
                  </p>
                  <p className="text-xs text-slate-400 truncate">
                    {user.email || user.roles?.[0] || 'User'}
                  </p>
                </div>
                <button
                  onClick={() => logout()}
                  className="p-2 text-slate-400 hover:text-white hover:bg-slate-800 rounded-lg transition-colors"
                  title="خروج"
                >
                  <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                </button>
              </div>
            )}
          </div>
        )}

        {/* Bottom Content */}
        {bottomContent && (
          <div className="p-4 border-t border-slate-800">
            {bottomContent}
          </div>
        )}
      </aside>
    </>
  );
}

export default Sidebar;

