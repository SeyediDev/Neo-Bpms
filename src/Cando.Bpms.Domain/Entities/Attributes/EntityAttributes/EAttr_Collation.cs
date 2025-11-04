namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EAttr_Collation : Attribute
{
    public string Collation;
    public override string ToString() => Collation;
}