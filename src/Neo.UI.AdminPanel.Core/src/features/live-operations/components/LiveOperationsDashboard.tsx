/**
 * Live Operations Dashboard Component
 * Neo BPMS Admin Panel Core
 */

import React, { useMemo } from 'react';
import clsx from 'clsx';
import { TimeFrameSelector } from './TimeFrameSelector';
import { LiveMetricCard } from './LiveMetricCard';
import { useLiveOperations } from '../hooks/useLiveOperations';
import { LiveMetric, TimeFrame } from '../types';

export interface LiveOperationsDashboardProps {
  /** API URL for fetching data */
  apiUrl?: string;
  /** Initial time frame */
  initialTimeFrame?: TimeFrame;
  /** Refresh interval in ms */
  refreshInterval?: number;
  /** Custom fetch function */
  fetchFn?: (timeFrame: TimeFrame) => Promise<LiveMetric[]>;
  /** Card click handler */
  onMetricClick?: (metric: LiveMetric) => void;
  /** Show category grouping */
  groupByCategory?: boolean;
  /** Custom title */
  title?: string;
  /** Custom subtitle */
  subtitle?: string;
  /** Additional header content */
  headerExtra?: React.ReactNode;
  /** Custom class name */
  className?: string;
}

const RefreshIndicator: React.FC<{ 
  isPaused: boolean; 
  lastUpdated: Date | null;
  onToggle: () => void;
}> = ({ isPaused, lastUpdated, onToggle }) => {
  const formatTime = (date: Date | null) => {
    if (!date) return '—';
    return date.toLocaleTimeString('fa-IR', {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
    });
  };

  return (
    <div className="flex items-center gap-3">
      <div className="flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400">
        <span
          className={clsx(
            'w-2 h-2 rounded-full',
            isPaused ? 'bg-gray-400' : 'bg-emerald-500 animate-pulse'
          )}
        />
        <span>آخرین بروزرسانی: {formatTime(lastUpdated)}</span>
      </div>
      <button
        onClick={onToggle}
        className={clsx(
          'px-3 py-1.5 text-sm font-medium rounded-lg',
          'transition-colors duration-200',
          isPaused
            ? 'bg-emerald-100 text-emerald-700 hover:bg-emerald-200 dark:bg-emerald-900/30 dark:text-emerald-400'
            : 'bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-300'
        )}
      >
        {isPaused ? (
          <span className="flex items-center gap-1.5">
            <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
              <path d="M8 5v14l11-7z" />
            </svg>
            ادامه
          </span>
        ) : (
          <span className="flex items-center gap-1.5">
            <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
              <path d="M6 4h4v16H6V4zm8 0h4v16h-4V4z" />
            </svg>
            توقف
          </span>
        )}
      </button>
    </div>
  );
};

export const LiveOperationsDashboard: React.FC<LiveOperationsDashboardProps> = ({
  apiUrl,
  initialTimeFrame = '5m',
  refreshInterval = 5000,
  fetchFn,
  onMetricClick,
  groupByCategory = true,
  title = 'عملیات لایو',
  subtitle = 'نمایش آنی داده‌های عملیاتی کسب‌وکار',
  headerExtra,
  className,
}) => {
  const {
    data,
    isLoading,
    error,
    timeFrame,
    setTimeFrame,
    isPaused,
    togglePause,
    refresh,
    lastUpdated,
  } = useLiveOperations({
    apiUrl,
    initialTimeFrame,
    refreshInterval,
    fetchFn,
  });

  // Group metrics by category
  const groupedMetrics = useMemo(() => {
    if (!data?.metrics || !groupByCategory) {
      return { ungrouped: data?.metrics || [] };
    }

    const groups: Record<string, LiveMetric[]> = {};
    data.metrics.forEach((metric) => {
      const category = metric.category || 'سایر';
      if (!groups[category]) {
        groups[category] = [];
      }
      groups[category].push(metric);
    });

    return groups;
  }, [data?.metrics, groupByCategory]);

  if (error) {
    return (
      <div className={clsx('bg-white dark:bg-gray-800 rounded-2xl p-8', className)}>
        <div className="text-center">
          <div className="w-16 h-16 bg-red-100 dark:bg-red-900/30 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
            خطا در دریافت داده‌ها
          </h3>
          <p className="text-gray-500 dark:text-gray-400 mb-4">{error}</p>
          <button
            onClick={refresh}
            className="px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-500 transition-colors"
          >
            تلاش مجدد
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className={clsx('space-y-6', className)}>
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
            {title}
          </h1>
          <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
            {subtitle}
          </p>
        </div>

        <div className="flex flex-wrap items-center gap-4">
          {headerExtra}
          <TimeFrameSelector
            value={timeFrame}
            onChange={setTimeFrame}
            size="md"
          />
        </div>
      </div>

      {/* Refresh indicator */}
      <div className="flex items-center justify-between">
        <RefreshIndicator
          isPaused={isPaused}
          lastUpdated={lastUpdated}
          onToggle={togglePause}
        />

        <button
          onClick={refresh}
          disabled={isLoading}
          className={clsx(
            'p-2 rounded-lg text-gray-500 hover:text-gray-700 hover:bg-gray-100',
            'dark:text-gray-400 dark:hover:text-gray-200 dark:hover:bg-gray-700',
            'transition-colors duration-200',
            isLoading && 'animate-spin'
          )}
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
        </button>
      </div>

      {/* Metrics Grid */}
      {groupByCategory ? (
        Object.entries(groupedMetrics).map(([category, metrics]) => (
          <div key={category}>
            {category !== 'ungrouped' && (
              <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-300 mb-4">
                {category}
              </h2>
            )}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
              {metrics.map((metric) => (
                <LiveMetricCard
                  key={metric.id}
                  metric={metric}
                  loading={isLoading && !data}
                  onClick={onMetricClick ? () => onMetricClick(metric) : undefined}
                />
              ))}
            </div>
          </div>
        ))
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          {(data?.metrics || []).map((metric) => (
            <LiveMetricCard
              key={metric.id}
              metric={metric}
              loading={isLoading && !data}
              onClick={onMetricClick ? () => onMetricClick(metric) : undefined}
            />
          ))}
          {isLoading && !data &&
            Array.from({ length: 6 }).map((_, i) => (
              <LiveMetricCard
                key={`skeleton-${i}`}
                metric={{
                  id: `skeleton-${i}`,
                  name: '',
                  value: 0,
                }}
                loading
              />
            ))}
        </div>
      )}

      {/* Empty state */}
      {!isLoading && data?.metrics.length === 0 && (
        <div className="bg-white dark:bg-gray-800 rounded-2xl p-12 text-center">
          <div className="w-16 h-16 bg-gray-100 dark:bg-gray-700 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg className="w-8 h-8 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
          <h3 className="text-lg font-medium text-gray-900 dark:text-white mb-2">
            داده‌ای یافت نشد
          </h3>
          <p className="text-gray-500 dark:text-gray-400">
            در این بازه زمانی داده‌ای برای نمایش وجود ندارد
          </p>
        </div>
      )}
    </div>
  );
};

export default LiveOperationsDashboard;








