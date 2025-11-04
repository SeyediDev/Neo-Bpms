namespace Neo.Bpms.Domain.Entities.Attributes.FieldAttributes;

public class Unique : FAttr_Index
{
    public Unique()
    {
        IsUnique = true;
    }
}