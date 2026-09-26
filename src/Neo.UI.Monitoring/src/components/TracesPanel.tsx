'use client';

import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import clsx from 'clsx';
import { Toast } from './Toast';

interface TraceSpan {
  spanId: string;
  parentSpanId?: string;
  operationName: string;
  serviceName: string;
  startTime: string;
  endTime: string;
  status: number;
  kind?: string;
  tags?: Record<string, string>;
}

interface TraceData {
  traceId: string;
  rootOperationName: string;
  serviceName: string;
  startTime: string;
  endTime: string;
  duration: string;
  status: number;
  spanCount: number;
  errorCount: number;
  spans?: TraceSpan[];
}

const STATUS_INFO: Record<number, { label: string; color: string }> = {
  0: { label: 'Unset', color: 'text-muted' },
  1: { label: 'Ok', color: 'text-status-healthy' },
  2: { label: 'Error', color: 'text-status-critical' },
};

interface TracesPanelProps {
  /** Optional callback for showing toast notifications (for admin panel integration) */
  onShowToast?: (message: string) => void;
  /** Optional callback for showing error toast notifications */
  onShowErrorToast?: (message: string) => void;
}

export function TracesPanel({ onShowToast, onShowErrorToast }: TracesPanelProps = {}) {
  const [traces, setTraces] = useState<TraceData[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedTrace, setSelectedTrace] = useState<string | null>(null);
  const [copiedId, setCopiedId] = useState<string | null>(null);
  const [showToast, setShowToast] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    fetchTraces();
    const interval = setInterval(fetchTraces, 5000);
    return () => clearInterval(interval);
  }, []);

  const fetchTraces = async () => {
    try {
      const response = await fetch('/api/monitoring/traces/recent?limit=50');
      if (response.ok) {
        const data = await response.json();
        setTraces(data);
      }
    } catch (error) {
      console.error('Error fetching traces:', error);
    } finally {
      setLoading(false);
    }
  };

  const formatDuration = (duration: string): string => {
    if (!duration) return '--';
    // Parse TimeSpan format
    const match = duration.match(/(\d+):(\d+):(\d+)\.?(\d*)/);
    if (!match) return duration;
    
    const hours = parseInt(match[1]);
    const minutes = parseInt(match[2]);
    const seconds = parseInt(match[3]);
    const ms = match[4] ? Math.round(parseFloat('0.' + match[4]) * 1000) : 0;
    
    if (hours > 0) return `${hours}h ${minutes}m`;
    if (minutes > 0) return `${minutes}m ${seconds}s`;
    if (seconds > 0) return `${seconds}.${ms.toString().padStart(3, '0')}s`;
    return `${ms}ms`;
  };

  const formatDurationMs = (ms: number): string => {
    if (!ms || ms < 1) return '<1 ms';
    if (ms < 1000) return `${Math.round(ms)} ms`;
    return `${(ms / 1000).toFixed(2)} s`;
  };

  const copyTraceToClipboard = async (trace: TraceData) => {
    const statusInfo = STATUS_INFO[trace.status] || STATUS_INFO[0];
    
    // Calculate duration correctly from startTime and endTime
    let durationMs = 0;
    if (trace.startTime && trace.endTime) {
      const start = new Date(trace.startTime);
      const end = new Date(trace.endTime);
      if (!isNaN(start.getTime()) && !isNaN(end.getTime())) {
        durationMs = end.getTime() - start.getTime();
      }
    }
    
    const traceText = [
      `Trace ID: ${trace.traceId}`,
      `Root Operation: ${trace.rootOperationName}`,
      `Service: ${trace.serviceName}`,
      `Status: ${statusInfo.label}`,
      `Duration: ${formatDurationMs(durationMs)}`,
      `Spans: ${trace.spanCount}`,
      trace.errorCount > 0 && `Errors: ${trace.errorCount}`,
      `Start Time: ${new Date(trace.startTime).toLocaleString('fa-IR')}`,
      `End Time: ${new Date(trace.endTime).toLocaleString('fa-IR')}`,
      '',
      '=== Spans ===',
    ].filter(Boolean);
    
    // Add spans information
    if (trace.spans && trace.spans.length > 0) {
      trace.spans.forEach((span, spanIndex) => {
        const spanStart = new Date(span.startTime);
        const spanEnd = new Date(span.endTime);
        let spanDurationMs = 0;
        if (!isNaN(spanStart.getTime()) && !isNaN(spanEnd.getTime())) {
          spanDurationMs = spanEnd.getTime() - spanStart.getTime();
        }
        
        const spanStatusInfo = STATUS_INFO[span.status] || STATUS_INFO[0];
        
        traceText.push('');
        traceText.push(`Span ${spanIndex + 1}:`);
        traceText.push(`  Span ID: ${span.spanId}`);
        if (span.parentSpanId) {
          traceText.push(`  Parent Span ID: ${span.parentSpanId}`);
        }
        traceText.push(`  Operation: ${span.operationName}`);
        traceText.push(`  Service: ${span.serviceName}`);
        traceText.push(`  Status: ${spanStatusInfo.label}`);
        traceText.push(`  Duration: ${formatDurationMs(spanDurationMs)}`);
        traceText.push(`  Start Time: ${new Date(span.startTime).toLocaleString('fa-IR')}`);
        traceText.push(`  End Time: ${new Date(span.endTime).toLocaleString('fa-IR')}`);
        if (span.kind) {
          traceText.push(`  Kind: ${span.kind}`);
        }
        
        // Add tags
        if (span.tags && Object.keys(span.tags).length > 0) {
          traceText.push(`  Tags:`);
          Object.entries(span.tags).forEach(([key, value]) => {
            traceText.push(`    ${key}: ${value}`);
          });
        }
      });
    } else {
      traceText.push('No spans available');
    }
    
    const finalText = traceText.join('\n');

    try {
      await navigator.clipboard.writeText(finalText);
      setCopiedId(trace.traceId);
      const message = 'تریس با موفقیت کپی شد';
      if (onShowToast) {
        onShowToast(message);
      } else {
        setShowToast(true);
        setTimeout(() => {
          setCopiedId(null);
          setShowToast(false);
        }, 2000);
      }
    } catch (error) {
      console.error('Failed to copy to clipboard:', error);
    }
  };

  const deleteAllTraces = async () => {
    if (!confirm('آیا از حذف کلیه تریس‌ها اطمینان دارید؟ این عمل غیرقابل بازگشت است.')) {
      return;
    }

    setDeleting(true);
    try {
      const response = await fetch('/api/monitoring/traces/clear', {
        method: 'DELETE',
      });

      if (response.ok) {
        setTraces([]);
        const message = 'کلیه تریس‌ها با موفقیت حذف شدند';
        if (onShowToast) {
          onShowToast(message);
        } else {
          setShowToast(true);
          setTimeout(() => setShowToast(false), 2000);
        }
      } else {
        const errorMessage = 'خطا در حذف تریس‌ها';
        if (onShowErrorToast) {
          onShowErrorToast(errorMessage);
        } else {
          alert(errorMessage);
        }
      }
    } catch (error) {
      console.error('Failed to delete traces:', error);
      const errorMessage = 'خطا در حذف تریس‌ها';
      if (onShowErrorToast) {
        onShowErrorToast(errorMessage);
      } else {
        alert(errorMessage);
      }
    } finally {
      setDeleting(false);
    }
  };

  return (
    <>
      {!onShowToast && (
        <Toast
          message="تریس با موفقیت کپی شد"
          show={showToast}
          onClose={() => setShowToast(false)}
        />
      )}
      <div className="space-y-4">
        {/* Header */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          <span className="text-muted">{traces.length} تریس</span>
        </div>
        <button
          onClick={fetchTraces}
          className="px-4 py-2 bg-accent hover:bg-accent-hover text-on-accent rounded-lg transition-colors"
        >
          بروزرسانی
        </button>
        <button
          onClick={deleteAllTraces}
          disabled={deleting || traces.length === 0}
          className="px-4 py-2 bg-red-600 hover:bg-red-700 disabled:bg-elevated disabled:cursor-not-allowed text-white rounded-lg transition-colors flex items-center gap-2"
        >
          {deleting ? (
            <>
              <svg className="w-4 h-4 animate-spin" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              در حال حذف...
            </>
          ) : (
            <>
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              حذف همه تریس‌ها
            </>
          )}
        </button>
      </div>

      {/* Traces List */}
      <div className="bg-surface backdrop-blur-sm border border-border rounded-xl overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-muted">
            در حال بارگذاری...
          </div>
        ) : traces.length === 0 ? (
          <div className="p-8 text-center text-muted">
            تریسی یافت نشد
          </div>
        ) : (
          <div className="divide-y divide-border">
            <AnimatePresence>
              {traces.map((trace, index) => {
                const statusInfo = STATUS_INFO[trace.status] || STATUS_INFO[0];
                const hasError = trace.errorCount > 0;
                
                return (
                  <motion.div
                    key={trace.traceId}
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -10 }}
                    transition={{ delay: index * 0.02 }}
                    onClick={() => setSelectedTrace(trace.traceId === selectedTrace ? null : trace.traceId)}
                    className={clsx(
                      'p-4 cursor-pointer hover:bg-hover transition-colors',
                      selectedTrace === trace.traceId && 'bg-elevated',
                      hasError && 'border-l-4 border-l-status-critical'
                    )}
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-4">
                        <div className={clsx('w-2 h-2 rounded-full', statusInfo.color.replace('text-', 'bg-'))} />
                        <div>
                          <p className="text-text font-medium">{trace.rootOperationName}</p>
                          <p className="text-sm text-muted font-mono">{trace.serviceName}</p>
                        </div>
                      </div>
                      
                      <div className="flex items-center gap-6 text-sm">
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            copyTraceToClipboard(trace);
                          }}
                          className={clsx(
                            'flex-shrink-0 p-2 rounded-lg transition-colors',
                            copiedId === trace.traceId
                              ? 'bg-accent text-on-accent'
                              : 'bg-elevated text-muted hover:bg-hover hover:text-text'
                          )}
                          title="کپی به کلیپ‌برد"
                        >
                          {copiedId === trace.traceId ? (
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                            </svg>
                          ) : (
                            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                            </svg>
                          )}
                        </button>
                        <div className="text-right">
                          <p className="text-text font-mono">{formatDuration(trace.duration)}</p>
                          <p className="text-muted">{trace.spanCount} spans</p>
                        </div>
                        {hasError && (
                          <span className="px-2 py-1 bg-red-900/30 text-status-critical rounded text-xs">
                            {trace.errorCount} خطا
                          </span>
                        )}
                        <span className="text-muted text-xs" dir="ltr">
                          {new Date(trace.startTime).toLocaleTimeString('fa-IR')}
                        </span>
                      </div>
                    </div>

                    {/* Expanded Details */}
                    <AnimatePresence>
                      {selectedTrace === trace.traceId && (
                        <motion.div
                          initial={{ height: 0, opacity: 0 }}
                          animate={{ height: 'auto', opacity: 1 }}
                          exit={{ height: 0, opacity: 0 }}
                          className="mt-4 pt-4 border-t border-border"
                        >
                          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                            <div>
                              <p className="text-muted">Trace ID</p>
                              <p className="text-text font-mono text-xs break-all">{trace.traceId}</p>
                            </div>
                            <div>
                              <p className="text-muted">شروع</p>
                              <p className="text-text" dir="ltr">{new Date(trace.startTime).toLocaleString('fa-IR')}</p>
                            </div>
                            <div>
                              <p className="text-muted">پایان</p>
                              <p className="text-text" dir="ltr">{new Date(trace.endTime).toLocaleString('fa-IR')}</p>
                            </div>
                            <div>
                              <p className="text-muted">وضعیت</p>
                              <p className={statusInfo.color}>{statusInfo.label}</p>
                            </div>
                          </div>
                        </motion.div>
                      )}
                    </AnimatePresence>
                  </motion.div>
                );
              })}
            </AnimatePresence>
          </div>
        )}
      </div>
      </div>
    </>
  );
}

