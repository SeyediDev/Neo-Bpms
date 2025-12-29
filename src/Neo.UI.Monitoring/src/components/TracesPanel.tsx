'use client';

import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import clsx from 'clsx';

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
}

const STATUS_INFO: Record<number, { label: string; color: string }> = {
  0: { label: 'Unset', color: 'text-slate-400' },
  1: { label: 'Ok', color: 'text-status-healthy' },
  2: { label: 'Error', color: 'text-status-critical' },
};

export function TracesPanel() {
  const [traces, setTraces] = useState<TraceData[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedTrace, setSelectedTrace] = useState<string | null>(null);

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

  return (
    <div className="space-y-4">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div className="flex items-center gap-4">
          <span className="text-slate-400">{traces.length} تریس</span>
        </div>
        <button
          onClick={fetchTraces}
          className="px-4 py-2 bg-neo-600 hover:bg-neo-700 text-white rounded-lg transition-colors"
        >
          بروزرسانی
        </button>
      </div>

      {/* Traces List */}
      <div className="bg-slate-900/50 backdrop-blur-sm border border-slate-800 rounded-xl overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-slate-400">
            در حال بارگذاری...
          </div>
        ) : traces.length === 0 ? (
          <div className="p-8 text-center text-slate-400">
            تریسی یافت نشد
          </div>
        ) : (
          <div className="divide-y divide-slate-800">
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
                      'p-4 cursor-pointer hover:bg-slate-800/50 transition-colors',
                      selectedTrace === trace.traceId && 'bg-slate-800/70',
                      hasError && 'border-l-4 border-l-status-critical'
                    )}
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-4">
                        <div className={clsx('w-2 h-2 rounded-full', statusInfo.color.replace('text-', 'bg-'))} />
                        <div>
                          <p className="text-white font-medium">{trace.rootOperationName}</p>
                          <p className="text-sm text-slate-500 font-mono">{trace.serviceName}</p>
                        </div>
                      </div>
                      
                      <div className="flex items-center gap-6 text-sm">
                        <div className="text-right">
                          <p className="text-white font-mono">{formatDuration(trace.duration)}</p>
                          <p className="text-slate-500">{trace.spanCount} spans</p>
                        </div>
                        {hasError && (
                          <span className="px-2 py-1 bg-red-900/30 text-status-critical rounded text-xs">
                            {trace.errorCount} خطا
                          </span>
                        )}
                        <span className="text-slate-500 text-xs" dir="ltr">
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
                          className="mt-4 pt-4 border-t border-slate-700"
                        >
                          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                            <div>
                              <p className="text-slate-500">Trace ID</p>
                              <p className="text-white font-mono text-xs break-all">{trace.traceId}</p>
                            </div>
                            <div>
                              <p className="text-slate-500">شروع</p>
                              <p className="text-white" dir="ltr">{new Date(trace.startTime).toLocaleString('fa-IR')}</p>
                            </div>
                            <div>
                              <p className="text-slate-500">پایان</p>
                              <p className="text-white" dir="ltr">{new Date(trace.endTime).toLocaleString('fa-IR')}</p>
                            </div>
                            <div>
                              <p className="text-slate-500">وضعیت</p>
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
  );
}

