namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

/// <summary>
/// Filter definition for record selection (specially for index forms)
/// </summary>
public class EntityFormFilter
{
    public ExpressionTree filter;
    public ExpressionTree condition;
    public string entityId { get; set; }
    public string Association { get; set; }

    public EntityFormFilter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFormFilter"/> class.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="entityId">The entity identifier.</param>
    /// <param name="association"></param>
    public EntityFormFilter(ExpressionTree filter, ExpressionTree condition, string entityId = null,
        string association = null)
    {
        this.entityId = entityId;
        this.filter = filter;
        this.condition = condition;
        Association = association;
    }
}

public class ReportHaving : EntityFormFilter
{
}
