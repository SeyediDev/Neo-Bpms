const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const path = require('node:path');
const fs = require('node:fs');
const { chromium } = require('playwright');
const mvc = path.resolve(__dirname, '../../../src/Neo.Bpms.UI.MVC');
const assets = path.join(mvc, 'CommonAssets');
let browser;
let browserServer;
before(async () => {
    browserServer = await chromium.launchServer({
        headless: true,
        ...(process.env.NEO_TEST_BROWSER ? { channel: process.env.NEO_TEST_BROWSER } : {})
    });
    browser = await chromium.connect(browserServer.wsEndpoint());
});
after(async () => {
    // Stop only the browser process owned by this suite (including on assertion failure).
    if (browserServer) await browserServer.kill();
});
const field = (name, extra = '') => '<div class="neo-control" data-id="' + name + '"><label>' + name +
    '</label><input id="field-' + name + '" name="' + name + '" ' + extra + '></div>';
const header = name => '<th><button type="button" class="neo-column-filter" data-filter-column="' + name + '">Filter</button></th>';
async function fixture(t, { root = 'filterDiv', content = field('City'), column = 'City', setup, nested = false } = {}) {
    const page = await browser.newPage();
    t.after(() => page.close());
    const errors = [];
    page.on('pageerror', error => errors.push(error.message));
    t.after(() => assert.deepEqual(errors, [], 'no browser runtime errors'));
    await page.setContent('<html dir="rtl"><style>' +
        '.neo-filter-advanced{display:none} .neo-filter-expanded .neo-filter-advanced{display:block}' +
        '.tab-pane:not(.active){display:none} .modern-multi-select-dropdown{display:none}' +
        '.modern-multi-select.open .modern-multi-select-dropdown{display:block}' +
        '</style><form id="query"><button id="showFilter" type="button">Filters</button>' +
        '<button data-action="toggle-filter-tooltip" type="button">Report filters</button>' +
        '<table><thead><tr>' + header(column) + '</tr></thead></table><div id="' + root + '" style="' +
        (root === 'DashboardFilter' ? '' : 'display:none') + '">' +
        (nested ? '<section>' : '') + '<div class="modern-filter-content">' + content + '</div>' +
        (nested ? '</section>' : '') + '</div></form></html>');
    await page.addScriptTag({ path: path.join(assets, 'LibmanLibs/jquery/jquery.min.js') });
    await page.evaluate(() => {
        window.submits = 0; window.sorts = 0; window.panelToggles = 0;
        document.querySelector('form').addEventListener('submit', e => { e.preventDefault(); window.submits++; });
        document.querySelector('th').addEventListener('click', () => window.sorts++);
        document.querySelector('#showFilter').addEventListener('click', () => {
            window.panelToggles++;
            const panel = document.querySelector('#filterDiv');
            if (panel) panel.style.display = getComputedStyle(panel).display === 'none' ? 'block' : 'none';
        });
    });
    if (root === 'filterTooltipPanel') {
        // Execute the real report toggle, extracted between its source-level function boundaries.
        const source = fs.readFileSync(path.join(mvc, 'Views/Report/Partials/_Scripts.toggles.cshtml'), 'utf8');
        const start = source.indexOf('window.toggleFilterTooltip = function');
        const end = source.indexOf('\n};', start) + 3;
        assert.ok(start >= 0 && end > start);
        await page.addScriptTag({ content: source.slice(start, end) });
        // Real outside-click listener, independent of script load order.
        const events = fs.readFileSync(path.join(mvc, 'Views/Report/Partials/_Scripts.event-manager.cshtml'), 'utf8');
        const eventStart = events.indexOf("document.addEventListener('click', function(e)");
        const eventEnd = events.indexOf('\n    });', eventStart) + 8;
        await page.addScriptTag({ content: events.slice(eventStart, eventEnd) });
    }
    if (setup) await setup(page);
    await page.addScriptTag({ path: path.join(assets, 'Scripts/Features/Filter/ColumnFilter.js') });
    return page;
}
async function clickAndFocus(page, name = 'City') {
    await page.locator('.neo-column-filter').click();
    await page.waitForFunction(name => document.activeElement.id === 'field-' + name, name, { timeout: 1800 });
}
for (const root of ['filterDiv', 'filterTooltipPanel', 'DashboardFilter']) {
    test(root + ': header reaches the existing field without submitting or sorting', async t => {
        const page = await fixture(t, { root });
        await clickAndFocus(page);
        assert.deepEqual(await page.evaluate(() => [window.submits, window.sorts]), [0, 0]);
    });
}
test('legacy panel stays open on repeated header activation', async t => {
    const page = await fixture(t);
    await clickAndFocus(page);
    await clickAndFocus(page);
    assert.equal(await page.evaluate(() => window.panelToggles), 1);
});
test('ninth control is revealed before receiving focus', async t => {
    const page = await fixture(t, {
        content: Array.from({ length: 10 }, (_, i) => field('Field' + i)).join(''), column: 'Field8'
    });
    await clickAndFocus(page, 'Field8');
    assert.equal(await page.locator('#field-Field8').isVisible(), true);
    assert.equal(await page.locator('.neo-filter-more-toggle').getAttribute('aria-expanded'), 'true');
});
test('inactive generated tab is activated', async t => {
    const page = await fixture(t, {
        content: '<ul class="nav nav-tabs" role="tablist"><li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#first">First</a></li>' +
            '<li class="nav-item"><a class="nav-link" data-toggle="tab" href="#second">Second</a></li></ul>' +
            '<div class="tab-content"><div class="tab-pane active" id="first">' + field('Other') +
            '</div><div class="tab-pane" id="second">' + field('City') + '</div></div>',
        setup: async page => page.addScriptTag({ path: path.join(assets, 'LibmanLibs/bootstrap/js/bootstrap.bundle.min.js') })
    });
    await clickAndFocus(page);
    assert.equal(await page.locator('#second').isVisible(), true);
    assert.equal(await page.locator('#first').isVisible(), false);
});
test('refresh does not duplicate nested toolbars', async t => {
    const page = await fixture(t, { nested: true });
    await page.evaluate(() => { for (let i = 0; i < 10; i++) window.NeoFilterUx.refresh(); });
    assert.equal(await page.locator('.neo-filter-toolbar').count(), 1);
});
test('array names, keyboard activation and active state reuse original values', async t => {
    const page = await fixture(t, { content: field('City').replace('name="City"', 'name="City[]"'), column: 'city' });
    await page.locator('.neo-column-filter').focus();
    await page.keyboard.press('Enter');
    await page.waitForFunction(() => document.activeElement.id === 'field-City', null, { timeout: 1800 });
    await page.locator('#field-City').fill('Tehran');
    await page.waitForFunction(() => document.querySelector('.neo-column-filter').getAttribute('aria-pressed') === 'true');
    assert.deepEqual(await page.evaluate(() => Array.from(new FormData(document.querySelector('form')).entries())), [['City[]', 'Tehran']]);
    await page.locator('.neo-filter-chip-clear').click();
    assert.equal(await page.locator('#field-City').inputValue(), '');
});
test('unavailable field is disabled, then enabled when inserted dynamically', async t => {
    const page = await fixture(t, { content: field('Other') });
    assert.equal(await page.locator('.neo-column-filter').isDisabled(), true);
    await page.locator('.modern-filter-content').evaluate((el, markup) => el.insertAdjacentHTML('beforeend', markup), field('City'));
    await page.waitForFunction(() => !document.querySelector('.neo-column-filter').disabled);
    await clickAndFocus(page);
});
test('real multi-select opens on repeated activation without changing selection', async t => {
    const page = await fixture(t, {
        content: '<div class="neo-control" data-id="City"><label>City</label><div id="choices"></div></div>',
        setup: async page => {
            await page.addScriptTag({ path: path.join(assets, 'Scripts/Features/Modern/modern-multi-select.js') });
            await page.evaluate(() => {
                window.choices = new ModernMultiSelect(document.querySelector('#choices'), { data: [{ value: '1', text: 'Tehran' }] });
            });
        }
    });
    for (let i = 0; i < 2; i++) {
        await page.locator('.neo-column-filter').click();
        await page.waitForFunction(() => window.choices.isOpen, null, { timeout: 1800 });
        assert.deepEqual(await page.evaluate(() => window.choices.selectedItems), []);
    }
});
test('real Persian datepicker opens without reinitializing or changing query values', async t => {
    const page = await fixture(t, {
        content: '<div class="neo-control" data-id="City"><input id="field-City" class="dateField" associated-hidden-name="City">' +
            '<input type="hidden" name="City" value=""></div>',
        setup: async page => {
            const dir = path.join(assets, 'Scripts/Common Plugins/Global/Pwt Date Picker');
            await page.addScriptTag({ path: path.join(dir, 'persian-date.min.js') });
            await page.addScriptTag({ path: path.join(dir, 'persian-datepicker.min.js') });
            await page.evaluate(() => {
                window.calendar = window.jQuery('#field-City').pDatepicker({ initialValue: false, altField: '[name="City"]' });
            });
        }
    });
    await clickAndFocus(page);
    assert.equal(await page.locator('.datepicker-container').count(), 1);
    await page.waitForFunction(() => Array.from(document.querySelectorAll('.datepicker-container')).some(el => getComputedStyle(el).display !== 'none'));
    assert.equal(await page.locator('[name="City"]').inputValue(), '');
});


test('report-specific multi-select remains open and preserves its backing select', async t => {
    const page = await fixture(t, {
        root: 'filterTooltipPanel',
        content: '<div class="neo-control" data-id="City"><label>City</label><select name="City" multiple><option value="1" selected>Tehran</option><option value="2">Shiraz</option></select></div>',
        setup: async page => {
            const source = fs.readFileSync(path.join(mvc, 'Views/Report/Partials/_Scripts.report-modern-multi-select.cshtml'), 'utf8');
            await page.addScriptTag({ content: source.slice(source.indexOf('<script>') + 8, source.indexOf('</script>')) });
            await page.evaluate(() => window.initializeReportModernMultiSelect());
            await page.waitForSelector('.modern-multi-select-control', { state: 'attached', timeout: 3000 });
        }
    });
    for (let i = 0; i < 2; i++) {
        await page.locator('.neo-column-filter').focus();
        await page.keyboard.press('Space');
        await page.waitForFunction(() => document.querySelector('.modern-multi-select-control').classList.contains('open'), null, { timeout: 1800 });
        assert.deepEqual(await page.locator('select[name="City[]"]').evaluate(el => Array.from(el.selectedOptions, o => o.value)), ['1']);
    }
});
test('report stays open regardless of outside-click listener registration order', async t => {
    const page = await fixture(t, { root: 'filterTooltipPanel' });
    const events = fs.readFileSync(path.join(mvc, 'Views/Report/Partials/_Scripts.event-manager.cshtml'), 'utf8');
    const start = events.indexOf("document.addEventListener('click', function(e)");
    await page.addScriptTag({ content: events.slice(start, events.indexOf('\n    });', start) + 8) });
    await clickAndFocus(page);
    // The real report overlay can cover the header in this small fixture.
    await page.locator('.neo-column-filter').focus();
    await page.keyboard.press('Space');
    await page.waitForFunction(() => document.activeElement.id === 'field-City');
    assert.equal(await page.locator('#filterTooltipPanel').evaluate(el => el.classList.contains('show')), true);
});


test('actual form toggle preserves form association and stays open', async t => {
    const page = await fixture(t, {
        setup: async page => {
            const source = fs.readFileSync(path.join(mvc, 'Views/Form/Index.cshtml'), 'utf8');
            const start = source.indexOf('function toggleFilter()');
            const end = source.indexOf('function collectFilterValues()', start);
            await page.evaluate(() => {
                document.querySelector('form').id = 'filter-form';
                window.FilterShow = false;
            });
            await page.addScriptTag({ content: source.slice(start, end) + '\nwindow.toggleFilter = toggleFilter;' });
        }
    });
    await clickAndFocus(page);
    await clickAndFocus(page);
    await page.waitForFunction(() => document.querySelector('#field-City').getAttribute('form') === 'filter-form');
    assert.equal(await page.locator('#filterDiv').isVisible(), true);
    assert.deepEqual(await page.evaluate(() => [window.submits, window.sorts]), [0, 0]);
});

