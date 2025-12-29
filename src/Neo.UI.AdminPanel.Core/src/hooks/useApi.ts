/**
 * API Hook
 * Neo BPMS Admin Panel Core
 * 
 * React Query wrapper for API calls with automatic caching and state management.
 */

import { 
  useQuery, 
  useMutation, 
  useQueryClient,
  UseQueryOptions,
  UseMutationOptions,
  QueryKey,
} from '@tanstack/react-query';
import { apiClient } from '../api/apiClient';
import type { ApiResponse, PaginatedResponse, QueryParams } from '../types';
import { useCallback } from 'react';

/**
 * Generic API fetch hook
 */
export function useApiQuery<T>(
  key: QueryKey,
  url: string,
  params?: Record<string, unknown>,
  options?: Omit<UseQueryOptions<ApiResponse<T>, Error>, 'queryKey' | 'queryFn'>
) {
  return useQuery<ApiResponse<T>, Error>({
    queryKey: key,
    queryFn: async () => {
      const response = await apiClient.get<T>(url, { params });
      return response;
    },
    ...options,
  });
}

/**
 * Paginated API fetch hook
 */
export function usePaginatedQuery<T>(
  key: QueryKey,
  url: string,
  queryParams?: QueryParams,
  options?: Omit<UseQueryOptions<PaginatedResponse<T>, Error>, 'queryKey' | 'queryFn'>
) {
  return useQuery<PaginatedResponse<T>, Error>({
    queryKey: [...key, queryParams],
    queryFn: async () => {
      const response = await apiClient.get<T[]>(url, { 
        params: queryParams as Record<string, unknown>
      }) as unknown as PaginatedResponse<T>;
      return response;
    },
    ...options,
  });
}

/**
 * API mutation hook (POST, PUT, PATCH, DELETE)
 */
export function useApiMutation<TData, TVariables = unknown>(
  url: string,
  method: 'post' | 'put' | 'patch' | 'delete' = 'post',
  options?: Omit<UseMutationOptions<ApiResponse<TData>, Error, TVariables>, 'mutationFn'>
) {
  return useMutation<ApiResponse<TData>, Error, TVariables>({
    mutationFn: async (variables) => {
      switch (method) {
        case 'post':
          return apiClient.post<TData>(url, variables);
        case 'put':
          return apiClient.put<TData>(url, variables);
        case 'patch':
          return apiClient.patch<TData>(url, variables);
        case 'delete':
          return apiClient.delete<TData>(url);
      }
    },
    ...options,
  });
}

/**
 * CRUD hooks factory
 */
export function useCrudApi<T extends { id: number | string }>(
  baseUrl: string,
  queryKey: string
) {
  const queryClient = useQueryClient();

  // List all
  const useList = (params?: QueryParams) => {
    return usePaginatedQuery<T>(
      [queryKey, 'list'],
      baseUrl,
      params
    );
  };

  // Get one by ID
  const useOne = (id: number | string, options?: Omit<UseQueryOptions<ApiResponse<T>, Error>, 'queryKey' | 'queryFn'>) => {
    return useApiQuery<T>(
      [queryKey, id],
      `${baseUrl}/${id}`,
      undefined,
      {
        enabled: !!id,
        ...options,
      }
    );
  };

  // Create
  const useCreate = (options?: UseMutationOptions<ApiResponse<T>, Error, Partial<T>>) => {
    return useMutation<ApiResponse<T>, Error, Partial<T>>({
      mutationFn: (data) => apiClient.post<T>(baseUrl, data),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: [queryKey] });
      },
      ...options,
    });
  };

  // Update
  const useUpdate = (options?: UseMutationOptions<ApiResponse<T>, Error, { id: number | string; data: Partial<T> }>) => {
    return useMutation<ApiResponse<T>, Error, { id: number | string; data: Partial<T> }>({
      mutationFn: ({ id, data }) => apiClient.put<T>(`${baseUrl}/${id}`, data),
      onSuccess: (_, variables) => {
        queryClient.invalidateQueries({ queryKey: [queryKey, variables.id] });
        queryClient.invalidateQueries({ queryKey: [queryKey, 'list'] });
      },
      ...options,
    });
  };

  // Delete
  const useDelete = (options?: UseMutationOptions<ApiResponse<void>, Error, number | string>) => {
    return useMutation<ApiResponse<void>, Error, number | string>({
      mutationFn: (id) => apiClient.delete<void>(`${baseUrl}/${id}`),
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: [queryKey] });
      },
      ...options,
    });
  };

  // Invalidate queries
  const invalidate = useCallback(() => {
    queryClient.invalidateQueries({ queryKey: [queryKey] });
  }, [queryClient]);

  return {
    useList,
    useOne,
    useCreate,
    useUpdate,
    useDelete,
    invalidate,
  };
}

/**
 * Hook to prefetch data
 */
export function usePrefetch() {
  const queryClient = useQueryClient();

  return useCallback(
    async <T>(key: QueryKey, url: string, params?: Record<string, unknown>) => {
      await queryClient.prefetchQuery({
        queryKey: key,
        queryFn: () => apiClient.get<T>(url, { params }),
        staleTime: 5 * 60 * 1000, // 5 minutes
      });
    },
    [queryClient]
  );
}

/**
 * Hook for infinite scroll/pagination
 */
export function useInfiniteApi<T>(
  key: QueryKey,
  url: string,
  pageSize = 20
) {
  return useQuery<PaginatedResponse<T>, Error>({
    queryKey: key,
    queryFn: async () => {
      const response = await apiClient.get<T[]>(url, { 
        params: { pageSize } 
      }) as unknown as PaginatedResponse<T>;
      return response;
    },
  });
}

export default useApiQuery;

