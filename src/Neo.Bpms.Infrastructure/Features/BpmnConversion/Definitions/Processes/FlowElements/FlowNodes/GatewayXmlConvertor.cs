using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.FlowNodes;

internal static class GatewayXmlConvertor
{
    internal static void Export(dynamic process, FlowElement flowElement)
    {
        if (flowElement is not Gateway gateway) return;
        dynamic node = new ElasticObject(flowElement.GetType().Name);
        FlowNodeXmlConvertor.Export(node, gateway);
        node.gatewayDirection = gateway.gatewayDirection;
        switch (gateway.gatewayType)
        {
            case Gateway.eGatewayType.Exclusive:
                node.InternalName = "exclusiveGateway";
                var exclusiveGateway = gateway as ExclusiveGateway;
                if (!string.IsNullOrEmpty(exclusiveGateway?.defaultSequenceFlowId))
                    node["default"] = exclusiveGateway.defaultSequenceFlowId;
                break;
            case Gateway.eGatewayType.Inclusive:
                node.InternalName = "inclusiveGateway";
                var inclusiveGateway = gateway as InclusiveGateway;
                if (!string.IsNullOrEmpty(inclusiveGateway?.defaultSequenceFlowId))
                    node["default"] = inclusiveGateway.defaultSequenceFlowId;
                break;
            case Gateway.eGatewayType.Parallel:
                node.InternalName = "parallelGateway";
                //var parallelGateway = gateway as ParallelGateway;
                break;
            case Gateway.eGatewayType.Complex:
                node.InternalName = "complexGateway";
                var complexGateway = gateway as ComplexGateway;
                var activationCondition = node.activationCondition();
                if (complexGateway?.activationCondition != null)
                    FormalExpressionXmlConvertor.Export(activationCondition, complexGateway.activationCondition);
                if (!string.IsNullOrEmpty(complexGateway?.defaultSequenceFlowId))
                    node["default"] = complexGateway.defaultSequenceFlowId;
                break;
            case Gateway.eGatewayType.EventBased:
                node.InternalName = "eventBasedGateway";
                var eventBasedGateway = gateway as EventBasedGateway;
                node.eventGatewayType = eventBasedGateway?.eventGatewayType.ToString();
                node.instantiate = eventBasedGateway?.instantiate;
                break;
        }

        process.AddElement(node);
    }

    internal static Gateway Import(IFlowElementsContainer flowElementsContainer, string elementName,
        ElasticObject elementNode, ref FlowElement flowElement, string id)
    {
        Gateway gatewayTypeCheckElement = null;
        var gateway = flowElement as Gateway;
        var gatewayDef = ImportGateway(flowElementsContainer, elementName, elementNode,
            gateway, ref gatewayTypeCheckElement);
        if (gatewayTypeCheckElement == null && gateway != null)
        {
            flowElementsContainer.flowElements.Remove(id);
            flowElement = null;
        }
        if (gatewayDef != null)
            BasicImport(elementNode, gatewayDef);
        return gatewayDef;
    }

    private static Gateway ImportGateway(IFlowElementsContainer flowElementsContainer, string elementsKey, ElasticObject elementNode,
        FlowElement flowElement, ref Gateway gatewayTypeCheckElement)
    {
        Gateway gatewayDef = null;
        switch (elementsKey)
        {
            case "exclusiveGateway":
                gatewayTypeCheckElement = flowElement as ExclusiveGateway;
                gatewayDef = ImportExclusiveGateway(flowElementsContainer, elementNode, flowElement as ExclusiveGateway);
                break;
            case "parallelGateway":
                gatewayTypeCheckElement = flowElement as ParallelGateway;
                gatewayDef = ImportParallelGateway(flowElementsContainer, elementNode, flowElement as ParallelGateway);
                break;
            case "inclusiveGateway":
                gatewayTypeCheckElement = flowElement as InclusiveGateway;
                gatewayDef = ImportInclusiveGateway(flowElementsContainer, elementNode, flowElement as InclusiveGateway);
                break;
            case "eventBasedGateway":
                gatewayTypeCheckElement = flowElement as EventBasedGateway;
                gatewayDef = ImportEventBasedGateway(flowElementsContainer, elementNode, flowElement as EventBasedGateway);
                break;
            case "complexGateway":
                gatewayTypeCheckElement = flowElement as ComplexGateway;
                gatewayDef = ImportComplexGateway(flowElementsContainer, elementNode, flowElement as ComplexGateway);
                break;
        }
        return gatewayDef;
    }

    private static void BasicImport(ElasticObject elementNode, GatewayBase gatewayDef)
    {
        FlowNodeXmlConvertor.Import(elementNode, gatewayDef);
        gatewayDef.gatewayDirection = elementNode.GetEnumText("gatewayDirection", GatewayBase.GatewayDirection.Unspecified);
    }

    private static ExclusiveGateway ImportExclusiveGateway(IFlowElementsContainer flowElementsContainer, ElasticObject element, ExclusiveGateway flowElement)
    {
        var id = element.GetString("id");
        var name = element.GetString("name");
        if (flowElement == null)
            flowElement = new ExclusiveGateway(flowElementsContainer, id, name);
        else
            flowElement.Name = name;
        flowElement.defaultSequenceFlowId = element.GetString("default");
        return flowElement;
    }

    private static ParallelGateway ImportParallelGateway(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        ParallelGateway flowElement)
    {
        var id = element.GetString("id");
        var name = element.GetString("name");
        if (flowElement == null)
            flowElement = new ParallelGateway(flowElementsContainer, id, name);
        else
            flowElement.Name = name;
        return flowElement;
    }

    private static InclusiveGateway ImportInclusiveGateway(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        InclusiveGateway flowElement)
    {
        var id = element.GetString("id");
        var name = element.GetString("name");
        if (flowElement == null)
            flowElement = new InclusiveGateway(flowElementsContainer, id, name);
        else
            flowElement.Name = name;
        flowElement.defaultSequenceFlowId = element.GetString("default");
        return flowElement;
    }

    private static EventBasedGateway ImportEventBasedGateway(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        EventBasedGateway flowElement)
    {
        var id = element.GetString("id");
        var name = element.GetString("name");
        var eventGatewayTypeStr = element.GetString("eventGatewayType");
        var eventGatewayType = EventBasedGateway.eEventBasedGatewayType.Exclusive;
        if (!string.IsNullOrEmpty(eventGatewayTypeStr))
            Enum.TryParse(eventGatewayTypeStr, out eventGatewayType);
        var instantiate = element.GetBool("instantiate");
        if (flowElement == null)
            flowElement = new EventBasedGateway(flowElementsContainer, id, name, instantiate, eventGatewayType);
        else
        {
            flowElement.Name = name;
            flowElement.instantiate = instantiate;
            flowElement.eventGatewayType = eventGatewayType;
        }

        return flowElement;
    }

    private static ComplexGateway ImportComplexGateway(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        ComplexGateway flowElement)
    {
        var id = element.GetString("id");
        var name = element.GetString("name");
        var bpmnFormalExpression = FormalExpressionXmlConvertor.Import(element.GetElement("activationCondition"));
        if (flowElement == null)
            flowElement = new ComplexGateway(flowElementsContainer, id, name, bpmnFormalExpression);
        else
        {
            flowElement.Name = name;
            flowElement.activationCondition = bpmnFormalExpression;
        }

        flowElement.defaultSequenceFlowId = element.GetString("default");
        return flowElement;
    }
}