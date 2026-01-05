/**
 * Sparkline Chart Component
 * Neo BPMS Admin Panel Core
 */

import React, { useMemo } from 'react';
import clsx from 'clsx';

export interface SparklineProps {
  data: number[];
  width?: number;
  height?: number;
  color?: 'primary' | 'success' | 'warning' | 'danger' | 'info' | string;
  showArea?: boolean;
  strokeWidth?: number;
  className?: string;
}

const colorMap: Record<string, { stroke: string; fill: string }> = {
  primary: { stroke: '#a855f7', fill: 'rgba(168, 85, 247, 0.2)' },
  success: { stroke: '#10b981', fill: 'rgba(16, 185, 129, 0.2)' },
  warning: { stroke: '#f59e0b', fill: 'rgba(245, 158, 11, 0.2)' },
  danger: { stroke: '#ef4444', fill: 'rgba(239, 68, 68, 0.2)' },
  info: { stroke: '#3b82f6', fill: 'rgba(59, 130, 246, 0.2)' },
};

export const Sparkline: React.FC<SparklineProps> = ({
  data,
  width = 100,
  height = 32,
  color = 'primary',
  showArea = true,
  strokeWidth = 2,
  className,
}) => {
  const { pathD, areaD, colors } = useMemo(() => {
    if (!data || data.length < 2) {
      return { pathD: '', areaD: '', colors: colorMap.primary };
    }

    const colors = colorMap[color] || { stroke: color, fill: `${color}33` };
    const padding = strokeWidth;
    const chartWidth = width - padding * 2;
    const chartHeight = height - padding * 2;

    const min = Math.min(...data);
    const max = Math.max(...data);
    const range = max - min || 1;

    const points = data.map((value, index) => {
      const x = padding + (index / (data.length - 1)) * chartWidth;
      const y = padding + chartHeight - ((value - min) / range) * chartHeight;
      return { x, y };
    });

    // Create path for line
    const pathD = points
      .map((point, index) => `${index === 0 ? 'M' : 'L'} ${point.x} ${point.y}`)
      .join(' ');

    // Create path for area
    const areaD = `${pathD} L ${points[points.length - 1].x} ${height - padding} L ${padding} ${height - padding} Z`;

    return { pathD, areaD, colors };
  }, [data, width, height, color, strokeWidth]);

  if (!data || data.length < 2) {
    return (
      <div
        className={clsx('flex items-center justify-center text-gray-400 text-xs', className)}
        style={{ width, height }}
      >
        —
      </div>
    );
  }

  return (
    <svg
      width={width}
      height={height}
      className={clsx('overflow-visible', className)}
      viewBox={`0 0 ${width} ${height}`}
    >
      {/* Gradient fill area */}
      {showArea && (
        <defs>
          <linearGradient id={`sparkline-gradient-${color}`} x1="0" y1="0" x2="0" y2="1">
            <stop offset="0%" stopColor={colors.fill} />
            <stop offset="100%" stopColor="transparent" />
          </linearGradient>
        </defs>
      )}

      {showArea && (
        <path
          d={areaD}
          fill={`url(#sparkline-gradient-${color})`}
          opacity={0.5}
        />
      )}

      {/* Line */}
      <path
        d={pathD}
        fill="none"
        stroke={colors.stroke}
        strokeWidth={strokeWidth}
        strokeLinecap="round"
        strokeLinejoin="round"
      />

      {/* End dot */}
      {data.length > 0 && (
        <circle
          cx={width - strokeWidth}
          cy={
            strokeWidth +
            (height - strokeWidth * 2) -
            ((data[data.length - 1] - Math.min(...data)) /
              (Math.max(...data) - Math.min(...data) || 1)) *
              (height - strokeWidth * 2)
          }
          r={strokeWidth + 1}
          fill={colors.stroke}
        />
      )}
    </svg>
  );
};

export default Sparkline;







