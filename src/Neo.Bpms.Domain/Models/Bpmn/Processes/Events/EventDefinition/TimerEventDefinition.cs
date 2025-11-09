using Neo.Bpms.Domain.Models.JobScheduling;

namespace Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;

/// <summary>
/// Catch in all events
/// </summary>
public class TimerEventDefinition : EventDefinition, ICatchEventDefinition, IDueTimeDurationContainer
{
    /// <summary>
    /// If the trigger is a Timer, then a timeDate MAY be entered. Timer attributes are mutually exclusive and if any of the other Timer attributes is set, 
    /// timeDate MUST NOT be set (if the isExecutable attribute of the Process is set to true). The return type of the attribute timeDate MUST conform to the 
    /// ISO-8601 format for date and time representations.
    /// </summary>
    public BpmnExpression timeDate;

    /// <summary>
    /// If the trigger is a Timer, then a timeCycle MAY be entered. Timer attributes are mutually exclusive and if any of the other Timer attributes is set, timeCycle MUST
    /// NOT be set (if the isExecutable attribute of the Process is set to true). The return type of the attribute timeCycle MUST conform to the ISO-8601 format for
    /// recurring time interval representations.
    /// </summary>
    public BpmnExpression timeCycle;

    /// <summary>
    /// If the trigger is a Timer, then a timeDuration MAY be entered. Timer attributes are mutually exclusive and if any of the other Timer attributes is set, 
    /// timeDuration MUST NOT be set (if the isExecutable attribute of the Process is set to true). 
    /// The return type of the attribute timeDuration MUST conform to the ISO-8601 format for time interval representations.
    /// </summary>
    public BpmnExpression timeDuration;

    public TimerEventDefinition(BpmnDefinitions parent, string id, DueTimeDuration.eResolution durationResolution,
        DueTimeDuration.eCalculationType durationCalculationType,
            BpmnExpression durationExpression, string durationFormula,
        DueTimeDuration.eTimeReference timerTimeReference, string referenceProperty)
        : base(parent, id, Event.eEventType.Timer)
    {
        timerEventType = eTimerEventType.timeDuration;
        timeDuration = durationExpression;
        dueDuration = new DueTimeDuration
        {
            resolution = durationResolution,
            calculationType = durationCalculationType,
            formula = durationFormula,
            timeReference = timerTimeReference,
            referenceProperty = referenceProperty
        };
    }

    public TimerEventDefinition(BpmnDefinitions parent, string id, DateTime timeDate)
        : base(parent, id, Event.eEventType.Timer)
    {
        timerEventType = eTimerEventType.timeDateTime;
        this.timeDate = new FormalExpression($"{id}.timeDate", new ExpressionTree { ExpressionString = timeDate.ToString("O") });
    }

    public TimerEventDefinition(BpmnDefinitions parent, string id, string customizeIso8601Cycle)
        : base(parent, id, Event.eEventType.Timer)
    {
        timeCycle = new FormalExpression($"{id}.timeCycle", new ExpressionTree { ExpressionString = customizeIso8601Cycle });
        timerEventType = eTimerEventType.timeCycle;
    }

    public eTimerEventType timerEventType { get; set; }
    public DueTimeDuration dueDuration { get; set; }
    public JobSchedule jobSchedule { get; set; }

    public long SecondsResolution
    {
        get
        {
            long seconds = 60;
            switch (dueDuration?.resolution)
            {
                case DueTimeDuration.eResolution.Second:
                    seconds = 1;
                    break;
                case DueTimeDuration.eResolution.Minute:
                    seconds = 60;
                    break;
                case DueTimeDuration.eResolution.Hour:
                case DueTimeDuration.eResolution.Day:
                    seconds = 3600;
                    break;
            }

            return seconds;

        }
    }

    public enum eTimerEventType
    {
        timeDateTime,
        timeCycle,
        timeDuration
    }
}
