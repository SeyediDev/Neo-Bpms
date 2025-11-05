using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.ioSpecification;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.DataFlows;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes.Events;

internal static class CatchEventXmlConvertor
{
    internal static dynamic Export(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic process, CatchEvent catchEvent)
    {
        dynamic node = null;
        switch (catchEvent.Location)
        {
            case CatchEventLocation.Start:
                node = StartExport(process, catchEvent as StartEvent);
                break;
            case CatchEventLocation.IntermediateCatch:
                node = IntermediateCatchExport(process, catchEvent as IntermediateCatchEvent);
                break;
            case CatchEventLocation.Boundary:
                node = BoundaryExport(process, catchEvent as BoundaryEvent);
                break;
        }
        if (node != null)
            BasicExport(bpmnDefinitions, bpmnElement, node, catchEvent);
        return node;
    }

    internal static Event Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, IFlowElementsContainer flowElementsContainer, string elementsKey, ElasticObject elementNode, Event @event, ref Event eventTypeCheckElement)
    {
        CatchEvent catchEvent = null;
        switch (elementsKey)
        {
            case "startEvent":
                eventTypeCheckElement = @event as StartEvent;
                catchEvent = StartImport(flowElementsContainer, elementNode, @event as StartEvent);
                break;
            case "intermediateCatchEvent":
                eventTypeCheckElement = @event as IntermediateCatchEvent;
                catchEvent = IntermediateCatchImport(flowElementsContainer, elementNode, @event as IntermediateCatchEvent);
                break;
            case "boundaryEvent":
                eventTypeCheckElement = @event as BoundaryEvent;
                catchEvent = BoundaryImport(flowElementsContainer, elementNode, @event as BoundaryEvent);
                break;
        }
        if (catchEvent != null)
            BasicImport(bpmnDefinitions, bpmnElement, elementNode, catchEvent);
        return catchEvent;
    }

    private static void BasicExport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, dynamic node, CatchEvent catchEvent)
    {
        node.parallelMultiple = catchEvent.parallelMultiple;
        DataOutputXmlConvertor.Export(bpmnElement, node, catchEvent);
        OutputSetXmlConvertor.Export(node, catchEvent);
        DataAssociationXmlConvertor.ExportOutputs(bpmnDefinitions, node, catchEvent);
        EventDefinitionXmlConvertor.Export(catchEvent, node, catchEvent.eventDefinitions);
    }

    private static void BasicImport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, ElasticObject element, CatchEvent catchEvent)
    {
        catchEvent.parallelMultiple = element.GetBool("parallelMultiple");
        DataOutputXmlConvertor.Import(bpmnDefinitions, bpmnElement, catchEvent, element);
        OutputSetXmlConvertor.Import(catchEvent, element);
        DataAssociationXmlConvertor.ImportOutputs(bpmnDefinitions, catchEvent, element);
        EventDefinitionXmlConvertor.Import(bpmnDefinitions, element, catchEvent);
    }

    private static dynamic StartExport(dynamic process, StartEvent startDef)
    {
        var startEvent = process.startEvent();
        startEvent.isInterrupting = startDef.isInterrupting;
        return startEvent;
    }

    private static StartEvent StartImport(IFlowElementsContainer flowElementsContainer, ElasticObject element, StartEvent startEvent)
    {
        var isInterrupting = element.GetBool("isInterrupting", true);
        if (startEvent == null)
            startEvent = new StartEvent(flowElementsContainer, "", "", isInterrupting);
        else
            startEvent.isInterrupting = isInterrupting;
        return startEvent;
    }

    private static dynamic BoundaryExport(dynamic process, BoundaryEvent boundaryEvent)
    {
        var node = process.boundaryEvent();
        node.cancelActivity = boundaryEvent.cancelActivity;
        node.attachedToRef = boundaryEvent.attachedToRef;
        return node;
    }

    private static BoundaryEvent BoundaryImport(IFlowElementsContainer flowElementsContainer, ElasticObject element, BoundaryEvent boundaryEvent)
    {
        var cancelActivity = element.GetBool("cancelActivity", true);
        var attachedToRef = element.GetString("attachedToRef");
        if (string.IsNullOrEmpty(attachedToRef))
            throw new Exception("BoundaryEvent must define attached To Reference.");
        if (boundaryEvent == null)
            boundaryEvent = new BoundaryEvent(flowElementsContainer, "", "", attachedToRef, cancelActivity);
        else
        {
            boundaryEvent.cancelActivity = cancelActivity;
            boundaryEvent.attachedToRef = attachedToRef;
        }
        return boundaryEvent;
    }

    // ReSharper disable once UnusedParameter.Local
    private static dynamic IntermediateCatchExport(dynamic process, IntermediateCatchEvent intermediateCatchEvent)
    {
        return process.intermediateCatchEvent();
    }

    // ReSharper disable once UnusedParameter.Local
    private static IntermediateCatchEvent IntermediateCatchImport(IFlowElementsContainer flowElementsContainer, ElasticObject element, IntermediateCatchEvent intermediateCatchEvent)
    {
        // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
        intermediateCatchEvent ??= new IntermediateCatchEvent(flowElementsContainer, "", "");
        return intermediateCatchEvent;
    }

    public static void Validate(BpmnDefinitions bpmnDefinitions, CatchEvent catchEvent)
    {
        if (catchEvent is BoundaryEvent b && catchEvent.TriggerType == Event.eEventType.Error && !b.cancelActivity)
        {
            var txt = "cancel activity property for error boundary event can't set to false in process" + bpmnDefinitions.Id;
            bpmnDefinitions.ErrorInfos.AddError(txt, catchEvent.Id, "14.10.0", txt);
            b.cancelActivity = true;
        }
        DataAssociationXmlConvertor.Validate(bpmnDefinitions, catchEvent);
    }
}
