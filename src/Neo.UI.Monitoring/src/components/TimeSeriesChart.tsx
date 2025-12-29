'use client';

import { useMemo } from 'react';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Area,
  AreaChart,
} from 'recharts';
import { format } from 'date-fns-jalali';

interface TimeSeriesPoint {
  timestamp: string;
  value: number;
}

interface TimeSeriesChartProps {
  title: string;
  data: TimeSeriesPoint[];
  unit?: string;
  color?: string;
  loading?: boolean;
  height?: number;
  showArea?: boolean;
  formatValue?: (value: number) => string;
}

export function TimeSeriesChart({
  title,
  data,
  unit = '',
  color = '#06b6d4',
  loading = false,
  height = 200,
  showArea = true,
  formatValue,
}: TimeSeriesChartProps) {
  const chartData = useMemo(() => {
    return data.map((point) => ({
      time: new Date(point.timestamp).getTime(),
      value: point.value,
      formattedTime: format(new Date(point.timestamp), 'HH:mm:ss'),
    }));
  }, [data]);

  const formatYAxis = (value: number) => {
    if (formatValue) return formatValue(value);
    if (value >= 1000000000) return `${(value / 1000000000).toFixed(1)}G`;
    if (value >= 1000000) return `${(value / 1000000).toFixed(1)}M`;
    if (value >= 1000) return `${(value / 1000).toFixed(1)}K`;
    return value.toFixed(1);
  };

  const CustomTooltip = ({ active, payload, label }: any) => {
    if (active && payload && payload.length) {
      const value = payload[0].value;
      const displayValue = formatValue ? formatValue(value) : value.toFixed(2);
      return (
        <div className="bg-slate-900/95 backdrop-blur-sm border border-slate-700 rounded-lg px-3 py-2 shadow-xl">
          <p className="text-slate-400 text-xs mb-1">
            {format(new Date(label), 'yyyy/MM/dd HH:mm:ss')}
          </p>
          <p className="text-white font-semibold">
            {displayValue} {unit}
          </p>
        </div>
      );
    }
    return null;
  };

  if (loading) {
    return (
      <div className="bg-slate-900/50 backdrop-blur-sm border border-slate-800 rounded-xl p-4">
        <h3 className="text-sm font-medium text-slate-400 mb-3">{title}</h3>
        <div className="flex items-center justify-center" style={{ height }}>
          <div className="animate-pulse flex flex-col items-center gap-2">
            <div className="w-8 h-8 rounded-full border-2 border-neo-500 border-t-transparent animate-spin" />
            <span className="text-slate-500 text-sm">در حال بارگذاری...</span>
          </div>
        </div>
      </div>
    );
  }

  if (!data || data.length === 0) {
    return (
      <div className="bg-slate-900/50 backdrop-blur-sm border border-slate-800 rounded-xl p-4">
        <h3 className="text-sm font-medium text-slate-400 mb-3">{title}</h3>
        <div className="flex items-center justify-center text-slate-500" style={{ height }}>
          داده‌ای موجود نیست
        </div>
      </div>
    );
  }

  const ChartComponent = showArea ? AreaChart : LineChart;

  return (
    <div className="bg-slate-900/50 backdrop-blur-sm border border-slate-800 rounded-xl p-4">
      <div className="flex items-center justify-between mb-3">
        <h3 className="text-sm font-medium text-slate-400">{title}</h3>
        {unit && <span className="text-xs text-slate-500">{unit}</span>}
      </div>
      <ResponsiveContainer width="100%" height={height}>
        <ChartComponent data={chartData} margin={{ top: 5, right: 5, left: 0, bottom: 5 }}>
          <defs>
            <linearGradient id={`gradient-${title}`} x1="0" y1="0" x2="0" y2="1">
              <stop offset="0%" stopColor={color} stopOpacity={0.3} />
              <stop offset="100%" stopColor={color} stopOpacity={0} />
            </linearGradient>
          </defs>
          <CartesianGrid strokeDasharray="3 3" stroke="#334155" vertical={false} />
          <XAxis
            dataKey="formattedTime"
            stroke="#64748b"
            fontSize={10}
            tickLine={false}
            axisLine={false}
          />
          <YAxis
            stroke="#64748b"
            fontSize={10}
            tickLine={false}
            axisLine={false}
            tickFormatter={formatYAxis}
            width={45}
          />
          <Tooltip content={<CustomTooltip />} />
          {showArea ? (
            <Area
              type="monotone"
              dataKey="value"
              stroke={color}
              strokeWidth={2}
              fill={`url(#gradient-${title})`}
              dot={false}
              activeDot={{ r: 4, fill: color, strokeWidth: 0 }}
            />
          ) : (
            <Line
              type="monotone"
              dataKey="value"
              stroke={color}
              strokeWidth={2}
              dot={false}
              activeDot={{ r: 4, fill: color, strokeWidth: 0 }}
            />
          )}
        </ChartComponent>
      </ResponsiveContainer>
    </div>
  );
}

