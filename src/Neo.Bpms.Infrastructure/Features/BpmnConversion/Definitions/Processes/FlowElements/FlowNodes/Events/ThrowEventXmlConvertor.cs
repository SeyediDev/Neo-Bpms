using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Events;

internal static class ThrowEventXmlConvertor
{
    internal static dynamic Export(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic containerElement, ThrowEvent throwEvent, string eventElementName = null)
    {
        dynamic node = null;
        switch (throwEvent.Location)
        {
            case ThrowEventLocation.IntermediateThrow:
                node = IntermediateThrowExport(containerElement, throwEvent as IntermediateThrowEvent);
                break;
            case ThrowEventLocation.End:
                node = EndExport(containerElement, throwEvent as EndEvent);
                break;
            case ThrowEventLocation.Implicit:
                node = ImplicitExport(containerElement, throwEvent as ImplicitThrowEvent, eventElementName);
                break;
        }
        if (node != null)
            BasicExport(bpmnDefinitions, bpmnElement, node, throwEvent);
        return node;
    }

    internal static Event Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, IFlowElementsContainer flowElementsContainer,
        string elementsKey, ElasticObject element,
        Event @event, ref Event eventTypeCheckElement)
    {
        Event eventDef = null;
        switch (elementsKey)
        {
            case "endEvent":
                eventTypeCheckElement = @event as EndEvent;
                eventDef = EndImport(flowElementsContainer, element, @event as EndEvent);
                break;
            case "intermediateThrowEvent":
                eventTypeCheckElement = @event as IntermediateThrowEvent;
                eventDef = IntermediateThrowImport(flowElementsContainer, element, @event as IntermediateThrowEvent);
                break;
            case "implicitThrowEvent":
                eventTypeCheckElement = @event as ImplicitThrowEvent;
                eventDef = ImplicitImport(flowElementsContainer, element, @event as ImplicitThrowEvent);
                break;
        }
        if (eventDef != null)
            BasicImport(bpmnDefinitions, bpmnElement, element, eventDef as ThrowEvent);
        return eventDef;
    }

    private static void BasicExport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic element, ThrowEvent throwEvent)
    {
        DataInputXmlConvertor.Export(bpmnElement, element, throwEvent);
        InputSetXmlConvertor.Export(element, throwEvent);
        DataAssociationXmlConvertor.ExportInputs(bpmnDefinitions, element, throwEvent);
        EventDefinitionXmlConvertor.Export(throwEvent, element, throwEvent.eventDefinitions);
    }

    private static void BasicImport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, ElasticObject element, ThrowEvent throwEvent)
    {
        DataInputXmlConvertor.Import(bpmnDefinitions, bpmnElement, throwEvent, element);
        InputSetXmlConvertor.Import(throwEvent, element);
        DataAssociationXmlConvertor.ImportInputs(bpmnDefinitions, throwEvent, element);
        EventDefinitionXmlConvertor.Import(bpmnDefinitions, element, throwEvent);
    }

    private static dynamic IntermediateThrowExport(dynamic containerElement, IntermediateThrowEvent intermediateThrowEvent)
    {
        return containerElement.intermediateThrowEvent();
    }

    private static IntermediateThrowEvent IntermediateThrowImport(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        IntermediateThrowEvent intermediateThrowEvent)
    {
        intermediateThrowEvent ??= new IntermediateThrowEvent(flowElementsContainer, "", "");
        return intermediateThrowEvent;
    }

    private static dynamic EndExport(dynamic containerElement, EndEvent endEvent)
    {
        return containerElement.endEvent();
    }

    private static EndEvent EndImport(IFlowElementsContainer flowElementsContainer, ElasticObject element, EndEvent endEvent)
    {
        endEvent ??= new EndEvent(flowElementsContainer, "", "");
        return endEvent;
    }

    private static dynamic ImplicitExport(dynamic containerElement, ImplicitThrowEvent intermediateThrowEvent, string eventElementName)
    {
        if (string.IsNullOrEmpty(eventElementName))
            return containerElement.implicitThrowEvent();
        var element = new ElasticObject(eventElementName);
        return containerElement.AddElement(element);
    }

    private static ImplicitThrowEvent ImplicitImport(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        ImplicitThrowEvent implicitThrowEvent)
    {
        implicitThrowEvent ??= new ImplicitThrowEvent(flowElementsContainer, "", "");
        return implicitThrowEvent;
    }

    public static void Validate(BpmnDefinitions bpmnDefinitions, ThrowEvent throwEvent)
    {
        DataAssociationXmlConvertor.Validate(bpmnDefinitions, throwEvent);
    }
}
