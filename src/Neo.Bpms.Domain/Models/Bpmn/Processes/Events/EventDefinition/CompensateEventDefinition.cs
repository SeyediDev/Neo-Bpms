namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
/// <summary>
/// Catch in start, event sub process(interrupting), boundary(interrupting)
/// Throw in end, intermediate
/// </summary>
public class CompensateEventDefinition : EventDefinition, ICatchEventDefinition, IThrowEventDefinition
{
    public CompensateEventDefinition(BpmnDefinitions parent, string id)
        : base(parent, id, Event.eEventType.Compensation)
    {
        activityRef = null;
        waitForCompletion = true;
    }

    public CompensateEventDefinition(BpmnDefinitions parent, string id, string activityRef, bool waitForCompletion) :
        base(parent, id, Event.eEventType.Compensation)
    {
        this.activityRef = activityRef;
        this.waitForCompletion = waitForCompletion;
    }

    /// <summary>
    /// For a Start Event:
    /// This Event “catches” the compensation for an Event Sub-Process. No further information is REQUIRED. 
    /// The Event Sub-Process will provide the Id necessary to match the Compensation Event with the Event that threw the compensation,
    /// or the compensation will have been a broadcast.
    /// For an End Event:
    /// The Activity to be compensated MAY be supplied. If an Activity is not supplied, then the compensation is broadcast to all completed Activities 
    /// in the current Sub-Process (if present), or the entire Process instance (if at the global level).
    /// For an Intermediate Throw Event within normal flow:
    /// This “throws” the compensation.
    /// For an Intermediate Event attached to the boundary of an Activity:
    /// This Event “catches” the compensation. No further information is REQUIRED. The Activity the Event is attached to will provide the Id 
    /// necessary to match the Compensation Event with the Event that threw the compensation, or the compensation will have been a broadcast.
    /// </summary>
    public string activityRef;

    /// <summary>
    /// For a throw Compensation Event, this flag determines whether the throw Intermediate Event waits for the triggered compensation to complete (the default),
    /// or just triggers the compensation and immediately continues.
    /// </summary>
    public bool waitForCompletion;
}
