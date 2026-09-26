/**
 * Button Component
 * Neo BPMS Admin Panel Core
 */

import { forwardRef, type ButtonHTMLAttributes, type ReactNode } from 'react';
import clsx from 'clsx';

export type ButtonVariant = 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger' | 'success';
export type ButtonSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  /** Button variant */
  variant?: ButtonVariant;
  /** Button size */
  size?: ButtonSize;
  /** Full width button */
  fullWidth?: boolean;
  /** Loading state */
  loading?: boolean;
  /** Icon before text */
  leftIcon?: ReactNode;
  /** Icon after text */
  rightIcon?: ReactNode;
  /** Icon only button (for accessibility) */
  iconOnly?: boolean;
}

const variantClasses: Record<ButtonVariant, string> = {
  primary: clsx(
    'bg-accent',
    'hover:bg-accent-hover',
    'text-on-accent shadow-sm',
    'focus:ring-accent'
  ),
  secondary: clsx(
    'bg-hover dark:bg-elevated',
    'hover:bg-hover dark:hover:bg-hover',
    'text-text dark:text-text',
    'focus:ring-accent'
  ),
  outline: clsx(
    'border-2 border-border dark:border-border',
    'hover:border-accent',
    'hover:bg-selection',
    'text-text dark:text-subtle',
    'focus:ring-accent'
  ),
  ghost: clsx(
    'hover:bg-hover dark:hover:bg-hover',
    'text-text dark:text-subtle',
    'focus:ring-accent'
  ),
  danger: clsx(
    'bg-red-600 hover:bg-red-500',
    'text-white shadow-lg shadow-red-500/25',
    'focus:ring-red-500'
  ),
  success: clsx(
    'bg-emerald-600 hover:bg-emerald-500',
    'text-white shadow-lg shadow-emerald-500/25',
    'focus:ring-emerald-500'
  ),
};

const sizeClasses: Record<ButtonSize, string> = {
  xs: 'px-2.5 py-1 text-xs rounded-md gap-1',
  sm: 'px-3 py-1.5 text-sm rounded-lg gap-1.5',
  md: 'px-4 py-2 text-sm rounded-lg gap-2',
  lg: 'px-5 py-2.5 text-base rounded-xl gap-2',
  xl: 'px-6 py-3 text-lg rounded-xl gap-2.5',
};

const iconSizeClasses: Record<ButtonSize, string> = {
  xs: 'p-1 rounded-md',
  sm: 'p-1.5 rounded-lg',
  md: 'p-2 rounded-lg',
  lg: 'p-2.5 rounded-xl',
  xl: 'p-3 rounded-xl',
};

/**
 * Button Component
 */
export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      variant = 'primary',
      size = 'md',
      fullWidth = false,
      loading = false,
      leftIcon,
      rightIcon,
      iconOnly = false,
      disabled,
      className,
      children,
      ...props
    },
    ref
  ) => {
    const isDisabled = disabled || loading;

    return (
      <button
        ref={ref}
        disabled={isDisabled}
        className={clsx(
          // Base styles
          'inline-flex items-center justify-center font-medium',
          'transition-all duration-200',
          'focus:outline-none focus:ring-2 focus:ring-offset-2 dark:focus:ring-offset-gray-900',
          // Variant
          variantClasses[variant],
          // Size
          iconOnly ? iconSizeClasses[size] : sizeClasses[size],
          // Width
          fullWidth && 'w-full',
          // Disabled
          isDisabled && 'opacity-60 cursor-not-allowed',
          // Custom
          className
        )}
        {...props}
      >
        {/* Loading spinner */}
        {loading && (
          <svg
            className="animate-spin h-4 w-4"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle
              className="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              strokeWidth="4"
            />
            <path
              className="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            />
          </svg>
        )}

        {/* Left icon */}
        {!loading && leftIcon && <span className="flex-shrink-0">{leftIcon}</span>}

        {/* Content */}
        {children && <span>{children}</span>}

        {/* Right icon */}
        {rightIcon && <span className="flex-shrink-0">{rightIcon}</span>}
      </button>
    );
  }
);

Button.displayName = 'Button';

export default Button;

