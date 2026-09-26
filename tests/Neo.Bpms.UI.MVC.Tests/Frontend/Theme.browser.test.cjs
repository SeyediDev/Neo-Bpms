const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { chromium } = require('playwright');
const mvc = path.resolve(__dirname, '../../../src/Neo.Bpms.UI.MVC');
const script = fs.readFileSync(path.join(mvc, 'wwwroot/js/neo-theme.js'), 'utf8');
const css = fs.readFileSync(path.join(mvc, 'wwwroot/css/neo-theme.css'), 'utf8');
const adapter = fs.readFileSync(path.join(mvc, 'wwwroot/css/neo-theme-mvc.css'), 'utf8');
const modern = fs.readFileSync(path.join(mvc, 'CommonAssets/Styles/Features/Modern/controls-modern.css'), 'utf8');
const vendors = process.env.NEO_TEST_VENDOR_ROOT || path.join(mvc, 'CommonAssets');
const bootstrap = fs.readFileSync(path.join(vendors, 'LibmanLibs/bootstrap/css/bootstrap.min.css'), 'utf8');
const palette = '--neo-dashboard-bg-secondary:#0f1419;--neo-dashboard-bg-card:#1a2234;--neo-form-background:#1a2234;' +
    '--neo-form-input-background:#101825;--neo-form-input-text:#f1f5f9;--neo-text-primary:#f1f5f9;' +
    '--neo-text-secondary:#cbd5e1;--neo-form-input-border:#526176;--neo-menu-primary:#a3e635;' +
    '--neo-form-button-text:#ffffff;--neo-menu-primary-hover:#bef264;';
let browser;
before(async () => {
    browser = await chromium.launch({ headless: true, ...(process.env.NEO_TEST_BROWSER ? { channel: process.env.NEO_TEST_BROWSER } : {}) });
});
after(async () => { if (browser) await browser.close(); });
function html({ host = '', init = '', frame = false, direction = 'rtl' } = {}) {
    return '<!doctype html><html dir="' + direction + '"><head><meta charset="utf-8"><style>' + bootstrap + '\n' + modern + '\n' + css + '\n' + adapter +
        '\nbody{padding:20px}.body-content{max-width:900px;margin:auto}.neo-control{padding:8px}.sample-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:12px}' +
        '</style><style>:root{' + host + '}</style><script>' + init + '\n' + script + '</script></head><body><main class="body-content">' +
        '<div class="neo-ux-toolbar"><h1>فرم سفارش / Order</h1></div><section class="card"><div id="filterDiv">' +
        '<div class="sample-grid"><div class="neo-control"><label for="title">عنوان</label><input class="form-control" id="title" value="مقدار حفظ شود"></div>' +
        '<div class="neo-control"><label for="status">وضعیت</label><select class="form-control" id="status"><option value="open">باز</option><option value="closed">بسته</option></select></div></div>' +
        '<button type="button" class="btn btn-primary" id="action">اعمال فیلتر</button></div></section>' +
        '<div id="popup" class="modern-multi-select-dropdown" style="display:block;position:relative">گزینهٔ اول / First option</div>' +
        '<table class="table table-striped"><thead><tr><th>نام</th><th>وضعیت</th></tr></thead><tbody><tr><td>نمونه</td><td>آماده</td></tr></tbody></table>' +
        (frame ? '<iframe title="Embedded grid" src="/child"></iframe>' : '') + '</main></body></html>';
}
async function fixture(t, options = {}) {
    const page = await browser.newPage({ viewport: options.viewport || { width: 1200, height: 800 }, colorScheme: options.os || 'light' });
    const errors = [];
    page.on('pageerror', e => errors.push(e.message));
    t.after(async () => { await page.close(); assert.deepEqual(errors, [], 'no runtime errors'); });
    await page.route('http://neo-theme.test/**', route => route.fulfill({ contentType: 'text/html',
        body: html(new URL(route.request().url()).pathname === '/child' ? { direction: 'ltr' } : options) }));
    await page.goto('http://neo-theme.test/');
    await page.waitForFunction(() => window.NeoTheme && window.NeoTheme.snapshot());
    return page;
}
async function value(page, selector, property) {
    return page.locator(selector).evaluate((el, name) => getComputedStyle(el).getPropertyValue(name), property);
}
test('SSR import exports a self-contained bootstrap without a document', () => {
    const module = require(path.join(mvc, 'wwwroot/js/neo-theme.js'));
    assert.equal(typeof module.installTheme, 'function');
    assert.ok(module.themeScript.includes('(window, document)'));
});
test('explicit light preference wins over a dark operating system', async t => {
    const page = await fixture(t, { os: 'dark', init: 'localStorage.setItem("neo_theme", JSON.stringify({mode:"light"}));' });
    assert.equal(await page.locator('html').getAttribute('data-neo-mode'), 'light');
    assert.equal(await value(page, '#title', 'background-color'), 'rgb(255, 255, 255)');
});
test('system preference responds to operating-system changes', async t => {
    const page = await fixture(t, { init: 'localStorage.setItem("neo_theme", JSON.stringify({mode:"system"}));' });
    await page.emulateMedia({ colorScheme: 'dark' });
    await page.waitForFunction(() => window.NeoTheme.snapshot().mode === 'dark');
    await page.emulateMedia({ colorScheme: 'light' });
    await page.waitForFunction(() => window.NeoTheme.snapshot().mode === 'light');
});
test('malformed and unavailable storage fall back without breaking controls', async t => {
    for (const init of ['localStorage.setItem("neo_theme", "{bad");',
        'Object.defineProperty(window,"localStorage",{get(){throw new Error("disabled")}});']) {
        const page = await fixture(t, { init });
        assert.equal(await page.locator('html').getAttribute('data-theme'), 'light');
        await page.locator('#title').fill('still editable');
        assert.equal(await page.locator('#title').inputValue(), 'still editable');
    }
});
test('server palette wins over stored preferences and repairs unreadable button text', async t => {
    const page = await fixture(t, { host: palette, init: 'localStorage.setItem("neo_theme", JSON.stringify({mode:"light",primaryColor:"#ff0000"}));' });
    assert.equal(await page.locator('html').getAttribute('data-theme'), 'dark');
    assert.equal(await value(page, '#title', 'background-color'), 'rgb(16, 24, 37)');
    assert.equal(await value(page, '#action', 'background-color'), 'rgb(163, 230, 53)');
    assert.equal(await value(page, '#action', 'color'), 'rgb(0, 0, 0)');
});
test('live host updates preserve form value, selection and open popup', async t => {
    const page = await fixture(t, { host: palette });
    await page.locator('#title').fill('draft');
    await page.locator('#status').selectOption('closed');
    await page.evaluate(() => {
        document.documentElement.style.setProperty('--neo-form-input-background', '#203040');
        document.documentElement.style.setProperty('--neo-menu-primary', '#8b5cf6');
    });
    await page.waitForFunction(() => getComputedStyle(document.querySelector('#title')).backgroundColor === 'rgb(32, 48, 64)');
    assert.equal(await page.locator('#title').inputValue(), 'draft');
    assert.equal(await page.locator('#status').inputValue(), 'closed');
    assert.equal(await page.locator('#popup').isVisible(), true);
    // Bootstrap animates button colors; assert the settled state, not an intermediate frame.
    await page.waitForFunction(() => getComputedStyle(document.querySelector('#action')).backgroundColor === 'rgb(139, 92, 246)');
});
test('same-origin iframe inherits palette and subsequent changes', async t => {
    const page = await fixture(t, { host: palette, frame: true });
    const child = page.frames().find(f => f.url().endsWith('/child'));
    assert.ok(child);
    await child.waitForFunction(() => window.NeoTheme.snapshot().inherited);
    assert.equal(await child.locator('html').getAttribute('dir'), 'rtl');
    assert.equal(await child.locator('html').getAttribute('data-theme'), 'dark');
    await page.evaluate(() => document.documentElement.style.setProperty('--neo-menu-primary', '#7c3aed'));
    await child.waitForFunction(() => getComputedStyle(document.querySelector('#action')).backgroundColor === 'rgb(124, 58, 237)');
});
test('late server style blocks are observed without polling', async t => {
    const page = await fixture(t);
    await page.evaluate(palette => {
        const style = document.createElement('style');
        style.textContent = ':root{' + palette + '}';
        document.head.appendChild(style);
    }, palette);
    await page.waitForFunction(() => window.NeoTheme.snapshot().mode === 'dark');
    assert.equal(await value(page, '#title', 'background-color'), 'rgb(16, 24, 37)');
});
test('root observation settles and emits only meaningful theme changes', async t => {
    const page = await fixture(t);
    await page.evaluate(() => {
        window.events = 0; window.addEventListener('neo:theme-changed', () => window.events++);
        window.NeoTheme.set({ mode: 'dark', primaryColor: '#7c3aed' });
    });
    await page.evaluate(() => new Promise(resolve => {
        let frames = 0;
        function tick() { if (++frames === 8) resolve(); else requestAnimationFrame(tick); }
        requestAnimationFrame(tick);
    }));
    assert.equal(await page.evaluate(() => window.events), 1);
});
test('real report combo keeps its themed surface after the source reapplies inline styles', async t => {
    const page = await fixture(t, { host: palette });
    await page.evaluate(() => {
        const panel = document.createElement('div');
        panel.id = 'filterTooltipPanel';
        panel.innerHTML = '<div class="neo-control" data-id="City"><select name="City" multiple><option value="1" selected>Tehran</option><option value="2">Shiraz</option></select></div>';
        document.body.appendChild(panel);
    });
    await page.addScriptTag({ path: path.join(vendors, 'LibmanLibs/jquery/jquery.min.js') });
    const source = fs.readFileSync(path.join(mvc, 'Views/Report/Partials/_Scripts.report-modern-multi-select.cshtml'), 'utf8');
    await page.addScriptTag({ content: source.slice(source.indexOf('<script>') + 8, source.indexOf('</script>')) });
    await page.evaluate(() => window.initializeReportModernMultiSelect());
    await page.locator('.modern-multi-select-input').click();
    await page.waitForFunction(() => document.querySelector('.modern-multi-select-control').classList.contains('open'));
    assert.equal(await value(page, '#filterTooltipPanel .modern-multi-select-dropdown', 'background-color'), 'rgb(26, 34, 52)');
    assert.deepEqual(await page.locator('select[name="City[]"]').evaluate(el => [...el.selectedOptions].map(o => o.value)), ['1']);
});
for (const mode of ['light', 'dark']) {
    test(mode + ': responsive RTL/LTR surfaces and focus', async t => {
        const page = await fixture(t, { viewport: { width: mode === 'dark' ? 390 : 1440, height: 900 }, direction: mode === 'dark' ? 'rtl' : 'ltr' });
        await page.evaluate(mode => window.NeoTheme.set({ mode }), mode);
        await page.locator('#title').focus();
        assert.equal(await page.locator('#title').evaluate(el => document.activeElement === el), true);
        assert.equal(await page.evaluate(() => document.documentElement.scrollWidth <= innerWidth), true);
        assert.notEqual(await value(page, '#title', 'color'), await value(page, '#title', 'background-color'));
        if (process.env.NEO_THEME_SCREENSHOTS) {
            fs.mkdirSync(process.env.NEO_THEME_SCREENSHOTS, { recursive: true });
            await page.screenshot({ path: path.join(process.env.NEO_THEME_SCREENSHOTS, mode + '.png'), fullPage: true });
        }
    });
}
