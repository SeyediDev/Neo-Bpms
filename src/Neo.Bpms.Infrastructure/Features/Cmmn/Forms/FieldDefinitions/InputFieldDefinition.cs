using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;

public class InputFieldDefinition(string alias) : FormFieldDefinition(alias)
{
    public string Label => Alias;
    public eControlTypeId ControlType { get; set; }
    public string FieldName { get; set; }
    public string IconField { get; set; }
    public string TooltipField { get; set; }
    public string parentControlId { get; set; }
    public FormField.Type FormFieldType { get; set; }
    public TVariableTypes FieldType { get; set; }
    public List<SBVR> SBVRs { get; set; } = [];

    public string NamespaceId => PropertyValue(eControlPropertyId.NamespaceId);
    public string EntityId => PropertyValue(eControlPropertyId.EntityId);
    public string FormId => PropertyValue(eControlPropertyId.EntityItemId);
    public string FormSubjectId => PropertyValue(eControlPropertyId.Subject);
    public IEnumerable<UIComponentProperty> GetProperties(eControlPropertyId propertyId)
    {
        return Properties?.Where(p => p.Id == propertyId) ?? [];
    }
    public List<UIComponentProperty> GetProperties()
    {
        return Properties;
    }
    public IDictionary<string, string> GetCustomProperties()
    {
        Dictionary<string, string> result = [];
        foreach (UIComponentProperty property in Properties.Where(p => p.Id == eControlPropertyId.CustomProperty))
        {
            string[] itemPair = property.Value?.ToString().Split(':');
            if (itemPair == null || itemPair.Length != 2)
                throw new Exception("Custom properties must follow a key:value format."); // todo DefinitionsException
            result.Add(itemPair[0], itemPair[1]);
        }
        return result;
    }
}
