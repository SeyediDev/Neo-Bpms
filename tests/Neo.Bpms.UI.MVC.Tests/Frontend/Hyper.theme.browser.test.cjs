const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const { chromium } = require('playwright');
const origin = new URL(process.env.NEO_HYPER_URL || 'http://localhost:5000');
assert.ok(['localhost', '127.0.0.1', '[::1]'].includes(origin.hostname), 'Use the local Development host.');
let browser;
before(async () => { browser = await chromium.launch({headless:true,channel:process.env.NEO_TEST_BROWSER || 'msedge'}); });
after(async () => { if (browser) await browser.close(); });
async function preview(t, width = 1440) {
  const page = await browser.newPage({viewport:{width,height:1000}});
  page.setDefaultTimeout(60000);
  const errors=[];
  page.on('pageerror', e=>errors.push(e.message));
  t.after(async()=>{await page.close();assert.deepEqual(errors,[]);});
  const response=await page.goto(new URL('/AdminDashboard/Preview',origin).href,{waitUntil:'domcontentloaded'});
  assert.equal(response.status(),200);
  assert.ok((await page.locator('.demo-notice').innerText()).includes('نمونه'), 'Only the explicit demo preview is tested.');
  await page.waitForFunction(()=>!!window.NeoTheme?.snapshot());
  return page;
}
test('Hyper preview uses shared light/dark surfaces and preserves chart series colors',async t=>{
  const page=await preview(t);
  const colors=[];
  const series=await page.locator('.chart-line:not(.secondary)').evaluate(el=>getComputedStyle(el).stroke);
  for(const mode of ['light','dark']) {
    await page.evaluate(mode=>window.NeoTheme.set({mode}),mode);
    await page.waitForFunction(mode=>document.documentElement.dataset.neoMode===mode,mode);
    const styles=await page.locator('.metric-card').first().evaluate(el=>({background:getComputedStyle(el).backgroundColor,text:getComputedStyle(el).color}));
    assert.notEqual(styles.background,styles.text);
    colors.push(styles.background);
    for (const selector of ['td', 'td strong', '.queue-health-label', '.queue-health-item strong']) {
      const ratio = await page.locator(selector).first().evaluate(el => {
        const luminance = css => {
          const rgb = css.match(/[\d.]+/g).slice(0, 3).map(Number).map(x => {
            const channel = x / 255;
            return channel <= .04045 ? channel / 12.92 : ((channel + .055) / 1.055) ** 2.4;
          });
          return rgb[0] * .2126 + rgb[1] * .7152 + rgb[2] * .0722;
        };
        const text = luminance(getComputedStyle(el).color);
        const background = luminance(getComputedStyle(el.closest('.dashboard-card')).backgroundColor);
        return (Math.max(text, background) + .05) / (Math.min(text, background) + .05);
      });
      assert.ok(ratio >= 4.5, `${mode} ${selector} contrast: ${ratio.toFixed(2)}`);
    }
    assert.equal(await page.locator('.chart-line:not(.secondary)').evaluate(el=>getComputedStyle(el).stroke),series);
    assert.equal(await page.locator('.chart-legend i').first().evaluate(el=>getComputedStyle(el).backgroundColor),series);
  }
  assert.notEqual(colors[0],colors[1]);
  await page.screenshot({path:require('node:path').join(process.env.NEO_THEME_SCREENSHOTS || require('node:os').tmpdir(),'hyper-dashboard-dark.png'),fullPage:true});
});
test('Hyper mobile menu remains usable with keyboard and without page overflow',async t=>{
  const page=await preview(t,390);
  await page.evaluate(()=>window.NeoTheme.set({mode:'dark'}));
  assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),true);
  const table=page.locator('.recent-card .table-scroll');
  assert.equal(await table.evaluate(el=>el.scrollWidth>el.clientWidth),true, 'Wide table stays independently scrollable.');
  const toggle=page.locator('.mobile-toggle');
  await toggle.click();
  assert.equal(await toggle.getAttribute('aria-expanded'),'true');
  assert.equal(await page.locator('#admin-navigation').evaluate(el=>el.inert),false);
  await page.keyboard.press('Escape');
  assert.equal(await toggle.getAttribute('aria-expanded'),'false');
  assert.equal(await page.locator('#admin-navigation').evaluate(el=>el.inert),true);
  assert.equal(await toggle.evaluate(el=>document.activeElement===el),true);
  await page.screenshot({path:require('node:path').join(process.env.NEO_THEME_SCREENSHOTS || require('node:os').tmpdir(),'hyper-dashboard-mobile.png'),fullPage:true});
});
test('Hyper preview period links still render the selected sample data',async t=>{
  const page=await preview(t);
  await page.getByRole('link',{name:'۳۰ روز اخیر',exact:true}).click();
  await page.waitForURL(url=>url.searchParams.get('days')==='30');
  assert.equal(await page.locator('.chart-data tbody tr').count(),30);
  assert.ok((await page.locator('.demo-notice').innerText()).includes('نمونه'));
});
