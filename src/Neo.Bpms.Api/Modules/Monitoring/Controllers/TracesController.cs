using Neo.Bpms.Api.Modules.Monitoring.Models;
using Neo.Bpms.Api.Modules.Monitoring.Services;

namespace Neo.Bpms.Api.Modules.Monitoring.Controllers;

/// <summary>
/// API controller for trace data
/// </summary>
[ApiController]
[Route("api/monitoring/traces")]
public class TracesController : ControllerBase
{
    private readonly ITraceStore _store;
    private readonly ILogger<TracesController> _logger;

    public TracesController(ITraceStore store, ILogger<TracesController> logger)
    {
        _store = store;
        _logger = logger;
    }

    /// <summary>
    /// Query traces
    /// </summary>
    [HttpPost("query")]
    public ActionResult<IEnumerable<TraceData>> Query([FromBody] TraceQueryRequest request)
    {
        return Ok(_store.Query(request));
    }

    /// <summary>
    /// Get recent traces
    /// </summary>
    [HttpGet("recent")]
    public ActionResult<IEnumerable<TraceData>> GetRecentTraces([FromQuery] int limit = 100)
    {
        return Ok(_store.GetRecentTraces(limit));
    }

    /// <summary>
    /// Get trace by ID
    /// </summary>
    [HttpGet("{traceId}")]
    public ActionResult<TraceData> GetTrace(string traceId)
    {
        var trace = _store.GetTrace(traceId);
        if (trace == null)
            return NotFound();
        return Ok(trace);
    }

    /// <summary>
    /// Get trace statistics
    /// </summary>
    [HttpGet("stats")]
    public ActionResult<TraceStats> GetStats(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        return Ok(_store.GetStats(from, to));
    }

    /// <summary>
    /// Get all service names
    /// </summary>
    [HttpGet("services")]
    public ActionResult<IEnumerable<string>> GetServiceNames()
    {
        return Ok(_store.GetServiceNames());
    }

    /// <summary>
    /// Get all operation names
    /// </summary>
    [HttpGet("operations")]
    public ActionResult<IEnumerable<string>> GetOperationNames([FromQuery] string? serviceName = null)
    {
        return Ok(_store.GetOperationNames(serviceName));
    }
}

