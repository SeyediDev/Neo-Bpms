using Neo.Bpms.Api.Modules.SmartDashboard.Services;
using Neo.Bpms.Api.Modules.EditableGrid;
using Neo.Bpms.Api.Modules.PowerBIDashboard;
using Neo.Bpms.Api.Modules.Version;
using Neo.Endpoint;

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

        // Register monitoring module services
        if (options.EnableMonitoring)
        {
            services.AddNeoMonitoringServices(configuration);
            AddSmartDashboardServices(services);
        }

        // Register editable grid module
        services.AddEditableGridModule();

        // Register PowerBI-like dashboard module
        services.AddPowerBIDashboardModule();

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

        // Map Neo endpoints (includes monitoring controllers)
        endpoints.MapNeoEndpoints();

        // Map API controllers from this assembly
        endpoints.MapControllers();

        return endpoints;
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

