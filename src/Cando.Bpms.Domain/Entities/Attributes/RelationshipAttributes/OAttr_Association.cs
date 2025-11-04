namespace Neo.Bpms.Domain.Entities.Attributes.RelationshipAttributes;

/// <summary>
/// Association attribute
/// 
/// defines additional info for association
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
// ReSharper disable once InconsistentNaming
public class OAttr_Association : Attribute
{
    /// <summary>
    /// تعریف فیلد از نوع ارتباط با جداول دیگر
    /// </summary>
    public OAttr_Association() : this(RelationDeleteUpdateBehavior.Error)
    {
    }

    /// <summary>
    /// تعریف فیلد از نوع ارتباط با جداول دیگر
    /// </summary>
    /// <param name="onDeleteBehaviour">نوع در حذف</param>
    public OAttr_Association(RelationDeleteUpdateBehavior onDeleteBehaviour)
    {
        OnDeleteBehaviour = onDeleteBehaviour;
        OnUpdateBehaviour = onDeleteBehaviour;
    }

    /// <summary>
    /// تعریف فیلد از نوع ارتباط با جداول دیگر
    /// </summary>
    /// <param name="onDeleteBehaviour">رفتار در هنگام حذف</param>
    /// <param name="onUpdateBehaviour">رفتار در هنگام اصلاح</param>
    /// <param name="constraint"></param>
    /// <param name="constraintDbName"></param>
    public OAttr_Association(RelationDeleteUpdateBehavior onDeleteBehaviour,
        RelationDeleteUpdateBehavior onUpdateBehaviour, string constraint = null,
        string constraintDbName = null)
    {
        OnDeleteBehaviour = onDeleteBehaviour;
        OnUpdateBehaviour = onUpdateBehaviour;
        Constraint = constraint;
        ConstraintDbName = constraintDbName;
    }

    public RelationDeleteUpdateBehavior OnDeleteBehaviour { get; set; }
    public RelationDeleteUpdateBehavior OnUpdateBehaviour { get; set; }
    public string Constraint { get; set; }
    public string ConstraintDbName { get; set; }
    public bool Hidden { get; set; }
}
