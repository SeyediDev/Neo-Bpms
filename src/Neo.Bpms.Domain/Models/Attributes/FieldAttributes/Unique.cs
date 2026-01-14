namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

public class Unique : FieldIndex
{
    public Unique()
    {
        IsUnique = true;
    }
}