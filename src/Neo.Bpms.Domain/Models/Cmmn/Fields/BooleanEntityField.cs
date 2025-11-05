namespace Neo.Bpms.Domain.Models.Cmmn.Fields;

public class BooleanEntityField
{
    public BooleanEntityField()
    {
    }

    public string TrueTitle { get; set; }
    public string FalseTitle { get; set; }
    public string AllTitle { get; set; }
    public string NullTitle { get; set; }

    public static BooleanItem GetValueItem(bool recordIsEmpty, object value)
    {
        BooleanItem ToBooleanItem(bool? @bool)
        {
            return @bool == null ? BooleanItem.All :
                @bool.Value ? BooleanItem.True : BooleanItem.False;
        }

        if (value == null)
            return recordIsEmpty ? BooleanItem.All : BooleanItem.False;
        if (value is bool b)
        {
            return ToBooleanItem(b);
        }

        if (int.TryParse(value.ToString(), out int intValue))
        {
            try
            {
                return (BooleanItem)intValue;
            }
            catch
            {
                return BooleanItem.All;
            }
        }
        return value is string ? ToBooleanItem(ToBoolean(value)) : BooleanItem.All;
    }

    private static bool? ToBoolean(object value)
    {
        try
        {
            return string.IsNullOrEmpty(value?.ToString()) || value.ToString() == "0" || value.ToString().ToLower() == "false"
                ? false
                : value.ToString() == "1" || value.ToString().ToLower() == "true"
                ? true
                : value.ToString() == "2" ? null : Convert.ToBoolean(value);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public BooleanEntityField Clone()
    {
        return new BooleanEntityField
        {
            AllTitle = AllTitle,
            FalseTitle = FalseTitle,
            NullTitle = NullTitle,
            TrueTitle = TrueTitle
        };
    }
}

public enum BooleanItem
{
    False = 0,
    True = 1,
    All = 2,
    Null = 3
}
