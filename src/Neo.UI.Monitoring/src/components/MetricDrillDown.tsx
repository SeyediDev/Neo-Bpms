'use client';

import { useState, useMemo } from 'react';
import { TimeSeriesChart } from './TimeSeriesChart';
import { TimeRangeSelector, TIME_RANGES, TimeRange } from './TimeRangeSelector';
import { MetricCard } from './MetricCard';
import { useMetricTimeSeries } from '../hooks/useMetricTimeSeries';

interface MetricDrillDownProps {
  metricName: string;
  displayName?: string;
  unit?: string;
  onClose: () => void;
}

export function MetricDrillDown({ metricName, displayName, unit, onClose }: MetricDrillDownProps) {
  const [selectedRange, setSelectedRange] = useState<TimeRange>(TIME_RANGES[2]); // Default: 1h

  const { from, to } = useMemo(() => {
    const now = new Date();
    const fromDate = new Date(now.getTime() - selectedRange.minutes * 60 * 1000);
    return { from: fromDate, to: now };
  }, [selectedRange]);

  // Calculate aggregation interval based on time range
  const aggregationIntervalSeconds = useMemo(() => {
    // Aim for ~60 data points
    return Math.max(60, Math.floor((selectedRange.minutes * 60) / 60));
  }, [selectedRange]);

  const { data, stats, loading, error } = useMetricTimeSeries({
    metricName,
    from,
    to,
    aggregationIntervalSeconds,
    autoRefresh: true,
    refreshIntervalMs: 5000,
  });

  const formatValue = (value: number) => {
    if (unit === '%') return `${value.toFixed(1)}%`;
    if (unit === 'bytes' || unit === 'B') {
      if (value >= 1073741824) return `${(value / 1073741824).toFixed(2)} GB`;
      if (value >= 1048576) return `${(value / 1048576).toFixed(2)} MB`;
      if (value >= 1024) return `${(value / 1024).toFixed(2)} KB`;
      return `${value.toFixed(0)} B`;
    }
    if (unit === 'ms') return `${value.toFixed(1)} ms`;
    return value.toFixed(2);
  };

  return (
    <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div className="bg-slate-900 border border-slate-700 rounded-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden shadow-2xl">
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800">
          <div className="flex items-center gap-4">
            <button
              onClick={onClose}
              className="p-2 rounded-lg hover:bg-slate-800 transition-colors text-slate-400 hover:text-white"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
              </svg>
            </button>
            <div>
              <h2 className="text-lg font-semibold text-white">
                {displayName || metricName}
              </h2>
              <p className="text-sm text-slate-500 font-mono">{metricName}</p>
            </div>
          </div>
          <TimeRangeSelector value={selectedRange.value} onChange={setSelectedRange} />
        </div>

        {/* Content */}
        <div className="p-6 overflow-y-auto max-h-[calc(90vh-80px)]">
          {error && (
            <div className="bg-red-500/10 border border-red-500/30 rounded-lg p-4 mb-6 text-red-400">
              {error}
            </div>
          )}

          {/* Main Chart */}
          <div className="mb-6">
            <TimeSeriesChart
              title={`${displayName || metricName} - ${selectedRange.labelFa}`}
              data={data?.dataPoints || []}
              unit={unit || data?.unit}
              loading={loading}
              height={300}
              showArea={true}
              formatValue={unit ? formatValue : undefined}
            />
          </div>

          {/* Stats Grid */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            <MetricCard
              title="مقدار فعلی"
              value={formatValue(stats?.currentValue || 0)}
              status="neutral"
              icon={
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
                </svg>
              }
            />
            <MetricCard
              title="میانگین"
              value={formatValue(stats?.avgValue || 0)}
              status="neutral"
              icon={
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              }
            />
            <MetricCard
              title="حداکثر"
              value={formatValue(stats?.maxValue || 0)}
              status="warning"
              icon={
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 10l7-7m0 0l7 7m-7-7v18" />
                </svg>
              }
            />
            <MetricCard
              title="حداقل"
              value={formatValue(stats?.minValue || 0)}
              status="healthy"
              icon={
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 14l-7 7m0 0l-7-7m7 7V3" />
                </svg>
              }
            />
          </div>

          {/* Additional Info */}
          <div className="mt-6 bg-slate-800/50 rounded-xl p-4">
            <h3 className="text-sm font-medium text-slate-400 mb-3">اطلاعات تکمیلی</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
              <div>
                <span className="text-slate-500">نوع متریک:</span>
                <span className="text-white mr-2">{data?.metricType || '-'}</span>
              </div>
              <div>
                <span className="text-slate-500">تعداد نمونه:</span>
                <span className="text-white mr-2">{stats?.count?.toLocaleString() || 0}</span>
              </div>
              <div>
                <span className="text-slate-500">مجموع:</span>
                <span className="text-white mr-2">{formatValue(stats?.sumValue || 0)}</span>
              </div>
              <div>
                <span className="text-slate-500">واحد:</span>
                <span className="text-white mr-2">{unit || data?.unit || '-'}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

