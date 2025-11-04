using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class TimerEventDefinitionXmlConvertor
{
    internal static void Export(Event @event, dynamic element, TimerEventDefinition timerEventDefinition)
    {
        if (@event is not CatchEvent) return; //todo throw exception
        if (timerEventDefinition == null) return;
        if (@event is IntermediateCatchEvent)
        {
            var sourceRef = @event.incoming?.FirstOrDefault()?.sourceRef;
            if (sourceRef is EventBasedGateway && (sourceRef.incoming == null || sourceRef.incoming.Count == 0))
            {

            }
        }
        else if (@event is StartEvent)
        {
            if (@event.Parent is SubProcess)
            {

            }
            else if (@event.Parent is Process)
            {

            }
        }

        if (timerEventDefinition.timeDate != null)
        {
            var timeDate = element.timeDate();
            FormalExpressionXmlConvertor.Export(timeDate, timerEventDefinition.timeDate as FormalExpression);
        }
        if (timerEventDefinition.timeDuration != null)
        {
            var timeDuration = element.timeDuration();
            DueTimeDurationXmlConvertor.Export(element, timerEventDefinition);
            FormalExpressionXmlConvertor.Export(timeDuration, timerEventDefinition.timeDuration as FormalExpression);
        }
        if (timerEventDefinition.timeCycle != null)
        {
            var timeCycle = element.timeCycle();
            FormalExpressionXmlConvertor.Export(timeCycle, timerEventDefinition.timeCycle as FormalExpression);
        }

    }

    public static TimerEventDefinition Import(BpmnDefinitions bpmnDefinitions, Event @event, ElasticObject element)
    {
        //todo
        var id = element.GetString("id");
        var timerEventDefinition = new TimerEventDefinition(bpmnDefinitions, id ?? "TimerEventDefinition", element.GetElement("timeDate")?.InternalValue.ToString());
        if (@event is IntermediateCatchEvent)
        {
            var sourceRef = @event.incoming?.FirstOrDefault()?.sourceRef;
            if (sourceRef is EventBasedGateway && (sourceRef.incoming == null || sourceRef.incoming.Count == 0))
            {

            }
        }
        var timeDuration = element.GetElement("timeDuration");
        if (timeDuration != null)
        {
            timerEventDefinition.timeDuration = FormalExpressionXmlConvertor.Import(timeDuration);
            DueTimeDurationXmlConvertor.Import(element, timerEventDefinition, timeDuration.InternalValue?.ToString());
            timerEventDefinition.timeDate = null;
            timerEventDefinition.timeCycle = null;
        }
        var timeCycle = element.GetElement("timeCycle");
        if (timeCycle != null)
        {
            timerEventDefinition.timeCycle = FormalExpressionXmlConvertor.Import(timeCycle);
            timerEventDefinition.timeDate = null;
            timerEventDefinition.timeDuration = null;
        }
        return timerEventDefinition;
    }
}
