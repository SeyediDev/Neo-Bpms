/* Shared MVC / React theme bridge. Host palette wins; no API requests or form mutations. */
(function () {
    'use strict';
    function installTheme(win, doc) {
        'use strict';
        if (win.NeoTheme) return win.NeoTheme;
        var root = doc.documentElement;
        var names = {
            canvas: '--neo-dashboard-bg-secondary', surface: '--neo-dashboard-bg-card',
            elevated: '--neo-form-background', input: '--neo-form-input-background',
            text: '--neo-text-primary', muted: '--neo-text-secondary', subtle: '--neo-text-muted',
            border: '--neo-form-input-border', hover: '--neo-menu-bg-hover',
            accent: '--neo-menu-primary', 'accent-hover': '--neo-menu-primary-hover',
            'on-accent': '--neo-form-button-text', success: '--neo-dashboard-success',
            warning: '--neo-dashboard-warning', danger: '--neo-dashboard-danger',
            'chart-text': '--neo-chart-text', 'chart-grid': '--neo-chart-grid-line',
            'chart-surface': '--neo-chart-background', 'report-surface': '--neo-report-background',
            'report-header': '--neo-report-header-background', 'report-alternate': '--neo-report-table-row-alternate',
            'header-surface': '--neo-header-background', 'header-text': '--neo-header-text',
            'input-text': '--neo-form-input-text'
        };
        var preference = readPreference();
        var snapshot = null;
        var frame = 0;
        var parentRoot = null;
        var inherited = false;
        var media = win.matchMedia('(prefers-color-scheme: dark)');
        try {
            if (win.parent !== win && win.parent.location.origin === win.location.origin) {
                parentRoot = win.parent.document.documentElement;
            }
        } catch (_) { /* Cross-origin documents must explicitly integrate their own theme. */ }
        var initialMode = root.getAttribute('data-theme');
        if (['light', 'dark', 'system'].indexOf(initialMode) >= 0) preference.mode = initialMode;
        function readPreference() {
            try {
                var value = JSON.parse(win.localStorage.getItem('neo_theme') || '{}');
                return value && typeof value === 'object' && !Array.isArray(value) ? value : {};
            } catch (_) { return {}; }
        }
        function property(style, name) { return style.getPropertyValue(name).trim(); }
        function color(value) { return value && win.CSS.supports('color', value) ? value : ''; }
        function rgb(value) {
            if (/^#[0-9a-f]{3}$/i.test(value)) value = '#' + value.slice(1).split('').map(function (v) { return v + v; }).join('');
            if (/^#[0-9a-f]{6}$/i.test(value)) return [1, 3, 5].map(function (i) { return parseInt(value.slice(i, i + 2), 16); });
            var parts = /^rgba?\(\s*([\d.]+)[,\s]+([\d.]+)[,\s]+([\d.]+)/i.exec(value);
            return parts ? parts.slice(1, 4).map(Number) : null;
        }
        function luminance(value) {
            var components = rgb(value);
            if (!components) return null;
            var linear = components.map(function (v) { v /= 255; return v <= 0.04045 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4); });
            return linear[0] * 0.2126 + linear[1] * 0.7152 + linear[2] * 0.0722;
        }
        function setAttribute(name, value) {
            if (root.getAttribute(name) !== value) root.setAttribute(name, value);
        }
        function setProperty(name, value) {
            if (root.style.getPropertyValue(name) !== value) root.style.setProperty(name, value);
        }
        function apply() {
            frame = 0;
            var parentTheme = null;
            try { parentTheme = parentRoot && win.parent.NeoTheme && win.parent.NeoTheme.snapshot(); } catch (_) {}
            var source = parentRoot ? win.parent.getComputedStyle(parentRoot) : win.getComputedStyle(root);
            var background = color(property(source, '--neo-dashboard-bg-secondary')) ||
                color(property(source, '--neo-dashboard-bg-primary'));
            var host = !!background;
            var brightness = luminance(background);
            var mode = parentTheme ? parentTheme.mode : host && brightness !== null ?
                (brightness < 0.18 ? 'dark' : 'light') :
                preference.mode === 'dark' || (preference.mode === 'system' && media.matches) ? 'dark' : 'light';
            inherited = !!parentTheme || (parentRoot && host);
            setAttribute('data-neo-mode', mode);
            setAttribute('data-theme', mode);
            setAttribute('data-bs-theme', mode);
            root.classList.toggle('dark', mode === 'dark');
            setAttribute('data-neo-theme', 'true');
            var defaults = win.getComputedStyle(root);
            var tokens = {};
            Object.keys(names).forEach(function (name) {
                tokens[name] = parentTheme ? parentTheme.tokens[name] :
                    color(property(source, names[name])) || property(defaults, '--neo-base-' + name);
            });
            if (!host && !inherited && color(preference.primaryColor)) {
                tokens.accent = preference.primaryColor;
                tokens['accent-hover'] = 'color-mix(in srgb, ' + tokens.accent + ', black 15%)';
                var lightness = luminance(tokens.accent);
                tokens['on-accent'] = lightness !== null && lightness > 0.179 ? '#000000' : '#ffffff';
            }
            // Repair unreadable on-accent pairs without changing the host's accent.
            var accentL = luminance(tokens.accent);
            var textL = luminance(tokens['on-accent']);
            if (accentL !== null && (textL === null || (Math.max(accentL, textL) + 0.05) / (Math.min(accentL, textL) + 0.05) < 4.5)) {
                tokens['on-accent'] = accentL > 0.179 ? '#000000' : '#ffffff';
            }
            Object.keys(tokens).forEach(function (name) { if (tokens[name]) setProperty('--neo-' + name, tokens[name]); });
            if (parentTheme && parentTheme.appearance) {
                ['font', 'radius'].forEach(function (name) {
                    if (parentTheme.appearance[name]) setProperty('--neo-' + name, parentTheme.appearance[name]);
                });
            }
            var direction = parentRoot ? parentRoot.getAttribute('dir') : preference.direction;
            if (direction === 'rtl' || direction === 'ltr') setAttribute('dir', direction);
            if (!inherited && typeof preference.fontFamily === 'string' && /^[\w \-,]{1,80}$/.test(preference.fontFamily)) {
                setProperty('--neo-font', preference.fontFamily + ', system-ui, sans-serif');
            }
            var radii = { none: '0', sm: '.25rem', md: '.5rem', lg: '.75rem', full: '1.5rem' };
            if (!inherited && radii[preference.borderRadius]) setProperty('--neo-radius', radii[preference.borderRadius]);
            // Compatibility aliases are one-way: never overwrite the host's native palette.
            var aliases = {
                '--primary-color': 'accent', '--primary-hover': 'accent-hover',
                '--primary-light': 'selection', '--first-color': 'accent', '--second-color': 'muted',
                '--border-color': 'border', '--border-color-active': 'accent',
                '--navtab-text-color': 'muted', '--navtab-text-color-active': 'text',
                '--secondary-color': 'muted', '--secondary-light': 'hover',
                '--success-color': 'success', '--warning-color': 'warning', '--error-color': 'danger'
            };
            Object.keys(aliases).forEach(function (name) { setProperty(name, 'var(--neo-' + aliases[name] + ')'); });
            var next = { mode: mode, tokens: tokens, direction: root.getAttribute('dir') || 'ltr', inherited: !!inherited, host: host, appearance: { font: property(win.getComputedStyle(root), '--neo-font'), radius: property(win.getComputedStyle(root), '--neo-radius') } };
            if (JSON.stringify(next) !== JSON.stringify(snapshot)) {
                snapshot = next;
                win.dispatchEvent(new win.CustomEvent('neo:theme-changed', { detail: next }));
            }
        }
        function schedule() { if (!frame) frame = win.requestAnimationFrame(apply); }
        function set(value, persist) {
            if (!value || typeof value !== 'object') return;
            preference = Object.assign({}, preference, value);
            if (persist) {
                try { win.localStorage.setItem('neo_theme', JSON.stringify(preference)); } catch (_) {}
            }
            apply();
        }
        win.NeoTheme = { set: set, refresh: apply, snapshot: function () { return snapshot; } };
        var observer = new win.MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.attributeName === 'data-theme') {
                    var requested = root.getAttribute('data-theme');
                    if (['light', 'dark', 'system'].indexOf(requested) >= 0 && (!snapshot || requested !== snapshot.mode)) preference.mode = requested;
                }
            });
            schedule();
        });
        observer.observe(root, { attributes: true, attributeFilter: ['style', 'class', 'data-theme', 'dir'] });
        if (parentRoot) {
            var parentObserver = new win.MutationObserver(schedule);
            parentObserver.observe(parentRoot, { attributes: true, attributeFilter: ['style', 'class', 'data-theme', 'dir'] });
            win.parent.addEventListener('neo:theme-changed', schedule);
            win.addEventListener('pagehide', function () {
                parentObserver.disconnect();
                win.parent.removeEventListener('neo:theme-changed', schedule);
            });
        }
        win.addEventListener('storage', function (event) {
            if (event.key === 'neo_theme' || event.key === null) { preference = readPreference(); apply(); }
        });
        media.addEventListener('change', schedule);
        // Server-rendered host variables may appear after CommonIncludes in <head>.
        if (doc.head) new win.MutationObserver(schedule).observe(doc.head, { childList: true, subtree: true, characterData: true });
        doc.addEventListener('load', schedule, true);
        doc.addEventListener('DOMContentLoaded', apply, { once: true });
        apply();
        return win.NeoTheme;
    }
    if (typeof module === 'object' && module.exports) {
        module.exports = { installTheme: installTheme, themeScript: '(' + installTheme.toString() + ')(window, document);' };
    }
    if (typeof window !== 'undefined' && typeof document !== 'undefined') installTheme(window, document);
})();
