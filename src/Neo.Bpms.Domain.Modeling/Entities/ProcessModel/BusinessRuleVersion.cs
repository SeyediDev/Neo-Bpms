namespace Neo.Bpms.Domain.Modeling.Entities.ProcessModel;

public class BusinessRuleVersion : BaseProcessModelStateBasedEntity
{
    public long BusinessRuleId;
    [DisplayNameAndEnName("Business Rule", "Business Rule")]
    [WeakEntityAssociation]
    public BusinessRule BusinessRule;

    [MaxLength(256)]
    public string Code;

    [MaxLength(256)]
    public string Name;
}
