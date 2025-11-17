namespace Neo.Bpms.Domain.Features.MetaDefinitions.Reports;

/// <summary>
/// Represents a category range for grouping numeric values in reports
/// </summary>
public class CategoryRange
{
    /// <summary>
    /// Display label for this range
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Lower bound of the range (inclusive). Null means open from below.
    /// </summary>
    public double? From { get; set; }

    /// <summary>
    /// Upper bound of the range (inclusive). Null means open from above.
    /// </summary>
    public double? To { get; set; }

    /// <summary>
    /// Creates a closed range [from, to]
    /// </summary>
    public static CategoryRange Closed(double from, double to, string label)
    {
        return new CategoryRange
        {
            From = from,
            To = to,
            Label = label
        };
    }

    /// <summary>
    /// Creates a range open from below (less than 'to')
    /// </summary>
    public static CategoryRange LessThan(double to, string label)
    {
        return new CategoryRange
        {
            From = null,
            To = to,
            Label = label
        };
    }

    /// <summary>
    /// Creates a range open from above (greater than 'from')
    /// </summary>
    public static CategoryRange GreaterThan(double from, string label)
    {
        return new CategoryRange
        {
            From = from,
            To = null,
            Label = label
        };
    }

    /// <summary>
    /// Converts the range to the internal format: "from_to" or "_to" or "from_" or "_"
    /// </summary>
    internal string ToRangeString()
    {
        string fromStr = From.HasValue ? From.Value.ToString() : "";
        string toStr = To.HasValue ? To.Value.ToString() : "";
        return $"{fromStr}_{toStr}";
    }

    /// <summary>
    /// Converts the range to the full category range format: "category,label,from_to"
    /// </summary>
    internal string ToCategoryRangeString(string category)
    {
        return $"{category};{Label};{ToRangeString()}";
    }
}

