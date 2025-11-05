# سیستم کنترل‌های مدرن Neo BPMS

## معرفی

این سیستم بهبود یافته برای کنترل‌های فرم در Neo BPMS طراحی شده است که شامل:

- **فریمورک CSS مدرن** با پشتیبانی از RTL و طراحی ریسپانسیو
- **کنترل‌های HTML بهبود یافته** با UX بهتر
- **سیستم متادیتا پیشرفته** برای تعریف موجودیت‌ها
- **رندرر مدرن** برای تولید HTML بهینه

## ویژگی‌های کلیدی

### 🎨 طراحی مدرن
- استفاده از CSS Variables برای مدیریت رنگ‌ها و فاصله‌ها
- پشتیبانی کامل از RTL (راست به چپ)
- طراحی ریسپانسیو برای موبایل و تبلت
- پشتیبانی از Dark Mode
- انیمیشن‌های نرم و جذاب

### 🔧 کنترل‌های بهبود یافته
- **ModernTextField**: فیلدهای متنی با اعتبارسنجی بهتر
- **ModernComboBox**: لیست‌های انتخاب با جستجو
- **ModernDatePicker**: انتخابگر تاریخ فارسی
- **ModernFileUpload**: آپلود فایل با UI بهتر
- **ModernCheckbox**: چک‌باکس‌های مدرن
- **ModernCard**: کارت‌های اطلاعاتی

### 📊 سیستم متادیتا
- تعریف کامل فیلدها و روابط
- اعتبارسنجی‌های پیشرفته
- ایندکس‌های بهینه
- قوانین UI پویا

## نصب و راه‌اندازی

### 1. اضافه کردن فایل‌های CSS

```html
<!-- در Layout اصلی -->
<link rel="stylesheet" href="~/Content/common-assets-includes/styles/features/modern/controls-modern.css" asp-append-version="true" />
```

### 2. اضافه کردن کنترلر مدرن

```csharp
// در Startup.cs یا Program.cs
services.AddScoped<ModernControlsRenderer>();
services.AddScoped<ModernFormController>();
```

### 3. تعریف متادیتا

```csharp
// در فایل MetaDefinition
public class ProductDefinition : DefaultCRUDDefinition
{
    public override Type DefinitionEntity => typeof(Product);

    protected override void DefineStructure()
    {
        base.DefineStructure();
        
        // تعریف فیلدها
        AddField("Title", "عنوان", "Title", TVariableTypes.String, typeof(string), EntityFieldFlags.Required);
        AddField("Description", "توضیحات", "Description", TVariableTypes.String, typeof(string));
        
        // تعریف روابط
        AddField("CategoryId", "دسته‌بندی", "Category", TVariableTypes.Integer, typeof(int));
        
        // تعریف اعتبارسنجی
        AddValidation("Title", "Title", "عنوان الزامی است", "Title is required", "Title != null && Title.Length > 0");
    }
}
```

## استفاده

### 1. فرم ایجاد

```csharp
[HttpGet]
public async Task<IActionResult> CreateModern(string namespaceId, string entityId, string formId)
{
    var structure = await GetFormStructure(namespaceId, entityId, formId);
    ViewBag.structure = structure;
    
    return View("CreateModern", new ElasticObject());
}
```

### 2. فرم ویرایش

```csharp
[HttpGet]
public async Task<IActionResult> EditModern(string namespaceId, string entityId, string formId, string id)
{
    var structure = await GetFormStructure(namespaceId, entityId, formId);
    var model = await GetEntityData(namespaceId, entityId, id);
    
    ViewBag.structure = structure;
    return View("EditModern", model);
}
```

### 3. لیست

```csharp
[HttpGet]
public async Task<IActionResult> IndexModern(string namespaceId, string entityId, string formId)
{
    var structure = await GetFormStructure(namespaceId, entityId, formId);
    var data = await GetEntityList(namespaceId, entityId, formId);
    
    ViewBag.structure = structure;
    return View("IndexModern", data);
}
```

## سفارشی‌سازی

### 1. تغییر رنگ‌ها

```css
:root {
  --primary-color: #your-color;
  --secondary-color: #your-color;
  --success-color: #your-color;
  --warning-color: #your-color;
  --error-color: #your-color;
}
```

### 2. تغییر فونت

```css
:root {
  --font-family: 'Your-Font', 'Vazir', sans-serif;
}
```

### 3. تغییر فاصله‌ها

```css
:root {
  --spacing-xs: 0.25rem;
  --spacing-sm: 0.5rem;
  --spacing-md: 1rem;
  --spacing-lg: 1.5rem;
  --spacing-xl: 2rem;
}
```

## کنترل‌های موجود

### ModernTextField
```csharp
public class ModernTextField : NeoHtmlControl
{
    // فیلد متنی با اعتبارسنجی و استایل مدرن
}
```

### ModernComboBox
```csharp
public class ModernComboBox : NeoHtmlControl
{
    // لیست انتخاب با جستجو و فیلتر
}
```

### ModernDatePicker
```csharp
public class ModernDatePicker : NeoHtmlControl
{
    // انتخابگر تاریخ فارسی
}
```

### ModernFileUpload
```csharp
public class ModernFileUpload : NeoHtmlControl
{
    // آپلود فایل با UI بهتر
}
```

### ModernCheckbox
```csharp
public class ModernCheckbox : NeoHtmlControl
{
    // چک‌باکس مدرن
}
```

### ModernCard
```csharp
public class ModernCard : NeoHtmlControl
{
    // کارت اطلاعاتی با لینک‌ها
}
```

## اعتبارسنجی

### اعتبارسنجی سمت کلاینت

```javascript
function validateForm(form) {
    let isValid = true;
    const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
    
    inputs.forEach(input => {
        if (!input.value.trim()) {
            showFieldError(input, 'این فیلد الزامی است');
            isValid = false;
        } else {
            clearFieldError(input);
        }
    });
    
    return isValid;
}
```

### اعتبارسنجی سمت سرور

```csharp
protected override void DefineProductValidations()
{
    AddValidation("Title", "Title", "عنوان محصول نمی‌تواند خالی باشد", "Product title cannot be empty", "Title != null && Title.Length > 0");
    AddValidation("Value", "Value", "ارزش دارایی باید مثبت باشد", "Asset value must be positive", "Value >= 0");
}
```

## پشتیبانی از RTL

سیستم به طور کامل از RTL پشتیبانی می‌کند:

```css
[dir="rtl"] .field-input {
    text-align: right;
}

[dir="rtl"] .field-select select {
    background-position: left var(--spacing-sm) center;
    padding-left: 2.5rem;
    padding-right: var(--spacing-md);
}
```

## طراحی ریسپانسیو

```css
@media (max-width: 768px) {
    .field-wrapper--horizontal {
        flex-direction: column;
        align-items: stretch;
    }
    
    .btn-modern {
        width: 100%;
        justify-content: center;
    }
}
```

## Dark Mode

```css
@media (prefers-color-scheme: dark) {
    :root {
        --gray-50: #0f172a;
        --gray-100: #1e293b;
        --gray-200: #334155;
        /* ... */
    }
}
```

## مثال کامل

### 1. تعریف موجودیت

```csharp
[DisplayName("محصول")]
public class Product : BaseCoreAuditableEntity<int>
{
    [DisplayName("عنوان")]
    [MaxLength(100)]
    public string Title { get; set; } = null!;

    [DisplayName("توضیحات")]
    public string? Description { get; set; }

    [DisplayName("قیمت")]
    public decimal Price { get; set; }

    [DisplayName("فعال")]
    public bool IsActive { get; set; } = true;
}
```

### 2. تعریف متادیتا

```csharp
public class ProductDefinition : DefaultCRUDDefinition
{
    public override Type DefinitionEntity => typeof(Product);

    protected override void DefineStructure()
    {
        base.DefineStructure();
        
        // فیلدها
        AddField("Title", "عنوان", "Title", TVariableTypes.String, typeof(string), EntityFieldFlags.Required);
        AddField("Description", "توضیحات", "Description", TVariableTypes.String, typeof(string));
        AddField("Price", "قیمت", "Price", TVariableTypes.Decimal, typeof(decimal));
        AddField("IsActive", "فعال", "Is Active", TVariableTypes.Boolean, typeof(bool));
        
        // اعتبارسنجی
        AddValidation("Title", "Title", "عنوان الزامی است", "Title is required", "Title != null && Title.Length > 0");
        AddValidation("Price", "Price", "قیمت باید مثبت باشد", "Price must be positive", "Price > 0");
    }
}
```

### 3. کنترلر

```csharp
public class ProductController : ModernFormController
{
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return await CreateModern("Club", "Product", "Create");
    }

    [HttpPost]
    public async Task<IActionResult> Create(ElasticObject model)
    {
        return await CreateModern(model, "Club", "Product", "Create");
    }
}
```

### 4. View

```html
@model ElasticObject

<div class="modern-form">
    <div class="modern-page-container">
        <div class="modern-form-content">
            @modernControlsRenderer.Render(controlsRendererData)
        </div>
    </div>
</div>
```

## نکات مهم

1. **سازگاری**: سیستم با کنترل‌های موجود سازگار است
2. **عملکرد**: بهینه‌سازی شده برای عملکرد بهتر
3. **امنیت**: اعتبارسنجی کامل سمت کلاینت و سرور
4. **قابلیت توسعه**: قابل توسعه و سفارشی‌سازی
5. **پشتیبانی**: پشتیبانی کامل از RTL و ریسپانسیو

## مشارکت

برای مشارکت در توسعه این سیستم:

1. Fork کنید
2. Branch جدید ایجاد کنید
3. تغییرات را commit کنید
4. Pull Request ارسال کنید

## مجوز

این پروژه تحت مجوز MIT منتشر شده است.
