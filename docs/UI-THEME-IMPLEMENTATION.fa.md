# قرارداد تم و وضعیت پیاده‌سازی

این سند قرارداد بستهٔ نخست هماهنگی تم است. سورس در ریپازیتوری اصلی
Neo-Bpms اعمال شده است. نتیجهٔ آزمون کنترل‌ها از پذیرش محیط واقعی Hyper
جدا ثبت می‌شود؛ این بسته ادعای تکمیل تمام مراحل برنامهٔ UI را ندارد.

## منشأ و اولویت تم

در Hyper، `Backend/src/Core/Hyper.Domain/Features/Themes/ThemeService.cs`
پالت را تولید می‌کند و
`Backend/src/AdminPanel/Hyper.AdminPanel.Web/Views/Shared/Layout/_ThemeVariables.cshtml`
آن را در صفحه قرار می‌دهد. پل مشترک از متغیرهای واقعی میزبان می‌خواند؛
درخواست API جدیدی اضافه نمی‌کند و دادهٔ فرم را تغییر نمی‌دهد.

ترتیب اولویت: تم والد هم‌مبدأ، پالت میزبان، سپس ترجیح مستقل `neo_theme`.
حالت مستقل روشن، تیره و system دارد. حالت مؤثر در `data-neo-mode`،
`data-theme`، `data-bs-theme` و کلاس `dark` روی ریشه منعکس می‌شود.
برای پالت میزبان، حالت روشن/تیره از روشنایی رنگ پس‌زمینه استنباط می‌شود.

| نقش | متغیر میزبان | توکن مشترک |
| --- | --- | --- |
| زمینه | `--neo-dashboard-bg-secondary` | `--neo-canvas` |
| کارت | `--neo-dashboard-bg-card` | `--neo-surface` |
| پنجره و کمبو | `--neo-form-background` | `--neo-elevated` |
| ورودی | `--neo-form-input-background` | `--neo-input` |
| متن ورودی | `--neo-form-input-text` | `--neo-input-text` |
| متن | `--neo-text-primary` | `--neo-text` |
| حاشیه | `--neo-form-input-border` | `--neo-border` |
| عملیات اصلی | `--neo-menu-primary` | `--neo-accent` |
| متن عملیات | `--neo-form-button-text` | `--neo-on-accent` |

توکن‌های نمودار، گزارش، هدر و وضعیت نیز در `neo-theme.js` نگاشت دارند.
در نبود مقدار میزبان، پیش‌فرض روشن/تیره استفاده می‌شود. اصلاح کنتراست
متن عملیات اصلی برای رنگ‌های hex و rgb قابل محاسبه انجام می‌شود؛ این
جایگزین ممیزی کامل کنتراست همهٔ اجزا نیست.

## فایل‌ها و استفاده

- `src/Neo.Bpms.UI.MVC/wwwroot/js/neo-theme.js`: API مشترک، تشخیص پالت،
  مشاهدهٔ تغییرات و رویداد؛ تعریف TypeScript در فایل هم‌نام `.d.ts`.
- `src/Neo.Bpms.UI.MVC/wwwroot/css/neo-theme.css`: توکن‌ها و سبک‌های پایه.
- `src/Neo.Bpms.UI.MVC/wwwroot/css/neo-theme-mvc.css`: سازگاری با کنترل‌های MVC.
- `src/Neo.UI.Shared/theme-colors.cjs`: رنگ‌های معنایی Tailwind.

`CommonIncludes` و `MinimalIframeIncludes` فایل‌های MVC را از مسیر
`/_content/Neo.Bpms.UI.MVC/` با نسخه‌گذاری بارگذاری می‌کنند. دو برنامهٔ
Next از همان CSS و bootstrap استفاده می‌کنند. AdminPanel از ماژول تم
مشترک در مسیر موجود Redux استفاده می‌کند. `src/styles.css` ابزارهای CSS
کامپوننت‌ها را بدون preflight عمومی میزبان می‌سازد؛ مصرف‌کننده باید
`@neo/admin-panel-core/styles` را هم import کند. برای بسته‌بندی این پروژه‌ها،
پوشه‌های هم‌جوار مشترک باید در checkout ساخت موجود باشند.

```javascript
// فقط ترجیح صفحات مستقل؛ پالت میزبان اولویت دارد.
window.NeoTheme.set({ mode: 'dark' }, true);
const resolved = window.NeoTheme.snapshot();
window.NeoTheme.refresh();
window.addEventListener('neo:theme-changed', event => {
  const { mode, tokens, direction } = event.detail;
  // به‌روزرسانی اجزایی که رنگ را در زمان ساخت ذخیره می‌کنند.
});
```

از snapshot فقط برای خواندن استفاده شود. فیلدهای آن شامل mode، tokens،
direction، inherited، host و appearance هستند. پارامتر دوم set تعیین
می‌کند ترجیح در کلید موجود `neo_theme` ذخیره شود یا خیر.

## پوشش بستهٔ نخست

کنترل‌ها و فیلترهای MVC، popupهای رایج، جدول، منو، تب، دیالوگ و ظاهر
نمودارهای SVG به توکن‌ها متصل شده‌اند. در React، نقش‌های خنثی زمینه،
سطح، حاشیه و متن مهاجرت داده شده‌اند. رنگ سری داده و وضعیت‌ها تعمداً
نیازمند بررسی معنایی جداگانه است. این فهرست تأیید دیداری همهٔ صفحات نیست.

قرارداد فیلتر سرستون، نام فیلدها، query و درخواست‌های کسب‌وکار تغییر نکرده‌اند.
آزمون تازه، همگامی پالت، ترجیح سیستم، storage نامعتبر، iframe هم‌مبدأ،
حفظ مقدار ورودی و popup واقعی گزارش را هدف می‌گیرد.

## محدودیت‌های باز

- انتخابگر تم Hyper هنوز `window.location.reload()` اجرا می‌کند؛ حذف reload
  مستلزم تغییر و آزمون جداگانه در ریپازیتوری میزبان است.
- همگامی iframe بین مبدأهای متفاوت پیاده‌سازی نشده است.
- مسیر واقعی asset، ترتیب CSS میزبان، cache، CSP و اولین نمایش React
  باید در Hyper بررسی شوند. typecheck جای build یا بررسی hydration نیست.
- همهٔ رنگ‌های ثابت، تمام ابزارهای طراحی، چاپ و خروجی‌ها هنوز ممیزی نشده‌اند.
- رفع کامل clipping و stacking popupها از روی این تغییر رنگ اثبات نمی‌شود.
- معیارهای پذیرش برنامه، تصاویر و آزمون سامانهٔ واقعی همچنان باز هستند.

## وضعیت بررسی‌ها

| بررسی | نتیجهٔ ثبت‌شده |
| --- | --- |
| نحو JavaScript پل و فایل آزمون | موفق |
| TypeScript Monitoring و EditableGrid با noEmit و incremental=false | موفق |
| ساخت CSS با Tailwind در Monitoring و EditableGrid | موفق؛ هر دو فرمان با کد صفر پایان یافتند |
| نصب وابستگی AdminPanel | موفق؛ با lockfile موجود |
| build کامل Monitoring، EditableGrid و AdminPanel | موفق؛ خروجی Production و declarationهای AdminPanel تولید شدند |
| build و اجرای میزبان MVC در Hyper | پذیرش نهایی هنوز ثبت نشده است |
| اعتبارسنج خودکار Skill | موفق؛ quick_validate.py |
| مرورگر Edge؛ فیلتر سرستون و تم | ۱۴ آزمون فیلتر و ۱۲ آزمون تم موفق؛ CSS واقعی Bootstrap و کمبوی گزارش بارگذاری شدند |
| مرورگر روی خروجی Production | دو آزمون Monitoring و EditableGrid موفق؛ asset و hydration بدون خطا و تغییر تم iframe بدون reload |
| انتقال سورس به ریپازیتوری | انجام شد؛ مانع دسترسی قبلی رفع شد |
| آزمون واقعی Hyper | انجام نشده |

برای اجرای آزمون‌ها پس از رفع مانع:

```powershell
Set-Location tests/Neo.Bpms.UI.MVC.Tests/Frontend
npm ci
$env:NEO_TEST_BROWSER = 'msedge'
npm run test:column-filters
npm run test:theme
```

آزمون فیلتر در همین بسته دوباره اجرا شد. آزمون تم پس از افزودن CSS واقعی
Bootstrap نیز دوباره اجرا و موفق شد. آزمون تغییر رنگ منتظر پایان transition
دکمه می‌ماند. تصاویر نمونه، سند پذیرش صفحات واقعی Hyper محسوب نمی‌شوند.

## Skill و MCP

Skill توسعهٔ Neo-Bpms به قرارداد بالا و محدودیت‌های آن ارجاع می‌دهد.
پنج ابزار Companion قراردادشان تغییر نکرده است؛ ابزار یا recipe ساختگی
برای تم معرفی نشده و hashهای منابع نامرتبط بازنویسی نشده‌اند.


### آزمون خروجی Production برنامه‌های React

پس از `npm run build` در Monitoring و EditableGrid، در پوشهٔ آزمون frontend:

```powershell
$env:NEO_MONITORING_DIST = (Resolve-Path '../../../src/Neo.UI.Monitoring/dist').Path
$env:NEO_GRID_DIST = (Resolve-Path '../../../src/Neo.UI.EditableGrid/dist').Path
$env:NEO_TEST_BROWSER = 'msedge'
npm run test:theme-apps
```

این آزمون‌ها خروجی واقعی Next را روی سرور موقت loopback سرو می‌کنند و
بارگذاری asset، اجرای React، وراثت تم iframe و تغییر رنگ بدون reload را
می‌سنجند. API در آزمون عمداً در حالت unavailable قرار دارد؛ درخواست به
حساب واقعی یا عملیات داده ارسال نمی‌شود. این پوشش، صحت API یا آزمون
سامانهٔ احراز هویت‌شده نیست.

در export ایستای Next، تنظیمات headers و rewrites اجرا نمی‌شوند؛ میزبان
MVC باید مسیر API و سیاست‌های پاسخ را فراهم کند. هشدار فعلی build دربارهٔ
همین محدودیت export است. دادهٔ قدیمی Browserslist و هم‌زمانی import ایستا
و پویا در AdminPanel نیز هشدار دادند ولی مانع build نشدند.
