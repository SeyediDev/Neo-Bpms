using System.Reflection;
using Microsoft.AspNetCore.Hosting;

namespace Neo.Bpms.Api.Modules.Version;

/// <summary>
/// Service to provide application version information
/// </summary>
public interface IVersionService
{
    /// <summary>
    /// Get application version information
    /// </summary>
    VersionInfo GetVersionInfo();
}

/// <summary>
/// Application version information
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// Full version string (e.g., "1.0.0-preview.0")
    /// </summary>
    public string Version { get; set; } = "1.0.0";
    
    /// <summary>
    /// Short version (e.g., "1.0.0")
    /// </summary>
    public string ShortVersion { get; set; } = "1.0.0";
    
    /// <summary>
    /// Assembly version
    /// </summary>
    public string AssemblyVersion { get; set; } = "1.0.0.0";
    
    /// <summary>
    /// File version
    /// </summary>
    public string FileVersion { get; set; } = "1.0.0.0";
    
    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = "Neo BPMS";
    
    /// <summary>
    /// Build date (if available)
    /// </summary>
    public DateTime? BuildDate { get; set; }
    
    /// <summary>
    /// Git commit hash (if available)
    /// </summary>
    public string? GitCommit { get; set; }
    
    /// <summary>
    /// Environment name (Development, Production, etc.)
    /// </summary>
    public string Environment { get; set; } = "Development";
    
    /// <summary>
    /// Machine name
    /// </summary>
    public string MachineName { get; set; } = System.Environment.MachineName;
    
    /// <summary>
    /// .NET runtime version
    /// </summary>
    public string RuntimeVersion { get; set; } = System.Environment.Version.ToString();
}

/// <summary>
/// Implementation of version service
/// </summary>
public class VersionService : IVersionService
{
    private readonly IWebHostEnvironment _environment;
    private readonly VersionInfo _cachedVersionInfo;

    public VersionService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _cachedVersionInfo = LoadVersionInfo();
    }

    public VersionInfo GetVersionInfo() => _cachedVersionInfo;

    private VersionInfo LoadVersionInfo()
    {
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "1.0.0";
        var assemblyVersion = assembly.GetName().Version?.ToString() ?? "1.0.0.0";
        var fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "1.0.0.0";
        var productName = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "Neo BPMS";
        
        // Extract short version (before +)
        var shortVersion = informationalVersion.Split('+')[0];
        
        // Extract git commit (after +)
        string? gitCommit = null;
        if (informationalVersion.Contains('+'))
        {
            gitCommit = informationalVersion.Split('+')[1];
            if (gitCommit.Length > 8)
            {
                gitCommit = gitCommit[..8]; // Short commit hash
            }
        }
        
        // Try to get build date from assembly
        DateTime? buildDate = null;
        try
        {
            var location = assembly.Location;
            if (!string.IsNullOrEmpty(location) && File.Exists(location))
            {
                buildDate = File.GetLastWriteTimeUtc(location);
            }
        }
        catch
        {
            // Ignore errors when getting build date
        }

        return new VersionInfo
        {
            Version = informationalVersion,
            ShortVersion = shortVersion,
            AssemblyVersion = assemblyVersion,
            FileVersion = fileVersion,
            ProductName = productName,
            BuildDate = buildDate,
            GitCommit = gitCommit,
            Environment = _environment.EnvironmentName,
            MachineName = System.Environment.MachineName,
            RuntimeVersion = System.Environment.Version.ToString()
        };
    }
}

