# PivotTable - کامپوننت Pivot Table سمت کلاینت

یک کامپوننت JavaScript خالص و سبک برای ساخت Pivot Table بدون نیاز به کتابخانه‌های خارجی.

## ویژگی‌ها

- ✅ بدون وابستگی به کتابخانه‌های خارجی
- ✅ سبک و سریع
- ✅ پشتیبانی از RTL (راست به چپ)
- ✅ قابلیت تغییر ابعاد به صورت داینامیک
- ✅ پشتیبانی از چندین aggregator (جمع، تعداد، میانگین، کمینه، بیشینه)
- ✅ پشتیبانی از چندین فیلد مقدار

## استفاده ساده

```html
<div id="pivot-container"></div>

<script src="pivot-table.js"></script>
<script>
    const data = [
        { Category: 'A', Region: 'North', Sales: 100, Count: 5 },
        { Category: 'A', Region: 'South', Sales: 200, Count: 10 },
        { Category: 'B', Region: 'North', Sales: 150, Count: 8 },
        { Category: 'B', Region: 'South', Sales: 250, Count: 12 }
    ];

    const pivot = new PivotTable('pivot-container', {
        data: data,
        rowFields: ['Category'],
        columnFields: ['Region'],
        valueFields: ['Sales', 'Count'],
        aggregator: 'sum',
        direction: 'rtl',
        locale: 'fa-IR'
    });
</script>
```

## API

### Constructor

```javascript
new PivotTable(containerId, options)
```

**Parameters:**
- `containerId` (string): ID عنصر HTML که جدول در آن نمایش داده می‌شود
- `options` (object): تنظیمات

**Options:**
- `data` (array): آرایه داده‌ها (الزامی)
- `rowFields` (array): فیلدهای سطر (مثال: `['Category']`)
- `columnFields` (array): فیلدهای ستون (مثال: `['Region']`)
- `valueFields` (array): فیلدهای مقدار (مثال: `['Sales', 'Count']`)
- `aggregator` (string): نوع جمع‌بندی: `'sum'`, `'count'`, `'avg'`, `'min'`, `'max'` (پیش‌فرض: `'sum'`)
- `direction` (string): جهت جدول: `'rtl'` یا `'ltr'` (پیش‌فرض: `'rtl'`)
- `locale` (string): locale برای فرمت اعداد (پیش‌فرض: `'fa-IR'`)

### Methods

#### `updateData(data)`
به‌روزرسانی داده‌ها و رندر مجدد

```javascript
pivot.updateData(newData);
```

#### `setDimensions(rowFields, columnFields, valueFields)`
تغییر ابعاد Pivot Table

```javascript
pivot.setDimensions(['Category'], ['Region', 'Year'], ['Sales']);
```

#### `setAggregator(aggregator)`
تغییر نوع جمع‌بندی

```javascript
pivot.setAggregator('avg');
```

## مثال کامل

```html
<!DOCTYPE html>
<html dir="rtl">
<head>
    <meta charset="UTF-8">
    <title>Pivot Table Example</title>
</head>
<body>
    <div id="pivot-container"></div>

    <script src="pivot-table.js"></script>
    <script>
        // داده‌های نمونه
        const data = [
            { Product: 'A', Region: 'North', Month: 'Jan', Sales: 100, Units: 10 },
            { Product: 'A', Region: 'North', Month: 'Feb', Sales: 120, Units: 12 },
            { Product: 'A', Region: 'South', Month: 'Jan', Sales: 150, Units: 15 },
            { Product: 'B', Region: 'North', Month: 'Jan', Sales: 200, Units: 20 },
            { Product: 'B', Region: 'South', Month: 'Jan', Sales: 250, Units: 25 }
        ];

        // ایجاد Pivot Table
        const pivot = new PivotTable('pivot-container', {
            data: data,
            rowFields: ['Product'],
            columnFields: ['Region', 'Month'],
            valueFields: ['Sales', 'Units'],
            aggregator: 'sum',
            direction: 'rtl',
            locale: 'fa-IR'
        });

        // تغییر aggregator بعد از 3 ثانیه
        setTimeout(() => {
            pivot.setAggregator('avg');
        }, 3000);
    </script>
</body>
</html>
```

## استایل‌دهی

کامپوننت به صورت خودکار استایل‌های پایه را اعمال می‌کند. می‌توانید با CSS سفارشی استایل‌ها را تغییر دهید:

```css
.pivot-table {
    /* استایل جدول */
}

.pivot-header {
    /* استایل هدرهای ستون */
}

.pivot-row-header {
    /* استایل هدرهای سطر */
}

.pivot-value {
    /* استایل سلول‌های مقدار */
}
```

## محدودیت‌ها

- فقط از داده‌های عددی برای value fields پشتیبانی می‌کند
- برای داده‌های بسیار بزرگ (بیش از 10000 ردیف) ممکن است کند باشد
- نیاز به مرورگرهای مدرن (ES6+)

## بهبودهای آینده

- [ ] پشتیبانی از فیلترها
- [ ] قابلیت sort
- [ ] Export به Excel/CSV
- [ ] پشتیبانی از drill-down
- [ ] پشتیبانی از conditional formatting

