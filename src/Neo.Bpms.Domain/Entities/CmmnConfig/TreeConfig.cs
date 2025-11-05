namespace Neo.Bpms.Domain.Entities.CmmnConfig;

[DisplayNameAndEnName("تنظیمات درخت")]
public class TreeConfig: BaseCmmnConfigEntity
{
    public long? ParentId;
    [DisplayNameAndEnName("درخت مافوق")]
    public TreeConfig Parent;

    [DisplayNameAndEnName("ترتیب")]
    public double OrderId;

    [DisplayNameAndEnName("نام")]
    [MaxLength(512)]
    public string Name;

    public TreeNodeTypeId NodeTypeId;
    [DisplayNameAndEnName("نوع")]
    public TreeNodeType NodeType;

    [DisplayNameAndEnName("کد استاندارد")]
    [MaxLength(512)]
    public string StandardCode;

    [DisplayNameAndEnName("Purpose")]
    [MaxLength(512)]
    public string Purpose;

    [DisplayNameAndEnName("Description")]
    [MaxLength(512)]
    public string Description;

    public BusinessRuleTypeId? BusinessRuleTypeId;
}
