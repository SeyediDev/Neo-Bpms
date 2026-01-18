namespace Neo.Bpms.Domain.Models.Attributes.EntityAttributes;

/// <summary>
/// Index Attribute
/// 
/// each entity can have multiple unique or non unique indexes.
/// </summary>
/// <remarks>
/// تعریف INDEX در دیتا بیس برای موجودیت
/// </remarks>
/// <param name="fields">فیلد های ایندکس یا با استفاده از کلید '#I' بعد از نام فیلد بصورت ستون ها شامل شده در ایندکس</param>
/// <param name="isUnique">منحصر به فرد است</param>
/// <param name="clustered">کلاستر شده است</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EntityIndexAttribute(string fields, bool isUnique = false, bool clustered = false) : Attribute
{
    public EntityIndexAttribute(bool isUnique, bool clustered, params string[] fields)
        : this(string.Join(',', fields), isUnique, clustered)
    {
    }
    public EntityIndexAttribute(bool isUnique, params string[] fields)
        : this(string.Join(',', fields), isUnique, false)
    {
    }
    public EntityIndexAttribute(params string[] fields)
        : this(string.Join(',', fields), false, false)
    {
    }

    /// <summary>
    /// Gets or sets the index fields.
    /// </summary>
    /// <value>
    /// The index fields.
    /// </value>
    public string Fields { get; set; } = fields;

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public string Id { get; set; } = fields;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; } = fields;

    public string EnName { get; set; } = fields;

    /// <summary>
    /// Gets or sets a value indicating whether the index [is unique].
    /// </summary>
    /// <value>
    ///   <c>true</c> if the index [is unique]; otherwise, <c>false</c>.
    /// </value>
    public bool IsUnique { get; set; } = isUnique;

    public bool Clustered { get; set; } = clustered;
}
