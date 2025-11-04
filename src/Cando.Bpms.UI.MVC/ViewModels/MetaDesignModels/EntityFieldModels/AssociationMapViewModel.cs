using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityFieldModels;

public class AssociationMapViewModel
{
    public AssociationMapViewModel()
    {

    }
    public AssociationMapViewModel(EntityRelationMap map)
    {
        sourceField = map.SourceField;
        destField = map.DestField;
    }

    public string sourceField { get; set; }
    public string destField { get; set; }

    public EntityRelationMap ToMap()
    {
        return sourceField == null || destField == null
            ? throw new ValidationException("لازم است فیلد مبدا و مقصد رابطه، تعریف شوند.")
            : new EntityRelationMap
            {
                SourceField = sourceField,
                DestField = destField
            };
    }
}
