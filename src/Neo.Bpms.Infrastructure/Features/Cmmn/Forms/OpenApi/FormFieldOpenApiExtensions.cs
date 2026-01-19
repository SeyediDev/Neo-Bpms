using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.OpenApi;

internal static class FormFieldOpenApiExtensions
{
    public static OpenApiSchema GenerateOpenApiSchema(this FormField formField, string culture, bool isResponse = false)
    {
        string name = formField.GetNameWithCulture(culture);
        switch (formField.Field.FieldType)
        {
            case TVariableTypes.invalid:
                break;
            case TVariableTypes.None:
                break;
            case TVariableTypes.Char:
                break;
            case TVariableTypes.Short:
                break;
            case TVariableTypes.Int:
            case TVariableTypes.Long:
            case TVariableTypes.Decimal:
                return new OpenApiSchema { Type = "integer", Description = name, Example = new OpenApiLong(1) };
            case TVariableTypes.UChar:
                break;
            case TVariableTypes.UShort:
                break;
            case TVariableTypes.ULong:
                break;
            case TVariableTypes.Double:
                return new OpenApiSchema { Type = "number", Description = name, Example = new OpenApiDouble(1.1) };
            case TVariableTypes.String:
            case TVariableTypes.ByteArray:
                return new OpenApiSchema
                {
                    Type = "string",
                    Description = name,
                    MaxLength = formField.Field.MaxLen,
                    Example = new OpenApiString("Example")
                };
            case TVariableTypes.Table:
                break;
            case TVariableTypes.DateTime:
                return new OpenApiSchema { Type = "string", Format = "date-time", Description = name, Example = new OpenApiDateTime(DateTimeOffset.Now) };
            case TVariableTypes.Date:
                return new OpenApiSchema { Type = "string", Format = "date", Description = name, Example = new OpenApiDate(DateTime.UtcNow) };
            case TVariableTypes.StringListItem:
                break;
            case TVariableTypes.HourMinute:
                break;
            case TVariableTypes.StringListBitMask:
                break;
            case TVariableTypes.DayHourMinute:
                break;
            case TVariableTypes.DoubleMinuteSecond:
                break;
            case TVariableTypes.BOOL:
                return new OpenApiSchema { Type = "boolean", Description = name, Example = new OpenApiBoolean(true) };
            case TVariableTypes.DurHourMinute:
                break;
            case TVariableTypes.Link:
                break;
            case TVariableTypes.DateStr:
                break;
            case TVariableTypes.BaseEntity:
                break;
            case TVariableTypes.Association:
                {
                    Entity entity = ProjectDefinition.Project.GetEntityFromType(formField.Field.CSharpType);
                    Enumeration enumeration = ProjectDefinition.Project.GetModel(entity.NamespaceId).GetEnum(entity.Id);
                    IOpenApiAny example;
                    if (isResponse)
                        example = new OpenApiString("Example");
                    else
                        example = new OpenApiLong(1);
                    if (enumeration != null)
                    {
                        (IList<IOpenApiAny> Items, string Description) r = GenerateEnum(name, enumeration, culture);
                        return new OpenApiSchema
                        {
                            Type = isResponse ? "string" : "integer",
                            Example = example,
                            Enum = r.Items,
                            Description = r.Description
                        };
                    }
                    else
                    {
                        return new OpenApiSchema
                        {
                            Type = isResponse ? "string" : "integer",
                            Description = name,
                            Example = example
                        };
                    }
                }
            case TVariableTypes.Composition:
                break;
            case TVariableTypes.ParentEntity:
                break;
            case TVariableTypes.WeakEntityAssociation:
                break;
            case TVariableTypes.BitMaskAssociation:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new Exception();
    }

    private static (IList<IOpenApiAny> Items, string Description) GenerateEnum(string name, Enumeration enumeration,
        string culture)
    {
        List<IOpenApiAny> list = [];
        string result = $"{name}:";
        foreach (EnumerationItem item in enumeration.items.Values)
        {
            list.Add(new OpenApiLong(long.Parse(item.Id)));
            result += $"\r\n\r\n - {item.Id}: {item.GetNameWithCulture(culture)} {item.GetDescription(culture)}";
        }
        return (list, result);
    }
}
