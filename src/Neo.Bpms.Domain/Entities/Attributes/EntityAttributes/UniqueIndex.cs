namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// تعریف ایندکس دیتا بیسی از نوع یکتا
/// </summary>
/// <remarks>
/// تعریف ایندکس دیتا بیسی از نوع یکتا
/// </remarks>
/// <param name="fields">فیلد های ایندکس یا با استفاده از کلید '#I' بعد از نام فیلد بصورت ستون ها شامل شده در ایندکس</param>
/// <param name="clustered">کلاستر شده است</param>
public class UniqueIndex(string fields, bool clustered = false) : Entity_Index(fields, true, clustered)
{
}