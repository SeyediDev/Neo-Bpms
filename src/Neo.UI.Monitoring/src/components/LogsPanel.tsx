'use client';

import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import clsx from 'clsx';

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

export function LogsPanel() {
  const [logs, setLogs] = useState<LogEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<number | null>(null);
  const [searchTerm, setSearchTerm] = useState('');

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

  return (
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
                    </div>
                  </motion.div>
                );
              })}
            </AnimatePresence>
          )}
        </div>
      </div>
    </div>
  );
}

