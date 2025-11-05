namespace Neo.Bpms.UI.MVC.ViewModels;

public class PostedRange
{
    public ValueRange.ValueType MinType { get; set; }
    public ValueRange.ValueType MaxType { get; set; }
    public string MinFixedValue { get; set; }
    public string MinColumnValue { get; set; }
    public string MaxFixedValue { get; set; }
    public string MaxColumnValue { get; set; }
    public string Color { get; set; }

    public ValueRange GetRange()
    {
        return new ValueRange
        {
            Color = Color,
            MinType = MinType,
            MaxType = MaxType,
            MinValue = MinType == ValueRange.ValueType.Fixed ? MinFixedValue : MinColumnValue,
            MaxValue = MaxType == ValueRange.ValueType.Fixed ? MaxFixedValue : MaxColumnValue
        };
    }
}