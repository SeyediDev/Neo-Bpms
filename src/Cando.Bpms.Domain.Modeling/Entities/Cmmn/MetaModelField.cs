namespace Neo.Bpms.Domain.Modeling.Entities.Cmmn;

[NotInMeta]
public abstract class MetaModelFieldDb : BaseCmmnStateBasedEntity
{
    [DisplayNameAndEnName("شناسه موجودیت")]
    public long MetaEntityId;

    [MaxLength(256)]
    [DisplayNameAndEnName("کد فیلد")]
    public string MetaFieldId;

    [MaxLength(256)]
    [DisplayNameAndEnName("نام")]
    [InDisplayString]
    public string Name;
}

[States(typeof(StateBaseEntityId))]
[DisplayNameAndEnName("فیلد موجودیت")]
public class MetaModelField : MetaModelFieldDb
{
    [DisplayNameAndEnName("وضعیت")]
    public EntityStateName State;

    [DisplayNameAndEnName("موجودیت")]
    public MetaModelEntity MetaEntity;
}
