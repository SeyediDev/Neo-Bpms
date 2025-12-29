/**
 * Admin Layout Component
 * Neo BPMS Admin Panel Core
 * 
 * Main layout wrapper for admin panel pages with sidebar, header, and content area.
 */

import { type ReactNode } from 'react';
import { Outlet } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import clsx from 'clsx';
import { Sidebar } from './Sidebar';
import { Header } from './Header';
import { useAppSelector } from '../state';
import { selectSidebarCollapsed, selectSidebarMobileOpen, selectTheme } from '../state/slices/uiSlice';

/**
 * Layout Props
 */
interface AdminLayoutProps {
  children?: ReactNode;
  className?: string;
  showHeader?: boolean;
  showSidebar?: boolean;
  headerContent?: ReactNode;
  sidebarContent?: ReactNode;
}

/**
 * Main content area animation variants
 */
const contentVariants = {
  hidden: { opacity: 0, y: 10 },
  visible: { 
    opacity: 1, 
    y: 0,
    transition: { duration: 0.2, ease: 'easeOut' }
  },
  exit: { 
    opacity: 0, 
    y: -10,
    transition: { duration: 0.15 }
  }
};

/**
 * Mobile overlay animation variants
 */
const overlayVariants = {
  hidden: { opacity: 0 },
  visible: { opacity: 1 },
};

/**
 * Admin Layout Component
 */
export function AdminLayout({
  children,
  className,
  showHeader = true,
  showSidebar = true,
  headerContent,
  sidebarContent,
}: AdminLayoutProps) {
  const sidebarCollapsed = useAppSelector(selectSidebarCollapsed);
  const sidebarMobileOpen = useAppSelector(selectSidebarMobileOpen);
  const theme = useAppSelector(selectTheme);

  return (
    <div
      className={clsx(
        'min-h-screen bg-slate-100 dark:bg-slate-900 transition-colors duration-300',
        theme.direction === 'rtl' ? 'rtl' : 'ltr',
        className
      )}
      dir={theme.direction}
    >
      {/* Mobile Overlay */}
      <AnimatePresence>
        {sidebarMobileOpen && (
          <motion.div
            variants={overlayVariants}
            initial="hidden"
            animate="visible"
            exit="hidden"
            className="fixed inset-0 bg-black/50 backdrop-blur-sm z-40 lg:hidden"
          />
        )}
      </AnimatePresence>

      {/* Sidebar */}
      {showSidebar && (
        <Sidebar>
          {sidebarContent}
        </Sidebar>
      )}

      {/* Main Content Area */}
      <div
        className={clsx(
          'flex flex-col min-h-screen transition-all duration-300 ease-in-out',
          showSidebar && (sidebarCollapsed ? 'lg:mr-20' : 'lg:mr-64'),
        )}
      >
        {/* Header */}
        {showHeader && (
          <Header>
            {headerContent}
          </Header>
        )}

        {/* Page Content */}
        <main className="flex-1 p-4 md:p-6 lg:p-8">
          <AnimatePresence mode="wait">
            <motion.div
              key={location.pathname}
              variants={contentVariants}
              initial="hidden"
              animate="visible"
              exit="exit"
              className="h-full"
            >
              {children || <Outlet />}
            </motion.div>
          </AnimatePresence>
        </main>

        {/* Footer */}
        <footer className="px-6 py-4 border-t border-slate-200 dark:border-slate-800">
          <div className="flex items-center justify-between text-sm text-slate-500 dark:text-slate-400">
            <span>© {new Date().getFullYear()} Neo BPMS</span>
            <span>Powered by Neo Platform</span>
          </div>
        </footer>
      </div>
    </div>
  );
}

export default AdminLayout;

