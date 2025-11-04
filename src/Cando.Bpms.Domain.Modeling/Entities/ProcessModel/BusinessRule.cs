namespace Neo.Bpms.Domain.Modeling.Entities.ProcessModel;

public class BusinessRule : BaseProcessModelStateBasedEntity
{
    [DisplayNameAndEnName("نام", "Name")]
    public string Name;

    [MaxLength(256)]
    public string Code;

    [DisplayNameAndEnName("نام", "Name")]
    [MaxLength(512)]
    public string? Purpose;

    [DisplayNameAndEnName("نام", "Name")]
    [MaxLength(512)]
    public string? Description;

    public BusinessRuleTypeId BusinessRuleTypeId;
    [DisplayNameAndEnName("نوع", "Type")]
    public BusinessRuleType BusinessRuleType;
}
