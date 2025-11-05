using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.RootElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Events;

internal static class EventDefinitionXmlConvertor
{
    internal static void Export(Event @event, dynamic node, List<EventDefinition> eventDefinitions)
    {
        foreach (var eventDefinition in eventDefinitions ?? Enumerable.Empty<EventDefinition>())
        {
            Export(@event, node, eventDefinition);
        }
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject node, Event @event)
    {
        List<EventDefinition> eventDefinitions = null;
        foreach (var elements in node.ElementCollections ?? Enumerable.Empty<KeyValuePair<string, List<ElasticObject>>>())
        {
            foreach (var element in elements.Value)
            {
                Import(bpmnDefinitions, @event, element, elements.Key, ref eventDefinitions);
            }
        }
        @event.eventDefinitions = eventDefinitions;
    }

    internal static void Export(Event @event, dynamic node, EventDefinition eventDefinition)
    {
        dynamic element = null;
        switch (eventDefinition.type)
        {
            case Event.eEventType.Cancel:
                element = node.cancelEventDefinition();
                break;
            case Event.eEventType.Compensation:
                element = node.compensateEventDefinition();
                var compensateEventDefinition = eventDefinition as CompensateEventDefinition;
                element.activityRef = compensateEventDefinition?.activityRef;
                element.waitForCompletion = compensateEventDefinition?.waitForCompletion;
                break;
            case Event.eEventType.Condition:
                element = node.conditionalEventDefinition();
                FormalExpressionXmlConvertor.Export(element.condition(),
                    (eventDefinition as ConditionalEventDefinition)?.condition);
                break;
            case Event.eEventType.Error:
                element = node.errorEventDefinition();
                element.errorRef = (eventDefinition as ErrorEventDefinition)?.error?.Id;
                break;
            case Event.eEventType.Escalation:
                element = node.escalationEventDefinition();
                element.escalationRef = (eventDefinition as EscalationEventDefinition)?.escalationRef?.Id;
                break;
            case Event.eEventType.Link:
                element = node.linkEventDefinition();
                var linkEventDefinition = eventDefinition as LinkEventDefinition;
                element.name = linkEventDefinition?.Name;
                foreach (var source in linkEventDefinition?.sources ?? Enumerable.Empty<string>())
                {
                    var sourceElement = element.source();
                    sourceElement.InternalValue = source;
                }
                var targetElement = element.target();
                targetElement.InternalValue = linkEventDefinition?.target;
                break;
            case Event.eEventType.Message:
                element = node.messageEventDefinition();
                var messageEventDefinition = eventDefinition as MessageEventDefinition;
                element.messageRef = messageEventDefinition?.messageRef?.Id;
                if (messageEventDefinition?.operationRef != null)
                {
                    element.operationInterfaceId = messageEventDefinition.operationRef.Interface?.Id;
                    element.operationRef = messageEventDefinition.operationRef.Id;
                }
                break;
            case Event.eEventType.Signal:
                element = node.signalEventDefinition();
                element.signalRef = (eventDefinition as SignalEventDefinition)?.signalRef?.Id;
                break;
            case Event.eEventType.Terminate:
                element = node.terminateEventDefinition();
                break;
            case Event.eEventType.Timer:
                element = node.timerEventDefinition();
                TimerEventDefinitionXmlConvertor.Export(@event, element, eventDefinition as TimerEventDefinition);
                break;
        }

        if (element != null)
        {
            BaseElementXmlConvertor.ExportWithId(@event.Id + (eventDefinition.Id ?? eventDefinition.Name), element, eventDefinition);
        }
    }

    private static void Import(BpmnDefinitions bpmnDefinitions, Event @event, ElasticObject element, string key,
        ref List<EventDefinition> eventDefinitions)
    {
        EventDefinition eventDefinition = null;
        var id = element.GetString("id");
        switch (key)
        {
            case "cancelEventDefinition":
                eventDefinition = new CancelEventDefinition(bpmnDefinitions, id);
                break;
            case "compensateEventDefinition":
                var activityRef = element.GetString("activityRef");
                var waitForCompletion = element.GetBool("waitForCompletion");
                eventDefinition = new CompensateEventDefinition(bpmnDefinitions, id, activityRef, waitForCompletion);
                break;
            case "conditionalEventDefinition":
                eventDefinition = new ConditionalEventDefinition(bpmnDefinitions, id,
                    FormalExpressionXmlConvertor.Import(element.GetElement("condition")));
                break;
            case "errorEventDefinition":
                eventDefinition = new ErrorEventDefinition(bpmnDefinitions, id,
                    ProjectDefinition.Project.GetError(element.GetString("errorRef")));
                break;
            case "escalationEventDefinition":
                eventDefinition = new EscalationEventDefinition(bpmnDefinitions, id,
                    ProjectDefinition.Project.GetEscalation(element.GetString("escalationRef")));
                break;
            case "linkEventDefinition":
                var linkEventDefinition = new LinkEventDefinition(bpmnDefinitions, id, element.GetString("name"));
                eventDefinition = linkEventDefinition;
                foreach (var sourceElement in element.GetElements("source"))
                {
                    linkEventDefinition.sources ??= [];
                    linkEventDefinition.sources.Add(sourceElement.InternalValue?.ToString());
                }
                linkEventDefinition.target = element.GetElement("target")?.InternalValue?.ToString();
                break;
            case "messageEventDefinition":
                var messageRef = ProjectDefinition.Project.GetMessage(element.GetString("messageRef"));
                var operationRef = ProjectDefinition.Project.GetOperation(element.GetString("operationRef"));
                eventDefinition = new MessageEventDefinition(bpmnDefinitions, id, messageRef, operationRef);
                break;
            case "signalEventDefinition":
                eventDefinition = new SignalEventDefinition(bpmnDefinitions, id,
                    ProjectDefinition.Project.GetSignal(element.GetString("signalRef")));
                break;
            case "terminateEventDefinition":
                eventDefinition = new TerminateEventDefinition(bpmnDefinitions, id);
                break;
            case "timerEventDefinition":
                eventDefinition = TimerEventDefinitionXmlConvertor.Import(bpmnDefinitions, @event, element);
                break;
            case "eventDefinitionRef":
                var eventDefinitionRef = element.GetString("id") ?? element.InternalValue?.ToString();
                eventDefinition = bpmnDefinitions.GetRootElement(eventDefinitionRef) as EventDefinition;
                break;
        }
        if (eventDefinition == null) return;
        RootElementXmlConvertor.Import(element, eventDefinition);
        eventDefinitions ??= [];
        eventDefinitions.Add(eventDefinition);
    }
}