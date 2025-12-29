'use client';

import { motion } from 'framer-motion';
import clsx from 'clsx';

interface GaugeChartProps {
  label: string;
  value: number;
  max: number;
  unit?: string;
  subtitle?: string;
  status?: 'healthy' | 'warning' | 'critical' | 'neutral';
  onClick?: () => void;
  metricName?: string;
}

export function GaugeChart({
  label,
  value,
  max,
  unit = '',
  subtitle,
  status = 'neutral',
  onClick,
  metricName,
}: GaugeChartProps) {
  const percentage = Math.min((value / max) * 100, 100);
  const circumference = 2 * Math.PI * 45;
  const strokeDashoffset = circumference - (percentage / 100) * circumference;

  const statusColors = {
    healthy: '#10B981',
    warning: '#F59E0B',
    critical: '#EF4444',
    neutral: '#0EA5E9',
  };

  return (
    <div 
      className={clsx(
        "metric-card flex flex-col items-center",
        onClick && "cursor-pointer hover:bg-slate-700/30 transition-colors"
      )}
      onClick={onClick}
      role={onClick ? "button" : undefined}
      tabIndex={onClick ? 0 : undefined}
      onKeyDown={onClick ? (e) => { if (e.key === 'Enter' || e.key === ' ') onClick(); } : undefined}
    >
      <div className="relative w-32 h-32">
        {/* Background Circle */}
        <svg className="w-full h-full transform -rotate-90" viewBox="0 0 100 100">
          <circle
            cx="50"
            cy="50"
            r="45"
            fill="none"
            stroke="#1E293B"
            strokeWidth="10"
          />
          {/* Progress Circle */}
          <motion.circle
            cx="50"
            cy="50"
            r="45"
            fill="none"
            stroke={statusColors[status]}
            strokeWidth="10"
            strokeLinecap="round"
            initial={{ strokeDashoffset: circumference }}
            animate={{ strokeDashoffset }}
            transition={{ duration: 1, ease: 'easeOut' }}
            style={{
              strokeDasharray: circumference,
            }}
          />
        </svg>

        {/* Center Text */}
        <div className="absolute inset-0 flex flex-col items-center justify-center">
          <motion.span
            key={value}
            initial={{ scale: 0.8, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            className={clsx(
              'text-2xl font-bold',
              status === 'healthy' && 'text-status-healthy',
              status === 'warning' && 'text-status-warning',
              status === 'critical' && 'text-status-critical',
              status === 'neutral' && 'text-neo-400'
            )}
          >
            {value.toFixed(1)}{unit}
          </motion.span>
        </div>
      </div>

      <h3 className="text-slate-400 mt-4 text-sm">{label}</h3>
      {subtitle && (
        <p className="text-slate-500 text-xs mt-1 font-mono" dir="ltr">{subtitle}</p>
      )}
    </div>
  );
}

