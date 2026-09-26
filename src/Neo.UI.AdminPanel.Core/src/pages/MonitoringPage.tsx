/**
 * Monitoring Page Component
 * Neo BPMS Admin Panel Core
 * 
 * Monitoring page with logs and traces panels integrated with admin panel toast system.
 */

import { useState } from 'react';
import clsx from 'clsx';

// Import monitoring components (assuming they're available)
// In a real scenario, these would be imported from @neo/ui-monitoring package
// For now, we'll create a placeholder that shows how to integrate them

export function MonitoringPage() {
  const [activeTab, setActiveTab] = useState<'logs' | 'traces'>('logs');

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-text dark:text-text">مانیتورینگ سیستم</h1>
          <p className="text-muted dark:text-muted mt-1">نظارت بر لاگ‌ها و تریس‌های سیستم</p>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-2 border-b border-border dark:border-border">
        <button
          onClick={() => setActiveTab('logs')}
          className={clsx(
            'px-4 py-2 font-medium transition-colors',
            activeTab === 'logs'
              ? 'text-blue-600 dark:text-blue-400 border-b-2 border-blue-600 dark:border-blue-400'
              : 'text-muted dark:text-muted hover:text-text dark:hover:text-subtle'
          )}
        >
          لاگ‌ها
        </button>
        <button
          onClick={() => setActiveTab('traces')}
          className={clsx(
            'px-4 py-2 font-medium transition-colors',
            activeTab === 'traces'
              ? 'text-blue-600 dark:text-blue-400 border-b-2 border-blue-600 dark:border-blue-400'
              : 'text-muted dark:text-muted hover:text-text dark:hover:text-subtle'
          )}
        >
          تریس‌ها
        </button>
      </div>

      {/* Content */}
      <div className="bg-surface dark:bg-elevated rounded-2xl shadow-sm border border-border dark:border-border p-6">
        {activeTab === 'logs' && (
          <div>
            {/* 
              Example usage with LogsPanel:
              <LogsPanel onShowToast={handleLogCopy} />
              
              Note: In a real implementation, you would import LogsPanel from @neo/ui-monitoring:
              import { LogsPanel } from '@neo/ui-monitoring';
            */}
            <div className="text-center py-12 text-muted">
              <p className="mb-4">کامپوننت LogsPanel اینجا قرار می‌گیرد</p>
              <p className="text-sm">
                برای استفاده، کامپوننت LogsPanel را از @neo/ui-monitoring import کنید و prop onShowToast را به آن بدهید:
              </p>
              <pre className="mt-4 p-4 bg-hover dark:bg-surface rounded-lg text-left text-xs">
{`import { LogsPanel } from '@neo/ui-monitoring';
import { useToast } from '../components/ui/Toast';

const { success, error } = useToast();

<LogsPanel 
  onShowToast={(msg) => success(msg)}
  onShowErrorToast={(msg) => error(msg)}
/>`}
              </pre>
            </div>
          </div>
        )}

        {activeTab === 'traces' && (
          <div>
            {/* 
              Example usage with TracesPanel:
              <TracesPanel onShowToast={handleTraceCopy} />
              
              Note: In a real implementation, you would import TracesPanel from @neo/ui-monitoring:
              import { TracesPanel } from '@neo/ui-monitoring';
            */}
            <div className="text-center py-12 text-muted">
              <p className="mb-4">کامپوننت TracesPanel اینجا قرار می‌گیرد</p>
              <p className="text-sm">
                برای استفاده، کامپوننت TracesPanel را از @neo/ui-monitoring import کنید و prop onShowToast را به آن بدهید:
              </p>
              <pre className="mt-4 p-4 bg-hover dark:bg-surface rounded-lg text-left text-xs">
{`import { TracesPanel } from '@neo/ui-monitoring';
import { useToast } from '../components/ui/Toast';

const { success, error } = useToast();

<TracesPanel 
  onShowToast={(msg) => success(msg)}
  onShowErrorToast={(msg) => error(msg)}
/>`}
              </pre>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default MonitoringPage;

