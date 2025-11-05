namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FormStructures;

public enum EditTypeId
{
    Save,
    SaveAndNext,
    SaveAndPrev,
    SaveAndDetails,
    SaveAndReturn
}

public enum eCreateType
{
    Save,
    Apply,
    SaveAndReturn,
}

public enum eDeleteType
{
    DelAndReturn,
    DelAndNext,
    DelAndPrev,
}

public class CSSAttribute
{
    public string Name;
    public string Value;
}

public class FormItemPropertiesViewModel
{
    public FormField.Type FormItemType { get; set; }
    public string Id { get; set; }
    public string ParentControlId { get; set; }
    public string Label { get; set; }
    public eControlTypeId ControlTypeId { get; set; }
    public List<UIComponentProperty> Properties { get; set; }
}

public class OtherInputField
{
    public OtherInputField(FormStructRoutines formStructRoutines, EntityField field, FormField.Type formFieldType, bool isFilter)
    {
        InputFieldDefinition = formStructRoutines.NewInputFieldDefinition(field, eControlTypeId.None, formFieldType, isFilter);
    }

    public OtherInputField()
    {
    }

    public InputFieldDefinition InputFieldDefinition { get; }
    public string Html { get; set; }
}

public class TableColumn
{
    public string Alias;
    public string ColumnName;
    //		public string FieldType;
    //public bool IsSorted;
    //public bool Ascending;
    //		public string CurrentFilter;
}

public class ColumnFieldDefinitionComparerBasedOnName : IEqualityComparer<ColumnFieldDefinition>
{
    public bool Equals(ColumnFieldDefinition x, ColumnFieldDefinition y)
    {
        return x?.ColumnTypeName == y?.ColumnTypeName && x?.entityId == y?.entityId &&
               (x?.AssociationName ?? "") == (y?.AssociationName ?? "");
    }

    public int GetHashCode(ColumnFieldDefinition obj)
    {
        return (obj.ColumnTypeName + obj.entityId).GetHashCode();
    }
}



public class EntityLinkId(string namespaceId, string entityId)
{
    public string NamespaceId { get; set; } = namespaceId;
    public string EntityId { get; set; } = entityId;
}
public class FormLinkId : EntityLinkId
{
    public FormLinkId(string namespaceId, string entityId, string formId, string alias)
    : base(namespaceId, entityId)
    {
        FormId = formId;
        Alias = alias;
    }
    public string FormId { get; set; }
    public string Alias { get; set; }
}

public class ProcessCreateFormLinkId : FormLinkId
{
    public ProcessCreateFormLinkId(string namespaceId, string entityId,
        string formId, string alias)
        : base(namespaceId, entityId, formId, alias)
    {
    }

    public string ProcessId { get; set; }
    public string ProcessVersion { get; set; }
    public string TaskId { get; set; }
    public string FormAlias { get; set; }
    public string AssociationFieldId { get; set; }
}


public class FormElementViewModel
{
    public string id { get; set; }
    public List<List<FormElementViewModel>> children { get; set; }
}
