namespace Neo.Bpms.Domain.Models.Attributes.EntityAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EAttr_Collation : Attribute
{
    public string Collation;
    public override string ToString() => Collation;
}