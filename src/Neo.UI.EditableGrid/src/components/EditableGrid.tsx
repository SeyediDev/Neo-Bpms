'use client';

import React, { useState, useEffect, useCallback, useMemo } from 'react';
import { CellEditor } from './CellEditor';
import { QueueManager, QueuedChange } from '../lib/queue-manager';
import { GridApiClient, GridData, GridColumn, GridRow } from '../lib/api-client';
import clsx from 'clsx';

export interface EditableGridProps {
  endpoint: string;
  columns: GridColumn[];
  initialData?: GridRow[];
  onDataChange?: (data: GridRow[]) => void;
  height?: string;
  enableVirtualization?: boolean;
}

export const EditableGrid: React.FC<EditableGridProps> = ({
  endpoint,
  columns,
  initialData = [],
  onDataChange,
  height = '600px',
  enableVirtualization = true,
}) => {
  const [data, setData] = useState<GridRow[]>(initialData);
  const [editingCell, setEditingCell] = useState<{ rowId: string | number; columnId: string } | null>(null);
  const [cellStatuses, setCellStatuses] = useState<Record<string, QueuedChange['status']>>({});
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

  // Load initial data
  useEffect(() => {
    const loadData = async () => {
      try {
        const gridData = await apiClient.getData(endpoint);
        setData(gridData.rows || []);
        if (onDataChange) {
          onDataChange(gridData.rows || []);
        }
      } catch (error) {
        console.error('Failed to load grid data:', error);
      }
    };

    if (initialData.length === 0) {
      loadData();
    }
  }, [endpoint]);

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

  return (
    <div className="editable-grid w-full" style={{ height }}>
      <div className="grid-container overflow-auto border border-gray-200 rounded-lg" style={{ height }}>
        <table className="w-full border-collapse" style={{ minWidth: '100%' }}>
          {/* Header */}
          <thead className="sticky top-0 z-10 bg-gray-50">
            <tr>
              {columns.map(column => (
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
            {data.map((row) => (
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

        {data.length === 0 && (
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

