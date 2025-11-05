namespace Neo.Bpms.Domain.Models.Cmmn;

public class BasicField
{
    // [Flags]
    // public enum eFlags
    // {
    // 	None = 0,
    // 	IgnoreIfNull = 0x02,
    // 	//ActualValue = 0x04,
    // 	//English = 0x08,
    // 	//BasicOrderBy = 0x10,
    // 	//BasicOrderByHide = 0x20,
    // 	//NoTitle = 0x40,
    // }
    public bool WithTitle { get; set; }
    public bool IgnoreIfNull { get; set; }

    public string FieldId;
    public string Culture;
    public string Key => FieldId;
    public string OtherFieldThatIgnoreMe { get; set; }
    public bool CheckCulture(string culture)
    {
        return string.IsNullOrEmpty(Culture) ? true : culture == Culture;
    }

    public BasicField Clone()
    {
        return new BasicField
        {
            FieldId = FieldId,
            Culture = Culture,
            WithTitle = WithTitle,
            IgnoreIfNull = IgnoreIfNull,
            OtherFieldThatIgnoreMe = OtherFieldThatIgnoreMe
        };
    }
}