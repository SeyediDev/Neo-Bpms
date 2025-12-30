/**
 * Data Table Component
 * Neo BPMS Admin Panel Core
 */

import { useState, useMemo, useCallback, type ReactNode } from 'react';
import clsx from 'clsx';

// ==================== Types ====================

export interface Column<T> {
  /** Unique column key */
  key: string;
  /** Column header */
  header: ReactNode;
  /** Accessor function or key path */
  accessor?: keyof T | ((row: T) => unknown);
  /** Custom cell renderer */
  render?: (value: unknown, row: T, index: number) => ReactNode;
  /** Column width */
  width?: string | number;
  /** Text alignment */
  align?: 'left' | 'center' | 'right';
  /** Sortable column */
  sortable?: boolean;
  /** Hidden column */
  hidden?: boolean;
}

export interface DataTableProps<T> {
  /** Table data */
  data: T[];
  /** Column definitions */
  columns: Column<T>[];
  /** Row key accessor */
  rowKey: keyof T | ((row: T) => string | number);
  /** Loading state */
  loading?: boolean;
  /** Empty state message */
  emptyMessage?: ReactNode;
  /** Striped rows */
  striped?: boolean;
  /** Hoverable rows */
  hoverable?: boolean;
  /** Compact size */
  compact?: boolean;
  /** Bordered style */
  bordered?: boolean;
  /** Sticky header */
  stickyHeader?: boolean;
  /** Row selection */
  selectable?: boolean;
  /** Selected row keys */
  selectedKeys?: Set<string | number>;
  /** Selection change callback */
  onSelectionChange?: (keys: Set<string | number>) => void;
  /** Row click callback */
  onRowClick?: (row: T, index: number) => void;
  /** Custom class name */
  className?: string;
  /** Max height with scroll */
  maxHeight?: string | number;
}

// ==================== Helpers ====================

function getRowKey<T>(row: T, keyAccessor: keyof T | ((row: T) => string | number)): string | number {
  if (typeof keyAccessor === 'function') {
    return keyAccessor(row);
  }
  return row[keyAccessor] as string | number;
}

function getCellValue<T>(row: T, accessor?: keyof T | ((row: T) => unknown)): unknown {
  if (!accessor) return null;
  if (typeof accessor === 'function') {
    return accessor(row);
  }
  return row[accessor];
}

// ==================== Sub Components ====================

function LoadingSkeleton({ columns }: { columns: number }) {
  return (
    <>
      {[1, 2, 3, 4, 5].map((row) => (
        <tr key={row} className="animate-pulse">
          {Array.from({ length: columns }).map((_, col) => (
            <td key={col} className="px-4 py-3">
              <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded" />
            </td>
          ))}
        </tr>
      ))}
    </>
  );
}

function EmptyState({ message }: { message: ReactNode }) {
  return (
    <tr>
      <td colSpan={100} className="px-4 py-12 text-center">
        <div className="text-gray-400 dark:text-gray-500">
          <svg className="w-12 h-12 mx-auto mb-3 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
          </svg>
          <p>{message || 'داده‌ای یافت نشد'}</p>
        </div>
      </td>
    </tr>
  );
}

// ==================== Main Component ====================

export function DataTable<T>({
  data,
  columns,
  rowKey,
  loading = false,
  emptyMessage,
  striped = false,
  hoverable = true,
  compact = false,
  bordered = false,
  stickyHeader = false,
  selectable = false,
  selectedKeys = new Set(),
  onSelectionChange,
  onRowClick,
  className,
  maxHeight,
}: DataTableProps<T>) {
  const [sortColumn, setSortColumn] = useState<string | null>(null);
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');

  const visibleColumns = useMemo(
    () => columns.filter((col) => !col.hidden),
    [columns]
  );

  // Handle sort
  const handleSort = useCallback((columnKey: string) => {
    if (sortColumn === columnKey) {
      setSortDirection((prev) => (prev === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortColumn(columnKey);
      setSortDirection('asc');
    }
  }, [sortColumn]);

  // Sorted data
  const sortedData = useMemo(() => {
    if (!sortColumn) return data;

    const column = columns.find((col) => col.key === sortColumn);
    if (!column) return data;

    return [...data].sort((a, b) => {
      const aVal = getCellValue(a, column.accessor);
      const bVal = getCellValue(b, column.accessor);

      if (aVal === bVal) return 0;
      if (aVal === null || aVal === undefined) return 1;
      if (bVal === null || bVal === undefined) return -1;

      const comparison = aVal < bVal ? -1 : 1;
      return sortDirection === 'asc' ? comparison : -comparison;
    });
  }, [data, columns, sortColumn, sortDirection]);

  // Selection handlers
  const allSelected = data.length > 0 && data.every((row) => selectedKeys.has(getRowKey(row, rowKey)));
  const someSelected = data.some((row) => selectedKeys.has(getRowKey(row, rowKey)));

  const handleSelectAll = useCallback(() => {
    if (allSelected) {
      onSelectionChange?.(new Set());
    } else {
      const allKeys = new Set(data.map((row) => getRowKey(row, rowKey)));
      onSelectionChange?.(allKeys);
    }
  }, [allSelected, data, rowKey, onSelectionChange]);

  const handleSelectRow = useCallback((row: T) => {
    const key = getRowKey(row, rowKey);
    const newKeys = new Set(selectedKeys);
    if (newKeys.has(key)) {
      newKeys.delete(key);
    } else {
      newKeys.add(key);
    }
    onSelectionChange?.(newKeys);
  }, [rowKey, selectedKeys, onSelectionChange]);

  return (
    <div
      className={clsx(
        'overflow-auto rounded-xl border border-gray-200 dark:border-gray-700',
        className
      )}
      style={maxHeight ? { maxHeight } : undefined}
    >
      <table className="w-full text-sm">
        {/* Header */}
        <thead
          className={clsx(
            'bg-gray-50 dark:bg-gray-800/50 text-gray-600 dark:text-gray-400',
            stickyHeader && 'sticky top-0 z-10'
          )}
        >
          <tr>
            {/* Selection checkbox */}
            {selectable && (
              <th className="w-12 px-4 py-3">
                <input
                  type="checkbox"
                  checked={allSelected}
                  ref={(el) => {
                    if (el) el.indeterminate = someSelected && !allSelected;
                  }}
                  onChange={handleSelectAll}
                  className="w-4 h-4 rounded border-gray-300 text-purple-600 focus:ring-purple-500"
                />
              </th>
            )}

            {/* Data columns */}
            {visibleColumns.map((column) => (
              <th
                key={column.key}
                className={clsx(
                  'px-4 font-medium',
                  compact ? 'py-2' : 'py-3',
                  bordered && 'border-l border-gray-200 dark:border-gray-700 first:border-l-0',
                  column.sortable && 'cursor-pointer select-none hover:bg-gray-100 dark:hover:bg-gray-700',
                  column.align === 'center' && 'text-center',
                  column.align === 'right' && 'text-left',
                  !column.align && 'text-right'
                )}
                style={column.width ? { width: column.width } : undefined}
                onClick={() => column.sortable && handleSort(column.key)}
              >
                <div className="flex items-center gap-1">
                  <span>{column.header}</span>
                  {column.sortable && sortColumn === column.key && (
                    <svg
                      className={clsx(
                        'w-4 h-4 transition-transform',
                        sortDirection === 'desc' && 'rotate-180'
                      )}
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 15l7-7 7 7" />
                    </svg>
                  )}
                </div>
              </th>
            ))}
          </tr>
        </thead>

        {/* Body */}
        <tbody className="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
          {loading ? (
            <LoadingSkeleton columns={visibleColumns.length + (selectable ? 1 : 0)} />
          ) : sortedData.length === 0 ? (
            <EmptyState message={emptyMessage} />
          ) : (
            sortedData.map((row, rowIndex) => {
              const key = getRowKey(row, rowKey);
              const isSelected = selectedKeys.has(key);

              return (
                <tr
                  key={key}
                  onClick={() => onRowClick?.(row, rowIndex)}
                  className={clsx(
                    'transition-colors',
                    striped && rowIndex % 2 === 1 && 'bg-gray-50/50 dark:bg-gray-900/20',
                    hoverable && 'hover:bg-gray-50 dark:hover:bg-gray-700/50',
                    onRowClick && 'cursor-pointer',
                    isSelected && 'bg-purple-50 dark:bg-purple-900/20'
                  )}
                >
                  {/* Selection checkbox */}
                  {selectable && (
                    <td className="w-12 px-4 py-3">
                      <input
                        type="checkbox"
                        checked={isSelected}
                        onChange={(e) => {
                          e.stopPropagation();
                          handleSelectRow(row);
                        }}
                        onClick={(e) => e.stopPropagation()}
                        className="w-4 h-4 rounded border-gray-300 text-purple-600 focus:ring-purple-500"
                      />
                    </td>
                  )}

                  {/* Data cells */}
                  {visibleColumns.map((column) => {
                    const value = getCellValue(row, column.accessor);
                    const content = column.render
                      ? column.render(value, row, rowIndex)
                      : String(value ?? '-');

                    return (
                      <td
                        key={column.key}
                        className={clsx(
                          'px-4 text-gray-900 dark:text-gray-100',
                          compact ? 'py-2' : 'py-3',
                          bordered && 'border-l border-gray-200 dark:border-gray-700 first:border-l-0',
                          column.align === 'center' && 'text-center',
                          column.align === 'right' && 'text-left',
                          !column.align && 'text-right'
                        )}
                        style={column.width ? { width: column.width } : undefined}
                      >
                        {content}
                      </td>
                    );
                  })}
                </tr>
              );
            })
          )}
        </tbody>
      </table>
    </div>
  );
}

export default DataTable;

