namespace Neo.Bpms.Domain.Models.Cmmn.Relationship;

public class BaseExtension : BaseEntity
{
    public BaseExtension()
    {
    }

    public BaseExtension(Entity extendingEntity, Entity extendedEntity, string booleanFieldIdInParentThatPresentMe)
        : base(extendingEntity,
            $"{extendingEntity.Id}.Extended.{extendedEntity.Id}",
            $"{extendingEntity.Id} Extended {extendedEntity.Id}", extendedEntity, null)
    {
        BooleanFieldIdInParentThatPresentMe = booleanFieldIdInParentThatPresentMe;
        if (string.IsNullOrEmpty(BooleanFieldIdInParentThatPresentMe))
            BooleanFieldIdInParentThatPresentMe = $"Is{extendingEntity.Id}";
    }

    public string BooleanFieldIdInParentThatPresentMe { get; set; }
}