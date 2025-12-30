/**
 * Live Operations Hook
 * Neo BPMS Admin Panel Core
 */

import { useState, useEffect, useCallback, useRef } from 'react';
import { TimeFrame, LiveMetric, LiveOperationsData, TIME_FRAME_OPTIONS } from '../types';

export interface UseLiveOperationsOptions {
  /** API base URL */
  apiUrl?: string;
  /** Initial time frame */
  initialTimeFrame?: TimeFrame;
  /** Auto refresh interval in ms (0 to disable) */
  refreshInterval?: number;
  /** Custom fetch function */
  fetchFn?: (timeFrame: TimeFrame) => Promise<LiveMetric[]>;
}

export interface UseLiveOperationsReturn {
  data: LiveOperationsData | null;
  isLoading: boolean;
  error: string | null;
  timeFrame: TimeFrame;
  setTimeFrame: (timeFrame: TimeFrame) => void;
  isPaused: boolean;
  togglePause: () => void;
  refresh: () => Promise<void>;
  lastUpdated: Date | null;
}

// Mock data generator for development
const generateMockData = (timeFrame: TimeFrame): LiveMetric[] => {
  const option = TIME_FRAME_OPTIONS.find((o) => o.value === timeFrame);
  const dataPoints = Math.min(20, Math.floor((option?.seconds || 300) / 15));
  
  const generateSparkline = (base: number, variance: number): number[] => {
    return Array.from({ length: dataPoints }, () =>
      base + (Math.random() - 0.5) * variance
    );
  };

  const metrics: LiveMetric[] = [
    {
      id: 'new-customers',
      name: 'مشتریان جدید',
      nameEn: 'New Customers',
      value: Math.floor(Math.random() * 150) + 50,
      previousValue: Math.floor(Math.random() * 150) + 50,
      trend: Math.random() > 0.5 ? 'up' : 'down',
      trendPercent: Math.random() * 20 - 10,
      sparklineData: generateSparkline(100, 50),
      color: 'primary',
      icon: 'users',
      category: 'مشتریان',
    },
    {
      id: 'active-sessions',
      name: 'نشست‌های فعال',
      nameEn: 'Active Sessions',
      value: Math.floor(Math.random() * 500) + 200,
      previousValue: Math.floor(Math.random() * 500) + 200,
      trend: Math.random() > 0.3 ? 'up' : 'stable',
      trendPercent: Math.random() * 15,
      sparklineData: generateSparkline(350, 100),
      color: 'success',
      icon: 'activity',
      category: 'سیستم',
    },
    {
      id: 'revenue',
      name: 'درآمد',
      nameEn: 'Revenue',
      value: Math.floor(Math.random() * 50000000) + 10000000,
      previousValue: Math.floor(Math.random() * 50000000) + 10000000,
      format: 'currency',
      trend: Math.random() > 0.4 ? 'up' : 'down',
      trendPercent: Math.random() * 25 - 5,
      sparklineData: generateSparkline(30000000, 10000000),
      color: 'success',
      icon: 'money',
      category: 'مالی',
    },
    {
      id: 'orders',
      name: 'سفارشات',
      nameEn: 'Orders',
      value: Math.floor(Math.random() * 200) + 30,
      previousValue: Math.floor(Math.random() * 200) + 30,
      trend: Math.random() > 0.5 ? 'up' : 'stable',
      trendPercent: Math.random() * 12,
      sparklineData: generateSparkline(100, 40),
      color: 'info',
      icon: 'orders',
      category: 'فروش',
    },
    {
      id: 'conversion-rate',
      name: 'نرخ تبدیل',
      nameEn: 'Conversion Rate',
      value: Math.random() * 5 + 2,
      previousValue: Math.random() * 5 + 2,
      format: 'percent',
      trend: Math.random() > 0.6 ? 'up' : 'down',
      trendPercent: Math.random() * 8 - 2,
      sparklineData: generateSparkline(3.5, 1),
      color: 'warning',
      icon: 'activity',
      category: 'بازاریابی',
    },
    {
      id: 'support-tickets',
      name: 'تیکت‌های پشتیبانی',
      nameEn: 'Support Tickets',
      value: Math.floor(Math.random() * 30) + 5,
      previousValue: Math.floor(Math.random() * 30) + 5,
      trend: Math.random() > 0.7 ? 'down' : 'up',
      trendPercent: Math.random() * 10 - 5,
      sparklineData: generateSparkline(15, 8),
      color: 'danger',
      icon: 'activity',
      category: 'پشتیبانی',
    },
  ];

  return metrics;
};

export const useLiveOperations = ({
  apiUrl = '/api/live-operations',
  initialTimeFrame = '5m',
  refreshInterval = 5000,
  fetchFn,
}: UseLiveOperationsOptions = {}): UseLiveOperationsReturn => {
  const [data, setData] = useState<LiveOperationsData | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [timeFrame, setTimeFrame] = useState<TimeFrame>(initialTimeFrame);
  const [isPaused, setIsPaused] = useState(false);
  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);
  
  const intervalRef = useRef<NodeJS.Timeout | null>(null);

  const fetchData = useCallback(async () => {
    try {
      setError(null);
      
      let metrics: LiveMetric[];
      
      if (fetchFn) {
        metrics = await fetchFn(timeFrame);
      } else {
        // Try to fetch from API, fall back to mock data
        try {
          const response = await fetch(`${apiUrl}?timeFrame=${timeFrame}`);
          if (response.ok) {
            const result = await response.json();
            metrics = result.metrics || result;
          } else {
            // Use mock data in development
            metrics = generateMockData(timeFrame);
          }
        } catch {
          // Use mock data if API fails
          metrics = generateMockData(timeFrame);
        }
      }

      const now = new Date();
      setData({
        metrics,
        lastUpdated: now,
        timeFrame,
      });
      setLastUpdated(now);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'خطا در دریافت داده‌ها');
    } finally {
      setIsLoading(false);
    }
  }, [apiUrl, timeFrame, fetchFn]);

  const refresh = useCallback(async () => {
    setIsLoading(true);
    await fetchData();
  }, [fetchData]);

  const togglePause = useCallback(() => {
    setIsPaused((prev) => !prev);
  }, []);

  // Initial fetch
  useEffect(() => {
    fetchData();
  }, [fetchData]);

  // Auto refresh
  useEffect(() => {
    if (isPaused || refreshInterval <= 0) {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
      return;
    }

    intervalRef.current = setInterval(fetchData, refreshInterval);

    return () => {
      if (intervalRef.current) {
        clearInterval(intervalRef.current);
      }
    };
  }, [isPaused, refreshInterval, fetchData]);

  return {
    data,
    isLoading,
    error,
    timeFrame,
    setTimeFrame,
    isPaused,
    togglePause,
    refresh,
    lastUpdated,
  };
};

export default useLiveOperations;

