'use client';

import React, { useState, useEffect, useRef } from 'react';
import DatePicker from 'react-datepicker';
import Select from 'react-select';
import { format, parse } from 'date-fns';
import { faIR } from 'date-fns-jalali/locale';
import 'react-datepicker/dist/react-datepicker.css';

export interface CellEditorProps {
  value: any;
  column: {
    id: string;
    type: 'text' | 'number' | 'date' | 'boolean' | 'select' | 'multiselect';
    options?: { value: any; label: string }[];
    format?: string;
    required?: boolean;
  };
  onChange: (value: any) => void;
  onBlur: () => void;
  onKeyDown?: (e: React.KeyboardEvent) => void;
}

export const CellEditor: React.FC<CellEditorProps> = ({
  value,
  column,
  onChange,
  onBlur,
  onKeyDown,
}) => {
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    // Focus input when editor opens
    if (inputRef.current) {
      inputRef.current.focus();
      if (column.type === 'text') {
        inputRef.current.select();
      }
    }
  }, []);

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && column.type !== 'multiselect') {
      e.preventDefault();
      onBlur();
    } else if (e.key === 'Escape') {
      e.preventDefault();
      onBlur();
    } else if (onKeyDown) {
      onKeyDown(e);
    }
  };

  // Text input
  if (column.type === 'text') {
    return (
      <input
        ref={inputRef}
        type="text"
        value={value || ''}
        onChange={(e) => onChange(e.target.value)}
        onBlur={onBlur}
        onKeyDown={handleKeyDown}
        className="w-full h-full px-2 border-0 outline-none bg-transparent"
        required={column.required}
      />
    );
  }

  // Number input
  if (column.type === 'number') {
    return (
      <input
        ref={inputRef}
        type="number"
        value={value ?? ''}
        onChange={(e) => {
          const numValue = e.target.value === '' ? null : parseFloat(e.target.value);
          onChange(isNaN(numValue!) ? null : numValue);
        }}
        onBlur={onBlur}
        onKeyDown={handleKeyDown}
        className="w-full h-full px-2 border-0 outline-none bg-transparent text-right"
        required={column.required}
      />
    );
  }

  // Boolean (checkbox)
  if (column.type === 'boolean') {
    return (
      <div className="w-full h-full flex items-center justify-center">
        <input
          type="checkbox"
          checked={value === true}
          onChange={(e) => onChange(e.target.checked)}
          onBlur={onBlur}
          className="w-4 h-4 cursor-pointer"
        />
      </div>
    );
  }

  // Date picker
  if (column.type === 'date') {
    const dateValue = value ? new Date(value) : null;
    return (
      <DatePicker
        selected={dateValue}
        onChange={(date: Date | null) => {
          onChange(date ? date.toISOString() : null);
          onBlur();
        }}
        dateFormat={column.format || 'yyyy/MM/dd'}
        locale={faIR}
        className="w-full h-full px-2 border-0 outline-none"
        calendarClassName="rtl"
        wrapperClassName="w-full h-full"
        onKeyDown={handleKeyDown}
      />
    );
  }

  // Select (single)
  if (column.type === 'select') {
    const options = column.options || [];
    const selectedOption = options.find(opt => opt.value === value);
    
    return (
      <Select
        value={selectedOption || null}
        onChange={(option) => {
          onChange(option ? option.value : null);
          onBlur();
        }}
        options={options}
        className="w-full"
        classNamePrefix="cell-select"
        menuPortalTarget={document.body}
        styles={{
          control: (base) => ({
            ...base,
            border: 'none',
            boxShadow: 'none',
            minHeight: '32px',
            height: '32px',
          }),
          menuPortal: (base) => ({
            ...base,
            zIndex: 9999,
          }),
        }}
        onKeyDown={handleKeyDown}
      />
    );
  }

  // Multiselect
  if (column.type === 'multiselect') {
    const options = column.options || [];
    const selectedValues = Array.isArray(value) ? value : [];
    const selectedOptions = options.filter(opt => selectedValues.includes(opt.value));
    
    return (
      <Select
        isMulti
        value={selectedOptions}
        onChange={(options) => {
          onChange(options ? options.map(opt => opt.value) : []);
        }}
        options={options}
        className="w-full"
        classNamePrefix="cell-select"
        menuPortalTarget={document.body}
        styles={{
          control: (base) => ({
            ...base,
            border: 'none',
            boxShadow: 'none',
            minHeight: '32px',
          }),
          menuPortal: (base) => ({
            ...base,
            zIndex: 9999,
          }),
        }}
        onBlur={onBlur}
      />
    );
  }

  return null;
};

