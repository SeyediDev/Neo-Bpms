namespace Neo.Bpms.Domain.Entities.Attributes.FieldAndEntityAttributes;

public class DisplayNameAndEnName : EFAttr_Id
{
    /// <summary>
    /// </summary>
    /// <param name="name">نام فارسی</param>
    /// <param name="enName">نام لاتین</param>
    public DisplayNameAndEnName(string name, string enName = null)
    {
        Name = name;
        EnName = enName;
    }

    public DisplayNameAndEnName()
    {
    }
}
