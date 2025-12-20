namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class FieldRecognizer(EntityField field)
{
    public string id { get; set; } = field.Id;
    public string name { get; set; } = field.Name;
    public TVariableTypes type { get; set; } = field.FieldType;
    public string associatedNamespaceId { get; set; } = field.AssociationEntity?.DestNamespaceId;
    public string associatedEntityId { get; set; } = field.AssociationEntity?.DestEntityId;
}
