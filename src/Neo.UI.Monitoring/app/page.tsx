'use client';

import { useState, useEffect } from 'react';
import { MetricCard } from '@/components/MetricCard';
import { SystemMetrics } from '@/components/SystemMetrics';
import { LogsPanel } from '@/components/LogsPanel';
import { TracesPanel } from '@/components/TracesPanel';
import { useMonitoringData } from '@/hooks/useMonitoringData';
import { useSignalR } from '@/hooks/useSignalR';

export default function MonitoringDashboard() {
  const [activeTab, setActiveTab] = useState<'overview' | 'logs' | 'traces'>('overview');
  const { data, loading, error, refresh } = useMonitoringData();
  const { isConnected, lastUpdate } = useSignalR();

  return (
    <div className="min-h-screen bg-canvas animated-gradient">
      {/* Header */}
      <header className="sticky top-0 z-50 backdrop-blur-xl bg-canvas border-b border-border">
        <div className="container mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <div className="w-10 h-10 rounded-lg bg-gradient-to-br from-accent to-accent-hover flex items-center justify-center">
                <svg className="w-6 h-6 text-on-accent" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              <div>
                <h1 className="text-xl font-bold text-text">مانیتورینگ سیستم</h1>
                <p className="text-sm text-muted">داشبورد نظارت لحظه‌ای</p>
              </div>
            </div>

            {/* Connection Status */}
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-2">
                <div className={`w-2 h-2 rounded-full ${isConnected ? 'bg-status-healthy live-indicator' : 'bg-subtle'}`} />
                <span className="text-sm text-muted">
                  {isConnected ? 'متصل' : 'قطع'}
                </span>
              </div>
              <button
                onClick={refresh}
                disabled={loading}
                className="px-4 py-2 bg-accent hover:bg-accent-hover text-on-accent rounded-lg transition-colors disabled:opacity-50"
              >
                {loading ? 'در حال بروزرسانی...' : 'بروزرسانی'}
              </button>
            </div>
          </div>

          {/* Tabs */}
          <nav className="flex gap-1 mt-4">
            {[
              { id: 'overview', label: 'نمای کلی', icon: 'M4 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2V6zM14 6a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2V6zM4 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2H6a2 2 0 01-2-2v-2zM14 16a2 2 0 012-2h2a2 2 0 012 2v2a2 2 0 01-2 2h-2a2 2 0 01-2-2v-2z' },
              { id: 'logs', label: 'لاگ‌ها', icon: 'M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z' },
              { id: 'traces', label: 'تریس‌ها', icon: 'M13 10V3L4 14h7v7l9-11h-7z' },
            ].map((tab) => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id as any)}
                className={`flex items-center gap-2 px-4 py-2 rounded-lg transition-colors ${
                  activeTab === tab.id
                    ? 'bg-accent text-on-accent'
                    : 'text-muted hover:text-text hover:bg-hover'
                }`}
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d={tab.icon} />
                </svg>
                <span>{tab.label}</span>
              </button>
            ))}
          </nav>
        </div>
      </header>

      {/* Main Content */}
      <main className="container mx-auto px-6 py-8">
        {error && (
          <div className="mb-6 p-4 bg-red-900/20 border border-red-800 rounded-lg text-red-400">
            خطا در دریافت اطلاعات: {error}
          </div>
        )}

        {activeTab === 'overview' && <SystemMetrics data={data} loading={loading} />}
        {activeTab === 'logs' && <LogsPanel />}
        {activeTab === 'traces' && <TracesPanel />}
      </main>

      {/* Footer */}
      <footer className="border-t border-border py-4 mt-auto">
        <div className="container mx-auto px-6 flex items-center justify-between text-sm text-muted">
          <span>Neo BPMS Monitoring v1.0.0</span>
          {lastUpdate && (
            <span>آخرین بروزرسانی: {new Date(lastUpdate).toLocaleTimeString('fa-IR')}</span>
          )}
        </div>
      </footer>
    </div>
  );
}

