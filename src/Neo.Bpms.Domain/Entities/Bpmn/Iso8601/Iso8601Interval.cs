using System.Text;
using System.Xml;
using Neo.Bpms.Domain.Model.Iso8601;

namespace Neo.Bpms.Domain.Entities.Bpmn.Iso8601;

public class Iso8601Interval
{
    public DateTime? StartDateTime;
    public DateTime? EndDateTime;
    public long? Repeat;
    public TimeSpan Duration;

    /// <summary>
    /// Converts the ISO 8601 string representation of a interval to its equivalent
    /// Neo <see cref="Iso8601Interval"/> representation.
    /// A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// <param name="interval">The ISO 8601 interval.
    /// </param>
    /// <param name="iso8601Interval">
    /// When the method returns, this parameter is set to the resulting <see cref="Iso8601Interval"/>
    /// if the conversion was successful, or null if the conversion was unsuccessful.</param>
    /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
    public static bool TryParse(string iso8601Interval, out Iso8601Interval interval)
    {
        interval = null;

        IntervalVisitor result = IntervalVisitor.Parse(iso8601Interval);
        if (result.IsValid)
        {
            interval = ConvertResult(result);
        }

        return result.IsValid;
    }

    /// <summary>
    /// Converts the Neo ISO 8601 string representation of a interval to its equivalent
    /// Neo <see cref="JobSchedule"/> representation.
    /// A return value indicates whether the conversion succeeded or failed.
    /// </summary>
    /// <param name="jobSchedule">The Neo ISO 8601 interval.
    /// </param>
    /// <param name="neoIso8601Interval">
    /// When the method returns, this parameter is set to the resulting <see cref="JobSchedule"/>
    /// if the conversion was successful, or null if the conversion was unsuccessful.</param>
    /// <returns><see langword="true"/> if the conversion succeeded; <see langword="false"/> otherwise.</returns>
    public static bool TryParse(string neoIso8601Interval, out JobSchedule jobSchedule)
    {
        jobSchedule = null;
        IntervalVisitor result = IntervalVisitor.Parse(neoIso8601Interval);
        if (result.IsValid)
        {
            jobSchedule = result.jobSchedule;
        }

        return result.IsValid;
    }

    private static Iso8601Interval ConvertResult(IntervalVisitor intervalVisitor)
    {
        return new Iso8601Interval
        {
            Repeat = intervalVisitor.Repeat,
            StartDateTime = intervalVisitor.StartDateTime,
            Duration = intervalVisitor.Duration.Value,
            EndDateTime = intervalVisitor.EndDateTime
        };
    }

    /// <summary>
    /// Converts the ISO 8601 string representation of a interval to its equivalent
    /// Neo <see cref="Iso8601Interval"/> representation.
    /// </summary>
    /// <param name="iso8601Interval">The ISO 8601 interval.</param>
    /// <exception cref="FormatException">The ISO 8601 string was not in the proper format.</exception>
    public static Iso8601Interval Parse(string iso8601Interval)
    {

        return !TryParse(iso8601Interval, out Iso8601Interval result) ? throw new FormatException("ISO 8601 string was not in the proper format.") : result;
    }


    /// <summary>
    /// Converts the Neo ISO 8601 Extended via Neo string representation of a interval to its equivalent
    /// Neo <see cref="JobSchedule"/> representation.
    /// </summary>
    /// <param name="neoIso8601Interval">The Neo ISO 8601 interval.</param>
    /// <exception cref="FormatException">The Neo ISO 8601 string was not in the proper format.</exception>
    public static JobSchedule ParseToJobSchedule(string neoIso8601Interval)
    {
        return !TryParse(neoIso8601Interval, out JobSchedule result)
            ? throw new FormatException("Neo ISO 8601 string was not in the proper format.")
            : result;
    }

    /// <summary>
    /// Formats a <see cref="Iso8601Interval"/> as an ISO 8601 interval string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These are problematic because of things like leap years, Daylight Savings Time, etc.
    /// More information is needed in order to convert to a <see cref="TimeSpan"/>;
    /// for that, a more powerful library such as NodaTime would be appropriate.
    /// </para>
    /// </remarks>
    /// <param name="iso8601Interval">The time span.</param>
    /// <returns>An ISO 8601 string representing the interval in <paramref name="iso8601Interval"/>.</returns>
    public static string Format(Iso8601Interval iso8601Interval)
    {
        if (iso8601Interval == null)
        {
            return string.Empty;
        }

        StringBuilder builder = new();
        if (iso8601Interval.Repeat.HasValue)
        {
            _ = builder.Append($"R{iso8601Interval.Repeat.Value}/");
        }

        if (iso8601Interval.StartDateTime.HasValue)
        {
            _ = builder.Append($"{iso8601Interval.StartDateTime.Value:O}/");
        }

        _ = builder.Append($"P{XmlConvert.ToString(iso8601Interval.Duration)}");
        if (iso8601Interval.EndDateTime.HasValue)
        {
            _ = builder.Append($"/{iso8601Interval.EndDateTime.Value:O}");
        }

        return builder.ToString();
    }
}
