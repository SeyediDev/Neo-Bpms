namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

public class DueTimeDuration
{
    public eCalculationType calculationType { get; set; }

    /// <summary>
    /// property id or number or formula appending by durationCalculationType
    /// </summary>
    public string formula { get; set; }
    public eTimeReference timeReference { get; set; }
    public string referenceProperty { get; set; }

    public eResolution resolution { get; set; }
    public enum eCalculationType
    {
        Iso8601 = 0,
        Number = 1,
        Property = 2,
        Formula = 3
    }

    public enum eResolution
    {
        Second = 0,
        Minute = 1,
        Hour = 2,
        Day = 3
    }

    public enum eTimeReference
    {
        Property = 0,
        ProcessStartTime = 1,
        ActivityStartTime = 2,
        EventStartTime = 3
    }
}

public interface IDueTimeDurationContainer
{
    DueTimeDuration dueDuration { get; set; }
}