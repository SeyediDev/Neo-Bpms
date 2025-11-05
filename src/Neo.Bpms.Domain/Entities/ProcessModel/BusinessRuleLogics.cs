namespace Neo.Bpms.Domain.Entities.ProcessModel;

public class BusinessRuleLogics : BaseProcessModelStateBasedEntity
{
    public long BusinessRuleVersionId;
    [DisplayNameAndEnName("Business Rule Version", "Business Rule Version")]
    [WeakEntityAssociation]
    public BusinessRuleVersion BusinessRuleVersion;

    public string Condition;

    public string Logic;

    [MaxLength(256)]
    public string ToField;

    public BusinessRuleLogicTypeId TypeId;
    public BusinessRuleLogicType Type;

    public long AssociationNamespaceId;
    public MetaModelNamespace AssociationNamespace;

    public long AssociationEntityId;
    public MetaModelEntity AssociationEntity;

    [MaxLength(256)]
    public string AssociationField;

}
