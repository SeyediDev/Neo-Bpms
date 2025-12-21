# Filter Modal Test Checklist - Report Page

**Date:** 2024-12-XX  
**Tester:** [Your Name]  
**Status:** ⏳ Pending Testing

---

## Test Environment
- [ ] Browser: _______________
- [ ] Screen Resolution: _______________
- [ ] RTL Mode: Yes/No
- [ ] Page: Report Page with Filter Modal

---

## 1. Combo Box Tests

### 1-1. Chip Overflow Management Test
**Priority:** High  
**Description:** بررسی مدیریت چیپس‌های overflow

**Test Steps:**
1. [ ] باز کردن صفحه گزارش
2. [ ] باز کردن مدال فیلتر
3. [ ] انتخاب یک کمبو (مثلاً "کشور")
4. [ ] انتخاب 3-4 آیتم
5. [ ] بررسی اینکه چیپس‌ها نمایش داده می‌شوند
6. [ ] انتخاب 10+ آیتم (تا چیپس‌ها overflow شوند)
7. [ ] بررسی اینکه badge "+N بیشتر" نمایش داده می‌شود
8. [ ] کلیک روی badge "+N بیشتر"
9. [ ] بررسی اینکه popover باز می‌شود
10. [ ] بررسی اینکه بقیه چیپس‌ها در popover نمایش داده می‌شوند
11. [ ] کلیک روی دکمه حذف (×) یک چیپس در popover
12. [ ] بررسی اینکه چیپس حذف می‌شود
13. [ ] بررسی اینکه اگر تعداد چیپس‌ها کم شد، badge مخفی می‌شود

**Expected Results:**
- ✅ Badge "+N بیشتر" وقتی چیپس‌ها overflow می‌شوند نمایش داده می‌شود
- ✅ Popover با کلیک روی badge باز می‌شود
- ✅ همه چیپس‌های overflow در popover نمایش داده می‌شوند
- ✅ حذف چیپس از popover کار می‌کند
- ✅ Badge وقتی overflow نیست مخفی می‌شود

**Actual Results:**
- [ ] Pass
- [ ] Fail - Notes: _______________

---

### 1-2. Combo Arrow Position Test
**Priority:** High  
**Description:** بررسی موقعیت آیکن فلش کمبو

**Test Steps:**
1. [ ] باز کردن صفحه گزارش
2. [ ] باز کردن مدال فیلتر
3. [ ] پیدا کردن یک کمبو (مثلاً "کشور")
4. [ ] بررسی موقعیت آیکن فلش (▼)
5. [ ] بررسی اینکه arrow در سمت راست است (RTL)
6. [ ] بررسی اینکه arrow در بالا align شده است
7. [ ] بررسی اینکه arrow عرض زیادی اشغال نمی‌کند

**Expected Results:**
- ✅ Arrow در سمت راست کمبو است
- ✅ Arrow در بالا align شده است (نه وسط)
- ✅ Arrow فقط 20px عرض دارد
- ✅ Arrow در وسط کنترل دیده نمی‌شود

**Actual Results:**
- [ ] Pass
- [ ] Fail - Notes: _______________

---

### 1-3. Combo Box Initial Size Test
**Priority:** Medium  
**Description:** بررسی سایز اولیه کمبو و عدم تغییر سایز

**Test Steps:**
1. [ ] باز کردن صفحه گزارش
2. [ ] باز کردن مدال فیلتر
3. [ ] مشاهده کمبوها در لود اولیه
4. [ ] بررسی اینکه کمبوها سایز کامل دارند
5. [ ] صبر کردن تا صفحه کاملاً لود شود
6. [ ] بررسی اینکه سایز کمبوها تغییر نمی‌کند
7. [ ] بررسی اینکه کمبوها حرکت نمی‌کنند
8. [ ] بررسی اینکه UI زشت نمی‌شود

**Expected Results:**
- ✅ کمبوها در لود اولیه سایز کامل دارند
- ✅ سایز کمبوها بعد از لود تغییر نمی‌کند
- ✅ کمبوها حرکت نمی‌کنند
- ✅ UI زشت نمی‌شود

**Actual Results:**
- [ ] Pass
- [ ] Fail - Notes: _______________

---

### 1-4. Unnecessary Scrollbar Test
**Priority:** Medium  
**Description:** بررسی اسکرول اضافی در کمبو

**Test Steps:**
1. [ ] باز کردن صفحه گزارش
2. [ ] باز کردن مدال فیلتر
3. [ ] پیدا کردن یک کمبو
4. [ ] بررسی input کمبو (باکس اصلی)
5. [ ] بررسی اینکه اسکرول در input نیست
6. [ ] کلیک روی کمبو برای باز کردن dropdown
7. [ ] بررسی dropdown
8. [ ] بررسی اینکه فقط یک اسکرول روی لیست options است
9. [ ] بررسی اینکه اسکرول اضافی در dropdown نیست
10. [ ] بررسی اینکه اسکرول اضافی در input نیست

**Expected Results:**
- ✅ هیچ اسکرولی در input کمبو نیست
- ✅ هیچ اسکرولی در items container نیست
- ✅ فقط یک اسکرول روی لیست options است (وقتی آیتم‌ها زیاد هستند)
- ✅ اسکرول اضافی وجود ندارد

**Actual Results:**
- [ ] Pass
- [ ] Fail - Notes: _______________

---

## 2. Apply Button Tests

### 2-1. Button Text, Font, and Icon Display Test
**Priority:** High  
**Description:** بررسی نمایش متن، فونت و آیکن دکمه اعمال فیلتر

**Test Steps:**
1. [ ] باز کردن صفحه گزارش
2. [ ] باز کردن مدال فیلتر
3. [ ] پیدا کردن دکمه "اعمال فیلتر"
4. [ ] بررسی اینکه متن "اعمال فیلتر" نمایش داده می‌شود
5. [ ] بررسی اینکه آیکن SVG نمایش داده می‌شود
6. [ ] بررسی فونت و سایز متن
7. [ ] بررسی رنگ متن (باید سفید باشد)
8. [ ] کلیک روی دکمه (بدون لود کردن)
9. [ ] بررسی اینکه متن و آیکن در حالت loading هم نمایش داده می‌شوند
10. [ ] بررسی اینکه spinner لادا نمایش داده نمی‌شود

**Expected Results:**
- ✅ متن "اعمال فیلتر" نمایش داده می‌شود
- ✅ آیکن SVG نمایش داده می‌شود
- ✅ فونت و سایز با سایر دکمه‌ها یکسان است (font-weight: 500, font-size: 14px)
- ✅ رنگ متن سفید است
- ✅ متن و آیکن در همه حالات نمایش داده می‌شوند
- ✅ Spinner لادا نمایش داده نمی‌شود

**Actual Results:**
- [ ] Pass
- [ ] Fail - Notes: _______________

---

### 2-2. Apply Filter Method Call and Page Post Test
**Priority:** Critical  
**Description:** بررسی فراخوانی متد اعمال فیلتر و پست صفحه

**Test Steps:**
1. [ ] باز کردن صفحه گزارش
2. [ ] باز کردن مدال فیلتر
3. [ ] تنظیم چند فیلتر (مثلاً انتخاب کشور، شهر، etc.)
4. [ ] کلیک روی دکمه "اعمال فیلتر"
5. [ ] بررسی console برای خطا
6. [ ] بررسی اینکه form submit می‌شود
7. [ ] بررسی اینکه صفحه reload می‌شود
8. [ ] بررسی اینکه فیلترها اعمال شده‌اند
9. [ ] بررسی اینکه داده‌های گزارش با فیلترها فیلتر شده‌اند

**Expected Results:**
- ✅ دکمه "اعمال فیلتر" کلیک می‌شود
- ✅ هیچ خطایی در console نیست
- ✅ Form submit می‌شود
- ✅ صفحه reload می‌شود
- ✅ فیلترها اعمال شده‌اند
- ✅ داده‌های گزارش با فیلترها فیلتر شده‌اند

**Actual Results:**
- [ ] Pass
- [ ] Fail - Notes: _______________

---

## 3. General UI Tests

### 3-1. Overall Layout Test
**Priority:** Medium  
**Description:** بررسی کلی layout و UI

**Test Steps:**
1. [ ] باز کردن صفحه گزارش
2. [ ] باز کردن مدال فیلتر
3. [ ] بررسی کلی layout
4. [ ] بررسی alignment عناصر
5. [ ] بررسی spacing بین عناصر
6. [ ] بررسی responsive behavior (اگر ممکن است)

**Expected Results:**
- ✅ Layout تمیز و مرتب است
- ✅ عناصر به درستی align شده‌اند
- ✅ Spacing مناسب است
- ✅ UI زشت نیست

**Actual Results:**
- [ ] Pass
- [ ] Fail - Notes: _______________

---

## Test Summary

**Total Tests:** 6  
**Passed:** ___  
**Failed:** ___  
**Blocked:** ___

**Overall Status:**
- [ ] ✅ All Tests Passed
- [ ] ⚠️ Some Tests Failed
- [ ] ⏸️ Blocked

**Notes:**
_______________
_______________
_______________

**Tester Signature:** _______________  
**Date:** _______________

