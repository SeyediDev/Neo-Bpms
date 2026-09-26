/**
 * Live Operations Page
 * Neo BPMS Admin Panel Core
 */

import React, { useState } from 'react';
import { LiveOperationsDashboard, LiveMetric } from '../features/live-operations';
import { Modal } from '../components/ui/Modal';
import { Sparkline } from '../features/live-operations/components/Sparkline';

export interface LiveOperationsPageProps {
  /** API URL */
  apiUrl?: string;
  /** Refresh interval */
  refreshInterval?: number;
}

const formatValue = (value: number, format?: string): string => {
  if (format === 'currency') {
    return new Intl.NumberFormat('fa-IR').format(value) + ' ریال';
  }
  if (format === 'percent') {
    return value.toFixed(1) + '%';
  }
  return new Intl.NumberFormat('fa-IR').format(value);
};

export const LiveOperationsPage: React.FC<LiveOperationsPageProps> = ({
  apiUrl,
  refreshInterval = 5000,
}) => {
  const [selectedMetric, setSelectedMetric] = useState<LiveMetric | null>(null);

  const handleMetricClick = (metric: LiveMetric) => {
    setSelectedMetric(metric);
  };

  const closeModal = () => {
    setSelectedMetric(null);
  };

  return (
    <div className="space-y-6">
      <LiveOperationsDashboard
        apiUrl={apiUrl}
        refreshInterval={refreshInterval}
        onMetricClick={handleMetricClick}
        groupByCategory
      />

      {/* Metric Detail Modal */}
      <Modal
        isOpen={!!selectedMetric}
        onClose={closeModal}
        title={selectedMetric?.name || ''}
        description={selectedMetric?.nameEn}
        size="lg"
      >
        {selectedMetric && (
          <div className="space-y-6">
            {/* Current Value */}
            <div className="text-center py-6 bg-canvas dark:bg-surface rounded-xl">
              <div className="text-4xl font-bold text-text dark:text-text mb-2">
                {formatValue(selectedMetric.value, selectedMetric.format)}
              </div>
              {selectedMetric.unit && (
                <div className="text-muted">{selectedMetric.unit}</div>
              )}
              {selectedMetric.trend && (
                <div
                  className={`mt-2 inline-flex items-center gap-1 px-3 py-1 rounded-full text-sm ${
                    selectedMetric.trend === 'up'
                      ? 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-400'
                      : selectedMetric.trend === 'down'
                      ? 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400'
                      : 'bg-hover text-muted dark:bg-elevated dark:text-muted'
                  }`}
                >
                  {selectedMetric.trend === 'up' ? '↑' : selectedMetric.trend === 'down' ? '↓' : '—'}
                  {selectedMetric.trendPercent !== undefined && (
                    <span>
                      {selectedMetric.trendPercent > 0 ? '+' : ''}
                      {selectedMetric.trendPercent.toFixed(1)}%
                    </span>
                  )}
                </div>
              )}
            </div>

            {/* Sparkline Chart */}
            {selectedMetric.sparklineData && selectedMetric.sparklineData.length > 0 && (
              <div>
                <h4 className="text-sm font-medium text-text dark:text-subtle mb-3">
                  روند تغییرات
                </h4>
                <div className="bg-canvas dark:bg-surface rounded-xl p-4">
                  <Sparkline
                    data={selectedMetric.sparklineData}
                    width={400}
                    height={120}
                    color={selectedMetric.color || 'primary'}
                    strokeWidth={3}
                  />
                </div>
              </div>
            )}

            {/* Statistics */}
            {selectedMetric.sparklineData && selectedMetric.sparklineData.length > 0 && (
              <div>
                <h4 className="text-sm font-medium text-text dark:text-subtle mb-3">
                  آمار
                </h4>
                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                  <div className="bg-canvas dark:bg-surface rounded-xl p-4 text-center">
                    <div className="text-xs text-muted mb-1">حداقل</div>
                    <div className="text-lg font-semibold text-text dark:text-text">
                      {formatValue(Math.min(...selectedMetric.sparklineData), selectedMetric.format)}
                    </div>
                  </div>
                  <div className="bg-canvas dark:bg-surface rounded-xl p-4 text-center">
                    <div className="text-xs text-muted mb-1">حداکثر</div>
                    <div className="text-lg font-semibold text-text dark:text-text">
                      {formatValue(Math.max(...selectedMetric.sparklineData), selectedMetric.format)}
                    </div>
                  </div>
                  <div className="bg-canvas dark:bg-surface rounded-xl p-4 text-center">
                    <div className="text-xs text-muted mb-1">میانگین</div>
                    <div className="text-lg font-semibold text-text dark:text-text">
                      {formatValue(
                        selectedMetric.sparklineData.reduce((a, b) => a + b, 0) /
                          selectedMetric.sparklineData.length,
                        selectedMetric.format
                      )}
                    </div>
                  </div>
                  <div className="bg-canvas dark:bg-surface rounded-xl p-4 text-center">
                    <div className="text-xs text-muted mb-1">آخرین</div>
                    <div className="text-lg font-semibold text-text dark:text-text">
                      {formatValue(
                        selectedMetric.sparklineData[selectedMetric.sparklineData.length - 1],
                        selectedMetric.format
                      )}
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Metadata */}
            <div className="border-t border-border dark:border-border pt-4">
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-muted">شناسه:</span>
                  <span className="mr-2 text-text dark:text-text font-mono">
                    {selectedMetric.id}
                  </span>
                </div>
                {selectedMetric.category && (
                  <div>
                    <span className="text-muted">دسته‌بندی:</span>
                    <span className="mr-2 text-text dark:text-text">
                      {selectedMetric.category}
                    </span>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default LiveOperationsPage;









