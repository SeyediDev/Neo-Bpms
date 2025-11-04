namespace Neo.Bpms.Domain.Model.Iso8601;

internal sealed partial class DurationVisitor
{
    public static DurationVisitor Parse(string duration)
    {
        DurationVisitor visitor = new(duration);
        visitor.Visit();

        return visitor;
    }

    public bool IsValid { get; private set; } = true;

    public double Weeks => weeks;

    public double Days => days;

    public double Hours => hours;

    public double Minutes => minutes;

    public double Seconds => seconds;
}