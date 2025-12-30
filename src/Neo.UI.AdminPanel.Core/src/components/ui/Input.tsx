/**
 * Input Component
 * Neo BPMS Admin Panel Core
 */

import { forwardRef, useState, type InputHTMLAttributes, type ReactNode } from 'react';
import clsx from 'clsx';

export type InputSize = 'sm' | 'md' | 'lg';

export interface InputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'size'> {
  /** Input label */
  label?: string;
  /** Helper text below input */
  helperText?: string;
  /** Error message */
  error?: string;
  /** Input size */
  size?: InputSize;
  /** Left icon/addon */
  leftIcon?: ReactNode;
  /** Right icon/addon */
  rightIcon?: ReactNode;
  /** Full width input */
  fullWidth?: boolean;
  /** Show password toggle for password inputs */
  showPasswordToggle?: boolean;
}

const sizeClasses: Record<InputSize, { input: string; label: string; helper: string }> = {
  sm: {
    input: 'px-3 py-1.5 text-sm rounded-lg',
    label: 'text-xs mb-1',
    helper: 'text-xs mt-1',
  },
  md: {
    input: 'px-4 py-2.5 text-sm rounded-xl',
    label: 'text-sm mb-1.5',
    helper: 'text-xs mt-1.5',
  },
  lg: {
    input: 'px-5 py-3 text-base rounded-xl',
    label: 'text-base mb-2',
    helper: 'text-sm mt-2',
  },
};

/**
 * Input Component
 */
export const Input = forwardRef<HTMLInputElement, InputProps>(
  (
    {
      label,
      helperText,
      error,
      size = 'md',
      leftIcon,
      rightIcon,
      fullWidth = false,
      showPasswordToggle = false,
      type = 'text',
      disabled,
      className,
      id,
      ...props
    },
    ref
  ) => {
    const [showPassword, setShowPassword] = useState(false);
    const inputId = id || `input-${Math.random().toString(36).substr(2, 9)}`;
    const isPassword = type === 'password';
    const actualType = isPassword && showPassword ? 'text' : type;

    const sizes = sizeClasses[size];

    return (
      <div className={clsx(fullWidth && 'w-full', className)}>
        {/* Label */}
        {label && (
          <label
            htmlFor={inputId}
            className={clsx(
              'block font-medium text-gray-700 dark:text-gray-300',
              sizes.label
            )}
          >
            {label}
          </label>
        )}

        {/* Input wrapper */}
        <div className="relative">
          {/* Left icon */}
          {leftIcon && (
            <div className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400">
              {leftIcon}
            </div>
          )}

          {/* Input */}
          <input
            ref={ref}
            id={inputId}
            type={actualType}
            disabled={disabled}
            className={clsx(
              // Base
              'w-full',
              'bg-white dark:bg-gray-800',
              'border transition-all duration-200',
              'placeholder-gray-400 dark:placeholder-gray-500',
              'text-gray-900 dark:text-white',
              'focus:outline-none focus:ring-2 focus:ring-offset-0',
              // Size
              sizes.input,
              // Left icon padding
              leftIcon && 'pr-10',
              // Right icon/toggle padding
              (rightIcon || (isPassword && showPasswordToggle)) && 'pl-10',
              // State
              error
                ? 'border-red-500 focus:border-red-500 focus:ring-red-500/20'
                : 'border-gray-300 dark:border-gray-600 focus:border-purple-500 focus:ring-purple-500/20',
              // Disabled
              disabled && 'opacity-60 cursor-not-allowed bg-gray-100 dark:bg-gray-900'
            )}
            {...props}
          />

          {/* Right icon or password toggle */}
          <div className="absolute left-3 top-1/2 -translate-y-1/2 flex items-center gap-2">
            {isPassword && showPasswordToggle && (
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300 transition-colors"
                tabIndex={-1}
              >
                {showPassword ? (
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                  </svg>
                ) : (
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                  </svg>
                )}
              </button>
            )}
            {rightIcon && <span className="text-gray-400">{rightIcon}</span>}
          </div>
        </div>

        {/* Helper text or error */}
        {(helperText || error) && (
          <p
            className={clsx(
              sizes.helper,
              error ? 'text-red-500' : 'text-gray-500 dark:text-gray-400'
            )}
          >
            {error || helperText}
          </p>
        )}
      </div>
    );
  }
);

Input.displayName = 'Input';

export default Input;

