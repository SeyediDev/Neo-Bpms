using Microsoft.Extensions.DependencyInjection;

namespace Neo.Bpms.Domain.Expressions.Model;

public static class Extensions
{
    public static IServiceCollection AddExpressionService(this IServiceCollection services)
    {
        services.AddSingleton<IBuiltInFunctionFinder, BuiltInFunctionFinder>();
        return services;
    }

}
