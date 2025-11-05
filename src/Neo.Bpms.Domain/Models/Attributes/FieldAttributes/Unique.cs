namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

public class Unique : FAttr_Index
{
    public Unique()
    {
        IsUnique = true;
    }
}