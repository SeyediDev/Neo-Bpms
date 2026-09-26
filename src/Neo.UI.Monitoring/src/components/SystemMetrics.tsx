'use client';

import { useState } from 'react';
import { MetricCard } from './MetricCard';
import { GaugeChart } from './GaugeChart';
import { MetricDrillDown } from './MetricDrillDown';

interface DashboardData {
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

interface SystemMetricsProps {
  data: DashboardData | null;
  loading: boolean;
  /** Optional callback for showing toast notifications (for admin panel integration) */
  onShowToast?: (message: string) => void;
  /** Optional callback for showing error toast notifications */
  onShowErrorToast?: (message: string) => void;
}

function formatBytes(bytes: number): string {
  if (bytes === 0) return '0 B';
  const k = 1024;
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + ' ' + sizes[i];
}

function formatUptime(uptime: string): string {
  if (!uptime) return '--';
  // Parse TimeSpan format: "HH:MM:SS.ffffff" or "D.HH:MM:SS.ffffff"
  const parts = uptime.split(':');
  if (parts.length >= 3) {
    const hours = parseInt(parts[0]);
    const minutes = parseInt(parts[1]);
    const seconds = parseInt(parts[2].split('.')[0]);
    
    if (hours >= 24) {
      const days = Math.floor(hours / 24);
      return `${days} روز ${hours % 24} ساعت`;
    }
    if (hours > 0) {
      return `${hours} ساعت ${minutes} دقیقه`;
    }
    if (minutes > 0) {
      return `${minutes} دقیقه ${seconds} ثانیه`;
    }
    return `${seconds} ثانیه`;
  }
  return uptime;
}

interface DrillDownInfo {
  metricName: string;
  displayName: string;
  unit?: string;
}

export function SystemMetrics({ data, loading, onShowToast, onShowErrorToast }: SystemMetricsProps) {
  const [drillDown, setDrillDown] = useState<DrillDownInfo | null>(null);
  const [resetting, setResetting] = useState(false);
  
  const system = data?.system;
  const app = data?.application;

  const memoryPercent = system?.memoryTotalBytes && system?.memoryUsedBytes
    ? (system.memoryUsedBytes / system.memoryTotalBytes) * 100
    : 0;

  const successRate = app?.totalRequests && app?.totalRequests > 0
    ? ((app.successfulRequests || 0) / app.totalRequests) * 100
    : 100;

  const getStatus = (value: number, thresholds: { warning: number; critical: number }) => {
    if (value >= thresholds.critical) return 'critical';
    if (value >= thresholds.warning) return 'warning';
    return 'healthy';
  };

  const openDrillDown = (metricName: string, displayName: string, unit?: string) => {
    setDrillDown({ metricName, displayName, unit });
  };

  const resetAllMetrics = async () => {
    if (!confirm('آیا از ریست کردن کلیه متریک‌ها اطمینان دارید؟ این عمل تمام آمار را به صفر برمی‌گرداند.')) {
      return;
    }

    setResetting(true);
    try {
      const response = await fetch('/api/monitoring/metrics/reset', {
        method: 'POST',
      });

      if (response.ok) {
        const message = 'کلیه متریک‌ها با موفقیت ریست شدند';
        if (onShowToast) {
          onShowToast(message);
        } else {
          alert(message);
        }
        // Refresh data after reset
        window.location.reload();
      } else {
        const errorMessage = 'خطا در ریست کردن متریک‌ها';
        if (onShowErrorToast) {
          onShowErrorToast(errorMessage);
        } else {
          alert(errorMessage);
        }
      }
    } catch (error) {
      console.error('Failed to reset metrics:', error);
      const errorMessage = 'خطا در ریست کردن متریک‌ها';
      if (onShowErrorToast) {
        onShowErrorToast(errorMessage);
      } else {
        alert(errorMessage);
      }
    } finally {
      setResetting(false);
    }
  };

  return (
    <div className="space-y-8">
      {/* Header with Reset Button */}
      <div className="flex justify-end">
        <button
          onClick={resetAllMetrics}
          disabled={resetting}
          className="px-4 py-2 bg-orange-600 hover:bg-orange-700 disabled:bg-elevated disabled:cursor-not-allowed text-text rounded-lg transition-colors flex items-center gap-2"
        >
          {resetting ? (
            <>
              <svg className="w-4 h-4 animate-spin" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              در حال ریست...
            </>
          ) : (
            <>
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              ریست همه متریک‌ها
            </>
          )}
        </button>
      </div>

      {/* System Gauges */}
      <section>
        <h2 className="text-lg font-semibold text-text mb-4 flex items-center gap-2">
          <svg className="w-5 h-5 text-neo-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 3v2m6-2v2M9 19v2m6-2v2M5 9H3m2 6H3m18-6h-2m2 6h-2M7 19h10a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v10a2 2 0 002 2zM9 9h6v6H9V9z" />
          </svg>
          منابع سیستم
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <GaugeChart
            label="CPU"
            value={system?.cpuUsagePercent || 0}
            max={100}
            unit="%"
            status={getStatus(system?.cpuUsagePercent || 0, { warning: 70, critical: 90 })}
            metricName="process.cpu_time"
            onClick={() => openDrillDown('process.cpu_time', 'CPU Usage', '%')}
          />
          <GaugeChart
            label="Memory"
            value={memoryPercent}
            max={100}
            unit="%"
            subtitle={`${formatBytes(system?.memoryUsedBytes || 0)} / ${formatBytes(system?.memoryTotalBytes || 0)}`}
            status={getStatus(memoryPercent, { warning: 80, critical: 95 })}
            metricName="dotnet.process.memory.working_set"
            onClick={() => openDrillDown('dotnet.process.memory.working_set', 'Memory Usage', 'bytes')}
          />
          <GaugeChart
            label="نرخ موفقیت"
            value={successRate}
            max={100}
            unit="%"
            status={getStatus(100 - successRate, { warning: 5, critical: 10 })}
            metricName="http.server.request.duration"
            onClick={() => openDrillDown('http.server.request.duration', 'Success Rate', '%')}
          />
        </div>
      </section>

      {/* Quick Stats */}
      <section>
        <h2 className="text-lg font-semibold text-text mb-4 flex items-center gap-2">
          <svg className="w-5 h-5 text-neo-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
          </svg>
          آمار سریع
        </h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <MetricCard
            title="زمان فعالیت"
            value={formatUptime(system?.uptime || '')}
            status="healthy"
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            }
          />
          <MetricCard
            title="ترد‌های فعال"
            value={system?.threadCount || 0}
            status="neutral"
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
            }
          />
          <MetricCard
            title="درخواست‌های فعال"
            value={app?.activeRequests || 0}
            status={getStatus(app?.activeRequests || 0, { warning: 100, critical: 500 })}
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
              </svg>
            }
          />
          <MetricCard
            title="میانگین پاسخ"
            value={`${(app?.averageResponseTimeMs || 0).toFixed(0)} ms`}
            status={getStatus(app?.averageResponseTimeMs || 0, { warning: 500, critical: 2000 })}
            icon={
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
              </svg>
            }
          />
        </div>
      </section>

      {/* GC Stats */}
      <section>
        <h2 className="text-lg font-semibold text-text mb-4 flex items-center gap-2">
          <svg className="w-5 h-5 text-neo-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
          Garbage Collection
        </h2>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <MetricCard
            title="GC Gen 0"
            value={system?.gcGen0Collections || 0}
            status="neutral"
          />
          <MetricCard
            title="GC Gen 1"
            value={system?.gcGen1Collections || 0}
            status="neutral"
          />
          <MetricCard
            title="GC Gen 2"
            value={system?.gcGen2Collections || 0}
            status={getStatus(system?.gcGen2Collections || 0, { warning: 10, critical: 50 })}
          />
          <MetricCard
            title="GC Total Memory"
            value={formatBytes(system?.gcTotalMemory || 0)}
            status="neutral"
          />
        </div>
      </section>

      {/* Top Metrics */}
      {data?.topMetrics && data.topMetrics.length > 0 && (
        <section>
          <h2 className="text-lg font-semibold text-text mb-4 flex items-center gap-2">
            <svg className="w-5 h-5 text-neo-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
            متریک‌های فعال
          </h2>
          <div className="bg-surface backdrop-blur-sm border border-border rounded-xl p-4">
            <div className="flex flex-wrap gap-2">
              {data.topMetrics.map((metric, index) => (
                <button
                  key={index}
                  onClick={() => openDrillDown(metric, metric)}
                  className="px-3 py-1 bg-elevated text-subtle rounded-full text-sm font-mono hover:bg-hover hover:text-text transition-colors cursor-pointer"
                >
                  {metric}
                </button>
              ))}
            </div>
          </div>
        </section>
      )}

      {/* Drill-Down Modal */}
      {drillDown && (
        <MetricDrillDown
          metricName={drillDown.metricName}
          displayName={drillDown.displayName}
          unit={drillDown.unit}
          onClose={() => setDrillDown(null)}
        />
      )}
    </div>
  );
}

