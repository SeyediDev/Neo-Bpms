export interface NeoThemeSnapshot {
    mode: 'light' | 'dark';
    tokens: Record<string, string>;
    direction: string;
    inherited: boolean;
    host: boolean;
    appearance: { font: string; radius: string };
}
export interface NeoThemeApi {
    set(value: Record<string, unknown>, persist?: boolean): void;
    refresh(): void;
    snapshot(): NeoThemeSnapshot | null;
}
export function installTheme(win: Window, doc: Document): NeoThemeApi;
export const themeScript: string;
declare global { interface Window { NeoTheme?: NeoThemeApi; } }
