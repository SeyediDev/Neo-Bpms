using Microsoft.OpenApi.Models;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.OpenApi;

internal static class FormOpenApiExtensions
{
    public static OpenApiOperation GenerateOpenApiOperation(this Form form, string culture, OperationType operationType)
    {
        return new OpenApiOperation
        {
            Summary = culture == "en" ? form.EnName : form.Name,//bold header
            Description = culture == "en" ? form.EnName : form.Name,//description
            Responses = FormResponse.Generate(form, culture),
            RequestBody = FormRequestBody.Generate(form, operationType, culture),
            Security =
            [
                new OpenApiSecurityRequirement
                {
                {new OpenApiSecurityScheme
                {
                    Name = "Authentication", Description = "Authentication",
                    Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme, Id = "Bearer Authentication"}
                },new List<string>()} }
            ],
            Parameters = FormParameters.Generate(form, operationType, culture)
        };
    }
}