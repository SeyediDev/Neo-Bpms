namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class EAttr_Audit : Attribute
{
    public bool dontAudit;
}