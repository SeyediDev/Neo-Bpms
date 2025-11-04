namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// تعریف ویو دیتا بیس
/// </summary>
/// <param name="query">کوئری</param>
/// <param name="isDbQuery">کوئری دییتا بیس است</param>
[AttributeUsage(AttributeTargets.Class)]
public class EAttr_View(string query, bool isDbQuery) : Attribute
{
    public string Query { get; set; } = query;
    public bool IsDbQuery { get; set; } = isDbQuery;
}