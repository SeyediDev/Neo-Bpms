namespace Neo.Bpms.Api;

/// <summary>
/// Configuration options for Neo.Bpms.Api
/// </summary>
public class NeoBpmsApiOptions
{
    /// <summary>
    /// Enable or disable the Monitoring module
    /// </summary>
    public bool EnableMonitoring { get; set; } = true;

    /// <summary>
    /// Enable or disable the Dashboard module
    /// </summary>
    public bool EnableDashboard { get; set; } = true;

    /// <summary>
    /// Enable or disable the Forms module
    /// </summary>
    public bool EnableForms { get; set; } = true;

    /// <summary>
    /// Enable or disable the Reports module
    /// </summary>
    public bool EnableReports { get; set; } = true;

    /// <summary>
    /// Enable or disable the Process module
    /// </summary>
    public bool EnableProcess { get; set; } = true;

    /// <summary>
    /// API route prefix (default: "api")
    /// </summary>
    public string RoutePrefix { get; set; } = "api";
}

