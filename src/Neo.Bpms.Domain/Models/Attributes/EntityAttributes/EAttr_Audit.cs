namespace Neo.Bpms.Domain.Models.Attributes.EntityAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class EAttr_Audit : Attribute
{
    public bool dontAudit;
}