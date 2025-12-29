'use client';

import { useState, useEffect, useCallback } from 'react';

interface TimeSeriesPoint {
  timestamp: string;
  value: number;
}

interface MetricTimeSeries {
  metricName: string;
  metricType: string;
  unit?: string;
  dataPoints: TimeSeriesPoint[];
}

interface MetricStats {
  metricName: string;
  metricType: string;
  currentValue: number;
  minValue: number;
  maxValue: number;
  avgValue: number;
  sumValue: number;
  count: number;
  firstTimestamp: string;
  lastTimestamp: string;
}

interface UseMetricTimeSeriesOptions {
  metricName: string;
  from?: Date;
  to?: Date;
  aggregationIntervalSeconds?: number;
  autoRefresh?: boolean;
  refreshIntervalMs?: number;
}

export function useMetricTimeSeries(options: UseMetricTimeSeriesOptions) {
  const {
    metricName,
    from,
    to,
    aggregationIntervalSeconds = 60,
    autoRefresh = true,
    refreshIntervalMs = 5000,
  } = options;

  const [data, setData] = useState<MetricTimeSeries | null>(null);
  const [stats, setStats] = useState<MetricStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Helper function to get property regardless of case (PascalCase vs camelCase)
  const getProp = (obj: any, key: string) => {
    if (!obj) return undefined;
    return obj[key] ?? obj[key.charAt(0).toUpperCase() + key.slice(1)];
  };

  const fetchData = useCallback(async () => {
    if (!metricName) return;

    try {
      setError(null);

      const params = new URLSearchParams();
      if (from) params.set('from', from.toISOString());
      if (to) params.set('to', to.toISOString());
      if (aggregationIntervalSeconds) params.set('aggregationIntervalSeconds', aggregationIntervalSeconds.toString());

      const queryString = params.toString();
      const url = `/api/monitoring/metrics/${encodeURIComponent(metricName)}/timeseries${queryString ? `?${queryString}` : ''}`;

      const response = await fetch(url);
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();

      // Normalize data (handle both PascalCase and camelCase)
      const normalizedData: MetricTimeSeries = {
        metricName: getProp(result, 'metricName') || metricName,
        metricType: getProp(result, 'metricType') || 'unknown',
        unit: getProp(result, 'unit'),
        dataPoints: (getProp(result, 'dataPoints') || []).map((dp: any) => ({
          timestamp: getProp(dp, 'timestamp'),
          value: getProp(dp, 'value') || 0,
        })),
      };

      setData(normalizedData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'خطا در دریافت اطلاعات');
      console.error('Error fetching time series:', err);
    } finally {
      setLoading(false);
    }
  }, [metricName, from, to, aggregationIntervalSeconds]);

  const fetchStats = useCallback(async () => {
    if (!metricName) return;

    try {
      const params = new URLSearchParams();
      if (from) params.set('from', from.toISOString());
      if (to) params.set('to', to.toISOString());

      const queryString = params.toString();
      const url = `/api/monitoring/metrics/${encodeURIComponent(metricName)}/stats${queryString ? `?${queryString}` : ''}`;

      const response = await fetch(url);
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();

      // Normalize data
      const normalizedStats: MetricStats = {
        metricName: getProp(result, 'metricName') || metricName,
        metricType: getProp(result, 'metricType') || 'unknown',
        currentValue: getProp(result, 'currentValue') || 0,
        minValue: getProp(result, 'minValue') || 0,
        maxValue: getProp(result, 'maxValue') || 0,
        avgValue: getProp(result, 'avgValue') || 0,
        sumValue: getProp(result, 'sumValue') || 0,
        count: getProp(result, 'count') || 0,
        firstTimestamp: getProp(result, 'firstTimestamp'),
        lastTimestamp: getProp(result, 'lastTimestamp'),
      };

      setStats(normalizedStats);
    } catch (err) {
      console.error('Error fetching stats:', err);
    }
  }, [metricName, from, to]);

  const refresh = useCallback(() => {
    setLoading(true);
    fetchData();
    fetchStats();
  }, [fetchData, fetchStats]);

  useEffect(() => {
    fetchData();
    fetchStats();

    if (autoRefresh) {
      const interval = setInterval(() => {
        fetchData();
        fetchStats();
      }, refreshIntervalMs);
      return () => clearInterval(interval);
    }
  }, [fetchData, fetchStats, autoRefresh, refreshIntervalMs]);

  return {
    data,
    stats,
    loading,
    error,
    refresh,
  };
}

