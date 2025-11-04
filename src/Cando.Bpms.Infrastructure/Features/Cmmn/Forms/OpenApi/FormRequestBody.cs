using Microsoft.OpenApi.Models;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.OpenApi;

internal static class FormRequestBody
{
    public static OpenApiRequestBody Generate(Form form, OperationType operationType, string culture)
    {
        return operationType.In(OperationType.Post, OperationType.Put)
            ? new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {{"application/json", new OpenApiMediaType
                {
                    //Example = new OpenApiString("application/json"),
                    Schema = GenerateRequestBodySchema(form, culture)
                }}},
                Required = true,
            }
            : null;
    }
    private static OpenApiSchema GenerateRequestBodySchema(Form form, string culture)
    {
        OpenApiSchema schema = new()
        {
            Type = "object"
        };
        foreach (FormField formField in form.formFields.Where(f => !f.CheckProperty(eControlPropertyId.ReadOnly)))
        {
            switch (formField.FieldOrControlType)
            {
                case FormField.Type.Field:
                    schema.Properties.Add(formField.Id, formField.GenerateOpenApiSchema(culture));
                    if (formField.CheckProperty(eControlPropertyId.Required))
                        schema.Required.Add(formField.Id);
                    break;
                case FormField.Type.SubTable:
                    //todo
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return schema;
    }
}