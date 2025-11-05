namespace Neo.Bpms.Domain.Models.Attributes.RelationshipAttributes;

/// <summary>
/// مشخص کردن فیلد های ارجاعی
/// </summary>
/// <remarks>
/// مشخص کردن فیلد های ارجاعی
/// </remarks>
/// <param name="sourceField">نام فیلد در مبدا</param>
/// <param name="destFeild">نام فیلد در مقصد</param>
public class AssociationMap(string sourceField, string destFeild) : OAttr_AssociationMap(sourceField, destFeild)
{
}