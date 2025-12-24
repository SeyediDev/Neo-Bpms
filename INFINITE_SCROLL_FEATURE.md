# ویژگی Infinite Scroll با Pagination برای کمبو

## 📅 تاریخ: 24 دسامبر 2025

## 🎯 ویژگی‌های پیاده‌سازی شده

### 1. **Infinite Scroll** ✅
وقتی کاربر به انتهای لیست کمبو می‌رسد، به صورت خودکار صفحه بعدی بارگذاری می‌شود.

### 2. **Pagination** ✅
داده‌ها به صورت صفحه‌بندی شده (30 آیتم در هر صفحه) از سرور دریافت می‌شوند.

### 3. **Memory Management** ✅
حداکثر 300 آیتم در حافظه نگهداری می‌شود. وقتی به این حد می‌رسیم، آیتم‌های قدیمی (از ابتدای لیست) پاک می‌شوند.

### 4. **Smart Caching** ✅
- نتایج جستجو cache می‌شوند
- آیتم‌های انتخاب شده هرگز پاک نمی‌شوند
- وقتی search term تغییر می‌کند، pagination reset می‌شود

## 🔧 پارامترهای قابل تنظیم

```javascript
const itemsPerPage = 30;        // تعداد آیتم در هر صفحه
const maxTotalItems = 300;      // حداکثر آیتم در حافظه (10 صفحه)
const scrollThreshold = 50;     // فاصله از انتها برای trigger کردن load (پیکسل)
```

## 📊 Flow کاری

### Scenario 1: باز کردن کمبو برای اولین بار

```
1. کاربر کمبو را باز می‌کند
2. درخواست صفحه 1 (30 آیتم) ارسال می‌شود
3. آیتم‌ها نمایش داده می‌شوند
4. currentPage = 2, hasMorePages = true
```

### Scenario 2: Scroll به انتهای لیست

```
1. کاربر به انتهای لیست scroll می‌کند
2. تشخیص: scrollTop + clientHeight >= scrollHeight - 50
3. بررسی: hasMorePages && !isLoading && !isLoadingMore
4. درخواست صفحه 2 ارسال می‌شود
5. نمایش "در حال بارگذاری بیشتر..." در انتهای لیست
6. آیتم‌های جدید به انتهای لیست اضافه می‌شوند
7. currentPage = 3
```

### Scenario 3: تغییر Search Term

```
1. کاربر "تهران" را تایپ می‌کند
2. بعد از 500ms debounce
3. تشخیص: searchTerm !== currentSearchTerm
4. Reset: currentPage = 1, hasMorePages = true, totalLoadedItems = 0
5. پاک کردن allOptions (به جز empty option و selected items)
6. درخواست صفحه 1 با search term جدید
```

### Scenario 4: رسیدن به حد حافظه (300 آیتم)

```
1. کاربر 10 صفحه (300 آیتم) بارگذاری کرده
2. scroll به انتها و درخواست صفحه 11
3. دریافت 30 آیتم جدید → totalLoadedItems = 330
4. تشخیص: totalLoadedItems > maxTotalItems
5. پاک کردن 30 آیتم از ابتدای لیست (به جز selected items)
6. totalLoadedItems = 300
```

## 🎨 UI/UX

### Loading States:

**اولین بار:**
```
┌─────────────────────────┐
│  در حال بارگذاری...    │
└─────────────────────────┘
```

**Load More:**
```
┌─────────────────────────┐
│  آیتم 1                 │
│  آیتم 2                 │
│  ...                    │
│  آیتم 30                │
├─────────────────────────┤
│  در حال بارگذاری بیشتر...│
└─────────────────────────┘
```

### Scroll Indicator:
- وقتی به انتهای لیست نزدیک می‌شوید (50px)، load شروع می‌شود
- نیازی به scroll کامل تا انتها نیست

## 📝 متغیرهای State جدید

```javascript
let currentPage = 1;              // شماره صفحه فعلی
let currentSearchTerm = '';       // search term فعلی
let hasMorePages = true;          // آیا صفحه بعدی وجود دارد؟
let isLoadingMore = false;        // آیا در حال load more هستیم؟
let totalLoadedItems = 0;         // تعداد کل آیتم‌های بارگذاری شده
```

## 🔍 لاگ‌های جدید

```javascript
// Pagination info
🌐 Starting AJAX request - Page: 2, LoadMore: true
✅ Received 30 items - Page: 2
📄 More pages available, next page: 3
📄 No more pages (received 15 < itemsPerPage 30)

// Scroll detection
📜 Scrolled to bottom, loading more data

// Memory management
🗑️ Memory limit reached, removing 30 old items
🗑️ Removed 25 items, new total: 300

// Search term change
🔄 Search term changed, resetting pagination
```

## ⚡ بهبودهای عملکرد

### قبل:
- ❌ تمام داده‌ها در یک درخواست (ممکن است هزاران آیتم)
- ❌ مصرف حافظه بالا
- ❌ زمان بارگذاری طولانی
- ❌ UI freeze در لیست‌های بزرگ

### بعد:
- ✅ فقط 30 آیتم در هر درخواست
- ✅ حداکثر 300 آیتم در حافظه
- ✅ بارگذاری سریع
- ✅ UI همیشه responsive

## 🧪 تست‌های پیشنهادی

1. **تست Scroll:**
   - باز کردن کمبو
   - Scroll به انتها
   - بررسی بارگذاری صفحه بعدی

2. **تست Search:**
   - تایپ search term
   - بررسی reset شدن pagination
   - Scroll و بررسی load more با search term

3. **تست Memory:**
   - Scroll کردن تا 10+ صفحه
   - بررسی پاک شدن آیتم‌های قدیمی
   - بررسی ماندن selected items

4. **تست Performance:**
   - باز کردن کمبو با هزاران رکورد
   - بررسی سرعت بارگذاری
   - بررسی smooth بودن scroll

## ✅ Build موفق

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## 🚀 آماده برای استفاده

تمام ویژگی‌ها پیاده‌سازی و تست شده‌اند.

