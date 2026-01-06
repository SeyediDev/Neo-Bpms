using Neo.Bpms.Api.Modules.SmartDashboard.Models;
using Neo.Bpms.Api.Modules.SmartDashboard.Services;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Controllers;

/// <summary>
/// API controller for Smart Dashboard generation
/// </summary>
[ApiController]
[Route("api/smart-dashboard")]
public class SmartDashboardController : ControllerBase
{
    private readonly ISmartDashboardGenerator _dashboardGenerator;
    private readonly IMetricAnalyzer _metricAnalyzer;

    public SmartDashboardController(
        ISmartDashboardGenerator dashboardGenerator,
        IMetricAnalyzer metricAnalyzer)
    {
        _dashboardGenerator = dashboardGenerator;
        _metricAnalyzer = metricAnalyzer;
    }

    /// <summary>
    /// Generate a smart dashboard based on available metrics
    /// </summary>
    /// <param name="request">Dashboard generation options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated dashboard with widgets</returns>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(GeneratedDashboard), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GeneratedDashboard>> GenerateDashboard(
        [FromBody] GenerateDashboardRequest? request,
        CancellationToken cancellationToken)
    {
        request ??= new GenerateDashboardRequest();
        
        var dashboard = await _dashboardGenerator.GenerateDashboardAsync(request, cancellationToken);
        return Ok(dashboard);
    }

    /// <summary>
    /// Preview a dashboard without saving
    /// </summary>
    /// <param name="request">Dashboard generation options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Preview of the generated dashboard</returns>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(GeneratedDashboard), StatusCodes.Status200OK)]
    public async Task<ActionResult<GeneratedDashboard>> PreviewDashboard(
        [FromBody] GenerateDashboardRequest? request,
        CancellationToken cancellationToken)
    {
        request ??= new GenerateDashboardRequest();
        
        var dashboard = await _dashboardGenerator.PreviewDashboardAsync(request, cancellationToken);
        return Ok(dashboard);
    }

    /// <summary>
    /// Get list of available dashboard templates
    /// </summary>
    /// <returns>List of templates</returns>
    [HttpGet("templates")]
    [ProducesResponseType(typeof(IEnumerable<DashboardTemplate>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DashboardTemplate>> GetTemplates()
    {
        var templates = _dashboardGenerator.GetTemplates();
        return Ok(templates);
    }

    /// <summary>
    /// Get a specific template by ID
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <returns>Template details</returns>
    [HttpGet("templates/{templateId}")]
    [ProducesResponseType(typeof(DashboardTemplate), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DashboardTemplate> GetTemplate(string templateId)
    {
        var template = _dashboardGenerator.GetTemplate(templateId);
        
        if (template == null)
            return NotFound(new { message = $"Template '{templateId}' not found" });
        
        return Ok(template);
    }

    /// <summary>
    /// Get analyzed metrics information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of analyzed metrics</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(IEnumerable<AnalyzedMetric>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AnalyzedMetric>>> GetAnalyzedMetrics(
        CancellationToken cancellationToken)
    {
        var metrics = await _metricAnalyzer.AnalyzeAllMetricsAsync(cancellationToken);
        return Ok(metrics);
    }

    /// <summary>
    /// Get analyzed metric by name
    /// </summary>
    /// <param name="metricName">Metric name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Analyzed metric details</returns>
    [HttpGet("metrics/{metricName}")]
    [ProducesResponseType(typeof(AnalyzedMetric), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AnalyzedMetric>> GetAnalyzedMetric(
        string metricName,
        CancellationToken cancellationToken)
    {
        var metric = await _metricAnalyzer.AnalyzeMetricAsync(metricName, cancellationToken);
        
        if (metric == null)
            return NotFound(new { message = $"Metric '{metricName}' not found" });
        
        return Ok(metric);
    }

    /// <summary>
    /// Get quick dashboard with default settings
    /// </summary>
    /// <param name="maxWidgets">Maximum number of widgets (default 8)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated dashboard</returns>
    [HttpGet("quick")]
    [ProducesResponseType(typeof(GeneratedDashboard), StatusCodes.Status200OK)]
    public async Task<ActionResult<GeneratedDashboard>> GetQuickDashboard(
        [FromQuery] int maxWidgets = 8,
        CancellationToken cancellationToken = default)
    {
        var request = new GenerateDashboardRequest
        {
            Name = "داشبورد سریع",
            MaxWidgets = maxWidgets,
            IncludeSystemMetrics = true,
            IncludeApplicationMetrics = true
        };
        
        var dashboard = await _dashboardGenerator.GenerateDashboardAsync(request, cancellationToken);
        return Ok(dashboard);
    }

    /// <summary>
    /// Get widget type recommendations for a metric
    /// </summary>
    /// <param name="metricName">Metric name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Widget recommendations</returns>
    [HttpGet("recommend/{metricName}")]
    [ProducesResponseType(typeof(WidgetRecommendation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WidgetRecommendation>> GetWidgetRecommendation(
        string metricName,
        CancellationToken cancellationToken)
    {
        var metric = await _metricAnalyzer.AnalyzeMetricAsync(metricName, cancellationToken);
        
        if (metric == null)
            return NotFound(new { message = $"Metric '{metricName}' not found" });

        // Get recommendation from recommender
        var recommender = HttpContext.RequestServices.GetRequiredService<IWidgetRecommender>();
        var recommendation = recommender.RecommendWidget(metric);
        
        return Ok(recommendation);
    }
}

