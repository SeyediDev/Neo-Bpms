# راهنمای تست‌های Jest برای فیلتر گزارشات

## نصب و راه‌اندازی

### 1. نصب Dependencies

```bash
cd tests/Neo.Bpms.UI.MVC.Tests/Frontend
npm install
```

این دستور پکیج‌های زیر را نصب می‌کند:
- `jest`: فریمورک تست
- `jest-environment-jsdom`: محیط DOM برای تست‌های مرورگر

### 2. ساختار فایل‌ها

```
Frontend/
├── package.json              # تنظیمات npm و Jest
├── jest.setup.js             # تنظیمات اولیه Jest
├── jest.jquery.mock.js        # Mock برای jQuery
├── Scripts/
│   └── filter-functions.js   # توابع JavaScript استخراج شده
└── FilterFrontendTests.test.js  # فایل تست
```

## اجرای تست‌ها

### اجرای تمام تست‌ها

```bash
npm test
```

### اجرای تست‌ها در حالت Watch (تغییرات خودکار)

```bash
npm run test:watch
```

### اجرای تست‌ها با Coverage Report

```bash
npm run test:coverage
```

## نوشتن تست جدید

### ساختار یک تست

```javascript
describe('نام گروه تست', () => {
    beforeEach(() => {
        // تنظیمات قبل از هر تست
        document.body.innerHTML = `
            <div id="test-container">
                <!-- HTML مورد نیاز برای تست -->
            </div>
        `;
    });

    afterEach(() => {
        // پاکسازی بعد از هر تست
        document.body.innerHTML = '';
        jest.clearAllMocks();
    });

    test('توضیح تست', () => {
        // Arrange: آماده‌سازی
        const element = document.getElementById('test-container');
        
        // Act: اجرای عمل
        // کد مورد تست
        
        // Assert: بررسی نتیجه
        expect(element).not.toBeNull();
    });
});
```

### Mock کردن jQuery

jQuery به صورت خودکار mock شده است. می‌توانید از `$` و `jQuery` استفاده کنید:

```javascript
const $element = $('#my-element');
$element.val('test value');
expect($element.val()).toBe('test value');
```

### Mock کردن توابع Window

```javascript
global.window.myFunction = jest.fn();
// یا
global.window.myFunction = function() {
    // implementation
};
```

### تست کردن DOM Manipulation

```javascript
test('should update DOM element', () => {
    const element = document.createElement('div');
    element.id = 'test';
    document.body.appendChild(element);
    
    // تغییر DOM
    element.innerHTML = '<span>Test</span>';
    
    // بررسی
    expect(element.innerHTML).toContain('Test');
});
```

## دستورات مفید Jest

### Matchers (بررسی‌ها)

```javascript
// برابری
expect(value).toBe(4);
expect(value).toEqual({a: 1});

// Truthiness
expect(value).toBeTruthy();
expect(value).toBeFalsy();
expect(value).toBeNull();
expect(value).toBeUndefined();

// اعداد
expect(value).toBeGreaterThan(3);
expect(value).toBeLessThan(5);
expect(value).toBeCloseTo(0.3);

// رشته‌ها
expect(str).toMatch(/pattern/);
expect(str).toContain('substring');

// آرایه‌ها
expect(array).toContain(item);
expect(array).toHaveLength(3);

// Objects
expect(obj).toHaveProperty('key');
expect(obj).toHaveProperty('key', 'value');

// Functions
expect(fn).toHaveBeenCalled();
expect(fn).toHaveBeenCalledTimes(2);
expect(fn).toHaveBeenCalledWith(arg1, arg2);
```

### Spy Functions

```javascript
// ایجاد spy
const spy = jest.fn();
spy('arg1', 'arg2');
expect(spy).toHaveBeenCalledWith('arg1', 'arg2');

// Spy روی تابع موجود
const originalFn = window.myFunction;
const spy = jest.spyOn(window, 'myFunction');
// بعد از تست
spy.mockRestore();
```

## مثال‌های عملی

### تست یک تابع ساده

```javascript
test('collectReportFilterValues should update form attribute', () => {
    // Arrange
    document.body.innerHTML = `
        <form id="filter-form"></form>
        <div id="filterTooltipPanel">
            <input name="field1" />
        </div>
    `;
    
    // Act
    collectReportFilterValues();
    
    // Assert
    const input = document.querySelector('input[name="field1"]');
    expect(input.getAttribute('form')).toBe('filter-form');
});
```

### تست یک Event Handler

```javascript
test('submitReportFilter should prevent multiple submissions', () => {
    // Arrange
    const submitBtn = document.createElement('button');
    submitBtn.id = 'submit-filter';
    submitBtn.classList.add('ladda-loading');
    document.body.appendChild(submitBtn);
    
    const form = document.createElement('form');
    form.id = 'filter-form';
    form.submit = jest.fn();
    document.body.appendChild(form);
    
    // Act
    window.submitReportFilter();
    
    // Assert
    expect(form.submit).not.toHaveBeenCalled();
});
```

## Debugging

### اجرای یک تست خاص

```bash
npm test -- -t "نام تست"
```

### نمایش خروجی Console

```javascript
test('debug test', () => {
    console.log('Debug message');
    // تست
});
```

### استفاده از Debugger

```javascript
test('debug test', () => {
    debugger; // توقف در این خط
    // تست
});
```

سپس اجرا کنید:
```bash
node --inspect-brk node_modules/.bin/jest --runInBand
```

## نکات مهم

1. **همیشه DOM را پاک کنید**: در `afterEach` همیشه `document.body.innerHTML = ''` را فراخوانی کنید
2. **Mock ها را پاک کنید**: از `jest.clearAllMocks()` استفاده کنید
3. **از async/await استفاده کنید**: برای تست‌های async
4. **تست‌ها را مستقل نگه دارید**: هر تست باید مستقل از تست‌های دیگر باشد

## منابع بیشتر

- [Jest Documentation](https://jestjs.io/docs/getting-started)
- [Jest DOM Testing](https://testing-library.com/docs/dom-testing-library/intro/)
- [Jest Matchers](https://jestjs.io/docs/expect)

