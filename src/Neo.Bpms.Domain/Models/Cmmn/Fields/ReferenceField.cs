namespace Neo.Bpms.Domain.Models.Cmmn.Fields;

public class ReferenceField(EntityField extractField)
{
    public EntityRelationship Relationship { get; set; }
    public EntityField ExtractField { get; set; } = extractField;
    public EntityField ReferencedField { get; set; }
    public EntityField ParentAssociationField { get; set; }

    public ReferenceField Clone()
    {
        return new ReferenceField(ExtractField)
        {
            Relationship = Relationship,
            ReferencedField = ReferencedField,
            ParentAssociationField = ParentAssociationField
        };
    }
}
