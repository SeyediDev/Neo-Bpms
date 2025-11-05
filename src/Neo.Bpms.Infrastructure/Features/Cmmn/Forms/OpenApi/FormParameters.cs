using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.OpenApi;

internal static class FormParameters
{
    public static IList<OpenApiParameter> Generate(Form form, OperationType operationType, string culture)
    {
        List<OpenApiParameter> parameters = CommonParameters(form, culture);
        if (operationType == OperationType.Get && form.FormType == Form.eFormType.Index)
            IndexFilterParameters(parameters, form, culture);
        if (form.FormType.In(Form.eFormType.Detail, Form.eFormType.Edit, Form.eFormType.VirtualDelete))
            IdPathParameter(parameters);
        return parameters;
    }

    private static List<OpenApiParameter> CommonParameters(Form form, string culture)
    {
        return
        [
            new OpenApiParameter
            {
                Name = "content-type",
                Description = "The media type contained in the request body. Valid value is: application/json.",
                Required = true,
                Schema = new OpenApiSchema {Type = "string", Description = "application/json", Example = new OpenApiString("application/json")},
                In = ParameterLocation.Header
            }
        ];
    }

    private static void IndexFilterParameters(List<OpenApiParameter> openApiParameters, Form form,
        string culture)
    {
        openApiParameters.AddRange(IndexBaseParameters());
        foreach (FormField field in form.formFields.Where(f => f.FieldOrControlType == FormField.Type.FilterField))
        {
            openApiParameters.Add(new OpenApiParameter
            {
                Name = field.Id,
                Description = field.Id,
                Required = field.CheckProperty(eControlPropertyId.Required),
                Schema = field.GenerateOpenApiSchema(culture),
                In = ParameterLocation.Query
            });
        }
    }

    private static void IdPathParameter(List<OpenApiParameter> openApiParameters)
    {
        openApiParameters.Add(new OpenApiParameter
        {
            Name = "Id",
            Description = "Id",
            Required = true,
            Schema = new OpenApiSchema { Type = "integer", Description = "Id", Example = new OpenApiLong(1) },
            In = ParameterLocation.Path
        });
    }

    private static List<OpenApiParameter> IndexBaseParameters()
    {
        return
        [
            new OpenApiParameter
            {
                Name = "sort",
                Description = "Sort",
                Schema = CommonMethods.SpecificSchema("string", "Sort"),
                In = ParameterLocation.Query
            },
            new OpenApiParameter
            {
                Name = "page",
                Description = "Page number",
                Schema = CommonMethods.SpecificSchema("integer", "Page number"),
                In = ParameterLocation.Query
            },
            new OpenApiParameter
            {
                Name = "pageSize",
                Description = "Records per page",
                Schema = CommonMethods.SpecificSchema("integer", "Records per page"),
                In = ParameterLocation.Query
            }
        ];
    }
}