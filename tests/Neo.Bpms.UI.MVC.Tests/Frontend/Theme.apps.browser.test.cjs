const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const http = require('node:http');
const { chromium } = require('playwright');
const mvc = path.resolve(__dirname, '../../../src/Neo.Bpms.UI.MVC');
const theme = fs.readFileSync(path.join(mvc, 'wwwroot/js/neo-theme.js'), 'utf8');
const css = fs.readFileSync(path.join(mvc, 'wwwroot/css/neo-theme.css'), 'utf8');
const roots = {
  monitoring: process.env.NEO_MONITORING_DIST,
  'editable-grid': process.env.NEO_GRID_DIST,
};
let server, browser, origin;
before(async () => {
  for (const [app, root] of Object.entries(roots)) {
    assert.ok(root && fs.existsSync(path.join(root, 'index.html')), 'Build ' + app + ' and set its NEO_*_DIST directory.');
    roots[app] = path.resolve(root);
  }
  server = http.createServer((req, res) => {
    const url = new URL(req.url, 'http://localhost');
    if (url.pathname === '/host') {
      res.setHeader('Content-Type', 'text/html; charset=utf-8');
      res.end('<!doctype html><html dir="rtl"><head><style>' + css + ':root{--neo-dashboard-bg-secondary:#152238;--neo-dashboard-bg-card:#243247;--neo-text-primary:#f1f5f9;--neo-menu-primary:#a3e635;}</style><script>' + theme + '</script></head><body><iframe title="app" style="width:100%;height:900px;border:0" src="/' + url.searchParams.get('app') + '/"></iframe></body></html>');
      return;
    }
    const [, app, ...parts] = url.pathname.split('/');
    const root = roots[app];
    const file = root && path.resolve(root, parts.join('/') || 'index.html');
    if (!root || !file.startsWith(root + path.sep) || !fs.existsSync(file) || !fs.statSync(file).isFile()) {
      res.writeHead(404); res.end(); return;
    }
    const types = { '.html': 'text/html; charset=utf-8', '.js': 'application/javascript', '.css': 'text/css', '.json': 'application/json', '.woff2': 'font/woff2' };
    res.setHeader('Content-Type', types[path.extname(file)] || 'application/octet-stream');
    fs.createReadStream(file).pipe(res);
  });
  await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
  origin = 'http://127.0.0.1:' + server.address().port;
  browser = await chromium.launch({ headless: true, ...(process.env.NEO_TEST_BROWSER ? { channel: process.env.NEO_TEST_BROWSER } : {}) });
});
after(async () => {
  if (browser) await browser.close();
  if (server) await new Promise(resolve => server.close(resolve));
});
for (const app of Object.keys(roots)) {
  test(app + ': production assets hydrate and inherit live host colors without reloading', async t => {
    const context = await browser.newContext({ viewport: { width: 1440, height: 1000 } });
    t.after(() => context.close());
    // Exercise the disconnected UI without contacting any real API or account.
    let apiSeen;
    const requestedApi = new Promise(resolve => { apiSeen = resolve; });
    await context.route('**/api/**', async route => {
      apiSeen();
      await route.fulfill({ status: 503, contentType: 'application/json', body: '{"message":"UI validation: API unavailable"}' });
    });
    await context.route('**/hubs/**', route => route.fulfill({ status: 503, body: '' }));
    const page = await context.newPage();
    page.setDefaultTimeout(60000);
    const errors = [], assets = [];
    page.on('pageerror', e => errors.push(e.message));
    page.on('response', response => {
      if (response.url().includes('/_next/') && !response.ok()) assets.push(response.url());
    });
    await page.goto(origin + '/host?app=' + app);
    const child = page.frames().find(frame => frame.url().includes('/' + app + '/'));
    assert.ok(child, 'embedded application loaded');
    await child.waitForFunction(() => window.NeoTheme?.snapshot()?.inherited);
    await Promise.race([requestedApi, new Promise((_, reject) => {
      const timer = setTimeout(() => reject(new Error('Client application did not request its API')), 60000);
      timer.unref();
    })]);
    assert.equal(await child.locator('html').getAttribute('data-neo-mode'), 'dark');
    assert.equal(await child.locator('body').evaluate(el => getComputedStyle(el).backgroundColor), 'rgb(21, 34, 56)');
    await child.evaluate(() => { window.themeProbe = 1; });
    await page.evaluate(() => document.documentElement.style.setProperty('--neo-dashboard-bg-secondary', '#fafaf9'));
    await child.waitForFunction(() => window.NeoTheme.snapshot().mode === 'light');
    assert.equal(await child.evaluate(() => window.themeProbe), 1, 'frame was not reloaded');
    assert.equal(await child.locator('body').evaluate(el => getComputedStyle(el).backgroundColor), 'rgb(250, 250, 249)');
    assert.deepEqual(assets, [], 'all built assets were served');
    assert.deepEqual(errors, [], 'no hydration or runtime errors');
  });
}
