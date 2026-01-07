'use client';

import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import clsx from 'clsx';
import { Toast } from './Toast';

interface LogEntry {
  id: string;
  timestamp: string;
  level: number;
  message: string;
  sourceContext?: string;
  traceId?: string;
  exceptionType?: string;
  exceptionMessage?: string;
}

const LOG_LEVELS: Record<number, { label: string; color: string; bg: string }> = {
  0: { label: 'Trace', color: 'text-slate-400', bg: 'bg-slate-800' },
  1: { label: 'Debug', color: 'text-slate-400', bg: 'bg-slate-800' },
  2: { label: 'Info', color: 'text-neo-400', bg: 'bg-neo-900/30' },
  3: { label: 'Warn', color: 'text-status-warning', bg: 'bg-yellow-900/30' },
  4: { label: 'Error', color: 'text-status-critical', bg: 'bg-red-900/30' },
  5: { label: 'Fatal', color: 'text-red-300', bg: 'bg-red-900/50' },
};

interface LogsPanelProps {
  /** Optional callback for showing toast notifications (for admin panel integration) */
  onShowToast?: (message: string) => void;
  /** Optional callback for showing error toast notifications */
  onShowErrorToast?: (message: string) => void;
}

export function LogsPanel({ onShowToast, onShowErrorToast }: LogsPanelProps = {}) {
  const [logs, setLogs] = useState<LogEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<number | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [copiedId, setCopiedId] = useState<string | null>(null);
  const [showToast, setShowToast] = useState(false);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    fetchLogs();
    const interval = setInterval(fetchLogs, 5000);
    return () => clearInterval(interval);
  }, []);

  const fetchLogs = async () => {
    try {
      const response = await fetch('/api/monitoring/logs/recent?limit=100');
      if (response.ok) {
        const data = await response.json();
        setLogs(data);
      }
    } catch (error) {
      console.error('Error fetching logs:', error);
    } finally {
      setLoading(false);
    }
  };

  const filteredLogs = logs.filter((log) => {
    if (filter !== null && log.level < filter) return false;
    if (searchTerm && !log.message.toLowerCase().includes(searchTerm.toLowerCase())) return false;
    return true;
  });

  const copyLogToClipboard = async (log: LogEntry) => {
    const levelInfo = LOG_LEVELS[log.level] || LOG_LEVELS[2];
    const logText = [
      `Level: ${levelInfo.label}`,
      `Timestamp: ${new Date(log.timestamp).toLocaleString('fa-IR')}`,
      `Message: ${log.message}`,
      log.sourceContext && `Source: ${log.sourceContext}`,
      log.traceId && `TraceId: ${log.traceId}`,
      log.exceptionType && `Exception Type: ${log.exceptionType}`,
      log.exceptionMessage && `Exception Message: ${log.exceptionMessage}`,
    ]
      .filter(Boolean)
      .join('\n');

    try {
      await navigator.clipboard.writeText(logText);
      setCopiedId(log.id);
      const message = 'لاگ با موفقیت کپی شد';
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

  const deleteAllLogs = async () => {
    if (!confirm('آیا از حذف کلیه لاگ‌ها اطمینان دارید؟ این عمل غیرقابل بازگشت است.')) {
      return;
    }

    setDeleting(true);
    try {
      const response = await fetch('/api/monitoring/logs/clear', {
        method: 'DELETE',
      });

      if (response.ok) {
        setLogs([]);
        const message = 'کلیه لاگ‌ها با موفقیت حذف شدند';
        if (onShowToast) {
          onShowToast(message);
        } else {
          setShowToast(true);
          setTimeout(() => setShowToast(false), 2000);
        }
      } else {
        const errorMessage = 'خطا در حذف لاگ‌ها';
        if (onShowErrorToast) {
          onShowErrorToast(errorMessage);
        } else {
          alert(errorMessage);
        }
      }
    } catch (error) {
      console.error('Failed to delete logs:', error);
      const errorMessage = 'خطا در حذف لاگ‌ها';
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
          message="لاگ با موفقیت کپی شد"
          show={showToast}
          onClose={() => setShowToast(false)}
        />
      )}
      <div className="space-y-4">
        {/* Filters */}
      <div className="flex flex-wrap gap-4 items-center">
        <div className="flex gap-2">
          <button
            onClick={() => setFilter(null)}
            className={clsx(
              'px-3 py-1.5 rounded-lg text-sm transition-colors',
              filter === null ? 'bg-neo-600 text-white' : 'bg-slate-800 text-slate-400 hover:text-white'
            )}
          >
            همه
          </button>
          {Object.entries(LOG_LEVELS).map(([level, info]) => (
            <button
              key={level}
              onClick={() => setFilter(parseInt(level))}
              className={clsx(
                'px-3 py-1.5 rounded-lg text-sm transition-colors',
                filter === parseInt(level)
                  ? `${info.bg} ${info.color}`
                  : 'bg-slate-800 text-slate-400 hover:text-white'
              )}
            >
              {info.label}
            </button>
          ))}
        </div>

        <input
          type="text"
          placeholder="جستجو در لاگ‌ها..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="flex-1 px-4 py-2 bg-slate-800 border border-slate-700 rounded-lg text-white placeholder-slate-500 focus:outline-none focus:border-neo-500"
        />

        <button
          onClick={fetchLogs}
          className="px-4 py-2 bg-neo-600 hover:bg-neo-700 text-white rounded-lg transition-colors"
        >
          بروزرسانی
        </button>
        <button
          onClick={deleteAllLogs}
          disabled={deleting || logs.length === 0}
          className="px-4 py-2 bg-red-600 hover:bg-red-700 disabled:bg-slate-700 disabled:cursor-not-allowed text-white rounded-lg transition-colors flex items-center gap-2"
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
              حذف همه لاگ‌ها
            </>
          )}
        </button>
      </div>

      {/* Logs List */}
      <div className="bg-slate-900/50 backdrop-blur-sm border border-slate-800 rounded-xl overflow-hidden">
        <div className="max-h-[600px] overflow-y-auto">
          {loading ? (
            <div className="p-8 text-center text-slate-400">
              در حال بارگذاری...
            </div>
          ) : filteredLogs.length === 0 ? (
            <div className="p-8 text-center text-slate-400">
              لاگی یافت نشد
            </div>
          ) : (
            <AnimatePresence>
              {filteredLogs.map((log, index) => {
                const levelInfo = LOG_LEVELS[log.level] || LOG_LEVELS[2];
                return (
                  <motion.div
                    key={log.id}
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    exit={{ opacity: 0, x: 20 }}
                    transition={{ delay: index * 0.02 }}
                    className={clsx(
                      'p-4 border-b border-slate-800 hover:bg-slate-800/50 transition-colors',
                      log.exceptionType && 'bg-red-900/10'
                    )}
                  >
                    <div className="flex items-start gap-4">
                      <span className={clsx('px-2 py-0.5 rounded text-xs font-medium', levelInfo.bg, levelInfo.color)}>
                        {levelInfo.label}
                      </span>
                      <div className="flex-1 min-w-0">
                        <div className="flex items-start justify-between gap-2">
                          <div className="flex-1 min-w-0">
                            <p className="text-white font-mono text-sm break-all">{log.message}</p>
                            <div className="flex flex-wrap gap-4 mt-2 text-xs text-slate-500">
                              <span dir="ltr">{new Date(log.timestamp).toLocaleString('fa-IR')}</span>
                              {log.sourceContext && (
                                <span className="font-mono">{log.sourceContext}</span>
                              )}
                              {log.traceId && (
                                <span className="font-mono">TraceId: {log.traceId.substring(0, 8)}...</span>
                              )}
                            </div>
                            {log.exceptionType && (
                              <div className="mt-2 p-2 bg-red-900/20 rounded text-status-critical text-xs font-mono">
                                <p>{log.exceptionType}</p>
                                <p className="text-red-400">{log.exceptionMessage}</p>
                              </div>
                            )}
                          </div>
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              copyLogToClipboard(log);
                            }}
                            className={clsx(
                              'flex-shrink-0 p-2 rounded-lg transition-colors',
                              copiedId === log.id
                                ? 'bg-neo-600 text-white'
                                : 'bg-slate-800 text-slate-400 hover:bg-slate-700 hover:text-white'
                            )}
                            title="کپی به کلیپ‌برد"
                          >
                            {copiedId === log.id ? (
                              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                              </svg>
                            ) : (
                              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
                              </svg>
                            )}
                          </button>
                        </div>
                      </div>
                    </div>
                  </motion.div>
                );
              })}
            </AnimatePresence>
          )}
        </div>
      </div>
      </div>
    </>
  );
}

