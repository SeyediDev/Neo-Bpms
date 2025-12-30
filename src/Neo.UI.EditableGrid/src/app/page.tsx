'use client';

import { EditableGrid } from '../components/EditableGrid';
import { GridColumn, GridApiClient, RowActionsConfig } from '../lib/api-client';
import { useState, useEffect } from 'react';

export default function EditableGridPage() {
  const [columns, setColumns] = useState<GridColumn[]>([]);
  const [rowActions, setRowActions] = useState<RowActionsConfig | undefined>(undefined);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [apiClient] = useState(() => new GridApiClient());

  // Get endpoint and other params from query params
  const getQueryParams = () => {
    if (typeof window === 'undefined') {
      return { endpoint: 'sample', namespaceId: undefined, entityId: undefined, formSubjectId: undefined };
    }
    const params = new URLSearchParams(window.location.search);
    return {
      endpoint: params.get('endpoint') || 'sample',
      namespaceId: params.get('namespaceId') || undefined,
      entityId: params.get('entityId') || undefined,
      formSubjectId: params.get('formSubjectId') || undefined,
    };
  };

  const { endpoint, namespaceId, entityId, formSubjectId } = getQueryParams();

  // Load columns from API config
  useEffect(() => {
    const loadConfig = async () => {
      setLoading(true);
      setError(null);
      try {
        const config = await apiClient.getConfig(endpoint);
        if (config && config.columns) {
          setColumns(config.columns);
          if (config.rowActions) {
            setRowActions(config.rowActions);
          }
        } else {
          // Fallback to sample columns if config not available
          setColumns([
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
          ]);
        }
      } catch (err: any) {
        console.error('Failed to load grid config:', err);
        setError(err.message || 'خطا در بارگذاری تنظیمات جدول');
        // Fallback to sample columns on error
        setColumns([
          { id: 'id', name: 'شناسه', type: 'number', editable: false, width: 100 },
          { id: 'name', name: 'نام', type: 'text', editable: true, required: true, width: 200 },
        ]);
      } finally {
        setLoading(false);
      }
    };

    loadConfig();
  }, [endpoint, apiClient]);

  if (loading) {
    return (
      <div className="p-6">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
          <p className="mt-4 text-gray-600">در حال بارگذاری...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-6">
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <p className="text-red-800">{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6">
      <h1 className="text-2xl font-bold mb-4 text-right">جدول قابل ویرایش</h1>
      {columns.length > 0 ? (
        <EditableGrid
          endpoint={endpoint}
          columns={columns}
          height="600px"
          rowActions={rowActions}
          namespaceId={namespaceId}
          entityId={entityId}
          formSubjectId={formSubjectId}
        />
      ) : (
        <div className="text-center text-gray-500">
          <p>هیچ ستونی برای نمایش وجود ندارد</p>
        </div>
      )}
    </div>
  );
}

