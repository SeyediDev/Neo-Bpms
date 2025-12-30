using Microsoft.Extensions.DependencyInjection;
using Neo.Bpms.Api.Modules.PowerBIDashboard.Services;

namespace Neo.Bpms.Api.Modules.PowerBIDashboard;

public static class DependencyInjection
{
    public static IServiceCollection AddPowerBIDashboardModule(this IServiceCollection services)
    {
        services.AddScoped<IDataModelService, DataModelService>();
        return services;
    }
}

