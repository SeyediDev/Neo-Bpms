using Neo.Bpms.Api.Modules.Monitoring.Hubs;
using Neo.Bpms.Api.Modules.Monitoring.Services;
using Neo.Bpms.Api.Modules.SmartDashboard.Services;
using Neo.Bpms.Api.Modules.Version;

namespace Neo.Bpms.Api;

/// <summary>
/// Extension methods for registering Neo.Bpms.Api services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add Neo.Bpms.Api services to the service collection
    /// </summary>
    public static IServiceCollection AddNeoBpmsApi(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<NeoBpmsApiOptions>? configureOptions = null)
    {
        // Configure options
        var options = new NeoBpmsApiOptions();
        configureOptions?.Invoke(options);
        services.AddSingleton(options);

        // Configure storage options from configuration
        services.Configure<MonitoringStorageOptions>(
            configuration.GetSection("NeoBpmsApi:Monitoring"));

        // Register monitoring module services
        if (options.EnableMonitoring)
        {
            AddMonitoringServices(services);
            AddSmartDashboardServices(services);
        }

        // Register SignalR
        services.AddSignalR();

        // Register version service
        services.AddSingleton<IVersionService, VersionService>();

        // Add controllers from this assembly
        services.AddControllers()
            .AddApplicationPart(typeof(DependencyInjection).Assembly);

        return services;
    }

    /// <summary>
    /// Map Neo.Bpms.Api endpoints
    /// </summary>
    public static IEndpointRouteBuilder MapNeoBpmsApiEndpoints(
        this IEndpointRouteBuilder endpoints,
        NeoBpmsApiOptions? options = null)
    {
        options ??= endpoints.ServiceProvider.GetService<NeoBpmsApiOptions>() ?? new NeoBpmsApiOptions();

        // Map API controllers from this assembly
        endpoints.MapControllers();

        // Map monitoring hub
        if (options.EnableMonitoring)
        {
            endpoints.MapHub<MonitoringHub>("/hubs/monitoring");
        }

        return endpoints;
    }

    private static void AddMonitoringServices(IServiceCollection services)
    {
        // Register stores as singletons (shared state)
        services.AddSingleton<IMetricsStore, MetricsStore>();
        services.AddSingleton<ITraceStore, TraceStore>();
        services.AddSingleton<ILogStore, LogStore>();

        // Register collectors as hosted services
        services.AddHostedService<MetricsCollector>();
        services.AddHostedService<TraceCollector>();
        services.AddHostedService<MonitoringCleanupService>();
        services.AddHostedService<MonitoringBroadcaster>();
        
        // Register system metrics publisher (built-in metrics)
        services.AddHostedService<SystemMetricsPublisher>();
        
        // Register Serilog monitoring service
        // This adds MonitoringSerilogSink to capture logs
        services.AddHostedService<SerilogMonitoringService>();
    }

    private static void AddSmartDashboardServices(IServiceCollection services)
    {
        // Register Smart Dashboard services
        services.AddSingleton<IMetricAnalyzer, MetricAnalyzer>();
        services.AddSingleton<IWidgetRecommender, WidgetRecommender>();
        services.AddSingleton<ILayoutEngine, LayoutEngine>();
        services.AddSingleton<ISmartDashboardGenerator, SmartDashboardGenerator>();
    }
}

