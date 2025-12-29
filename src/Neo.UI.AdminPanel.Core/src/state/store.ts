/**
 * Redux Store Configuration
 * Neo BPMS Admin Panel Core
 */

import { configureStore, combineReducers, Middleware } from '@reduxjs/toolkit';
import { TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';
import authReducer from './slices/authSlice';
import uiReducer from './slices/uiSlice';

/**
 * Root reducer combining all slices
 */
const rootReducer = combineReducers({
  auth: authReducer,
  ui: uiReducer,
});

/**
 * Logger middleware for development
 */
const loggerMiddleware: Middleware = (store) => (next) => (action: unknown) => {
  if (process.env.NODE_ENV === 'development') {
    const actionObj = action as { type?: string };
    console.group(actionObj.type || 'unknown');
    console.log('Previous State:', store.getState());
    console.log('Action:', action);
    const result = next(action);
    console.log('Next State:', store.getState());
    console.groupEnd();
    return result;
  }
  return next(action);
};

/**
 * Error handling middleware
 */
const errorMiddleware: Middleware = () => (next) => (action) => {
  try {
    return next(action);
  } catch (error) {
    console.error('Redux error:', error);
    throw error;
  }
};

/**
 * Create store with configuration
 */
export const createAppStore = (preloadedState?: Partial<RootState>) => {
  return configureStore({
    reducer: rootReducer,
    preloadedState,
    middleware: (getDefaultMiddleware) => {
      const middleware = getDefaultMiddleware({
        serializableCheck: {
          // Ignore these paths in the state for serialization check
          ignoredPaths: ['auth.user.metadata'],
          ignoredActions: ['auth/setUser'],
        },
        thunk: {
          extraArgument: undefined,
        },
      });
      
      if (process.env.NODE_ENV === 'development') {
        return middleware.concat(loggerMiddleware, errorMiddleware);
      }
      return middleware.concat(errorMiddleware);
    },
    devTools: process.env.NODE_ENV === 'development',
  });
};

/**
 * Default store instance
 */
export const store = createAppStore();

/**
 * Type definitions
 */
export type RootState = ReturnType<typeof rootReducer>;
export type AppStore = ReturnType<typeof createAppStore>;
export type AppDispatch = AppStore['dispatch'];

/**
 * Typed hooks
 */
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

/**
 * Helper to get state outside of React components
 */
export const getState = () => store.getState();
export const dispatch = store.dispatch;

export default store;

