'use client';

import { useState } from 'react';
import clsx from 'clsx';

export interface TimeRange {
  label: string;
  labelFa: string;
  value: string;
  minutes: number;
}

export const TIME_RANGES: TimeRange[] = [
  { label: '5m', labelFa: '۵ دقیقه', value: '5m', minutes: 5 },
  { label: '15m', labelFa: '۱۵ دقیقه', value: '15m', minutes: 15 },
  { label: '1h', labelFa: '۱ ساعت', value: '1h', minutes: 60 },
  { label: '6h', labelFa: '۶ ساعت', value: '6h', minutes: 360 },
  { label: '24h', labelFa: '۲۴ ساعت', value: '24h', minutes: 1440 },
];

interface TimeRangeSelectorProps {
  value: string;
  onChange: (range: TimeRange) => void;
  className?: string;
}

export function TimeRangeSelector({ value, onChange, className }: TimeRangeSelectorProps) {
  return (
    <div className={clsx('inline-flex rounded-lg bg-slate-800/50 p-1', className)}>
      {TIME_RANGES.map((range) => (
        <button
          key={range.value}
          onClick={() => onChange(range)}
          className={clsx(
            'px-3 py-1.5 text-sm font-medium rounded-md transition-all duration-200',
            value === range.value
              ? 'bg-neo-500 text-white shadow-lg shadow-neo-500/25'
              : 'text-slate-400 hover:text-white hover:bg-slate-700/50'
          )}
        >
          {range.label}
        </button>
      ))}
    </div>
  );
}

