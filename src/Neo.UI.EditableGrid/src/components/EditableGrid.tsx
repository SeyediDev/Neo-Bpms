'use client';

import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { CellEditor } from './CellEditor';
import { FilterBar } from './FilterBar';
import { QueueManager, QueuedChange } from '../lib/queue-manager';
import { GridApiClient, GridData, GridColumn, GridRow, RowActionsConfig } from '../lib/api-client';
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
  rowActions?: RowActionsConfig;
  namespaceId?: string;
  entityId?: string;
  formSubjectId?: string;
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
  rowActions,
  namespaceId,
  entityId,
  formSubjectId,
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

  // Track new rows added to current page
  const [newRows, setNewRows] = useState<Map<string | number, GridRow>>(new Map());

  // Get paginated data (only current page) - only editable rows are from current page
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

  // Handle cell value change - only for current page rows
  const handleCellChange = useCallback((rowId: string | number, columnId: string, value: any) => {
    // Check if row is in current page
    const isInCurrentPage = paginatedData.some(row => row.id === rowId);
    const isNewRow = newRows.has(rowId);
    
    if (!isInCurrentPage && !isNewRow) {
      console.warn('Cannot edit row outside current page');
      return;
    }

    // Get current value
    const currentRow = data.find(row => row.id === rowId);
    const currentValue = currentRow?.[columnId];
    
    // Check if value actually changed (deep comparison)
    const valueChanged = JSON.stringify(currentValue) !== JSON.stringify(value);
    
    if (!valueChanged) {
      // Value didn't change, don't send to server
      return;
    }

    // Update local state immediately
    setData(prev => prev.map(row => 
      row.id === rowId 
        ? { ...row, [columnId]: value }
        : row
    ));

    // Update newRows if it's a new row
    if (isNewRow) {
      setNewRows(prev => {
        const updated = new Map(prev);
        const row = updated.get(rowId);
        if (row) {
          updated.set(rowId, { ...row, [columnId]: value });
        }
        return updated;
      });
    }

    // Queue the change (for both new and existing rows) - only if value changed
    queueManager.enqueue({
      rowId,
      columnId,
      value,
      isNew: isNewRow,
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
  }, [data, queueManager, onDataChange, paginatedData, newRows]);

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

  // Add new row to current page
  const handleAddNewRow = useCallback(() => {
    const newRowId = `new-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
    const newRow: GridRow = {
      id: newRowId,
      ...visibleColumns.reduce((acc, col) => {
        acc[col.id] = col.type === 'boolean' ? false : 
                      col.type === 'number' ? 0 : 
                      col.type === 'date' ? new Date().toISOString() : '';
        return acc;
      }, {} as Record<string, any>)
    };
    
    // Add to newRows map
    setNewRows(prev => new Map(prev).set(newRowId, newRow));
    
    // Add to current page data
    setData(prev => {
      const start = (currentPage - 1) * pageSize;
      const updated = [...prev];
      updated.splice(start, 0, newRow);
      return updated;
    });
    
    // Set editing cell to first editable column
    const firstEditableColumn = visibleColumns.find(col => col.editable !== false);
    if (firstEditableColumn) {
      setEditingCell({ rowId: newRowId, columnId: firstEditableColumn.id });
    }
  }, [visibleColumns, currentPage, pageSize]);

  // Handle keyboard navigation (Arrow keys)
  const handleKeyDown = useCallback((e: React.KeyboardEvent, rowId: string | number, columnId: string) => {
    if (!editingCell || editingCell.rowId !== rowId || editingCell.columnId !== columnId) {
      return;
    }

    const currentRowIndex = paginatedData.findIndex(row => row.id === rowId);
    const currentColumnIndex = visibleColumns.findIndex(col => col.id === columnId);

    if (currentRowIndex === -1 || currentColumnIndex === -1) {
      return;
    }

    let newRowIndex = currentRowIndex;
    let newColumnIndex = currentColumnIndex;
    let shouldAddNewRow = false;

    switch (e.key) {
      case 'ArrowUp':
        e.preventDefault();
        if (currentRowIndex > 0) {
          newRowIndex = currentRowIndex - 1;
        }
        break;
      case 'ArrowDown':
        e.preventDefault();
        if (currentRowIndex < paginatedData.length - 1) {
          newRowIndex = currentRowIndex + 1;
        } else {
          // Add new row if at the end
          shouldAddNewRow = true;
        }
        break;
      case 'ArrowLeft':
        e.preventDefault();
        if (currentColumnIndex > 0) {
          newColumnIndex = currentColumnIndex - 1;
        }
        break;
      case 'ArrowRight':
        e.preventDefault();
        if (currentColumnIndex < visibleColumns.length - 1) {
          newColumnIndex = currentColumnIndex + 1;
        }
        break;
      case 'Tab':
        e.preventDefault();
        if (e.shiftKey) {
          // Shift+Tab: move left
          if (currentColumnIndex > 0) {
            newColumnIndex = currentColumnIndex - 1;
          } else if (currentRowIndex > 0) {
            newRowIndex = currentRowIndex - 1;
            newColumnIndex = visibleColumns.length - 1;
          }
        } else {
          // Tab: move right
          if (currentColumnIndex < visibleColumns.length - 1) {
            newColumnIndex = currentColumnIndex + 1;
          } else if (currentRowIndex < paginatedData.length - 1) {
            newRowIndex = currentRowIndex + 1;
            newColumnIndex = 0;
          } else {
            // Add new row if at the end
            shouldAddNewRow = true;
          }
        }
        break;
      default:
        return; // Don't handle other keys
    }

    if (shouldAddNewRow) {
      // Save current cell first
      handleCellBlur();
      // Add new row after a small delay
      setTimeout(() => {
        handleAddNewRow();
      }, 10);
      return;
    }

    // Move to new cell
    const newRow = paginatedData[newRowIndex];
    const newColumn = visibleColumns[newColumnIndex];
    
    if (newRow && newColumn && newColumn.editable !== false) {
      // Save current cell first
      handleCellBlur();
      // Small delay to ensure blur completes
      setTimeout(() => {
        setEditingCell({ rowId: newRow.id, columnId: newColumn.id });
      }, 10);
    }
  }, [editingCell, paginatedData, visibleColumns, handleCellBlur, handleAddNewRow]);

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

  // Excel export - export current page data only
  const handleExportExcel = async () => {
    try {
      // Export only current page data
      const response = await apiClient.exportExcel(endpoint, {
        filters,
        sortBy,
        columns: visibleColumns.map(c => c.id),
        page: currentPage,
        pageSize: pageSize,
      });
      
      // Download file
      const blob = new Blob([response], { 
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' 
      });
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `grid-export-page-${currentPage}-${new Date().toISOString().split('T')[0]}.xlsx`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Failed to export Excel:', error);
      alert('خطا در خروجی گرفتن از اکسل');
    }
  };

  // Excel import - import to current page
  const handleImportExcel = async (file: File) => {
    try {
      const formData = new FormData();
      formData.append('file', file);
      
      const response = await apiClient.importExcel(endpoint, formData);
      
      if (response.success) {
        alert(`تعداد ${response.imported} رکورد با موفقیت وارد شد`);
        // Reload current page data
        loadData();
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
      <div className="toolbar bg-surface border-b border-border p-3 flex items-center justify-between">
        <div className="flex items-center gap-3">
          {pageSections && pageSections.length > 0 && (
            <select
              value={currentSection || ''}
              onChange={(e) => setCurrentSection(e.target.value || undefined)}
              className="px-3 py-2 border border-border rounded"
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
                  ? 'bg-accent text-on-accent'
                  : 'bg-selection text-text hover:bg-hover'
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
            <label className="px-4 py-2 bg-accent text-on-accent rounded hover:bg-accent-hover cursor-pointer">
              📤 آپلود اکسل
              <input
                type="file"
                accept=".xlsx,.xls"
                onChange={(e) => {
                  const file = e.target.files?.[0];
                  if (file) handleImportExcel(file);
                  // Reset input to allow selecting same file again
                  e.target.value = '';
                }}
                className="hidden"
              />
            </label>
          )}

          {/* Add New Row Button - only for current page */}
          <button
            onClick={handleAddNewRow}
            className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
            title="افزودن سطر جدید به صفحه فعلی"
          >
            ➕ افزودن سطر
          </button>
        </div>

        {enablePagination && (
          <div className="flex items-center gap-2">
            <button
              onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
              disabled={currentPage === 1}
              className="px-3 py-2 border border-border rounded disabled:opacity-50 disabled:cursor-not-allowed"
            >
              قبلی
            </button>
            <span className="px-4">
              صفحه {currentPage} از {totalPages} ({totalCount} رکورد)
            </span>
            <button
              onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
              disabled={currentPage === totalPages}
              className="px-3 py-2 border border-border rounded disabled:opacity-50 disabled:cursor-not-allowed"
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
      <div className="grid-container overflow-auto border border-border rounded-lg" style={{ height: `calc(${height} - 120px)` }}>
        {loading ? (
          <div className="flex items-center justify-center h-64">
            <div className="spinner border-4 border-border border-t-blue-600 rounded-full w-12 h-12 animate-spin"></div>
          </div>
        ) : (
          <table className="w-full border-collapse" style={{ minWidth: '100%' }}>
            {/* Header */}
            <thead className="sticky top-0 z-10 bg-canvas">
              <tr>
                {visibleColumns.map(column => {
                  const isSorted = sortBy?.column === column.id;
                  const sortDirection = isSorted ? sortBy.direction : null;
                  
                  return (
                    <th
                      key={column.id}
                      className="px-4 py-3 text-right font-semibold text-sm text-text border-b border-border cursor-pointer hover:bg-hover select-none"
                      style={{ width: column.width || 'auto', minWidth: column.width || '150px' }}
                      onClick={() => {
                        if (sortBy?.column === column.id) {
                          // Toggle direction
                          setSortBy(sortBy.direction === 'asc' 
                            ? { column: column.id, direction: 'desc' }
                            : null);
                        } else {
                          // New sort
                          setSortBy({ column: column.id, direction: 'asc' });
                        }
                      }}
                    >
                      <div className="flex items-center justify-end gap-2">
                        <span>{column.name}</span>
                        {sortDirection === 'asc' && <span className="text-blue-600">▲</span>}
                        {sortDirection === 'desc' && <span className="text-blue-600">▼</span>}
                        {!sortDirection && <span className="text-muted opacity-0 hover:opacity-100">⇅</span>}
                      </div>
                    </th>
                  );
                })}
                {/* Actions Column */}
                {rowActions && (
                  <th
                    className="px-4 py-3 text-center font-semibold text-sm text-text border-b border-border sticky right-0 bg-canvas"
                    style={{ width: '80px', minWidth: '80px' }}
                  >
                    عملیات
                  </th>
                )}
            </tr>
          </thead>

          {/* Body */}
          <tbody>
            {paginatedData.map((row) => {
              const isNewRow = newRows.has(row.id);
              return (
                <tr 
                  key={row.id} 
                  className={clsx('hover:bg-hover', {
                    'bg-green-50': isNewRow,
                    'border-l-4 border-green-500': isNewRow,
                  })}
                >
                  {visibleColumns.map(column => {
                    const isEditing = editingCell?.rowId === row.id && editingCell?.columnId === column.id;
                    const cellStatus = getCellStatus(row.id, column.id);
                    const cellValue = row[column.id];
                    // Only allow editing for current page rows
                    const canEdit = column.editable !== false && (isNewRow || paginatedData.includes(row));

                    return (
                      <td
                        key={column.id}
                        className={clsx(
                          'grid-cell',
                          {
                            'cursor-pointer': canEdit,
                            'cursor-not-allowed opacity-50': !canEdit,
                            'editing': isEditing,
                            'pending': cellStatus === 'pending',
                            'saving': cellStatus === 'saving',
                            'saved': cellStatus === 'saved',
                            'error': cellStatus === 'error',
                          }
                        )}
                        onClick={() => canEdit && handleCellClick(row.id, column.id)}
                        style={{ width: column.width || 'auto' }}
                        title={!canEdit ? 'فقط ردیف‌های صفحه فعلی قابل ویرایش هستند' : undefined}
                      >
                        {isEditing ? (
                          <CellEditor
                            value={cellValue}
                            column={column}
                            onChange={(value) => handleCellChange(row.id, column.id, value)}
                            onBlur={handleCellBlur}
                            onKeyDown={(e) => handleKeyDown(e, row.id, column.id)}
                          />
                        ) : (
                          <span className="px-2 py-1 block">
                            {formatCellValue(cellValue, column)}
                            {isNewRow && column.id === visibleColumns[0]?.id && (
                              <span className="ml-2 text-xs text-green-600">(جدید)</span>
                            )}
                          </span>
                        )}
                      </td>
                    );
                  })}
                  {/* Actions Cell */}
                  {rowActions && (
                    <td
                      className="px-2 py-2 text-center border-b border-border sticky right-0 bg-surface"
                      style={{ width: '80px', minWidth: '80px' }}
                    >
                      <div className="flex items-center justify-center gap-1 flex-wrap">
                        {/* Main Actions (Details, Edit, Delete) */}
                        {rowActions.hasDetails && (
                          <a
                            href={`/Form/IframeForm?NamespaceId=${namespaceId}&EntityId=${entityId}&FormId=${rowActions.detailFormId}&Id=${row.id}`}
                            target="_self"
                            className="text-blue-600 hover:text-blue-800 p-1 rounded hover:bg-blue-50"
                            title="جزئیات"
                          >
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                              <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/>
                            </svg>
                          </a>
                        )}
                        {rowActions.hasEdit && (
                          <a
                            href={`/Form/IframeForm?NamespaceId=${namespaceId}&EntityId=${entityId}&FormId=${rowActions.editFormId}&Id=${row.id}`}
                            target="_self"
                            className="text-green-600 hover:text-green-800 p-1 rounded hover:bg-green-50"
                            title="ویرایش"
                          >
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                              <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/>
                            </svg>
                          </a>
                        )}
                        {rowActions.hasDelete && (
                          <a
                            href={`/Form/Delete?NamespaceId=${namespaceId}&EntityId=${entityId}&FormId=${rowActions.deleteFormId}&Id=${row.id}`}
                            target="_self"
                            className="text-red-600 hover:text-red-800 p-1 rounded hover:bg-red-50"
                            title="حذف"
                          >
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                              <path d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"/>
                            </svg>
                          </a>
                        )}
                        
                        {/* Subject Forms Links */}
                        {rowActions.subjectForms?.map((subject, idx) => (
                          <div key={idx} className="flex gap-1">
                            {subject.hasDetailsForm && (
                              <a
                                href={`/Form/IframeForm?NamespaceId=${namespaceId}&EntityId=${entityId}&FormId=${subject.detailFormId}&Id=${row.id}`}
                                target="_self"
                                className="text-purple-600 hover:text-purple-800 p-1 rounded hover:bg-purple-50"
                                title={`جزئیات ${subject.alias}`}
                              >
                                <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
                                  <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/>
                                </svg>
                              </a>
                            )}
                            {subject.hasEditForm && (
                              <a
                                href={`/Form/IframeForm?NamespaceId=${namespaceId}&EntityId=${entityId}&FormId=${subject.editFormId}&Id=${row.id}`}
                                target="_self"
                                className="text-orange-600 hover:text-orange-800 p-1 rounded hover:bg-orange-50"
                                title={`ویرایش ${subject.alias}`}
                              >
                                <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
                                  <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/>
                                </svg>
                              </a>
                            )}
                          </div>
                        ))}
                        
                        {/* Specific Links */}
                        {rowActions.specificLinks?.map((link, idx) => {
                          // Build URL with parameters from row data
                          const params = new URLSearchParams();
                          if (link.linkParameters) {
                            Object.entries(link.linkParameters).forEach(([key, fieldName]) => {
                              const value = row[fieldName] ?? row.id;
                              if (value != null) {
                                params.append(key, String(value));
                              }
                            });
                          }
                          const linkUrl = `${link.linkTarget}?${params.toString()}`;
                          
                          return (
                            <a
                              key={idx}
                              href={linkUrl}
                              target="_self"
                              className="text-indigo-600 hover:text-indigo-800 p-1 rounded hover:bg-indigo-50"
                              title={link.label}
                            >
                              <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
                                <path d="M3.9 12c0-1.71 1.39-3.1 3.1-3.1h4V7H7c-2.76 0-5 2.24-5 5s2.24 5 5 5h4v-1.9H7c-1.71 0-3.1-1.39-3.1-3.1zM8 13h8v-2H8v2zm9-6h-4v1.9h4c1.71 0 3.1 1.39 3.1 3.1s-1.39 3.1-3.1 3.1h-4V17h4c2.76 0 5-2.24 5-5s-2.24-5-5-5z"/>
                              </svg>
                            </a>
                          );
                        })}
                      </div>
                    </td>
                  )}
                </tr>
              );
            })}
          </tbody>
        </table>

        )}
        {!loading && paginatedData.length === 0 && (
          <div className="flex items-center justify-center h-64 text-muted">
            هیچ داده‌ای یافت نشد
          </div>
        )}
      </div>

      {/* Queue Status Indicator */}
      <div className="mt-2 text-xs text-muted flex items-center gap-4">
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

