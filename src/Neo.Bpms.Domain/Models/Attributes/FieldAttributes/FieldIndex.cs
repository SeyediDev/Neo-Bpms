namespace Neo.Bpms.Domain.Models.Attributes.FieldAttributes;

/// <summary>
/// Index Attribute
/// 
/// each entity can have multiple unique or non unique indexes.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class FieldIndex : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating whether the index [is unique].
    /// </summary>
    /// <value>
    ///   <c>true</c> if the index [is unique]; otherwise, <c>false</c>.
    /// </value>
    public bool IsUnique { get; set; }

    public bool Clustered { get; set; }
    public bool IsDescending { get; set; }
}
