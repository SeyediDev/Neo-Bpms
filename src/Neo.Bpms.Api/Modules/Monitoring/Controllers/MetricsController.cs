using Neo.Bpms.Api.Modules.Monitoring.Models;
using Neo.Bpms.Api.Modules.Monitoring.Services;

namespace Neo.Bpms.Api.Modules.Monitoring.Controllers;

/// <summary>
/// API controller for metrics data
/// </summary>
[ApiController]
[Route("api/monitoring/metrics")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsStore _store;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(IMetricsStore store, ILogger<MetricsController> logger)
    {
        _store = store;
        _logger = logger;
    }

    /// <summary>
    /// Get all metric definitions
    /// </summary>
    [HttpGet("definitions")]
    public ActionResult<IEnumerable<MetricDefinition>> GetDefinitions()
    {
        return Ok(_store.GetMetricDefinitions());
    }

    /// <summary>
    /// Get metric definition by name
    /// </summary>
    [HttpGet("definitions/{name}")]
    public ActionResult<MetricDefinition> GetDefinition(string name)
    {
        var definition = _store.GetMetricDefinition(name);
        if (definition == null)
            return NotFound();
        return Ok(definition);
    }

    /// <summary>
    /// Query metric data points
    /// </summary>
    [HttpPost("query")]
    public ActionResult<IEnumerable<MetricDataPoint>> Query([FromBody] MetricsQueryRequest request)
    {
        return Ok(_store.Query(request));
    }

    /// <summary>
    /// Get time series data for a metric
    /// </summary>
    [HttpGet("{name}/timeseries")]
    public ActionResult<MetricTimeSeries> GetTimeSeries(
        string name,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int? aggregationIntervalSeconds = null)
    {
        return Ok(_store.GetTimeSeries(name, from, to, null, aggregationIntervalSeconds));
    }

    /// <summary>
    /// Get statistics for a metric
    /// </summary>
    [HttpGet("{name}/stats")]
    public ActionResult<MetricStats> GetStats(
        string name,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var stats = _store.GetStats(name, from, to);
        if (stats == null)
            return NotFound();
        return Ok(stats);
    }

    /// <summary>
    /// Get current system metrics
    /// </summary>
    [HttpGet("system")]
    public ActionResult<SystemMetrics> GetSystemMetrics()
    {
        return Ok(_store.GetSystemMetrics());
    }

    /// <summary>
    /// Get application metrics
    /// </summary>
    [HttpGet("application")]
    public ActionResult<ApplicationMetrics> GetApplicationMetrics()
    {
        return Ok(_store.GetApplicationMetrics());
    }

    /// <summary>
    /// Get all collected metrics with current values - useful for debugging
    /// </summary>
    [HttpGet("all")]
    public ActionResult<object> GetAllMetrics()
    {
        var definitions = _store.GetMetricDefinitions().ToList();
        var result = definitions.Select(d => new 
        {
            Name = d.Name,
            Type = d.Type,
            Unit = d.Unit,
            Description = d.Description,
            Stats = _store.GetStats(d.Name)
        }).OrderByDescending(m => m.Stats?.Count ?? 0).ToList();
        
        return Ok(new 
        { 
            TotalMetrics = result.Count,
            Metrics = result 
        });
    }
}

