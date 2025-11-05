using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
using Event = Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events.Event;
namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Events;

internal static class EventXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic process, FlowElement flowElement)
    {
        var eventDef = flowElement as Event;
        if (eventDef == null) return;
        dynamic node = null;
        if (eventDef is CatchEvent)
            node = CatchEventXmlConvertor.Export(bpmnDefinitions, bpmnElement, process, eventDef as CatchEvent);
        else if (eventDef is ThrowEvent)
            node = ThrowEventXmlConvertor.Export(bpmnDefinitions, bpmnElement, process, eventDef as ThrowEvent);
        if (node != null)
            BasicExport(bpmnElement, node, eventDef);
    }

    internal static FlowElement Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IFlowElementsContainer flowElementsContainer, string elementName,
        ElasticObject elementNode, ref FlowElement flowElement, string id)
    {
        Event eventTypeCheckElement = null;
        var @event = flowElement as Event;
        var eventDef = CatchEventXmlConvertor.Import(bpmnDefinitions, bpmnElement, flowElementsContainer, elementName, elementNode, @event, ref eventTypeCheckElement)
                       ?? ThrowEventXmlConvertor.Import(bpmnDefinitions, bpmnElement, flowElementsContainer, elementName, elementNode, @event, ref eventTypeCheckElement);
        if (eventTypeCheckElement == null && @event != null)
        {
            flowElementsContainer.flowElements.Remove(id);
            flowElement = null;
        }
        if (eventDef != null)
            BasicImport(bpmnDefinitions, bpmnElement, elementNode, eventDef);
        return eventDef;
    }

    private static void BasicExport(ElasticObject bpmnElement, dynamic node, Event eventDef)
    {
        PropertyXmlConvertor.Export(bpmnElement, node, eventDef);
        FlowNodeXmlConvertor.Export(node, eventDef);
    }

    private static void BasicImport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, ElasticObject element, Event eventDef)
    {
        PropertyXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, eventDef);
        FlowNodeXmlConvertor.Import(element, eventDef);
        Validate(bpmnDefinitions, eventDef);
    }

    internal static void Validate(BpmnDefinitions bpmnDefinitions, Event @event)
    {
        if (@event is CatchEvent)
            CatchEventXmlConvertor.Validate(bpmnDefinitions, @event as CatchEvent);
        if (@event is ThrowEvent)
            ThrowEventXmlConvertor.Validate(bpmnDefinitions, @event as ThrowEvent);
    }
}
