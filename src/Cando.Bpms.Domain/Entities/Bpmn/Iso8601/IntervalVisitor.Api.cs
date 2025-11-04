namespace Neo.Bpms.Domain.Model.Iso8601;

internal sealed partial class IntervalVisitor
{
    public static IntervalVisitor Parse(string interval)
    {
        IntervalVisitor visitor = new(interval);
        visitor.Visit();

        return visitor;
    }
    public bool IsValid { get; private set; }

    public DateTime? StartDateTime { get; private set; }

    public DateTime? EndDateTime { get; private set; }

    public long? Repeat { get; private set; }

    public TimeSpan? Duration { get; private set; }
    public JobSchedule jobSchedule { get; set; }
}
