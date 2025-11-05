using Microsoft.OpenApi.Models;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.OpenApi;

internal static class FormResponse
{
    public static OpenApiResponses Generate(Form form, string culture)
    {
        OpenApiResponses responses = AddCommonResponses();

        switch (form.FormType)
        {
            case Form.eFormType.Index:
                responses.Add("200", AddCommonResponseSchema(form, culture));
                break;
            case Form.eFormType.Create:
                responses.Add("201", new OpenApiResponse { Description = "Created" });
                break;
            case Form.eFormType.Edit:
                responses.Add("200", new OpenApiResponse { Description = "Ok" });
                responses.Add("404", new OpenApiResponse { Description = "Not found" });
                break;
            case Form.eFormType.Detail:
                responses.Add("200", AddCommonResponseSchema(form, culture));
                responses.Add("404", new OpenApiResponse { Description = "Not found" });
                break;
            case Form.eFormType.VirtualDelete:
                responses.Add("204", new OpenApiResponse { Description = "No content" });
                responses.Add("404", new OpenApiResponse { Description = "Not found" });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(form.FormType), form.FormType, null);
        }

        return responses;
    }
    private static OpenApiResponses AddCommonResponses()
    {
        return new OpenApiResponses
        {
            { "400", new OpenApiResponse { Description = "Validation error" } },
            { "401", new OpenApiResponse { Description = "Unauthenticated user" } },
            { "403", new OpenApiResponse { Description = "Unauthorized user" } },
            { "500", new OpenApiResponse { Description = "Server error" } }
        };
    }
    private static OpenApiResponse AddCommonResponseSchema(Form form, string culture)
    {
        return new OpenApiResponse
        {
            Description = "Ok",
            Content = new Dictionary<string, OpenApiMediaType>
            {{"application/json", new OpenApiMediaType
            {
                Schema = GenerateResponseSchema(form, culture)
            }}}
        };
    }
    private static OpenApiSchema GenerateResponseSchema(Form form, string culture)
    {
        if (form.FormType == Form.eFormType.Index)
        {
            OpenApiSchema schema = new() { Type = "object" };

            OpenApiSchema array = CommonMethods.SpecificSchema("array", "rows");
            array.Items = new OpenApiSchema { Type = "object" };
            GenerateResponseSchemaProperties(form, culture, array.Items);

            schema.Properties.Add("Rows", array);
            schema.Properties.Add("TotalCount", CommonMethods.SpecificSchema("integer", "Total records count"));
            schema.Properties.Add("PageSize", CommonMethods.SpecificSchema("integer", "Records per page"));
            return schema;
        }
        else if (form.FormType == Form.eFormType.Detail)
        {
            OpenApiSchema schema = new() { Type = "object" };
            GenerateResponseSchemaProperties(form, culture, schema);
            return schema;
        }
        return null;
    }
    private static void GenerateResponseSchemaProperties(Form form, string culture, OpenApiSchema schema)
    {
        foreach (FormField formField in form.formFields)
        {
            switch (formField.FieldOrControlType)
            {
                case FormField.Type.Field:
                    schema.Properties.Add(formField.Id, formField.GenerateOpenApiSchema(culture, true));
                    break;
                case FormField.Type.ColumnField:
                    schema.Properties.Add(formField.Id, formField.GenerateOpenApiSchema(culture, true));
                    break;
                case FormField.Type.SubTable:
                    //todo
                    break;
                case FormField.Type.FilterField:
                case FormField.Type.Control:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}