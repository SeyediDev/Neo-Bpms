/**
 * Time Frame Selector Component
 * Neo BPMS Admin Panel Core
 */

import React from 'react';
import clsx from 'clsx';
import { TimeFrame, TIME_FRAME_OPTIONS } from '../types';

export interface TimeFrameSelectorProps {
  value: TimeFrame;
  onChange: (value: TimeFrame) => void;
  size?: 'sm' | 'md' | 'lg';
  showLabels?: boolean;
  className?: string;
}

export const TimeFrameSelector: React.FC<TimeFrameSelectorProps> = ({
  value,
  onChange,
  size = 'md',
  showLabels = true,
  className,
}) => {
  const sizeClasses = {
    sm: 'px-2 py-1 text-xs',
    md: 'px-3 py-1.5 text-sm',
    lg: 'px-4 py-2 text-base',
  };

  return (
    <div
      className={clsx(
        'inline-flex rounded-xl p-1',
        'bg-hover dark:bg-elevated',
        className
      )}
    >
      {TIME_FRAME_OPTIONS.map((option) => {
        const isSelected = value === option.value;
        
        return (
          <button
            key={option.value}
            onClick={() => onChange(option.value)}
            className={clsx(
              'rounded-lg font-medium transition-all duration-200',
              sizeClasses[size],
              isSelected
                ? 'bg-surface dark:bg-elevated text-purple-600 dark:text-purple-300 shadow-sm'
                : 'text-muted dark:text-muted hover:text-text dark:hover:text-white'
            )}
          >
            {showLabels ? option.label : option.labelEn}
          </button>
        );
      })}
    </div>
  );
};

export default TimeFrameSelector;








