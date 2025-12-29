'use client';

import { useState, useEffect, useCallback } from 'react';

interface DashboardData {
  generatedAt?: string;
  system?: {
    cpuUsagePercent?: number;
    memoryUsedBytes?: number;
    memoryTotalBytes?: number;
    threadCount?: number;
    uptime?: string;
    gcGen0Collections?: number;
    gcGen1Collections?: number;
    gcGen2Collections?: number;
    gcTotalMemory?: number;
  };
  application?: {
    totalRequests?: number;
    successfulRequests?: number;
    failedRequests?: number;
    activeRequests?: number;
    averageResponseTimeMs?: number;
  };
  topMetrics?: string[];
}

export function useMonitoringData() {
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch('/api/monitoring/dashboard');
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
      const result = await response.json();
      
      // Helper function to get property regardless of case
      const getProp = (obj: any, key: string) => {
        if (!obj) return undefined;
        return obj[key] ?? obj[key.charAt(0).toUpperCase() + key.slice(1)];
      };
      
      // Normalize system data
      const system = getProp(result, 'system') || {};
      const normalizedSystem = {
        cpuUsagePercent: getProp(system, 'cpuUsagePercent') ?? getProp(system, 'cpuUsage') ?? 0,
        memoryUsedBytes: getProp(system, 'memoryUsedBytes') ?? 0,
        memoryTotalBytes: getProp(system, 'memoryTotalBytes') ?? 0,
        threadCount: getProp(system, 'threadCount') ?? 0,
        uptime: getProp(system, 'uptime') ?? '',
        gcGen0Collections: getProp(system, 'gcGen0Collections') ?? 0,
        gcGen1Collections: getProp(system, 'gcGen1Collections') ?? 0,
        gcGen2Collections: getProp(system, 'gcGen2Collections') ?? 0,
        gcTotalMemory: getProp(system, 'gcTotalMemory') ?? 0,
      };
      
      // Normalize application data
      const app = getProp(result, 'application') || {};
      const normalizedApp = {
        totalRequests: getProp(app, 'totalRequests') ?? 0,
        successfulRequests: getProp(app, 'successfulRequests') ?? 0,
        failedRequests: getProp(app, 'failedRequests') ?? 0,
        activeRequests: getProp(app, 'activeRequests') ?? 0,
        averageResponseTimeMs: getProp(app, 'averageResponseTimeMs') ?? 0,
      };
      
      // Normalize topMetrics - extract just the metric names if they are objects
      let topMetrics = getProp(result, 'topMetrics') || [];
      if (Array.isArray(topMetrics) && topMetrics.length > 0 && typeof topMetrics[0] === 'object') {
        topMetrics = topMetrics.map((m: any) => getProp(m, 'metricName') || String(m));
      }
      
      const normalizedData: DashboardData = {
        generatedAt: getProp(result, 'generatedAt'),
        system: normalizedSystem,
        application: normalizedApp,
        topMetrics: topMetrics,
      };
      
      setData(normalizedData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'خطا در دریافت اطلاعات');
      console.error('Error fetching monitoring data:', err);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
    
    // Auto-refresh every 5 seconds
    const interval = setInterval(fetchData, 5000);
    
    return () => clearInterval(interval);
  }, [fetchData]);

  return {
    data,
    loading,
    error,
    refresh: fetchData,
  };
}

