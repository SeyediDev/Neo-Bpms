using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Models.Cmmn.UI.Reports;
using Microsoft.Extensions.Caching.Memory;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Dashboards;

public class DashboardDataRoutines(ReportDataRoutines reportDataRoutines, 
    SlowQueryLogger slowQueryLogger, IConfiguration configuration, IMemoryCache memoryCache)
{
    internal async Task GetDashboardData(
        DashboardData dashboardData, ConfiguredDashboard config,
        ConfiguredReport parentReportConfig, ConfiguredReport.ConfiguredSubReport subReport,
        string parentReportIds, ElasticObject parentFilters,
        string culture, bool forPrint, IdentityUser user, ReportConfigBackupRestore reportConfigBackupRestore,
        CancellationToken cancellationToken = default)
    {
        Dashboard dashboard = config.Dashboard;
        IEnumerable<ConfiguredDashboard.ConfigWidget> widgets = config.Widgets.Where(w => !string.IsNullOrEmpty(w.ReportConfigId));
        SetFilterValuesWithParentDataAndGetIds(parentFilters, dashboardData);
        List<object> parentReportIdList = parentReportIds != null ? [.. parentReportIds.Split(',')] : null;
        
        foreach (ConfiguredDashboard.ConfigWidget widget in widgets)
        {
            ConfiguredDashboard.ConfigDiv div = GetWidgetDiv(config.Divs, widget);
            if (div == null)
                continue;

            UiEntity entity = ProjectDefinition.Project.GetEntity(widget.ReportNamespaceId, widget.ReportEntityId) as UiEntity;
            Report report = entity?.GetReport(widget.ReportId);
            if (report == null) continue;
            ConfiguredReport reportConfig = await reportConfigBackupRestore.GetConfig(report, widget.ReportConfigId);
            if (reportConfig == null) continue;
            int maxRecord = GetWidgetMaxRecord(widget, reportConfig.ViewType);
            ReportData reportData = dashboardData.ReportsData.FirstOrDefault(rd => rd.Structure.ConfigId == reportConfig.ConfigId);
            if (reportData != null)
                continue;

            // خواندن timeout از widget properties (پیش‌فرض: 30 ثانیه)
            int widgetTimeoutMs = GetWidgetTimeoutMs(widget);
            int slowQueryThresholdMs = GetWidgetSlowQueryThresholdMs(widget);
            
            // خواندن زمان کش از widget properties (پیش‌فرض: 10 دقیقه)
            int cacheTimeMinutes = GetWidgetCacheTimeMinutes(widget);
            
            // بررسی کش قبل از اجرای پرس‌وجو
            if (cacheTimeMinutes > 0)
            {
                string cacheKey = GenerateCacheKey(widget, reportConfig, dashboardData.Structure.FilterValues, user, maxRecord, culture, forPrint);
                if (memoryCache.TryGetValue(cacheKey, out ReportData? cachedReportData))
                {
                    if (cachedReportData != null)
                    {
                        cachedReportData.Structure.Name = string.IsNullOrEmpty(reportConfig.Name) ? report.Name : reportConfig.Name;
                        dashboardData.ReportsData.Add(cachedReportData);
                        continue;
                    }
                }
            }

            // اجرای پرس‌وجو با timeout
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            CancellationTokenSource timeoutCts = null;
            try
            {
                timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(widgetTimeoutMs));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                
                reportData = await reportDataRoutines.GetReportData(
                    reportConfig, true, dashboardData.Structure.FilterValues, 1, null,
                    parentReportConfig, subReport, parentReportIdList, culture, forPrint, user, maxRecord,
                    cancellationToken: linkedCts.Token);
                
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                
                // لاگ کردن پرس‌وجوهای کند
                if (reportData.QueryInfo != null && elapsedMs >= slowQueryThresholdMs)
                {
                    // جمع‌آوری متن پرس‌وجوها از QueryInfo
                    var queryTexts = reportData.QueryInfo.GetQueries().ToList();
                    
                    if (queryTexts.Count > 0)
                    {
                        var queryText = string.Join("\n", queryTexts);
                        slowQueryLogger.LogSlowQuery(
                            widget.Id,
                            reportConfig.ConfigId,
                            reportConfig.Name ?? report.Name,
                            queryText,
                            elapsedMs,
                            slowQueryThresholdMs,
                            user?.Id,
                            user?.UserName);
                    }
                }
                
                reportData.Structure.Name = string.IsNullOrEmpty(reportConfig.Name) ? report.Name : reportConfig.Name;
                
                // ذخیره در کش اگر زمان کش تنظیم شده باشد
                if (cacheTimeMinutes > 0)
                {
                    string cacheKey = GenerateCacheKey(widget, reportConfig, dashboardData.Structure.FilterValues, user, maxRecord, culture, forPrint);
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeMinutes),
                        Priority = CacheItemPriority.Normal
                    };
                    memoryCache.Set(cacheKey, reportData, cacheOptions);
                }
                
                dashboardData.ReportsData.Add(reportData);
            }
            catch (OperationCanceledException) when (timeoutCts?.Token.IsCancellationRequested == true)
            {
                stopwatch.Stop();
                
                // لاگ کردن timeout
                slowQueryLogger.LogQueryTimeout(
                    widget.Id,
                    reportConfig.ConfigId,
                    reportConfig.Name ?? report.Name,
                    widgetTimeoutMs,
                    user?.Id,
                    user?.UserName);
                
                // ایجاد ReportData با خطای timeout
                reportData = new ReportData(reportConfig, null, dashboardData.Structure.FilterValues, false)
                {
                    Structure = new ReportStructure()
                };
                reportData.Errors.AddError(
                    $"پرس‌وجو به دلیل طولانی شدن زمان اجرا (بیش از {widgetTimeoutMs / 1000} ثانیه) متوقف شد.",
                    reportConfig.ConfigId,
                    "TIMEOUT",
                    $"Widget: {widget.Id}, Report: {reportConfig.Name ?? report.Name}");
                reportData.Structure.Name = string.IsNullOrEmpty(reportConfig.Name) ? report.Name : reportConfig.Name;
                dashboardData.ReportsData.Add(reportData);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                // ایجاد ReportData با خطا
                reportData = new ReportData(reportConfig, null, dashboardData.Structure.FilterValues, false)
                {
                    Structure = new ReportStructure()
                };
                reportData.Errors.AddError(
                    $"خطا در اجرای پرس‌وجو: {ex.Message}",
                    reportConfig.ConfigId,
                    "ERROR",
                    ex.ToString());
                reportData.Structure.Name = string.IsNullOrEmpty(reportConfig.Name) ? report.Name : reportConfig.Name;
                dashboardData.ReportsData.Add(reportData);
            }
        }

        dashboardData.Structure.Name = string.IsNullOrEmpty(config.Name) ? dashboard.Name : config.Name;
    }

    private static void SetFilterValuesWithParentDataAndGetIds(ElasticObject parentFilters, DashboardData dashboardData)
    {
        if (parentFilters != null)
        {
            if (dashboardData.Structure.FilterValues != null)
            {
                foreach (KeyValuePair<string, ElasticObject> att in parentFilters.Attributes)
                {
                    if (!dashboardData.Structure.FilterValues.GetField(att.Key, out object v) || v == null)
                        dashboardData.Structure.FilterValues.SetField(att.Key, att.Value);
                }
            }
            else dashboardData.Structure.FilterValues = parentFilters;
        }
    }

    private static ConfiguredDashboard.ConfigDiv GetWidgetDiv(List<ConfiguredDashboard.ConfigDiv> divs,
        ConfiguredDashboard.ConfigWidget widget)
    {
        ConfiguredDashboard.ConfigDiv div = divs.FirstOrDefault(d => d.WidgetId == widget.Id);
        if (div == null)
        {
            foreach (ConfiguredDashboard.ConfigDiv parent in divs)
                if (parent.Children != null)
                {
                    ConfiguredDashboard.ConfigDiv child = GetWidgetDiv(parent.Children, widget);
                    if (child != null)
                        return child;
                }
        }

        return div;
    }


    public static int GetWidgetMaxRecord(ConfiguredDashboard.ConfigWidget widget, ReportViewType viewType)
    {
        int maxRecord = widget.GetPropertyValueInt(eControlPropertyId.MaxRecordCount);
        if (maxRecord <= 0)
            maxRecord = viewType == ReportViewType.Chart ? 15 : 5;
        return maxRecord;
    }

    /// <summary>
    /// دریافت timeout ویجت از properties (پیش‌فرض: 30000 میلی‌ثانیه)
    /// </summary>
    private int GetWidgetTimeoutMs(ConfiguredDashboard.ConfigWidget widget)
    {
        // خواندن از widget property
        var timeout = widget.GetPropertyValueInt(eControlPropertyId.WidgetTimeoutMs);
        if (timeout > 0)
            return timeout;

        // خواندن از appsettings
        var appSettingsTimeout = configuration.GetValue<int>("Dashboard:WidgetTimeoutMs", 0);
        if (appSettingsTimeout > 0)
            return appSettingsTimeout;

        // پیش‌فرض
        return 30000; // 30 ثانیه
    }

    /// <summary>
    /// دریافت threshold برای لاگ‌گیری پرس‌وجوهای کند (پیش‌فرض: 5000 میلی‌ثانیه)
    /// </summary>
    private int GetWidgetSlowQueryThresholdMs(ConfiguredDashboard.ConfigWidget widget)
    {
        // خواندن از widget property
        var threshold = widget.GetPropertyValueInt(eControlPropertyId.SlowQueryThresholdMs);
        if (threshold > 0)
            return threshold;

        // خواندن از appsettings
        var appSettingsThreshold = configuration.GetValue<int>("Dashboard:SlowQueryThresholdMs", 0);
        if (appSettingsThreshold > 0)
            return appSettingsThreshold;

        // پیش‌فرض
        return 5000; // 5 ثانیه
    }

    /// <summary>
    /// دریافت زمان کش ویجت از properties (پیش‌فرض: 10 دقیقه)
    /// </summary>
    private int GetWidgetCacheTimeMinutes(ConfiguredDashboard.ConfigWidget widget)
    {
        // خواندن از widget property
        var cacheTime = widget.GetPropertyValueInt(eControlPropertyId.WidgetCacheTimeMinutes);
        if (cacheTime > 0)
            return cacheTime;

        // خواندن از appsettings
        var appSettingsCacheTime = configuration.GetValue<int>("Dashboard:WidgetCacheTimeMinutes", 0);
        if (appSettingsCacheTime > 0)
            return appSettingsCacheTime;

        // پیش‌فرض: 10 دقیقه
        return 10;
    }

    /// <summary>
    /// تولید کلید کش منحصر به فرد برای داده‌های ویجت
    /// </summary>
    private static string GenerateCacheKey(
        ConfiguredDashboard.ConfigWidget widget,
        ConfiguredReport reportConfig,
        ElasticObject filterValues,
        IdentityUser user,
        int maxRecord,
        string culture,
        bool forPrint)
    {
        // ایجاد کلید کش بر اساس تمام پارامترهای مؤثر
        var keyParts = new List<string>
        {
            "DashboardWidget",
            widget.Id,
            reportConfig.ConfigId,
            widget.ReportNamespaceId,
            widget.ReportEntityId,
            widget.ReportId,
            maxRecord.ToString(),
            culture ?? "fa",
            forPrint.ToString(),
            user?.Id ?? "anonymous"
        };

        // اضافه کردن فیلترها به کلید کش
        if (filterValues != null && filterValues.Attributes.Any())
        {
            var filterHash = filterValues.ToString().GetHashCode().ToString();
            keyParts.Add($"Filters:{filterHash}");
        }

        return string.Join(":", keyParts);
    }

    /// <summary>
    /// پاک کردن کش یک ویجت خاص
    /// </summary>
    public void InvalidateWidgetCache(
        ConfiguredDashboard.ConfigWidget widget,
        ConfiguredReport reportConfig,
        ElasticObject filterValues,
        IdentityUser user,
        int maxRecord,
        string culture,
        bool forPrint)
    {
        if (widget == null || reportConfig == null)
            return;

        string cacheKey = GenerateCacheKey(widget, reportConfig, filterValues, user, maxRecord, culture, forPrint);
        memoryCache.Remove(cacheKey);
    }

    /// <summary>
    /// پاک کردن تمام کش‌های مربوط به یک ویجت (بدون در نظر گیری فیلترها)
    /// </summary>
    public void InvalidateAllWidgetCache(ConfiguredDashboard.ConfigWidget widget)
    {
        if (widget == null)
            return;

        // از آنجایی که IMemoryCache نمی‌تواند pattern-based remove کند،
        // این متد فقط برای استفاده در آینده با IDistributedCache است
        // فعلاً باید از InvalidateWidgetCache با پارامترهای مشخص استفاده شود
    }
}
