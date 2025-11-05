using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Control;

public class SubTableControlModel
{
    public SubTableControlModel()
    {

    }
    public SubTableControlModel(FormField formField)
    {
        NamespaceId = formField.TableEntity?.NamespaceId;
        EntityId = formField.TableEntityId;
        TableAssociationId = formField.TableAssociationId;
        AssociationId = formField.AssociationId;
        IndexFormSubjectId = formField.Property(eControlPropertyId.Subject);
    }

    public SubTableControlModel(TableDefinition tableDefinition)
    {
        NamespaceId = tableDefinition.NamespaceId;
        EntityId = tableDefinition.EntityId;
        //TableAssociationId = tableDefinition.;
        // todo
        // TableAssociationId = tableDefinition.TableAssociationId;
        // AssociationId = tableDefinition.AssociationId;
        // IndexFormSubjectId = tableDefinition.IndexFormSubjectId;
    }

    public string NamespaceId { get; set; }
    public string EntityId { get; set; }
    public string TableAssociationId { get; set; }
    public string AssociationId { get; set; }
    public string IndexFormSubjectId { get; set; }
}
