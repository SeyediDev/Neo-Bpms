using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Iso8601;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Entities.JobScheduling;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the timer event definition.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="durationResolution">The duration resolution</param>
    /// <param name="durationCalculationType">The duration Calculation Type</param>
    /// <param name="durationFormula">The duration.</param>
    /// <param name="timerTimeReference">The time reference</param>
    /// <param name="referenceProperty">The reference property</param>
    /// <returns></returns>
    protected TimerEventDefinition AddTimerDuration(string id,
                                                                    DueTimeDuration.eResolution durationResolution,
                                                                    DueTimeDuration.eCalculationType durationCalculationType,
                                                                    string durationFormula,
                                                                    DueTimeDuration.eTimeReference timerTimeReference,
                                                                    string referenceProperty)
    {
        BpmnExpression durationExpression = null;
        if (durationCalculationType == DueTimeDuration.eCalculationType.Iso8601)
        {
            durationExpression = new FormalExpression(id + ".durationTime",
                                                                    new ExpressionTree { ExpressionString = durationFormula });
        }

        TimerEventDefinition eventDefinition = new(definitions, id, durationResolution,
                                                                        durationCalculationType, durationExpression, durationFormula,
                                                                        timerTimeReference, referenceProperty);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Adds the timer event definition.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="timeDate">The time date.</param>
    /// <returns></returns>
    protected TimerEventDefinition AddTimerDate(string id, string timeDate)
    {
        _ = DateTime.TryParse(timeDate, out DateTime dateTime);
        TimerEventDefinition eventDefinition = new(definitions, id, dateTime);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Adds the timer event definition.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="iso8601Interval">The iso8601Interval</param>
    /// <returns></returns>
    protected TimerEventDefinition AddTimerCycle(string id, string iso8601Interval)
    {
        if (string.IsNullOrEmpty(iso8601Interval))
        {
            throw new Exception($"CycleTimerEvent {id} iso8601Interval is NUll");
        }

        JobSchedule iso = Iso8601Interval.ParseToJobSchedule(iso8601Interval) ?? throw new Exception($"CycleTimerEvent {id} iso8601Interval is not valid");
        TimerEventDefinition eventDefinition = new(definitions, id, iso8601Interval);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }
}
