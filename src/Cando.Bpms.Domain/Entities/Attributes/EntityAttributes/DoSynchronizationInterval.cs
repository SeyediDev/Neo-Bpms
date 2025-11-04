namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;
[AttributeUsage(AttributeTargets.Class)]
public class DoSynchronizationIntervalAttribute(int hour, int minute, int second) : Attribute
{
    public TimeSpan Interval { get; set; } = new TimeSpan(0, hour, minute, second);
}