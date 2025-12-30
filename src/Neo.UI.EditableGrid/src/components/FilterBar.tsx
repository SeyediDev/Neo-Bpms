'use client';

import React, { useState } from 'react';
import { FilterCondition } from './EditableGrid';
import { GridColumn } from '../lib/api-client';
import clsx from 'clsx';

export interface FilterBarProps {
  columns: GridColumn[];
  filters: FilterCondition[];
  onFiltersChange: (filters: FilterCondition[]) => void;
  onClose: () => void;
}

export const FilterBar: React.FC<FilterBarProps> = ({
  columns,
  filters,
  onFiltersChange,
  onClose,
}) => {
  const [localFilters, setLocalFilters] = useState<FilterCondition[]>(filters);

  const addFilter = () => {
    setLocalFilters([
      ...localFilters,
      { columnId: columns[0]?.id || '', operator: 'equals', value: '' },
    ]);
  };

  const removeFilter = (index: number) => {
    setLocalFilters(localFilters.filter((_, i) => i !== index));
  };

  const updateFilter = (index: number, updates: Partial<FilterCondition>) => {
    setLocalFilters(localFilters.map((f, i) => 
      i === index ? { ...f, ...updates } : f
    ));
  };

  const applyFilters = () => {
    onFiltersChange(localFilters.filter(f => f.value !== '' && f.value !== null));
  };

  const clearFilters = () => {
    setLocalFilters([]);
    onFiltersChange([]);
  };

  const operators = [
    { value: 'equals', label: 'برابر با' },
    { value: 'notEquals', label: 'مخالف' },
    { value: 'contains', label: 'شامل' },
    { value: 'startsWith', label: 'شروع با' },
    { value: 'endsWith', label: 'پایان با' },
    { value: 'greaterThan', label: 'بزرگتر از' },
    { value: 'lessThan', label: 'کوچکتر از' },
    { value: 'greaterThanOrEqual', label: 'بزرگتر یا مساوی' },
    { value: 'lessThanOrEqual', label: 'کوچکتر یا مساوی' },
    { value: 'in', label: 'در لیست' },
    { value: 'notIn', label: 'خارج از لیست' },
  ];

  return (
    <div className="filter-bar bg-white border-b border-gray-200 p-4 shadow-sm">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold">فیلترها</h3>
        <div className="flex gap-2">
          <button
            onClick={applyFilters}
            className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            اعمال فیلتر
          </button>
          <button
            onClick={clearFilters}
            className="px-4 py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
          >
            پاک کردن
          </button>
          <button
            onClick={onClose}
            className="px-4 py-2 text-gray-600 hover:text-gray-800"
          >
            ✕
          </button>
        </div>
      </div>

      <div className="space-y-3">
        {localFilters.map((filter, index) => (
          <div key={index} className="flex items-center gap-3 p-3 bg-gray-50 rounded">
            <select
              value={filter.columnId}
              onChange={(e) => updateFilter(index, { columnId: e.target.value })}
              className="px-3 py-2 border border-gray-300 rounded flex-1"
            >
              {columns.map(col => (
                <option key={col.id} value={col.id}>{col.name}</option>
              ))}
            </select>

            <select
              value={filter.operator}
              onChange={(e) => updateFilter(index, { operator: e.target.value as any })}
              className="px-3 py-2 border border-gray-300 rounded"
            >
              {operators.map(op => (
                <option key={op.value} value={op.value}>{op.label}</option>
              ))}
            </select>

            <input
              type="text"
              value={filter.value || ''}
              onChange={(e) => updateFilter(index, { value: e.target.value })}
              placeholder="مقدار فیلتر"
              className="px-3 py-2 border border-gray-300 rounded flex-1"
            />

            <button
              onClick={() => removeFilter(index)}
              className="px-3 py-2 text-red-600 hover:text-red-800"
            >
              حذف
            </button>
          </div>
        ))}

        <button
          onClick={addFilter}
          className="w-full px-4 py-2 border-2 border-dashed border-gray-300 rounded text-gray-600 hover:border-gray-400 hover:text-gray-800"
        >
          + افزودن فیلتر
        </button>
      </div>
    </div>
  );
};

