namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

/// <summary>
/// سرویس لاگ‌گیری برای پرس‌وجوهای کند در داشبوردها
/// </summary>
public class SlowQueryLogger
{
    private readonly ILogger<SlowQueryLogger> _logger;
    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;

    public SlowQueryLogger(ILogger<SlowQueryLogger> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        // بررسی اینکه آیا در حالت توسعه هستیم یا نه
        var environment = _configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") 
            ?? _configuration.GetValue<string>("Environment", "Production");
        _isDevelopment = environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// لاگ کردن پرس‌وجوی کند
    /// </summary>
    public void LogSlowQuery(string widgetId, string reportConfigId, string reportName, 
        string queryText, long elapsedMilliseconds, long threshold, string? userId = null, string? userName = null)
    {
        if (elapsedMilliseconds < threshold)
            return;

        var logMessage = $@"
╔═══════════════════════════════════════════════════════════════════════════════╗
║                    پرس‌وجوی کند در داشبورد شناسایی شد                        ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║ Widget ID: {widgetId,-60} ║
║ Report Config ID: {reportConfigId,-55} ║
║ Report Name: {reportName,-58} ║
║ Execution Time: {elapsedMilliseconds} ms ({elapsedMilliseconds / 1000.0:F2} seconds) ║
║ User: {userName ?? userId ?? "Unknown",-60} ║
║ Threshold: {threshold} ms                                        ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║ Query Text:                                                                    ║
╠═══════════════════════════════════════════════════════════════════════════════╣
{queryText}
╚═══════════════════════════════════════════════════════════════════════════════╝";

        if (_isDevelopment)
        {
            // در حالت توسعه، همیشه لاگ می‌کنیم
            _logger.LogWarning(logMessage);
        }
        else
        {
            // در حالت production، فقط پرس‌وجوهای کند را لاگ می‌کنیم
            _logger.LogWarning(
                "Slow query detected - Widget: {WidgetId}, Report: {ReportName}, " +
                "Time: {ElapsedMs}ms, User: {UserId}",
                widgetId, reportName, elapsedMilliseconds, userId ?? "Unknown");
        }
    }

    /// <summary>
    /// لاگ کردن timeout پرس‌وجو
    /// </summary>
    public void LogQueryTimeout(string widgetId, string reportConfigId, string reportName,
        int timeoutMs, string? userId = null, string? userName = null)
    {
        var logMessage = $@"
╔═══════════════════════════════════════════════════════════════════════════════╗
║                    پرس‌وجو به دلیل timeout متوقف شد                          ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║ Widget ID: {widgetId,-60} ║
║ Report Config ID: {reportConfigId,-55} ║
║ Report Name: {reportName,-58} ║
║ Timeout: {timeoutMs} ms ({timeoutMs / 1000.0:F2} seconds)                    ║
║ User: {userName ?? userId ?? "Unknown",-60} ║
╚═══════════════════════════════════════════════════════════════════════════════╝";

        _logger.LogError(logMessage);
    }
}

