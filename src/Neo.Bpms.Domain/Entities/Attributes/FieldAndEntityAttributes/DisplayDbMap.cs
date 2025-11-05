namespace Neo.Bpms.Domain.Entities.Attributes.FieldAndEntityAttributes;

public class DisplayDbMap : EFAttr_Id
{
    /// <summary>
    /// نام در نمایش و نام فیلد در پایگاه داده
    /// </summary>
    /// <param name="name">نام فارسی</param>
    /// <param name="dbMap">نام فیلد در پایگاه داده</param>
    public DisplayDbMap(string name, string dbMap = null)
    {
        Name = name;
        DBName = dbMap;
    }

    public DisplayDbMap()
    {
    }
}
