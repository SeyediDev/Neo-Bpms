# Filter Modal Fix Backlog - Report Page

**Status Legend:**
- ⏳ Pending - Not started
- 🔄 In Progress - Currently working on
- ✅ Completed - Finished and verified
- ⏸️ Blocked - Waiting for coordination/decision
- 🔍 Review - Needs review before proceeding

---

## Issues to Fix

### 1. Combo Box Issues

#### 1-1. Chip Overflow Management
**Status:** ✅ Completed  
**Priority:** High  
**Description:**  
مدیریت چیپس‌های کمبو - وقتی تعداد چیپس‌ها زیاد می‌شود، باید badge "+N بیشتر" نمایش داده شود و با کلیک روی آن، popover با بقیه چیپس‌ها باز شود.

**Problems:**
- Badge نمایش داده نمی‌شود
- CSS selector `[data-visible="true"]` کار نمی‌کند
- Overflow count محاسبه نمی‌شود

**Files:**
- `_Scripts.report-modern-multi-select.cshtml`
- `_Modal.filter-tooltip.cshtml`

**Solution Applied:**
- ✅ حذف `display: none !important` از inline style badge
- ✅ اضافه کردن `data-visible="false"` در initialization
- ✅ استفاده از `$overflowBadge.css('display', 'flex')` برای نمایش badge
- ✅ استفاده از `$overflowBadge.css('display', 'none')` برای مخفی کردن badge

---

#### 1-2. Combo Arrow Position
**Status:** ✅ Completed  
**Priority:** High  
**Description:**  
آیکن فلش کمبو باید در سمت چپ بالا باشد، نه وسط.

**Problems:**
- Arrow در وسط قرار دارد
- `align-self: flex-start` کار نمی‌کند
- `align-items` در input container باید `flex-start` باشد
- Arrow عرض زیادی اشغال می‌کرد

**Files:**
- `_Scripts.report-modern-multi-select.cshtml`
- `_Modal.filter-tooltip.cshtml`

**Solution Applied:**
- ✅ محدود کردن عرض arrow به 20px (`width: 20px`, `min-width: 20px`, `max-width: 20px`)
- ✅ اضافه کردن `flex-grow: 0` برای جلوگیری از رشد
- ✅ تنظیم `align-self: flex-start` در CSS و JavaScript
- ✅ اضافه کردن `display: flex` با centering برای محتوا

---

### 2. Apply Button Issues

#### 2-1. Button Text, Font, and Icon Display
**Status:** ✅ Completed  
**Priority:** High  
**Description:**  
متن، فونت و آیکن دکمه اعمال فیلتر باید به درستی نمایش داده شود. استایل `ladda-label` باید حذف شود یا خراب کاری نکند.

**Problems:**
- Ladda library به صورت خودکار `.ladda-label` اضافه می‌کند
- استایل `ladda-label` ممکن است متن و آیکن را مخفی کند
- رنگ متن در ApplyButton.cshtml به صورت inline تنظیم شده (`color: #2563EB`)

**Files:**
- `Filter/ApplyButton.cshtml`
- `_Modal.filter-tooltip.cshtml`

**Solution Applied:**
- ✅ حذف inline style `color: #2563EB` از ApplyButton.cshtml
- ✅ اضافه کردن CSS برای `.ladda-label` در filter panel
- ✅ اطمینان از اینکه متن و آیکن همیشه نمایش داده می‌شوند

---

#### 2-2. Apply Filter Method Call and Page Post
**Status:** ✅ Completed  
**Priority:** Critical  
**Description:**  
فراخوانی متد اعمال فیلتر و پست صفحه باید به درستی کار کند.

**Problems:**
- دکمه `onclick="submitFilter()"` دارد
- تابع `submitFilter()` در `_Scripts.filter.cshtml` تعریف شده
- باید مطمئن شویم که form submit می‌شود

**Files:**
- `Filter/ApplyButton.cshtml`
- `_Scripts.filter.cshtml`

**Solution Applied:**
- ✅ بررسی شد: `submitFilter()` به درستی تعریف شده و `window.submitFilter = window.submitReportFilter`
- ✅ بررسی شد: form با id `filter-form` در `_Container.cshtml` وجود دارد
- ✅ بررسی شد: همه select elements با `form="filter-form"` attribute مرتبط هستند
- ✅ تابع `submitReportFilter()` form را submit می‌کند

#### 1-3. Combo Box Initial Size Issue
**Status:** ✅ Completed  
**Priority:** Medium  
**Description:**  
سایز کمبو در لود اولیه کم است و بعد زیاد می‌شود که باعث حرکت و زشتی می‌شود.

**Problems:**
- کمبو در لود اولیه سایز کوچکی دارد
- بعد از لود کامل، سایز افزایش می‌یابد
- این تغییر سایز باعث حرکت و زشتی UI می‌شود

**Files:**
- `_Scripts.report-modern-multi-select.cshtml`
- `_Modal.filter-tooltip.cshtml`

**Solution Applied:**
- ✅ اضافه کردن `min-width: 100%` و `max-width: 100%` به wrapper
- ✅ اضافه کردن `min-width: 100%` و `max-width: 100%` به input
- ✅ اضافه کردن `box-sizing: border-box` برای محاسبه صحیح سایز
- ✅ تنظیم سایز اولیه در JavaScript initialization

---

#### 1-4. Unnecessary Scrollbar in Combo Box
**Status:** ✅ Completed  
**Priority:** Medium  
**Description:**  
اسکرول بیخود در کمبو - باید فقط اسکرول روی لیست options باشد، نه روی input یا items container.

**Problems:**
- اسکرول اضافی در input یا items container
- فقط باید اسکرول روی options container باشد

**Files:**
- `_Scripts.report-modern-multi-select.cshtml`
- `_Modal.filter-tooltip.cshtml`

**Solution Applied:**
- ✅ اضافه کردن `overflow-x: hidden` و `overflow-y: hidden` به input
- ✅ اضافه کردن `overflow-x: hidden` و `overflow-y: hidden` به items container
- ✅ اطمینان از اینکه فقط options container دارای `overflow-y: auto` است
- ✅ اطمینان از اینکه dropdown دارای `overflow: visible` است

---

#### 1-5. Combo Box Position Issue
**Status:** ✅ Completed  
**Priority:** High  
**Description:**  
محل کمبو مشکل دارد - باید بررسی شود که wrapper در جای درست insert می‌شود.

**Problems:**
- کمبو در جای اشتباه قرار می‌گیرد
- ممکن است wrapper در parent اشتباه insert شود

**Files:**
- `_Scripts.report-modern-multi-select.cshtml`

**Solution Applied:**
- ✅ بررسی parent container (neo-control, modern-form)
- ✅ اطمینان از اینکه wrapper در جای درست insert می‌شود
- ✅ اضافه کردن logic برای بررسی control container

---

#### 1-6. Combo Cross-Contamination & Initial Values Issue
**Status:** ✅ Completed  
**Priority:** Critical  
**Description:**  
تداخل مقادیر بین کمبوهای مختلف در لود اولیه و نمایش مقادیر عددی به جای متن ترجمه شده.

**Problems:**
- مقادیر اولیه کمبوی اکوسیستم با کمبوی RFM قاطی می‌شد
- کمبوی کشور و شهر مقادیر عددی به جای متن ترجمه شده نشان می‌داد
- کمبوی اکوسیستم متن اشتباه از entity دیگر نشان می‌داد (NamespaceId و EntityId اشتباه)

**Files:**
- `_Scripts.report-modern-multi-select.cshtml`

**Solution Applied:**
- ✅ اضافه کردن unique combo instance ID برای هر کمبو
- ✅ تغییر API از GetComboData به GetComboInitValues برای مقادیر اولیه
- ✅ اضافه کردن helper function `hasValidDisplayText` برای تشخیص متن‌های عددی
- ✅ Remote combos همیشه از سرور fetch می‌کنند (جلوگیری از نمایش متن اشتباه)
- ✅ اضافه کردن validation برای جلوگیری از cross-contamination

---

#### 1-7. Popular Filter Loading Error
**Status:** ✅ Completed  
**Priority:** High  
**Description:**  
وقتی فیلتر پرکاربرد ذخیره شده انتخاب می‌شد، همه کمبوها با خطای Authorization مواجه می‌شدند.

**Problems:**
- خطای Authorization 500 در لود فیلتر پرکاربرد
- FormId خالی به سرور ارسال می‌شد
- URL از ItemId استفاده می‌کرد اما کد فقط ReportId را می‌خواند

**Files:**
- `_Scripts.report-modern-multi-select.cshtml`

**Solution Applied:**
- ✅ اضافه کردن `getUrlParameter('ItemId')` به منطق بازیابی FormId
- ✅ بهبود error logging با نمایش جزئیات خطا

---

#### 1-8. Popular Filter Save Issue
**Status:** ⏳ Pending  
**Priority:** High  
**Description:**  
ذخیره مقادیر فیلتر به درستی انجام نمی‌شود.

**Problems:**
- ذخیره مقادیر فیلتر درست کار نمی‌کند
- نیاز به بررسی بیشتر دارد

**Files:**
- `_Scripts.report-modern-multi-select.cshtml` (احتمالاً)
- فایل‌های مربوط به ذخیره فیلتر پرکاربرد

**Solution Applied:**
- ⏳ نیاز به بررسی دارد

---

## Progress Log

### 2024-12-XX - Initial Analysis & Fixes
- ✅ Created backlog
- ✅ Fixed overflow badge - added `data-visible="false"` when no overflow
- ✅ Fixed apply button - removed inline styles, added ladda-label CSS
- ✅ Fixed apply button text color - removed inline `color: #2563EB`
- ✅ Fixed arrow position - limited width to 20px, set align-self to flex-start
- ✅ Fixed overflow badge display - using CSS display property directly
- ✅ Fixed combo box initial size - added min-width and max-width to prevent size change
- ✅ Verified apply filter method - submitFilter() works correctly with filter-form

### 2024-12-XX - All Issues Fixed
- ✅ 1-1. Chip Overflow Management - Completed
- ✅ 1-2. Combo Arrow Position - Completed
- ✅ 1-3. Combo Box Initial Size Issue - Completed
- ✅ 2-1. Button Text, Font, and Icon Display - Completed
- ✅ 2-2. Apply Filter Method Call and Page Post - Completed

### 2024-12-24 - Combo Cross-Contamination & Popular Filter Fixes
- ✅ 1-5. Combo Box Position Issue - Completed
- ✅ 1-6. Combo Cross-Contamination & Initial Values Issue - Completed (unique IDs, GetComboInitValues API, hasValidDisplayText)
- ✅ 1-7. Popular Filter Loading Error - Completed (ItemId URL parameter support)
- ⏳ 1-8. Popular Filter Save Issue - **Pending** - نیاز به بررسی بیشتر دارد

**Current Status:** ⏳ Pending - مشکل ذخیره فیلتر پرکاربرد باید بررسی شود

