/**
 * Authentication Redux Slice
 * Neo BPMS Admin Panel Core
 */

import { createSlice, createAsyncThunk, PayloadAction } from '@reduxjs/toolkit';
import { jwtDecode } from 'jwt-decode';
import type {
  AuthState,
  User,
  LoginRequest,
  LoginResponse,
  TokenPayload,
} from '../../types';
import { STORAGE_KEYS, EVENTS } from '../../config/defaults';

/**
 * Initial authentication state
 */
const initialState: AuthState = {
  user: null,
  accessToken: null,
  refreshToken: null,
  expiresAt: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
  lastActivity: null,
};

/**
 * Helper to load persisted auth state
 */
const loadPersistedState = (): Partial<AuthState> => {
  try {
    const accessToken = localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
    const refreshToken = localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
    const userJson = localStorage.getItem(STORAGE_KEYS.USER);

    if (!accessToken || !userJson) {
      return {};
    }

    // Check if token is expired
    const decoded = jwtDecode<TokenPayload>(accessToken);
    const expiresAt = decoded.exp * 1000;

    if (Date.now() >= expiresAt) {
      // Token expired, clear storage
      localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
      localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
      localStorage.removeItem(STORAGE_KEYS.USER);
      return {};
    }

    const user = JSON.parse(userJson) as User;

    return {
      user,
      accessToken,
      refreshToken,
      expiresAt,
      isAuthenticated: true,
      lastActivity: Date.now(),
    };
  } catch {
    return {};
  }
};

/**
 * Helper to persist auth state
 */
const persistState = (state: AuthState) => {
  if (state.accessToken && state.user) {
    localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, state.accessToken);
    if (state.refreshToken) {
      localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, state.refreshToken);
    }
    localStorage.setItem(STORAGE_KEYS.USER, JSON.stringify(state.user));
  } else {
    localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
    localStorage.removeItem(STORAGE_KEYS.USER);
  }
};

/**
 * Helper to dispatch custom events
 */
const dispatchAuthEvent = (eventName: string, detail?: unknown) => {
  window.dispatchEvent(new CustomEvent(eventName, { detail }));
};

/**
 * Async thunk: Login
 */
export const login = createAsyncThunk<
  LoginResponse,
  { credentials: LoginRequest; apiClient: { post: (url: string, data: unknown) => Promise<{ data: LoginResponse }> } },
  { rejectValue: string }
>('auth/login', async ({ credentials, apiClient }, { rejectWithValue }) => {
  try {
    const response = await apiClient.post('/api/auth/login', credentials);
    return response.data;
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Login failed';
    return rejectWithValue(message);
  }
});

/**
 * Async thunk: Logout
 */
export const logout = createAsyncThunk<
  void,
  { apiClient?: { post: (url: string) => Promise<void> } } | undefined,
  { rejectValue: string }
>('auth/logout', async (params) => {
  try {
    if (params?.apiClient) {
      await params.apiClient.post('/api/auth/logout');
    }
  } catch (error) {
    // Log but don't fail logout
    console.error('Logout API error:', error);
  }
});

/**
 * Async thunk: Refresh Token
 */
export const refreshAccessToken = createAsyncThunk<
  { accessToken: string; refreshToken: string; expiresIn: number },
  { apiClient: { post: (url: string, data: unknown) => Promise<{ data: { accessToken: string; refreshToken: string; expiresIn: number } }> } },
  { rejectValue: string }
>('auth/refreshToken', async ({ apiClient }, { getState, rejectWithValue }) => {
  try {
    const state = getState() as { auth: AuthState };
    const refreshToken = state.auth.refreshToken;

    if (!refreshToken) {
      return rejectWithValue('No refresh token available');
    }

    const response = await apiClient.post('/api/auth/refresh', { refreshToken });
    return response.data;
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Token refresh failed';
    return rejectWithValue(message);
  }
});

/**
 * Async thunk: Fetch current user
 */
export const fetchCurrentUser = createAsyncThunk<
  User,
  { apiClient: { get: (url: string) => Promise<{ data: User }> } },
  { rejectValue: string }
>('auth/fetchUser', async ({ apiClient }, { rejectWithValue }) => {
  try {
    const response = await apiClient.get('/api/auth/me');
    return response.data;
  } catch (error) {
    const message = error instanceof Error ? error.message : 'Failed to fetch user';
    return rejectWithValue(message);
  }
});

/**
 * Auth slice
 */
const authSlice = createSlice({
  name: 'auth',
  initialState: { ...initialState, ...loadPersistedState() },
  reducers: {
    /**
     * Set user manually
     */
    setUser: (state, action: PayloadAction<User | null>) => {
      state.user = action.payload;
      state.isAuthenticated = !!action.payload;
      persistState(state);
      dispatchAuthEvent(EVENTS.AUTH_STATE_CHANGED, { user: action.payload });
    },

    /**
     * Update user partially
     */
    updateUser: (state, action: PayloadAction<Partial<User>>) => {
      if (state.user) {
        state.user = { ...state.user, ...action.payload };
        persistState(state);
      }
    },

    /**
     * Set tokens
     */
    setTokens: (
      state,
      action: PayloadAction<{
        accessToken: string;
        refreshToken?: string;
        expiresIn: number;
      }>
    ) => {
      const { accessToken, refreshToken, expiresIn } = action.payload;
      state.accessToken = accessToken;
      if (refreshToken) {
        state.refreshToken = refreshToken;
      }
      state.expiresAt = Date.now() + expiresIn * 1000;
      persistState(state);
      dispatchAuthEvent(EVENTS.TOKEN_REFRESHED);
    },

    /**
     * Update last activity timestamp
     */
    updateLastActivity: (state) => {
      state.lastActivity = Date.now();
    },

    /**
     * Clear auth state (for logout)
     */
    clearAuth: (state) => {
      state.user = null;
      state.accessToken = null;
      state.refreshToken = null;
      state.expiresAt = null;
      state.isAuthenticated = false;
      state.error = null;
      state.lastActivity = null;
      persistState(state);
      dispatchAuthEvent(EVENTS.AUTH_STATE_CHANGED, { user: null });
    },

    /**
     * Set error
     */
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },

    /**
     * Clear error
     */
    clearError: (state) => {
      state.error = null;
    },

    /**
     * Handle session expiry
     */
    sessionExpired: (state) => {
      state.user = null;
      state.accessToken = null;
      state.refreshToken = null;
      state.expiresAt = null;
      state.isAuthenticated = false;
      state.error = 'Session expired. Please login again.';
      persistState(state);
      dispatchAuthEvent(EVENTS.SESSION_EXPIRED);
    },
  },
  extraReducers: (builder) => {
    // Login
    builder
      .addCase(login.pending, (state) => {
        state.isLoading = true;
        state.error = null;
      })
      .addCase(login.fulfilled, (state, action) => {
        const { user, accessToken, refreshToken, expiresIn } = action.payload;
        state.user = user;
        state.accessToken = accessToken;
        state.refreshToken = refreshToken;
        state.expiresAt = Date.now() + expiresIn * 1000;
        state.isAuthenticated = true;
        state.isLoading = false;
        state.error = null;
        state.lastActivity = Date.now();
        persistState(state);
        dispatchAuthEvent(EVENTS.AUTH_STATE_CHANGED, { user });
      })
      .addCase(login.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload || 'Login failed';
        state.isAuthenticated = false;
      });

    // Logout
    builder
      .addCase(logout.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(logout.fulfilled, (state) => {
        state.user = null;
        state.accessToken = null;
        state.refreshToken = null;
        state.expiresAt = null;
        state.isAuthenticated = false;
        state.isLoading = false;
        state.error = null;
        state.lastActivity = null;
        persistState(state);
        dispatchAuthEvent(EVENTS.AUTH_STATE_CHANGED, { user: null });
      })
      .addCase(logout.rejected, (state) => {
        // Even if API fails, clear local state
        state.user = null;
        state.accessToken = null;
        state.refreshToken = null;
        state.expiresAt = null;
        state.isAuthenticated = false;
        state.isLoading = false;
        persistState(state);
      });

    // Refresh token
    builder
      .addCase(refreshAccessToken.fulfilled, (state, action) => {
        const { accessToken, refreshToken, expiresIn } = action.payload;
        state.accessToken = accessToken;
        state.refreshToken = refreshToken;
        state.expiresAt = Date.now() + expiresIn * 1000;
        persistState(state);
        dispatchAuthEvent(EVENTS.TOKEN_REFRESHED);
      })
      .addCase(refreshAccessToken.rejected, (state, action) => {
        // Token refresh failed, session expired
        state.user = null;
        state.accessToken = null;
        state.refreshToken = null;
        state.expiresAt = null;
        state.isAuthenticated = false;
        state.error = action.payload || 'Session expired';
        persistState(state);
        dispatchAuthEvent(EVENTS.SESSION_EXPIRED);
      });

    // Fetch user
    builder
      .addCase(fetchCurrentUser.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(fetchCurrentUser.fulfilled, (state, action) => {
        state.user = action.payload;
        state.isLoading = false;
        persistState(state);
      })
      .addCase(fetchCurrentUser.rejected, (state, action) => {
        state.isLoading = false;
        state.error = action.payload || 'Failed to fetch user';
      });
  },
});

// Export actions
export const {
  setUser,
  updateUser,
  setTokens,
  updateLastActivity,
  clearAuth,
  setError,
  clearError,
  sessionExpired,
} = authSlice.actions;

// Export selectors
export const selectUser = (state: { auth: AuthState }) => state.auth.user;
export const selectIsAuthenticated = (state: { auth: AuthState }) => state.auth.isAuthenticated;
export const selectIsLoading = (state: { auth: AuthState }) => state.auth.isLoading;
export const selectAuthError = (state: { auth: AuthState }) => state.auth.error;
export const selectAccessToken = (state: { auth: AuthState }) => state.auth.accessToken;
export const selectExpiresAt = (state: { auth: AuthState }) => state.auth.expiresAt;

/**
 * Check if user has specific permission
 */
export const selectHasPermission = (permission: string) => (state: { auth: AuthState }) => {
  const user = state.auth.user;
  if (!user) return false;
  if (user.isAdmin) return true;
  return user.permissions.some((p) => p.id === permission || p.name === permission);
};

/**
 * Check if user has specific role
 */
export const selectHasRole = (role: string) => (state: { auth: AuthState }) => {
  const user = state.auth.user;
  if (!user) return false;
  if (user.isAdmin) return true;
  return user.roles.includes(role);
};

export default authSlice.reducer;

