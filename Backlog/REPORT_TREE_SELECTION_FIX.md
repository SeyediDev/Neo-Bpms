# رفع مشکل بیرون زدن باکس آیتم انتخاب شده در درختواره

## وضعیت: ⏳ Pending
## اولویت: کم (Low)

## شرح مشکل
در مدال "طراحی گزارش‌ها"، وقتی یک آیتم در درختواره (jstree) انتخاب می‌شود، پس‌زمینه highlight شده از کادر container بیرون می‌زند.

## محل مشکل
- فایل: `Views/Report/Partials/_Modal.designs.cshtml`
- Container: `#configs-tree-modal.report-tree-container`
- کلاس: `.jstree-clicked`

## تلاش‌های قبلی (ناموفق)
1. اضافه کردن `overflow-x: hidden` به container - باعث خراب شدن ارتفاع نودها شد
2. اضافه کردن `display: block` و `overflow: hidden` به `.jstree-anchor` - همه چیز را خراب کرد

## راه‌حل‌های پیشنهادی برای بررسی
1. بررسی عرض پیش‌فرض jstree و تنظیم آن
2. استفاده از `max-width: 100%` روی anchor بدون تغییر display
3. بررسی استایل‌های پیش‌فرض jstree theme

## تاریخ ایجاد
2025-12-28

