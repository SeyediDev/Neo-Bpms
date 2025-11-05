namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// Default Order By attribute for the entity
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EAttr_DefaultOrderBy"/> class.
/// </remarks>
/// <param name="OrderByField">The order by field.</param>
/// <param name="Ascending">if set to <c>true</c> default order by is ascending otherwise default order by is descending.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EAttr_DefaultOrderBy(string OrderByField, bool Ascending = true) : Attribute
{
    public string OrderByField { get; set; } = OrderByField;
    public bool Ascending { get; set; } = Ascending;
}
