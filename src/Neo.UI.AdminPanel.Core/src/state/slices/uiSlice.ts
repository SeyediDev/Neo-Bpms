/**
 * UI Redux Slice
 * Neo BPMS Admin Panel Core
 */

import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { ThemeConfig, Notification } from '../../types';
import { STORAGE_KEYS } from '../../config/defaults';
import { defaultTheme, loadTheme, applyTheme, normalizeTheme } from '../../theme';

/**
 * UI State interface
 */
export interface UiState {
  theme: ThemeConfig;
  language: string;
  sidebarCollapsed: boolean;
  sidebarMobileOpen: boolean;
  notifications: Notification[];
  isLoading: boolean;
  loadingMessage: string | null;
  breadcrumbs: Array<{ label: string; path?: string }>;
  pageTitle: string | null;
}

/**
 * Load persisted UI state
 */
const loadPersistedUiState = (): Partial<UiState> => {
  try {
    const language = localStorage.getItem(STORAGE_KEYS.LANGUAGE);
    const sidebarCollapsed = localStorage.getItem(STORAGE_KEYS.SIDEBAR_COLLAPSED);

    return {
      theme: loadTheme(),
      language: language || 'fa',
      sidebarCollapsed: sidebarCollapsed === 'true',
    };
  } catch {
    return {};
  }
};

/**
 * Initial UI state
 */
const persistedState = loadPersistedUiState();
const initialState: UiState = {
  theme: defaultTheme,
  language: 'fa',
  sidebarCollapsed: false,
  sidebarMobileOpen: false,
  notifications: [],
  isLoading: false,
  loadingMessage: null,
  breadcrumbs: [],
  pageTitle: null,
  ...persistedState,
};

applyTheme(initialState.theme);

/**
 * Generate unique notification ID
 */
let notificationIdCounter = 0;
const generateNotificationId = () => `notification_${++notificationIdCounter}_${Date.now()}`;

/**
 * UI Slice
 */
const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    /**
     * Set theme
     */
    setTheme: (state, action: PayloadAction<Partial<ThemeConfig>>) => {
      state.theme = normalizeTheme({ ...state.theme, ...action.payload });
      applyTheme(state.theme, true);
    },

    /**
     * Toggle theme mode
     */
    toggleThemeMode: (state) => {
      const resolvedMode = typeof window !== 'undefined' ? window.NeoTheme?.snapshot()?.mode : state.theme.mode;
      state.theme.mode = resolvedMode === 'dark' ? 'light' : 'dark';
      applyTheme(state.theme, true);
    },

    /**
     * Set language
     */
    setLanguage: (state, action: PayloadAction<string>) => {
      state.language = action.payload;
      try { localStorage.setItem(STORAGE_KEYS.LANGUAGE, action.payload); } catch { /* Storage may be disabled. */ }

      // Update direction based on language
      const isRtl = ['fa', 'ar', 'he'].includes(action.payload);
      state.theme.direction = isRtl ? 'rtl' : 'ltr';
      applyTheme(state.theme, true);
      if (typeof document !== 'undefined') document.documentElement.setAttribute('lang', action.payload);
    },

    /**
     * Toggle sidebar collapsed state
     */
    toggleSidebar: (state) => {
      state.sidebarCollapsed = !state.sidebarCollapsed;
      localStorage.setItem(STORAGE_KEYS.SIDEBAR_COLLAPSED, String(state.sidebarCollapsed));
    },

    /**
     * Set sidebar collapsed state
     */
    setSidebarCollapsed: (state, action: PayloadAction<boolean>) => {
      state.sidebarCollapsed = action.payload;
      localStorage.setItem(STORAGE_KEYS.SIDEBAR_COLLAPSED, String(action.payload));
    },

    /**
     * Toggle mobile sidebar
     */
    toggleMobileSidebar: (state) => {
      state.sidebarMobileOpen = !state.sidebarMobileOpen;
    },

    /**
     * Set mobile sidebar state
     */
    setMobileSidebarOpen: (state, action: PayloadAction<boolean>) => {
      state.sidebarMobileOpen = action.payload;
    },

    /**
     * Add notification
     */
    addNotification: (
      state,
      action: PayloadAction<Omit<Notification, 'id'>>
    ) => {
      const notification: Notification = {
        ...action.payload,
        id: generateNotificationId(),
      };
      state.notifications.push(notification);

      // Auto-remove after duration (if specified)
      if (notification.duration && notification.duration > 0) {
        setTimeout(() => {
          // Dispatch remove action - handled by component
        }, notification.duration);
      }
    },

    /**
     * Remove notification
     */
    removeNotification: (state, action: PayloadAction<string>) => {
      state.notifications = state.notifications.filter(
        (n) => n.id !== action.payload
      );
    },

    /**
     * Clear all notifications
     */
    clearNotifications: (state) => {
      state.notifications = [];
    },

    /**
     * Show loading overlay
     */
    showLoading: (state, action: PayloadAction<string | undefined>) => {
      state.isLoading = true;
      state.loadingMessage = action.payload || null;
    },

    /**
     * Hide loading overlay
     */
    hideLoading: (state) => {
      state.isLoading = false;
      state.loadingMessage = null;
    },

    /**
     * Set breadcrumbs
     */
    setBreadcrumbs: (
      state,
      action: PayloadAction<Array<{ label: string; path?: string }>>
    ) => {
      state.breadcrumbs = action.payload;
    },

    /**
     * Set page title
     */
    setPageTitle: (state, action: PayloadAction<string | null>) => {
      state.pageTitle = action.payload;
      if (action.payload) {
        document.title = `${action.payload} | Neo BPMS`;
      }
    },
  },
});

// Export actions
export const {
  setTheme,
  toggleThemeMode,
  setLanguage,
  toggleSidebar,
  setSidebarCollapsed,
  toggleMobileSidebar,
  setMobileSidebarOpen,
  addNotification,
  removeNotification,
  clearNotifications,
  showLoading,
  hideLoading,
  setBreadcrumbs,
  setPageTitle,
} = uiSlice.actions;

// Export selectors
export const selectTheme = (state: { ui: UiState }) => state.ui.theme;
export const selectLanguage = (state: { ui: UiState }) => state.ui.language;
export const selectSidebarCollapsed = (state: { ui: UiState }) => state.ui.sidebarCollapsed;
export const selectSidebarMobileOpen = (state: { ui: UiState }) => state.ui.sidebarMobileOpen;
export const selectNotifications = (state: { ui: UiState }) => state.ui.notifications;
export const selectIsLoading = (state: { ui: UiState }) => state.ui.isLoading;
export const selectLoadingMessage = (state: { ui: UiState }) => state.ui.loadingMessage;
export const selectBreadcrumbs = (state: { ui: UiState }) => state.ui.breadcrumbs;
export const selectPageTitle = (state: { ui: UiState }) => state.ui.pageTitle;

export default uiSlice.reducer;

