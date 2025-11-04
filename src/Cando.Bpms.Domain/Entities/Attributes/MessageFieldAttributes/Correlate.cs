namespace Neo.Bpms.Domain.Entities.Attributes.MessageFieldAttributes;

/// <summary>
/// تطبیق فیلد پیام با مشخصه فرآیند
/// </summary>
public class Correlate : EFAttr_Id
{
    /// <summary>
    /// تطبیق فیلد پیام با مشخصه فرآیند
    /// </summary>
    /// <param name="propertyName">name of property</param>
    public Correlate(string propertyName = null)
    {
        DBName = propertyName;
    }
}