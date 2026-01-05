/**
 * Live Metric Card Component
 * Neo BPMS Admin Panel Core
 */

import React from 'react';
import clsx from 'clsx';
import { LiveMetric, TrendDirection } from '../types';
import { Sparkline } from './Sparkline';

export interface LiveMetricCardProps {
  metric: LiveMetric;
  loading?: boolean;
  onClick?: () => void;
  className?: string;
}

const formatValue = (value: number, format?: string): string => {
  if (format === 'currency') {
    return new Intl.NumberFormat('fa-IR').format(value) + ' ریال';
  }
  if (format === 'percent') {
    return value.toFixed(1) + '%';
  }
  if (value >= 1000000) {
    return (value / 1000000).toFixed(1) + 'M';
  }
  if (value >= 1000) {
    return (value / 1000).toFixed(1) + 'K';
  }
  return new Intl.NumberFormat('fa-IR').format(value);
};

const TrendIndicator: React.FC<{ direction: TrendDirection; percent?: number }> = ({
  direction,
  percent,
}) => {
  const colors: Record<TrendDirection, string> = {
    up: 'text-emerald-500',
    down: 'text-red-500',
    stable: 'text-gray-400',
  };

  const icons: Record<TrendDirection, React.ReactNode> = {
    up: (
      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 17l9.2-9.2M17 17V7H7" />
      </svg>
    ),
    down: (
      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 7l-9.2 9.2M7 7v10h10" />
      </svg>
    ),
    stable: (
      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 12h14" />
      </svg>
    ),
  };

  return (
    <div className={clsx('flex items-center gap-1', colors[direction])}>
      {icons[direction]}
      {percent !== undefined && (
        <span className="text-xs font-medium">
          {percent > 0 ? '+' : ''}
          {percent.toFixed(1)}%
        </span>
      )}
    </div>
  );
};

const colorClasses: Record<string, { bg: string; text: string; icon: string }> = {
  primary: {
    bg: 'bg-purple-50 dark:bg-purple-900/20',
    text: 'text-purple-600 dark:text-purple-400',
    icon: 'bg-purple-100 dark:bg-purple-800',
  },
  success: {
    bg: 'bg-emerald-50 dark:bg-emerald-900/20',
    text: 'text-emerald-600 dark:text-emerald-400',
    icon: 'bg-emerald-100 dark:bg-emerald-800',
  },
  warning: {
    bg: 'bg-amber-50 dark:bg-amber-900/20',
    text: 'text-amber-600 dark:text-amber-400',
    icon: 'bg-amber-100 dark:bg-amber-800',
  },
  danger: {
    bg: 'bg-red-50 dark:bg-red-900/20',
    text: 'text-red-600 dark:text-red-400',
    icon: 'bg-red-100 dark:bg-red-800',
  },
  info: {
    bg: 'bg-blue-50 dark:bg-blue-900/20',
    text: 'text-blue-600 dark:text-blue-400',
    icon: 'bg-blue-100 dark:bg-blue-800',
  },
};

const defaultIcons: Record<string, React.ReactNode> = {
  users: (
    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
    </svg>
  ),
  money: (
    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
    </svg>
  ),
  orders: (
    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
    </svg>
  ),
  activity: (
    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
    </svg>
  ),
};

export const LiveMetricCard: React.FC<LiveMetricCardProps> = ({
  metric,
  loading = false,
  onClick,
  className,
}) => {
  const colors = colorClasses[metric.color || 'primary'];

  if (loading) {
    return (
      <div
        className={clsx(
          'bg-white dark:bg-gray-800 rounded-2xl p-5',
          'border border-gray-200 dark:border-gray-700',
          'animate-pulse',
          className
        )}
      >
        <div className="flex items-start justify-between">
          <div className="space-y-3 flex-1">
            <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded w-24" />
            <div className="h-8 bg-gray-200 dark:bg-gray-700 rounded w-32" />
            <div className="h-3 bg-gray-200 dark:bg-gray-700 rounded w-16" />
          </div>
          <div className="w-24 h-10 bg-gray-200 dark:bg-gray-700 rounded" />
        </div>
      </div>
    );
  }

  return (
    <div
      onClick={onClick}
      className={clsx(
        'bg-white dark:bg-gray-800 rounded-2xl p-5',
        'border border-gray-200 dark:border-gray-700',
        'transition-all duration-200',
        onClick && 'cursor-pointer hover:shadow-lg hover:border-purple-300 dark:hover:border-purple-600',
        className
      )}
    >
      <div className="flex items-start justify-between gap-4">
        {/* Content */}
        <div className="flex-1 min-w-0">
          {/* Icon & Title */}
          <div className="flex items-center gap-3 mb-3">
            <div
              className={clsx(
                'w-10 h-10 rounded-xl flex items-center justify-center',
                colors.icon,
                colors.text
              )}
            >
              {metric.icon ? (
                defaultIcons[metric.icon] || defaultIcons.activity
              ) : (
                defaultIcons.activity
              )}
            </div>
            <div>
              <h3 className="text-sm font-medium text-gray-500 dark:text-gray-400">
                {metric.name}
              </h3>
              {metric.category && (
                <span className="text-xs text-gray-400">{metric.category}</span>
              )}
            </div>
          </div>

          {/* Value */}
          <div className="flex items-end gap-2">
            <span className="text-2xl font-bold text-gray-900 dark:text-white">
              {formatValue(metric.value, metric.format)}
            </span>
            {metric.unit && (
              <span className="text-sm text-gray-500 dark:text-gray-400 mb-1">
                {metric.unit}
              </span>
            )}
          </div>

          {/* Trend */}
          {metric.trend && (
            <div className="mt-2">
              <TrendIndicator
                direction={metric.trend}
                percent={metric.trendPercent}
              />
            </div>
          )}
        </div>

        {/* Sparkline */}
        {metric.sparklineData && metric.sparklineData.length > 0 && (
          <div className="flex-shrink-0">
            <Sparkline
              data={metric.sparklineData}
              width={100}
              height={40}
              color={metric.color || 'primary'}
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default LiveMetricCard;







