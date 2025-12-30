/**
 * Select Component
 * Neo BPMS Admin Panel Core
 */

import { 
  useState, 
  useRef, 
  useEffect, 
  useCallback,
  useMemo,
  type ReactNode,
} from 'react';
import { createPortal } from 'react-dom';
import clsx from 'clsx';

// ==================== Types ====================

export interface SelectOption<T = string> {
  value: T;
  label: string;
  icon?: ReactNode;
  disabled?: boolean;
  group?: string;
}

export interface SelectProps<T = string> {
  /** Options */
  options: SelectOption<T>[];
  /** Selected value */
  value?: T;
  /** Change handler */
  onChange?: (value: T | undefined) => void;
  /** Placeholder */
  placeholder?: string;
  /** Label */
  label?: string;
  /** Error message */
  error?: string;
  /** Helper text */
  helperText?: string;
  /** Disabled */
  disabled?: boolean;
  /** Clearable */
  clearable?: boolean;
  /** Searchable */
  searchable?: boolean;
  /** Full width */
  fullWidth?: boolean;
  /** Size */
  size?: 'sm' | 'md' | 'lg';
  /** Custom class */
  className?: string;
}

// ==================== Component ====================

export function Select<T = string>({
  options,
  value,
  onChange,
  placeholder = 'انتخاب کنید...',
  label,
  error,
  helperText,
  disabled = false,
  clearable = false,
  searchable = false,
  fullWidth = false,
  size = 'md',
  className,
}: SelectProps<T>) {
  const [isOpen, setIsOpen] = useState(false);
  const [search, setSearch] = useState('');
  const containerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);
  const [dropdownPosition, setDropdownPosition] = useState({ top: 0, left: 0, width: 0 });

  // Selected option
  const selectedOption = useMemo(
    () => options.find((opt) => opt.value === value),
    [options, value]
  );

  // Filtered options
  const filteredOptions = useMemo(() => {
    if (!searchable || !search) return options;
    const lowerSearch = search.toLowerCase();
    return options.filter((opt) =>
      opt.label.toLowerCase().includes(lowerSearch)
    );
  }, [options, search, searchable]);

  // Group options
  const groupedOptions = useMemo(() => {
    const groups: Record<string, SelectOption<T>[]> = {};
    const ungrouped: SelectOption<T>[] = [];

    filteredOptions.forEach((opt) => {
      if (opt.group) {
        if (!groups[opt.group]) groups[opt.group] = [];
        groups[opt.group].push(opt);
      } else {
        ungrouped.push(opt);
      }
    });

    return { groups, ungrouped };
  }, [filteredOptions]);

  // Calculate dropdown position
  useEffect(() => {
    if (!isOpen || !containerRef.current) return;

    const rect = containerRef.current.getBoundingClientRect();
    setDropdownPosition({
      top: rect.bottom + 4,
      left: rect.left,
      width: rect.width,
    });
  }, [isOpen]);

  // Close on click outside
  useEffect(() => {
    if (!isOpen) return;

    const handleClickOutside = (e: MouseEvent) => {
      if (
        containerRef.current &&
        !containerRef.current.contains(e.target as Node) &&
        dropdownRef.current &&
        !dropdownRef.current.contains(e.target as Node)
      ) {
        setIsOpen(false);
        setSearch('');
      }
    };

    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        setIsOpen(false);
        setSearch('');
      }
    };

    document.addEventListener('mousedown', handleClickOutside as EventListener);
    document.addEventListener('keydown', handleEscape);

    return () => {
      document.removeEventListener('mousedown', handleClickOutside as EventListener);
      document.removeEventListener('keydown', handleEscape);
    };
  }, [isOpen]);

  // Focus search input when opened
  useEffect(() => {
    if (isOpen && searchable && inputRef.current) {
      inputRef.current.focus();
    }
  }, [isOpen, searchable]);

  const handleSelect = useCallback(
    (opt: SelectOption<T>) => {
      if (opt.disabled) return;
      onChange?.(opt.value);
      setIsOpen(false);
      setSearch('');
    },
    [onChange]
  );

  const handleClear = useCallback(
    (e: React.MouseEvent) => {
      e.stopPropagation();
      onChange?.(undefined);
    },
    [onChange]
  );

  const sizeClasses = {
    sm: 'px-3 py-1.5 text-sm',
    md: 'px-4 py-2.5 text-sm',
    lg: 'px-5 py-3 text-base',
  };

  const renderOption = (opt: SelectOption<T>) => (
    <button
      key={String(opt.value)}
      onClick={() => handleSelect(opt)}
      disabled={opt.disabled}
      className={clsx(
        'w-full flex items-center gap-2 px-4 py-2 text-right',
        'transition-colors',
        opt.disabled
          ? 'text-gray-400 cursor-not-allowed'
          : opt.value === value
          ? 'bg-purple-50 dark:bg-purple-900/30 text-purple-700 dark:text-purple-300'
          : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
      )}
    >
      {opt.icon && <span className="flex-shrink-0">{opt.icon}</span>}
      <span className="flex-1">{opt.label}</span>
      {opt.value === value && (
        <svg className="w-4 h-4 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
        </svg>
      )}
    </button>
  );

  return (
    <div className={clsx(fullWidth && 'w-full', className)}>
      {/* Label */}
      {label && (
        <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
          {label}
        </label>
      )}

      {/* Select container */}
      <div
        ref={containerRef}
        onClick={() => !disabled && setIsOpen(!isOpen)}
        className={clsx(
          'relative flex items-center gap-2 cursor-pointer',
          'bg-white dark:bg-gray-800 border rounded-xl',
          'transition-all duration-200',
          sizeClasses[size],
          error
            ? 'border-red-500 focus-within:ring-2 focus-within:ring-red-500/20'
            : isOpen
            ? 'border-purple-500 ring-2 ring-purple-500/20'
            : 'border-gray-300 dark:border-gray-600 hover:border-gray-400 dark:hover:border-gray-500',
          disabled && 'opacity-60 cursor-not-allowed bg-gray-100 dark:bg-gray-900'
        )}
      >
        {/* Selected value or placeholder */}
        <div className="flex-1 flex items-center gap-2 min-w-0">
          {selectedOption?.icon && (
            <span className="flex-shrink-0">{selectedOption.icon}</span>
          )}
          <span
            className={clsx(
              'truncate',
              selectedOption
                ? 'text-gray-900 dark:text-white'
                : 'text-gray-400'
            )}
          >
            {selectedOption?.label || placeholder}
          </span>
        </div>

        {/* Clear button */}
        {clearable && value !== undefined && !disabled && (
          <button
            onClick={handleClear}
            className="flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        )}

        {/* Chevron */}
        <svg
          className={clsx(
            'w-4 h-4 text-gray-400 transition-transform',
            isOpen && 'rotate-180'
          )}
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
        </svg>
      </div>

      {/* Helper/Error text */}
      {(helperText || error) && (
        <p className={clsx(
          'text-xs mt-1.5',
          error ? 'text-red-500' : 'text-gray-500 dark:text-gray-400'
        )}>
          {error || helperText}
        </p>
      )}

      {/* Dropdown */}
      {isOpen &&
        createPortal(
          <div
            ref={dropdownRef}
            className={clsx(
              'fixed z-50 py-1 max-h-60 overflow-auto',
              'bg-white dark:bg-gray-800',
              'border border-gray-200 dark:border-gray-700',
              'rounded-xl shadow-lg',
              'animate-in fade-in zoom-in-95 duration-150'
            )}
            style={{
              top: dropdownPosition.top,
              left: dropdownPosition.left,
              width: dropdownPosition.width,
            }}
          >
            {/* Search input */}
            {searchable && (
              <div className="p-2 border-b border-gray-200 dark:border-gray-700">
                <input
                  ref={inputRef}
                  type="text"
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  placeholder="جستجو..."
                  className={clsx(
                    'w-full px-3 py-2 text-sm rounded-lg',
                    'bg-gray-100 dark:bg-gray-700',
                    'border-0 focus:ring-2 focus:ring-purple-500/20',
                    'text-gray-900 dark:text-white',
                    'placeholder-gray-400'
                  )}
                />
              </div>
            )}

            {/* Options */}
            {filteredOptions.length === 0 ? (
              <div className="px-4 py-3 text-sm text-gray-500 text-center">
                نتیجه‌ای یافت نشد
              </div>
            ) : (
              <>
                {/* Ungrouped options */}
                {groupedOptions.ungrouped.map(renderOption)}

                {/* Grouped options */}
                {Object.entries(groupedOptions.groups).map(([group, opts]) => (
                  <div key={group}>
                    <div className="px-4 py-2 text-xs font-medium text-gray-500 dark:text-gray-400 uppercase">
                      {group}
                    </div>
                    {opts.map(renderOption)}
                  </div>
                ))}
              </>
            )}
          </div>,
          document.body
        )}
    </div>
  );
}

export default Select;

