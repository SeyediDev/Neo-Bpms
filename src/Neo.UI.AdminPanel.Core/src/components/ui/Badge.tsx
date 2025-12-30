/**
 * Badge Component
 * Neo BPMS Admin Panel Core
 */

import { type HTMLAttributes, type ReactNode } from 'react';
import clsx from 'clsx';

export type BadgeVariant = 'default' | 'primary' | 'success' | 'warning' | 'danger' | 'info';
export type BadgeSize = 'sm' | 'md' | 'lg';

export interface BadgeProps extends HTMLAttributes<HTMLSpanElement> {
  /** Badge variant */
  variant?: BadgeVariant;
  /** Badge size */
  size?: BadgeSize;
  /** Dot indicator (no text) */
  dot?: boolean;
  /** Pulsing animation */
  pulse?: boolean;
  /** Icon before text */
  icon?: ReactNode;
  /** Removable badge */
  removable?: boolean;
  /** Remove callback */
  onRemove?: () => void;
}

const variantClasses: Record<BadgeVariant, string> = {
  default: 'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300',
  primary: 'bg-purple-100 text-purple-700 dark:bg-purple-900/50 dark:text-purple-300',
  success: 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/50 dark:text-emerald-300',
  warning: 'bg-amber-100 text-amber-700 dark:bg-amber-900/50 dark:text-amber-300',
  danger: 'bg-red-100 text-red-700 dark:bg-red-900/50 dark:text-red-300',
  info: 'bg-blue-100 text-blue-700 dark:bg-blue-900/50 dark:text-blue-300',
};

const dotColors: Record<BadgeVariant, string> = {
  default: 'bg-gray-500',
  primary: 'bg-purple-500',
  success: 'bg-emerald-500',
  warning: 'bg-amber-500',
  danger: 'bg-red-500',
  info: 'bg-blue-500',
};

const sizeClasses: Record<BadgeSize, string> = {
  sm: 'px-2 py-0.5 text-xs',
  md: 'px-2.5 py-1 text-xs',
  lg: 'px-3 py-1.5 text-sm',
};

/**
 * Badge Component
 */
export function Badge({
  variant = 'default',
  size = 'md',
  dot = false,
  pulse = false,
  icon,
  removable = false,
  onRemove,
  className,
  children,
  ...props
}: BadgeProps) {
  if (dot) {
    return (
      <span
        className={clsx(
          'inline-flex w-2.5 h-2.5 rounded-full',
          dotColors[variant],
          pulse && 'animate-pulse',
          className
        )}
        {...props}
      />
    );
  }

  return (
    <span
      className={clsx(
        'inline-flex items-center gap-1 font-medium rounded-full',
        variantClasses[variant],
        sizeClasses[size],
        className
      )}
      {...props}
    >
      {icon && <span className="flex-shrink-0">{icon}</span>}
      {children}
      {removable && (
        <button
          type="button"
          onClick={(e) => {
            e.stopPropagation();
            onRemove?.();
          }}
          className="flex-shrink-0 -mr-1 hover:opacity-75 transition-opacity"
        >
          <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      )}
    </span>
  );
}

export default Badge;

