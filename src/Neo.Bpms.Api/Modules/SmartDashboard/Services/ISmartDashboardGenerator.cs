using Neo.Bpms.Api.Modules.SmartDashboard.Models;

namespace Neo.Bpms.Api.Modules.SmartDashboard.Services;

/// <summary>
/// Main service for generating smart dashboards
/// </summary>
public interface ISmartDashboardGenerator
{
    /// <summary>
    /// Generate a smart dashboard based on available metrics
    /// </summary>
    Task<GeneratedDashboard> GenerateDashboardAsync(
        GenerateDashboardRequest request, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get available dashboard templates
    /// </summary>
    IEnumerable<DashboardTemplate> GetTemplates();
    
    /// <summary>
    /// Get a specific template by ID
    /// </summary>
    DashboardTemplate? GetTemplate(string templateId);
    
    /// <summary>
    /// Preview a dashboard generation without saving
    /// </summary>
    Task<GeneratedDashboard> PreviewDashboardAsync(
        GenerateDashboardRequest request, 
        CancellationToken cancellationToken = default);
}

