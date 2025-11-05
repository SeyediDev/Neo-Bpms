using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class FieldRecognizer
{
    public FieldRecognizer(EntityField field)
    {
        id = field.Id;
        name = field.Name;
        type = field.FieldType;
        associatedNamespaceId = field.AssociationEntity?.DestNamespaceId;
        associatedEntityId = field.AssociationEntity?.DestEntityId;
    }
    public string id { get; set; }
    public string name { get; set; }
    public TVariableTypes type { get; set; }
    public string associatedNamespaceId { get; set; }
    public string associatedEntityId { get; set; }
}
