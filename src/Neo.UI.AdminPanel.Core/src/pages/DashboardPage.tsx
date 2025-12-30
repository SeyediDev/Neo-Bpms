/**
 * Dashboard Page Component
 * Neo BPMS Admin Panel Core
 * 
 * Main dashboard landing page with stats and widgets.
 */

import { useMemo, type ReactNode } from 'react';
import clsx from 'clsx';

// ==================== Types ====================

export interface StatCardProps {
  title: string;
  value: string | number;
  change?: number;
  changeLabel?: string;
  icon?: ReactNode;
  color?: 'blue' | 'green' | 'purple' | 'orange' | 'red' | 'cyan';
  onClick?: () => void;
}

export interface QuickActionProps {
  label: string;
  icon: ReactNode;
  onClick: () => void;
  color?: string;
}

export interface ActivityItem {
  id: string;
  title: string;
  description?: string;
  time: string;
  icon?: ReactNode;
  type?: 'success' | 'warning' | 'error' | 'info';
}

export interface DashboardPageProps {
  /** Page title */
  title?: string;
  /** Welcome message */
  welcomeMessage?: string;
  /** User name for greeting */
  userName?: string;
  /** Stat cards to display */
  stats?: StatCardProps[];
  /** Quick action buttons */
  quickActions?: QuickActionProps[];
  /** Recent activity items */
  recentActivity?: ActivityItem[];
  /** Custom widgets */
  widgets?: ReactNode;
  /** Loading state */
  isLoading?: boolean;
  /** Custom class name */
  className?: string;
}

// ==================== Sub Components ====================

/**
 * Stat Card Component
 */
function StatCard({ title, value, change, changeLabel, icon, color = 'blue', onClick }: StatCardProps) {
  const colorClasses = {
    blue: 'from-blue-500 to-blue-600',
    green: 'from-emerald-500 to-emerald-600',
    purple: 'from-purple-500 to-purple-600',
    orange: 'from-orange-500 to-orange-600',
    red: 'from-red-500 to-red-600',
    cyan: 'from-cyan-500 to-cyan-600',
  };

  const isPositive = change && change > 0;
  const isNegative = change && change < 0;

  return (
    <div
      onClick={onClick}
      className={clsx(
        'relative overflow-hidden rounded-2xl p-6',
        'bg-gradient-to-br',
        colorClasses[color],
        'text-white shadow-lg',
        'transition-all duration-300',
        onClick && 'cursor-pointer hover:scale-[1.02] hover:shadow-xl'
      )}
    >
      {/* Background decoration */}
      <div className="absolute top-0 left-0 w-32 h-32 bg-white/10 rounded-full -translate-x-1/2 -translate-y-1/2" />
      <div className="absolute bottom-0 right-0 w-24 h-24 bg-white/10 rounded-full translate-x-1/2 translate-y-1/2" />

      <div className="relative">
        <div className="flex items-center justify-between mb-4">
          <span className="text-sm font-medium text-white/80">{title}</span>
          {icon && <div className="text-white/80">{icon}</div>}
        </div>

        <div className="text-3xl font-bold mb-2" style={{ direction: 'ltr', textAlign: 'right' }}>
          {typeof value === 'number' ? value.toLocaleString('fa-IR') : value}
        </div>

        {change !== undefined && (
          <div className="flex items-center gap-1 text-sm">
            {isPositive && (
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 10l7-7m0 0l7 7m-7-7v18" />
              </svg>
            )}
            {isNegative && (
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 14l-7 7m0 0l-7-7m7 7V3" />
              </svg>
            )}
            <span className={clsx(
              'font-medium',
              isPositive && 'text-green-200',
              isNegative && 'text-red-200',
              !change && 'text-white/60'
            )}>
              {Math.abs(change)}%
            </span>
            {changeLabel && <span className="text-white/60">{changeLabel}</span>}
          </div>
        )}
      </div>
    </div>
  );
}

/**
 * Quick Action Button
 */
function QuickAction({ label, icon, onClick, color }: QuickActionProps) {
  return (
    <button
      onClick={onClick}
      className={clsx(
        'flex flex-col items-center gap-2 p-4 rounded-xl',
        'bg-white dark:bg-gray-800',
        'border border-gray-200 dark:border-gray-700',
        'hover:border-blue-300 dark:hover:border-blue-600',
        'hover:shadow-md',
        'transition-all duration-200',
        'group'
      )}
    >
      <div
        className={clsx(
          'w-12 h-12 rounded-xl flex items-center justify-center',
          'bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900/30 dark:to-blue-800/30',
          'group-hover:scale-110 transition-transform duration-200'
        )}
        style={color ? { background: `linear-gradient(135deg, ${color}20, ${color}30)` } : undefined}
      >
        {icon}
      </div>
      <span className="text-sm font-medium text-gray-700 dark:text-gray-300">{label}</span>
    </button>
  );
}

/**
 * Activity Item Component
 */
function ActivityItemComponent({ title, description, time, icon, type = 'info' }: ActivityItem) {
  const typeColors = {
    success: 'bg-green-100 text-green-600 dark:bg-green-900/30 dark:text-green-400',
    warning: 'bg-yellow-100 text-yellow-600 dark:bg-yellow-900/30 dark:text-yellow-400',
    error: 'bg-red-100 text-red-600 dark:bg-red-900/30 dark:text-red-400',
    info: 'bg-blue-100 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400',
  };

  return (
    <div className="flex items-start gap-3 p-3 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-800/50 transition-colors">
      <div className={clsx('w-10 h-10 rounded-full flex items-center justify-center flex-shrink-0', typeColors[type])}>
        {icon || (
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        )}
      </div>
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-gray-900 dark:text-white">{title}</p>
        {description && <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">{description}</p>}
        <p className="text-xs text-gray-400 dark:text-gray-500 mt-1">{time}</p>
      </div>
    </div>
  );
}

/**
 * Loading Skeleton
 */
function DashboardSkeleton() {
  return (
    <div className="animate-pulse space-y-6">
      <div className="h-8 w-64 bg-gray-200 dark:bg-gray-700 rounded" />
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {[1, 2, 3, 4].map(i => (
          <div key={i} className="h-36 bg-gray-200 dark:bg-gray-700 rounded-2xl" />
        ))}
      </div>
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 h-64 bg-gray-200 dark:bg-gray-700 rounded-2xl" />
        <div className="h-64 bg-gray-200 dark:bg-gray-700 rounded-2xl" />
      </div>
    </div>
  );
}

// ==================== Main Component ====================

/**
 * Dashboard Page Component
 */
export function DashboardPage({
  title = 'داشبورد',
  welcomeMessage,
  userName,
  stats = [],
  quickActions = [],
  recentActivity = [],
  widgets,
  isLoading = false,
  className,
}: DashboardPageProps) {
  // Generate greeting based on time
  const greeting = useMemo(() => {
    const hour = new Date().getHours();
    if (hour < 12) return 'صبح بخیر';
    if (hour < 17) return 'عصر بخیر';
    return 'شب بخیر';
  }, []);

  if (isLoading) {
    return (
      <div className={clsx('p-6', className)}>
        <DashboardSkeleton />
      </div>
    );
  }

  return (
    <div className={clsx('p-6 space-y-6', className)}>
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">{title}</h1>
          {(welcomeMessage || userName) && (
            <p className="text-gray-500 dark:text-gray-400 mt-1">
              {welcomeMessage || `${greeting}${userName ? `، ${userName}` : ''}`}
            </p>
          )}
        </div>

        {/* Quick Date/Time */}
        <div className="text-sm text-gray-500 dark:text-gray-400">
          {new Date().toLocaleDateString('fa-IR', {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
          })}
        </div>
      </div>

      {/* Stats Grid */}
      {stats.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {stats.map((stat, index) => (
            <StatCard key={index} {...stat} />
          ))}
        </div>
      )}

      {/* Quick Actions */}
      {quickActions.length > 0 && (
        <div className="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">دسترسی سریع</h2>
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
            {quickActions.map((action, index) => (
              <QuickAction key={index} {...action} />
            ))}
          </div>
        </div>
      )}

      {/* Main Content Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Custom Widgets or Placeholder */}
        <div className="lg:col-span-2">
          {widgets || (
            <div className="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700 h-full min-h-[300px] flex items-center justify-center">
              <div className="text-center text-gray-400">
                <svg className="w-16 h-16 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 5a1 1 0 011-1h14a1 1 0 011 1v2a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM4 13a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H5a1 1 0 01-1-1v-6zM16 13a1 1 0 011-1h2a1 1 0 011 1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-6z" />
                </svg>
                <p className="text-sm">ویجت‌های سفارشی خود را اینجا اضافه کنید</p>
              </div>
            </div>
          )}
        </div>

        {/* Recent Activity */}
        <div className="bg-white dark:bg-gray-800 rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700">
          <h2 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">فعالیت‌های اخیر</h2>
          {recentActivity.length > 0 ? (
            <div className="space-y-1 max-h-80 overflow-y-auto">
              {recentActivity.map((activity) => (
                <ActivityItemComponent key={activity.id} {...activity} />
              ))}
            </div>
          ) : (
            <div className="text-center py-8 text-gray-400">
              <svg className="w-12 h-12 mx-auto mb-3 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <p className="text-sm">فعالیتی ثبت نشده است</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default DashboardPage;

