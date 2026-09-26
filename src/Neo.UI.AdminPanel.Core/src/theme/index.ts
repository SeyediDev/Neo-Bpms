import type { ThemeConfig } from '../types';
import '../../../Neo.Bpms.UI.MVC/wwwroot/js/neo-theme.js';

export const defaultTheme: ThemeConfig = {
  mode: 'light', primaryColor: '#0ea5e9', borderRadius: 'md',
  fontFamily: 'Vazirmatn', direction: 'rtl',
};
export function normalizeTheme(value: unknown): ThemeConfig {
  const input = value && typeof value === 'object' ? value as Partial<ThemeConfig> : {};
  return {
    mode: ['light', 'dark', 'system'].includes(input.mode || '') ? input.mode! : defaultTheme.mode,
    primaryColor: typeof input.primaryColor === 'string' && /^#(?:[a-f\d]{3}|[a-f\d]{6})$/i.test(input.primaryColor)
      ? input.primaryColor : defaultTheme.primaryColor,
    borderRadius: ['none', 'sm', 'md', 'lg', 'full'].includes(input.borderRadius || '') ? input.borderRadius! : defaultTheme.borderRadius,
    fontFamily: typeof input.fontFamily === 'string' && /^[\w \-,]{1,80}$/.test(input.fontFamily)
      ? input.fontFamily : defaultTheme.fontFamily,
    direction: input.direction === 'ltr' ? 'ltr' : 'rtl',
  };
}
export function loadTheme(): ThemeConfig {
  try { return normalizeTheme(JSON.parse(localStorage.getItem('neo_theme') || '{}')); }
  catch { return { ...defaultTheme }; }
}
export function applyTheme(theme: ThemeConfig, persist = false): void {
  if (typeof window !== 'undefined') window.NeoTheme?.set({ ...normalizeTheme(theme) }, persist);
}
