'use client';

import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { CellEditor } from './CellEditor';
import { FilterBar } from './FilterBar';
import { QueueManager, QueuedChange } from '../lib/queue-manager';
import { GridApiClient, GridData, GridColumn, GridRow } from '../lib/api-client';
import clsx from 'clsx';

export interface FilterCondition {
  columnId: string;
  operator: 'equals' | 'notEquals' | 'contains' | 'startsWith' | 'endsWith' | 'greaterThan' | 'lessThan' | 'greaterThanOrEqual' | 'lessThanOrEqual' | 'in' | 'notIn';
  value: any;
}

export interface PageSection {
  id: string;
  name: string;
  columns: string[]; // Column IDs to show in this section
}

export interface EditableGridProps {
  endpoint: string;
  columns: GridColumn[];
  initialData?: GridRow[];
  onDataChange?: (data: GridRow[]) => void;
  height?: string;
  enableVirtualization?: boolean;
  pageSize?: number;
  enableFilters?: boolean;
  enablePagination?: boolean;
  enableExcelExport?: boolean;
  enableExcelImport?: boolean;
  pageSections?: PageSection[];
  activeSection?: string;
}

export const EditableGrid: React.FC<EditableGridProps> = ({
  endpoint,
  columns,
  initialData = [],
  onDataChange,
  height = '600px',
  enableVirtualization = true,
  pageSize = 50,
  enableFilters = true,
  enablePagination = true,
  enableExcelExport = true,
  enableExcelImport = true,
  pageSections,
  activeSection,
}) => {
  const [data, setData] = useState<GridRow[]>(initialData);
  const [editingCell, setEditingCell] = useState<{ rowId: string | number; columnId: string } | null>(null);
  const [cellStatuses, setCellStatuses] = useState<Record<string, QueuedChange['status']>>({});
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [filters, setFilters] = useState<FilterCondition[]>([]);
  const [sortBy, setSortBy] = useState<{ column: string; direction: 'asc' | 'desc' } | null>(null);
  const [loading, setLoading] = useState(false);
  const [showFilterBar, setShowFilterBar] = useState(false);
  const [currentSection, setCurrentSection] = useState<string | undefined>(activeSection);
  const [apiClient] = useState(() => new GridApiClient());
  const [queueManager] = useState(() => {
    return new QueueManager(
      {
        batchSize: 10,
        batchDelay: 500, // 500ms debounce
        maxRetries: 3,
        retryDelay: 1000,
      },
      async (changes) => {
        // Batch update API call
        const response = await apiClient.batchUpdate(endpoint, changes);
        if (!response.success && response.errors) {
          throw new Error('Batch update failed');
        }
      },
      (change) => {
        // Update cell status
        const cellKey = `${change.rowId}_${change.columnId}`;
        setCellStatuses(prev => ({
          ...prev,
          [cellKey]: change.status,
        }));
      }
    );
  });

  // Get visible columns based on active section
  const visibleColumns = useMemo(() => {
    if (currentSection && pageSections) {
      const section = pageSections.find(s => s.id === currentSection);
      if (section) {
        return columns.filter(col => section.columns.includes(col.id));
      }
    }
    return columns;
  }, [columns, currentSection, pageSections]);

  // Get paginated data (only current page)
  const paginatedData = useMemo(() => {
    if (!enablePagination) return data;
    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    return data.slice(start, end);
  }, [data, currentPage, pageSize, enablePagination]);

  const totalPages = useMemo(() => {
    if (!enablePagination) return 1;
    return Math.ceil(totalCount / pageSize);
  }, [totalCount, pageSize, enablePagination]);

  // Load data with filters, pagination, and sorting
  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const params: any = {
        page: currentPage,
        pageSize: pageSize,
      };

      if (sortBy) {
        params.sortBy = sortBy.column;
        params.sortDirection = sortBy.direction;
      }

      if (filters.length > 0) {
        params.filter = JSON.stringify(filters);
      }

      const gridData = await apiClient.getData(endpoint, params);
      setData(gridData.rows || []);
      setTotalCount(gridData.totalCount || gridData.rows?.length || 0);
      
      if (onDataChange) {
        onDataChange(gridData.rows || []);
      }
    } catch (error) {
      console.error('Failed to load grid data:', error);
    } finally {
      setLoading(false);
    }
  }, [endpoint, currentPage, pageSize, sortBy, filters, apiClient, onDataChange]);

  // Load initial data
  useEffect(() => {
    if (initialData.length === 0) {
      loadData();
    } else {
      setTotalCount(initialData.length);
    }
  }, [endpoint]);

  // Reload when filters, pagination, or sorting changes
  useEffect(() => {
    if (initialData.length === 0) {
      loadData();
    }
  }, [currentPage, filters, sortBy]);

  // Handle cell value change
  const handleCellChange = useCallback((rowId: string | number, columnId: string, value: any) => {
    // Update local state immediately
    setData(prev => prev.map(row => 
      row.id === rowId 
        ? { ...row, [columnId]: value }
        : row
    ));

    // Queue the change
    queueManager.enqueue({
      rowId,
      columnId,
      value,
    });

    // Notify parent
    if (onDataChange) {
      const updatedData = data.map(row => 
        row.id === rowId 
          ? { ...row, [columnId]: value }
          : row
      );
      onDataChange(updatedData);
    }
  }, [data, queueManager, onDataChange]);

  // Get cell status
  const getCellStatus = useCallback((rowId: string | number, columnId: string): QueuedChange['status'] => {
    const cellKey = `${rowId}_${columnId}`;
    return cellStatuses[cellKey] || 'saved';
  }, [cellStatuses]);

  // Handle cell click
  const handleCellClick = useCallback((rowId: string | number, columnId: string) => {
    const column = columns.find(c => c.id === columnId);
    if (column && column.editable !== false) {
      setEditingCell({ rowId, columnId });
    }
  }, [columns]);

  // Handle cell blur
  const handleCellBlur = useCallback(() => {
    setEditingCell(null);
  }, []);

  // Format cell value for display
  const formatCellValue = useCallback((value: any, column: GridColumn): string => {
    if (value === null || value === undefined) return '';

    switch (column.type) {
      case 'date':
        try {
          const date = new Date(value);
          return date.toLocaleDateString('fa-IR');
        } catch {
          return String(value);
        }
      case 'boolean':
        return value ? '✓' : '';
      case 'select':
        const option = column.options?.find(opt => opt.value === value);
        return option ? option.label : String(value);
      case 'multiselect':
        if (!Array.isArray(value)) return '';
        const selectedOptions = column.options?.filter(opt => value.includes(opt.value));
        return selectedOptions?.map(opt => opt.label).join(', ') || '';
      case 'number':
        return typeof value === 'number' ? value.toLocaleString('fa-IR') : String(value);
      default:
        return String(value);
    }
  }, []);

  // Excel export
  const handleExportExcel = async () => {
    try {
      const response = await apiClient.exportExcel(endpoint, {
        filters,
        sortBy,
        columns: visibleColumns.map(c => c.id),
      });
      
      // Download file
      const blob = new Blob([response], { 
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' 
      });
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `grid-export-${new Date().toISOString().split('T')[0]}.xlsx`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Failed to export Excel:', error);
      alert('خطا در خروجی گرفتن از اکسل');
    }
  };

  // Excel import
  const handleImportExcel = async (file: File) => {
    try {
      const formData = new FormData();
      formData.append('file', file);
      
      const response = await apiClient.importExcel(endpoint, formData);
      
      if (response.success) {
        alert(`تعداد ${response.imported} رکورد با موفقیت وارد شد`);
        loadData(); // Reload data
      } else {
        alert(`خطا: ${response.errors?.join(', ')}`);
      }
    } catch (error) {
      console.error('Failed to import Excel:', error);
      alert('خطا در وارد کردن از اکسل');
    }
  };

  return (
    <div className="editable-grid w-full" style={{ height }}>
      {/* Toolbar */}
      <div className="toolbar bg-white border-b border-gray-200 p-3 flex items-center justify-between">
        <div className="flex items-center gap-3">
          {pageSections && pageSections.length > 0 && (
            <select
              value={currentSection || ''}
              onChange={(e) => setCurrentSection(e.target.value || undefined)}
              className="px-3 py-2 border border-gray-300 rounded"
            >
              <option value="">همه بخش‌ها</option>
              {pageSections.map(section => (
                <option key={section.id} value={section.id}>{section.name}</option>
              ))}
            </select>
          )}

          {enableFilters && (
            <button
              onClick={() => setShowFilterBar(!showFilterBar)}
              className={clsx(
                'px-4 py-2 rounded',
                showFilterBar 
                  ? 'bg-blue-600 text-white' 
                  : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
              )}
            >
              🔍 فیلترها {filters.length > 0 && `(${filters.length})`}
            </button>
          )}

          {enableExcelExport && (
            <button
              onClick={handleExportExcel}
              className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
            >
              📥 دانلود اکسل
            </button>
          )}

          {enableExcelImport && (
            <label className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 cursor-pointer">
              📤 آپلود اکسل
              <input
                type="file"
                accept=".xlsx,.xls"
                onChange={(e) => {
                  const file = e.target.files?.[0];
                  if (file) handleImportExcel(file);
                }}
                className="hidden"
              />
            </label>
          )}
        </div>

        {enablePagination && (
          <div className="flex items-center gap-2">
            <button
              onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
              disabled={currentPage === 1}
              className="px-3 py-2 border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed"
            >
              قبلی
            </button>
            <span className="px-4">
              صفحه {currentPage} از {totalPages} ({totalCount} رکورد)
            </span>
            <button
              onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
              disabled={currentPage === totalPages}
              className="px-3 py-2 border border-gray-300 rounded disabled:opacity-50 disabled:cursor-not-allowed"
            >
              بعدی
            </button>
          </div>
        )}
      </div>

      {/* Filter Bar */}
      {showFilterBar && enableFilters && (
        <FilterBar
          columns={columns}
          filters={filters}
          onFiltersChange={setFilters}
          onClose={() => setShowFilterBar(false)}
        />
      )}

      {/* Grid */}
      <div className="grid-container overflow-auto border border-gray-200 rounded-lg" style={{ height: `calc(${height} - 120px)` }}>
        {loading ? (
          <div className="flex items-center justify-center h-64">
            <div className="spinner border-4 border-gray-200 border-t-blue-600 rounded-full w-12 h-12 animate-spin"></div>
          </div>
        ) : (
          <table className="w-full border-collapse" style={{ minWidth: '100%' }}>
            {/* Header */}
            <thead className="sticky top-0 z-10 bg-gray-50">
              <tr>
                {visibleColumns.map(column => (
                <th
                  key={column.id}
                  className="px-4 py-3 text-right font-semibold text-sm text-gray-700 border-b border-gray-200"
                  style={{ width: column.width || 'auto', minWidth: column.width || '150px' }}
                >
                  {column.name}
                </th>
              ))}
            </tr>
          </thead>

          {/* Body */}
          <tbody>
            {paginatedData.map((row) => (
              <tr key={row.id} className="hover:bg-gray-50">
                {columns.map(column => {
                  const isEditing = editingCell?.rowId === row.id && editingCell?.columnId === column.id;
                  const cellStatus = getCellStatus(row.id, column.id);
                  const cellValue = row[column.id];

                  return (
                    <td
                      key={column.id}
                      className={clsx(
                        'grid-cell cursor-pointer',
                        {
                          'editing': isEditing,
                          'pending': cellStatus === 'pending',
                          'saving': cellStatus === 'saving',
                          'saved': cellStatus === 'saved',
                          'error': cellStatus === 'error',
                        }
                      )}
                      onClick={() => handleCellClick(row.id, column.id)}
                      style={{ width: column.width || 'auto' }}
                    >
                      {isEditing ? (
                        <CellEditor
                          value={cellValue}
                          column={column}
                          onChange={(value) => handleCellChange(row.id, column.id, value)}
                          onBlur={handleCellBlur}
                        />
                      ) : (
                        <span className="px-2 py-1 block">
                          {formatCellValue(cellValue, column)}
                        </span>
                      )}
                    </td>
                  );
                })}
              </tr>
            ))}
          </tbody>
        </table>

        )}
        {!loading && paginatedData.length === 0 && (
          <div className="flex items-center justify-center h-64 text-gray-500">
            هیچ داده‌ای یافت نشد
          </div>
        )}
      </div>

      {/* Queue Status Indicator */}
      <div className="mt-2 text-xs text-gray-500 flex items-center gap-4">
        <span>
          وضعیت صف: {queueManager.getStatus().pending} در انتظار, {queueManager.getStatus().saving} در حال ذخیره
        </span>
        {apiClient.getCircuitState() === 'open' && (
          <span className="text-red-500">⚠️ اتصال قطع شده</span>
        )}
      </div>
    </div>
  );
};

