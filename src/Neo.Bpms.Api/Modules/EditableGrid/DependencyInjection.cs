using Neo.Bpms.Api.Modules.EditableGrid.Services;

namespace Neo.Bpms.Api.Modules.EditableGrid;

public static class DependencyInjection
{
    public static IServiceCollection AddEditableGridModule(this IServiceCollection services)
    {
        services.AddScoped<IEditableGridService, EditableGridService>();
        return services;
    }
}

