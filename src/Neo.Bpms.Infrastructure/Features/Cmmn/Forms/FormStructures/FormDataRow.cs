using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;

public class FormDataRow
{
    public string Ids { get; set; }
    public string DisplayValue { get; set; }
    public Dictionary<string, object> FieldValues { get; set; }
    public string Style => _properties == null ? null : GetContextualStyle()?.ToString();

    private ContextualStyle? GetContextualStyle()
    {
        foreach (UIComponentProperty property in _properties)
        {
            bool valueAsBoolean = property.GetValueAsBoolean();
            if (property.Id == eControlPropertyId.DangerWhen && valueAsBoolean)
                return ContextualStyle.Danger;
            if (property.Id == eControlPropertyId.WarningWhen && valueAsBoolean)
                return ContextualStyle.Warning;
            if (property.Id == eControlPropertyId.SuccessWhen && valueAsBoolean)
                return ContextualStyle.Success;
            if (property.Id == eControlPropertyId.InfoWhen && valueAsBoolean)
                return ContextualStyle.Info;
            if (property.Id == eControlPropertyId.ActiveWhen && valueAsBoolean)
                return ContextualStyle.Active;
        }

        return null;
    }

    public void AddProperty(eControlPropertyId propertyId, object value)
    {
        _properties ??= [];
        _properties.Add(new UIComponentProperty(propertyId, value));
    }

    private List<UIComponentProperty> _properties;
}
