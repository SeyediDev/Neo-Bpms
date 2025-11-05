namespace Neo.Bpms.Domain.Models.Cmmn.Relationship;

public class EntityRelationMap
{
    public string SourceField { get; set; }
    public string DestField { get; set; }

    public EntityRelationMap Clone()
    {
        return new EntityRelationMap
        {
            SourceField = SourceField,
            DestField = DestField
        };
    }
}