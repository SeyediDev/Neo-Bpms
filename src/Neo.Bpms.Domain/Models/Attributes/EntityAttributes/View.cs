namespace Neo.Bpms.Domain.Models.Attributes.EntityAttributes;

/// <summary>
/// تعریف ویو دیتا بیس
/// </summary>
/// <param name="query">کوئری</param>
/// <param name="isDbQuery">کوئری دییتا بیس است</param>
public class View(string query, bool isDbQuery) : EAttr_View(query, isDbQuery)
{
}