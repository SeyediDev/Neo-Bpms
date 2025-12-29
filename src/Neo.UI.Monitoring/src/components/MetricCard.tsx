'use client';

import { motion } from 'framer-motion';
import clsx from 'clsx';

interface MetricCardProps {
  title: string;
  value: string | number;
  subtitle?: string;
  icon?: React.ReactNode;
  status?: 'healthy' | 'warning' | 'critical' | 'neutral';
  trend?: {
    direction: 'up' | 'down' | 'stable';
    percentage: number;
  };
  className?: string;
}

export function MetricCard({
  title,
  value,
  subtitle,
  icon,
  status = 'neutral',
  trend,
  className,
}: MetricCardProps) {
  const statusColors = {
    healthy: 'text-status-healthy',
    warning: 'text-status-warning',
    critical: 'text-status-critical',
    neutral: 'text-slate-400',
  };

  const statusBorders = {
    healthy: 'border-l-status-healthy',
    warning: 'border-l-status-warning',
    critical: 'border-l-status-critical',
    neutral: 'border-l-slate-700',
  };

  const trendIcons = {
    up: (
      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 10l7-7m0 0l7 7m-7-7v18" />
      </svg>
    ),
    down: (
      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 14l-7 7m0 0l-7-7m7 7V3" />
      </svg>
    ),
    stable: (
      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 12h14" />
      </svg>
    ),
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.3 }}
      className={clsx(
        'metric-card border-l-4',
        statusBorders[status],
        className
      )}
    >
      <div className="flex items-start justify-between mb-4">
        <div className={clsx('p-2 rounded-lg bg-slate-800', statusColors[status])}>
          {icon || (
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          )}
        </div>
        {trend && (
          <div className={clsx(
            'flex items-center gap-1 text-sm',
            trend.direction === 'up' ? 'text-status-healthy' :
            trend.direction === 'down' ? 'text-status-critical' :
            'text-slate-400'
          )}>
            {trendIcons[trend.direction]}
            <span>{trend.percentage}%</span>
          </div>
        )}
      </div>

      <h3 className="text-sm text-slate-400 mb-1">{title}</h3>
      <motion.p
        key={String(value)}
        initial={{ opacity: 0, y: 5 }}
        animate={{ opacity: 1, y: 0 }}
        className={clsx('text-3xl font-bold', statusColors[status])}
      >
        {value}
      </motion.p>
      {subtitle && (
        <p className="text-sm text-slate-500 mt-2">{subtitle}</p>
      )}
    </motion.div>
  );
}

