using Neo.Bpms.Api.Modules.Monitoring.Models;
using Neo.Bpms.Api.Modules.Monitoring.Services;

namespace Neo.Bpms.Api.Modules.Monitoring.Controllers;

/// <summary>
/// API controller for log data
/// </summary>
[ApiController]
[Route("api/monitoring/logs")]
public class LogsController : ControllerBase
{
    private readonly ILogStore _store;
    private readonly ILogger<LogsController> _logger;

    public LogsController(ILogStore store, ILogger<LogsController> logger)
    {
        _store = store;
        _logger = logger;
    }

    /// <summary>
    /// Query logs
    /// </summary>
    [HttpPost("query")]
    public ActionResult<IEnumerable<LogEntry>> Query([FromBody] LogQueryRequest request)
    {
        return Ok(_store.Query(request));
    }

    /// <summary>
    /// Get recent logs
    /// </summary>
    [HttpGet("recent")]
    public ActionResult<IEnumerable<LogEntry>> GetRecentLogs(
        [FromQuery] int limit = 100,
        [FromQuery] string? minLevel = null)
    {
        Models.LogLevel? level = null;
        if (!string.IsNullOrEmpty(minLevel) && Enum.TryParse<Models.LogLevel>(minLevel, true, out var parsed))
        {
            level = parsed;
        }
        return Ok(_store.GetRecentLogs(limit, level));
    }

    /// <summary>
    /// Get log statistics
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<LogStats> GetStats(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        return Ok(_store.GetStats(from, to));
    }

    /// <summary>
    /// Get log timeline for charts
    /// </summary>
    [HttpGet("timeline")]
    public ActionResult<LogTimeline> GetTimeline(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int intervalSeconds = 60)
    {
        var fromDate = from ?? DateTime.UtcNow.AddHours(-1);
        var toDate = to ?? DateTime.UtcNow;
        return Ok(_store.GetTimeline(fromDate, toDate, intervalSeconds));
    }

    /// <summary>
    /// Get logs by trace ID
    /// </summary>
    [HttpGet("trace/{traceId}")]
    public ActionResult<IEnumerable<LogEntry>> GetByTraceId(string traceId)
    {
        return Ok(_store.GetByTraceId(traceId));
    }

    /// <summary>
    /// Get distinct source contexts
    /// </summary>
    [HttpGet("sources")]
    public ActionResult<IEnumerable<string>> GetSourceContexts()
    {
        return Ok(_store.GetSourceContexts());
    }
}

