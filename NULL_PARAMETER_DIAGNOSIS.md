# تشخیص و حل مشکل پارامترهای Null در کمبو

## 📅 تاریخ: 24 دسامبر 2025

## 🔍 چرا NamespaceId یا EntityId ممکن است null شوند؟

### منابع دریافت پارامترها (به ترتیب اولویت):

#### 1. **Attributes روی خود select element**
```html
<select name="Country" 
        data-namespace-id="Customers" 
        data-entity-id="CustomerTenant">
</select>
```

#### 2. **Attributes روی parent div**
```html
<div class="neo-control" 
     data-namespace-id="Customers" 
     data-entity-id="CustomerTenant">
    <select name="Country"></select>
</div>
```

#### 3. **Attributes روی filter panel**
```html
<div id="filterTooltipPanel" 
     data-namespace-id="Customers" 
     data-entity-id="CustomerTenant">
    <!-- selects here -->
</div>
```

#### 4. **Window context (JavaScript global)**
```javascript
window.reportContext = {
    namespaceId: "Customers",
    entityId: "CustomerTenant"
};
```

#### 5. **URL parameters**
```
?NamespaceId=Customers&EntityId=CustomerTenant
```

#### 6. **PageAddressManager**
```javascript
window.PageAddressManager.getCurrentAddress()
```

## ⚠️ شرایطی که پارامترها null می‌شوند:

### Scenario 1: Attributes فراموش شده
```html
<!-- ❌ اشتباه - هیچ attribute ای نیست -->
<select name="Country" data-is-remote="true">
</select>

<!-- ✅ درست -->
<select name="Country" 
        data-is-remote="true"
        data-namespace-id="Customers" 
        data-entity-id="CustomerTenant">
</select>
```

### Scenario 2: Typo در نام attribute
```html
<!-- ❌ اشتباه - نام اشتباه -->
<select data-namespaceid="Customers">  <!-- باید data-namespace-id باشد -->
</select>

<!-- ✅ درست -->
<select data-namespace-id="Customers">
</select>
```

### Scenario 3: Parent div مناسب نیست
```html
<!-- ❌ اشتباه - parent div هیچ attribute ای ندارد -->
<div class="some-wrapper">
    <select name="Country"></select>
</div>

<!-- ✅ درست -->
<div class="neo-control" 
     data-namespace-id="Customers" 
     data-entity-id="CustomerTenant">
    <select name="Country"></select>
</div>
```

### Scenario 4: Filter panel بدون attributes
```html
<!-- ❌ اشتباه -->
<div id="filterTooltipPanel">
    <select name="Country"></select>
</div>

<!-- ✅ درست -->
<div id="filterTooltipPanel" 
     data-namespace-id="Customers" 
     data-entity-id="CustomerTenant">
    <select name="Country"></select>
</div>
```

### Scenario 5: URL parameters در دسترس نیستند
```
<!-- ❌ اشتباه - URL بدون parameters -->
/Report

<!-- ✅ درست -->
/Report?NamespaceId=Customers&EntityId=CustomerTenant
```

## 🔬 ابزارهای تشخیص

### 1. لاگ خودکار در initialization
هر بار که کمبو initialize می‌شود، این لاگ نمایش داده می‌شود:

```javascript
🔍 Parameter initialization for field: Country
  namespaceId: "Customers"
  entityId: "CustomerTenant"
  sources: {
    selectAttr: { namespaceId: "Customers", entityId: "CustomerTenant" },
    parentDiv: { namespaceId: null, entityId: null },
    filterPanel: { namespaceId: null, entityId: null },
    windowContext: { namespaceId: undefined, entityId: undefined },
    url: { namespaceId: "Customers", entityId: "CustomerTenant" },
    pageAddressManager: "available"
  }
```

### 2. Warning برای پارامترهای null
اگر پارامترها null باشند:

```javascript
⚠️ WARNING: Missing required parameters for field: Country
  namespaceId: MISSING
  entityId: MISSING
  isRemote: true
  message: "This field will not be able to load data from server"
  recommendation: "Add data-namespace-id and data-entity-id attributes"
```

### 3. Error هنگام تلاش برای load
اگر سعی شود با پارامترهای null درخواست ارسال شود:

```javascript
❌ CRITICAL: Missing required parameters!
  fieldName: "Country"
  namespaceId: null
  entityId: null
  message: "Cannot send request without NamespaceId and EntityId"
```

### 4. تابع تشخیص دستی
می‌توانید در console این تابع را فراخوانی کنید:

```javascript
// در console مرورگر
diagnoseNullParameters('Country');
```

خروجی:
```
🔬 Diagnosing null parameters for field: Country
  ✅ Select element found
  Select attributes: {
    data-namespace-id: "Customers",
    data-entity-id: "CustomerTenant",
    data-is-remote: "true",
    name: "Country"
  }
  Parent div attributes: { ... }
  Filter panel attributes: { ... }
  Window context: { ... }
  URL parameters: { ... }
```

## 🛠️ راه‌حل‌ها

### راه‌حل 1: اضافه کردن attributes به select
```html
<select name="Country" 
        data-is-remote="true"
        data-namespace-id="Customers" 
        data-entity-id="CustomerTenant"
        data-field-id="Country">
</select>
```

### راه‌حل 2: اضافه کردن attributes به parent div
```html
<div class="neo-control" 
     data-namespace-id="Customers" 
     data-entity-id="CustomerTenant">
    <label>کشور</label>
    <select name="Country" data-is-remote="true"></select>
</div>
```

### راه‌حل 3: اضافه کردن attributes به filter panel
```html
<div id="filterTooltipPanel" 
     data-namespace-id="@Model.NamespaceId" 
     data-entity-id="@Model.EntityId">
    <!-- All filter selects here will inherit these -->
    <select name="Country"></select>
    <select name="City"></select>
</div>
```

### راه‌حل 4: تنظیم window context در JavaScript
```javascript
// در ابتدای صفحه
window.reportContext = {
    namespaceId: "@Model.NamespaceId",
    entityId: "@Model.EntityId",
    formId: "@Model.FormId"
};
```

### راه‌حل 5: اطمینان از وجود URL parameters
```csharp
// در Controller
return Redirect($"/Report?NamespaceId={namespaceId}&EntityId={entityId}");
```

## 📊 اولویت راه‌حل‌ها

| اولویت | راه‌حل | مزایا | معایب |
|--------|--------|-------|-------|
| 1 | Filter panel attributes | یک بار تنظیم، همه selects استفاده می‌کنند | فقط برای filter panel |
| 2 | Select attributes | دقیق و مشخص | باید برای هر select تنظیم شود |
| 3 | Parent div attributes | خوانا و سازمان‌یافته | نیاز به ساختار HTML مناسب |
| 4 | Window context | مرکزی و قابل مدیریت | Global scope |
| 5 | URL parameters | همیشه در دسترس | ممکن است تغییر کند |

## ✅ Build موفق

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## 🧪 تست

1. صفحه را refresh کنید
2. Console را باز کنید
3. به دنبال این لاگ‌ها بگردید:
   - `🔍 Parameter initialization`
   - `⚠️ WARNING: Missing required parameters`
   - `❌ CRITICAL: Missing required parameters`
4. اگر مشکلی بود، از `diagnoseNullParameters('FieldName')` استفاده کنید

