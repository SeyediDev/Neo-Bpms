using Neo.Bpms.Infrastructure.Features.Security.Authentication.Core;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.PostForms.ApplyFormsData;

internal class ReformFormData
{
    internal static object ReformLong(object value)
    {
        if (string.IsNullOrEmpty(value?.ToString()))
            value = null;
        return value;
    }

    internal static object ReformText(object value)
    {
        if (value is ElasticObject elasticValue)
            value = elasticValue.ToString();
        if (value is string valueStr)
            value = valueStr.Replace("ي", "ی").Replace("ك", "ک");
        return value;
    }

    internal static object ReformBool(EntityField field, ElasticObject record, object value)
    {
        if (!string.IsNullOrEmpty(field.Boolean?.FalseTitle))
        {
            BooleanItem valueItem =
                BooleanEntityField.GetValueItem(record.Attributes != null && record.Attributes.Count() != 0, value);
            value = valueItem switch
            {
                BooleanItem.True => true,
                BooleanItem.False => false,
                BooleanItem.Null => null,
                _ => null,
            };
        }

        return value;
    }

    internal static object ReformTimeSpan(FormField formField, object value)
    {
        return formField == null || !formField.ControlTypeId.In(eControlTypeId.DurationInput, eControlTypeId.None,
             eControlTypeId.TimeInput)
            ? value
            : ReformTimeSpanValueToTicks(value);
    }

    internal static object ReformTimeSpanValueToTicks(object value)
    {
        string v = value?.ToString() ?? "";
        if (string.IsNullOrEmpty(v))
            return null;
        TimeSpan timeSpan = ConvUtill.FetchTimeSpan(v);
        return timeSpan.Ticks;
    }
    internal static object ReformPassword(FormField formField, string value)
    {
        return formField == null || !formField.ControlTypeId.In(eControlTypeId.TextInput, eControlTypeId.None)
            ? (object)value
            : new PasswordHasher().HashPassword(value);
    }
}
