namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// تعریف INDEX در دیتا بیس برای موجودیت
/// </summary>
/// <remarks>
/// تعریف INDEX در دیتا بیس برای موجودیت
/// </remarks>
/// <param name="fields">فیلد های ایندکس یا با استفاده از کلید '#I' بعد از نام فیلد بصورت ستون ها شامل شده در ایندکس</param>
/// <param name="clustered"></param>
public class TableIndex(string fields, bool clustered = false) : Entity_Index(fields, false, clustered)
{
}