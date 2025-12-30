'use client';

import { EditableGrid } from '../components/EditableGrid';
import { GridColumn } from '../lib/api-client';

export default function EditableGridPage() {
  // Example columns configuration
  const columns: GridColumn[] = [
    { id: 'id', name: 'شناسه', type: 'number', editable: false, width: 100 },
    { id: 'name', name: 'نام', type: 'text', editable: true, required: true, width: 200 },
    { id: 'email', name: 'ایمیل', type: 'text', editable: true, width: 250 },
    { id: 'age', name: 'سن', type: 'number', editable: true, width: 100 },
    { id: 'status', name: 'وضعیت', type: 'select', editable: true, width: 150, options: [
      { value: 'active', label: 'فعال' },
      { value: 'inactive', label: 'غیرفعال' },
      { value: 'pending', label: 'در انتظار' },
    ]},
    { id: 'createdAt', name: 'تاریخ ایجاد', type: 'date', editable: true, width: 150 },
    { id: 'isActive', name: 'فعال', type: 'boolean', editable: true, width: 80 },
  ];

  // Get endpoint from query params or use default
  const endpoint = typeof window !== 'undefined' 
    ? new URLSearchParams(window.location.search).get('endpoint') || '/api/grid/sample'
    : '/api/grid/sample';

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4 text-right">جدول قابل ویرایش</h1>
      <EditableGrid
        endpoint={endpoint}
        columns={columns}
        height="600px"
      />
    </div>
  );
}

