namespace Neo.Bpms.Domain.Models.Attributes.FormAttributes;

/// <summary>
/// Form Input Records Filter Attribute
/// This Attribute is only for Index Forms
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FormAttr_InputRecordsFilter"/> class.
/// </remarks>
/// <param name="filter">The filter.</param>
/// <param name="condition"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class FormAttr_InputRecordsFilter(string filter, string condition = null) : Attribute
{
    public string Filter { get; set; } = filter;
    /// <summary>
    /// Gets or sets the condition. 
    /// 
    /// filter will be applied on records only if condition passed.
    /// </summary>
    /// <value>
    /// The condition.
    /// </value>
    public string Condition { get; set; } = condition;
}
