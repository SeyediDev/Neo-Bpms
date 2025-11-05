namespace Neo.Bpms.Domain.Models.Attributes.FieldAndEntityAttributes;

/// <summary>
/// نام در دیتا بیس
/// </summary>
/// <remarks>
/// نام در دیتا بیس
/// </remarks>
/// <param name="dbMap">نام در دیتا بیس</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class DbMapAttribute(string dbMap = null): Attribute
{
    public string DBName { get; set; } = dbMap;
    public string OldDbName { get; set; }
}

public class EntityFieldMap(string fieldId) : DbMapAttribute(fieldId)
{
}

/// <summary>
/// نام در دیتا بیس
/// </summary>
/// <remarks>
/// نام در دیتا بیس
/// </remarks>
/// <param name="oldDbName">نام در دیتا بیس</param>
[AttributeUsage(AttributeTargets.Class|AttributeTargets.Field|AttributeTargets.Property)]
public class OldDbMapAttribute(string oldDbName) : Attribute
{
    public string OldDbName { get; set; } = oldDbName;
}
