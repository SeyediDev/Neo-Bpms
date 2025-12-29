/**
 * Hooks Module Index
 * Neo BPMS Admin Panel Core
 */

// API hooks
export {
  useApiQuery,
  usePaginatedQuery,
  useApiMutation,
  useCrudApi,
  usePrefetch,
  useInfiniteApi,
} from './useApi';

// Re-export auth hooks
export {
  useAuth,
  useIsAuthenticated,
  useCurrentUser,
  useAuthLoading,
  useAuthError,
  usePermission,
  useRole,
  useRequireAuth,
} from '../auth/useAuth';

// Re-export state hooks
export {
  useAppDispatch,
  useAppSelector,
} from '../state/store';

