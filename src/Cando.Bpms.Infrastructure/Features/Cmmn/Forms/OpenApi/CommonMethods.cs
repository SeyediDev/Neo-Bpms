using Microsoft.OpenApi.Models;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.OpenApi;

public static class CommonMethods
{
    public static OpenApiSchema SpecificSchema(string type, string description)
    {
        return new OpenApiSchema { Type = type, Description = description };
    }
}