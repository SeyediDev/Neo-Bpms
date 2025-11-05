using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;

public class FormFieldDefinition(string alias)
{
    public string Alias { get; set; } = alias;
    protected List<UIComponentProperty> Properties { get; set; }
    public string CreateFormId { get; set; }
    public string DetailFormId { get; set; }
    public string EditFormId { get; set; }
    public string DeleteFormId { get; set; }
    public string IndexFormId { get; set; }

    public bool HasCreateForm => !string.IsNullOrEmpty(CreateFormId);
    public bool HasEditForm => !string.IsNullOrEmpty(EditFormId);
    public bool HasDeleteForm => !string.IsNullOrEmpty(DeleteFormId);
    public bool HasDetailsForm => !string.IsNullOrEmpty(DetailFormId);
    public bool HasIndexForm => !string.IsNullOrEmpty(IndexFormId);
    public UIComponentProperty GetProperty(eControlPropertyId propertyId)
    {
        return Properties?.FirstOrDefault(p => p.PropertyId == propertyId);
    }

    public string PropertyValue(eControlPropertyId propertyId) => GetProperty(propertyId)?.Value?.ToString();
    public bool PropertyBoolean(eControlPropertyId propertyId) => ConvUtill.ToBoolean(GetProperty(propertyId)?.Value ?? false);

    public void AddProperty(eControlPropertyId propertyId, object value)
    {
        Properties ??= [];
        Properties.Add(new UIComponentProperty { PropertyId = propertyId, Value = value });
    }

    public void SetProperty(eControlPropertyId propertyId, object value)
    {
        UIComponentProperty property = GetProperty(propertyId);
        if (property != null) property.Value = value;
        else AddProperty(propertyId, value);
    }
    public bool CheckProperty(eControlPropertyId propertyId) => PropertyBoolean(propertyId);
    public bool HasProperty(eControlPropertyId propertyId) => GetProperty(propertyId) != null;
}
