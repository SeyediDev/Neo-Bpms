namespace Neo.Bpms.Domain.Models.Attributes.EntityAttributes;

/// <summary>
/// EAttr_State
/// </summary>
/// <remarks>
/// وضعیت رکورد
/// </remarks>
/// <param name="id">شناسه</param>
/// <param name="name">نام</param>
/// <param name="enName">نام انگلیسی</param>
/// <param name="category">نوع</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
public class EAttr_State(int id, string enName, string name, EntityStateCategory category) : Attribute
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string EnName { get; set; } = enName;

    public EntityStateCategory Category { get; set; } = category;

    /// <summary>
    /// Gets or sets the name of the state field.
    /// </summary>
    /// <value>
    /// The name of the state field (default Name is "State").
    /// </value>
    public string StateFieldName { get; set; } = null;
}
