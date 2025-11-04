using System.Xml;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
internal class TimerInInstance(ProcessInstance pi, FlowNodeInstance ai, DueTimeDuration dueDuration)
{
    internal bool CheckTimeDuration(DateTime now)
    {
        var trigDuration = CalculateTrigDuration();
        var referenceTime = CalculateReferenceTime();
        var dur = now - referenceTime;
        return dur > trigDuration;
    }

    private DateTime CalculateReferenceTime()
    {
        var referenceTime = DateTime.MaxValue;
        switch (dueDuration?.timeReference)
        {
            case DueTimeDuration.eTimeReference.Property:
                referenceTime = Convert.ToDateTime(GetPropertyValue(dueDuration.referenceProperty));
                break;
            case DueTimeDuration.eTimeReference.ProcessStartTime:
                referenceTime = pi.CreationTime;
                break;
            case DueTimeDuration.eTimeReference.ActivityStartTime:
            case DueTimeDuration.eTimeReference.EventStartTime:
                referenceTime = ai?.CreationTime ?? pi.CreationTime;
                break;
        }

        return referenceTime;
    }

    private TimeSpan CalculateTrigDuration()
    {
        var trigDuration = TimeSpan.MaxValue;
        switch (dueDuration.calculationType)
        {
            case DueTimeDuration.eCalculationType.Iso8601:
                trigDuration = XmlConvert.ToTimeSpan(dueDuration.formula);
                break;
            case DueTimeDuration.eCalculationType.Number:
                var num = Convert.ToInt64(dueDuration.formula);
                switch (dueDuration.resolution)
                {
                    case DueTimeDuration.eResolution.Second:
                        trigDuration = TimeSpan.FromSeconds(num);
                        break;
                    case DueTimeDuration.eResolution.Minute:
                        trigDuration = TimeSpan.FromMinutes(num);
                        break;
                    case DueTimeDuration.eResolution.Hour:
                        trigDuration = TimeSpan.FromHours(num);
                        break;
                    case DueTimeDuration.eResolution.Day:
                        trigDuration = TimeSpan.FromDays(num);
                        break;
                    default:
                        trigDuration = TimeSpan.FromMinutes(num);
                        break;
                }

                break;
            case DueTimeDuration.eCalculationType.Property:
                var value = GetPropertyValue(dueDuration.formula);
                trigDuration = XmlConvert.ToTimeSpan(value?.ToString() ?? "0");
                break;
            case DueTimeDuration.eCalculationType.Formula:
                var expressionInInstance = new ExpressionInInstance(pi, ai, [],
                    Parser.ParseTree(dueDuration.formula));
                var v = expressionInInstance.EvalExpression("CheckTimeDuration");
                trigDuration = XmlConvert.ToTimeSpan(v?.ToString() ?? "0");
                break;
        }

        return trigDuration;
    }

    private object GetPropertyValue(string name)
    {
        object value;
        if (ai != null)
            ai.GetData(name, out value);
        else
            pi.GetData(name, out value);
        return value;
    }
}
