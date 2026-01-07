/**
 * Monitoring Page Component
 * Neo BPMS Admin Panel Core
 * 
 * Monitoring page with logs and traces panels integrated with admin panel toast system.
 */

import { useState } from 'react';
import { useToast } from '../components/ui/Toast';
import clsx from 'clsx';

// Import monitoring components (assuming they're available)
// In a real scenario, these would be imported from @neo/ui-monitoring package
// For now, we'll create a placeholder that shows how to integrate them

export function MonitoringPage() {
  const { success, error: showError } = useToast();
  const [activeTab, setActiveTab] = useState<'logs' | 'traces'>('logs');

  // Toast callbacks for LogsPanel
  const handleLogToast = (message: string) => {
    success(message);
  };

  const handleLogError = (message: string) => {
    showError(message);
  };

  // Toast callbacks for TracesPanel
  const handleTraceToast = (message: string) => {
    success(message);
  };

  const handleTraceError = (message: string) => {
    showError(message);
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">مانیتورینگ سیستم</h1>
          <p className="text-gray-500 dark:text-gray-400 mt-1">نظارت بر لاگ‌ها و تریس‌های سیستم</p>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex gap-2 border-b border-gray-200 dark:border-gray-700">
        <button
          onClick={() => setActiveTab('logs')}
          className={clsx(
            'px-4 py-2 font-medium transition-colors',
            activeTab === 'logs'
              ? 'text-blue-600 dark:text-blue-400 border-b-2 border-blue-600 dark:border-blue-400'
              : 'text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300'
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
              : 'text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300'
          )}
        >
          تریس‌ها
        </button>
      </div>

      {/* Content */}
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        {activeTab === 'logs' && (
          <div>
            {/* 
              Example usage with LogsPanel:
              <LogsPanel onShowToast={handleLogCopy} />
              
              Note: In a real implementation, you would import LogsPanel from @neo/ui-monitoring:
              import { LogsPanel } from '@neo/ui-monitoring';
            */}
            <div className="text-center py-12 text-gray-400">
              <p className="mb-4">کامپوننت LogsPanel اینجا قرار می‌گیرد</p>
              <p className="text-sm">
                برای استفاده، کامپوننت LogsPanel را از @neo/ui-monitoring import کنید و prop onShowToast را به آن بدهید:
              </p>
              <pre className="mt-4 p-4 bg-gray-100 dark:bg-gray-900 rounded-lg text-left text-xs">
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
            <div className="text-center py-12 text-gray-400">
              <p className="mb-4">کامپوننت TracesPanel اینجا قرار می‌گیرد</p>
              <p className="text-sm">
                برای استفاده، کامپوننت TracesPanel را از @neo/ui-monitoring import کنید و prop onShowToast را به آن بدهید:
              </p>
              <pre className="mt-4 p-4 bg-gray-100 dark:bg-gray-900 rounded-lg text-left text-xs">
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

