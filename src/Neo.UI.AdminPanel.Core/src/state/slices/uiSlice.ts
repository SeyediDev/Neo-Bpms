/**
 * UI Redux Slice
 * Neo BPMS Admin Panel Core
 */

import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { ThemeConfig, Notification } from '../../types';
import { STORAGE_KEYS } from '../../config/defaults';

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
 * Default theme configuration
 */
const defaultTheme: ThemeConfig = {
  mode: 'light',
  primaryColor: '#0ea5e9',
  borderRadius: 'md',
  fontFamily: 'Vazirmatn',
  direction: 'rtl',
};

/**
 * Load persisted UI state
 */
const loadPersistedUiState = (): Partial<UiState> => {
  try {
    const theme = localStorage.getItem(STORAGE_KEYS.THEME);
    const language = localStorage.getItem(STORAGE_KEYS.LANGUAGE);
    const sidebarCollapsed = localStorage.getItem(STORAGE_KEYS.SIDEBAR_COLLAPSED);

    return {
      theme: theme ? JSON.parse(theme) : defaultTheme,
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
  ...loadPersistedUiState(),
};

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
      state.theme = { ...state.theme, ...action.payload };
      localStorage.setItem(STORAGE_KEYS.THEME, JSON.stringify(state.theme));

      // Apply theme to document
      const root = document.documentElement;
      root.setAttribute('data-theme', state.theme.mode);
      root.setAttribute('dir', state.theme.direction);
      root.style.setProperty('--primary-color', state.theme.primaryColor);
    },

    /**
     * Toggle theme mode
     */
    toggleThemeMode: (state) => {
      state.theme.mode = state.theme.mode === 'light' ? 'dark' : 'light';
      localStorage.setItem(STORAGE_KEYS.THEME, JSON.stringify(state.theme));
      document.documentElement.setAttribute('data-theme', state.theme.mode);
    },

    /**
     * Set language
     */
    setLanguage: (state, action: PayloadAction<string>) => {
      state.language = action.payload;
      localStorage.setItem(STORAGE_KEYS.LANGUAGE, action.payload);

      // Update direction based on language
      const isRtl = ['fa', 'ar', 'he'].includes(action.payload);
      state.theme.direction = isRtl ? 'rtl' : 'ltr';
      document.documentElement.setAttribute('dir', state.theme.direction);
      document.documentElement.setAttribute('lang', action.payload);
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

