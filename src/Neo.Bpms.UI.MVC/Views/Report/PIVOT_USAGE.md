# راهنمای استفاده از Pivot Table

## دو روش نمایش ماتریس

سیستم از دو روش برای نمایش ماتریس پشتیبانی می‌کند:

### 1. روش سمت سرور (پیش‌فرض)
- استفاده از `MatrixReportRenderer.cs` و `MatrixRoutines.cs`
- پردازش داده‌ها در سمت سرور
- فایل: `_matrix.cshtml`

### 2. روش سمت کلاینت (جدید)
- استفاده از `PivotTable.js` (JavaScript خالص)
- پردازش داده‌ها در مرورگر
- فایل: `_pivotClient.cshtml`

## نحوه انتخاب روش

برای انتخاب روش نمایش، از `ViewBag.UseClientSidePivot` استفاده کنید:

### استفاده از روش سمت کلاینت:

```csharp
ViewBag.UseClientSidePivot = true;
return View("_listView", reportData);
```

### استفاده از روش سمت سرور (پیش‌فرض):

```csharp
ViewBag.UseClientSidePivot = false; // یا اصلاً تنظیم نکنید
return View("_listView", reportData);
```

## مثال کامل در Controller:

```csharp
public IActionResult ShowReport(string reportId, bool useClientSide = false)
{
    var reportData = GetReportData(reportId);
    
    // انتخاب روش نمایش
    ViewBag.UseClientSidePivot = useClientSide;
    
    return View("_listView", reportData);
}
```

## مقایسه دو روش:

| ویژگی | سمت سرور | سمت کلاینت |
|-------|----------|------------|
| سرعت پردازش | کندتر (بستگی به سرور) | سریع‌تر (در مرورگر) |
| بار سرور | بالا | کم |
| قابلیت تغییر ابعاد | نیاز به رفرش | بدون رفرش |
| پشتیبانی از داده‌های بزرگ | بهتر | محدود (بیش از 10000 ردیف کند می‌شود) |
| وابستگی | نیاز به C# | فقط JavaScript |

## توصیه:

- **برای داده‌های کوچک تا متوسط (< 5000 ردیف)**: استفاده از روش سمت کلاینت
- **برای داده‌های بزرگ (> 5000 ردیف)**: استفاده از روش سمت سرور
- **برای نیاز به تغییر داینامیک ابعاد**: استفاده از روش سمت کلاینت

## فعال‌سازی پیش‌فرض:

اگر می‌خواهید روش سمت کلاینت به صورت پیش‌فرض فعال باشد، می‌توانید در Controller یا Action Filter تنظیم کنید:

```csharp
// در Controller
public class ReportController : Controller
{
    public ReportController()
    {
        ViewBag.UseClientSidePivot = true; // پیش‌فرض
    }
}
```

یا در `_ViewStart.cshtml`:

```csharp
@{
    ViewBag.UseClientSidePivot = true; // پیش‌فرض برای همه View ها
}
```

