using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

/// <summary>
/// 14.3
/// </summary>
internal static class SequenceFlowXmlConvertor
{
    internal static void Export(dynamic process, FlowElement flowElement)
    {
        var sequenceFlow = flowElement as SequenceFlow;
        var node = process.sequenceFlow();
        FlowElementXmlConvertor.Export(node, flowElement);
        node.sourceRef = sequenceFlow?.sourceRef.Id;
        node.targetRef = sequenceFlow?.targetRef.Id;
        node.isImmediate = sequenceFlow?.isImmediate ?? true;

        if (sequenceFlow?.conditionExpression != null)
        {
            var conditionExpressionNode = node.conditionExpression();
            FormalExpressionXmlConvertor.Export(conditionExpressionNode,
                sequenceFlow.conditionExpression as FormalExpression);
            node.condition = conditionExpressionNode.InternalValue;
        }

        if (sequenceFlow?.StateId > 0)
            node.stateId = sequenceFlow.StateId;
    }

    internal static FlowElement Import(BpmnDefinitions bpmnDefinitions,
        IFlowElementsContainer flowElementsContainer, string elementName,
        ElasticObject elementNode, ref FlowElement flowElement, string id)
    {
        if (elementName != "sequenceFlow") return null;
        var sequenceFlow = flowElement as SequenceFlow;
        var sequenceFlowCheckElement = flowElement as SequenceFlow;
        var sourceRef = flowElementsContainer.GetFlowNode(elementNode.GetString("sourceRef"));
        var targetRef = flowElementsContainer.GetFlowNode(elementNode.GetString("targetRef"));
        if (sourceRef == null || targetRef == null)
        {
            if (sourceRef == null)
                bpmnDefinitions.ErrorInfos.AddFatal($"Could not find sourceRef {elementNode.GetString("sourceRef")}",
                    "sequenceFlow " + elementNode.GetString("id"), "14.3.0",
                    "Invalid element ReferenceId");
            if (targetRef == null)
                bpmnDefinitions.ErrorInfos.AddFatal($"Could not find targetRef {elementNode.GetString("targetRef")}",
                    "sequenceFlow " + elementNode.GetString("id"), "14.3.1",
                    "Invalid element ReferenceId");
            return null;
        }

        var isImmediate = elementNode.GetBool("isImmediate");
        var conditionExpressionElement = elementNode.GetElement("conditionExpression");
        BpmnExpression conditionExpression = null;
        if (conditionExpressionElement != null) //todo FormalExpressionXmlConvertor and BpmnExpressionXmlConvertor
            conditionExpression = FormalExpressionXmlConvertor.Import(conditionExpressionElement);
        else
        {
            var condition = elementNode.GetString("condition");
            if (!string.IsNullOrEmpty(condition))
            {
                condition = System.Net.WebUtility.HtmlDecode(condition);
                conditionExpression = new FormalExpression(id, Parser.ParseTree(condition));
            }
        }

        if (sequenceFlow == null)
            sequenceFlow = new SequenceFlow(flowElementsContainer, id, "", sourceRef, targetRef, conditionExpression, isImmediate);
        else
        {
            sequenceFlow.sourceRef = sourceRef;
            sequenceFlow.targetRef = targetRef;
            sequenceFlow.conditionExpression = conditionExpression;
            sequenceFlow.isImmediate = isImmediate;
        }

        sequenceFlow.StateId = elementNode.GetInteger("stateId", 0);

        if (sequenceFlowCheckElement == null && flowElement != null)
        {
            flowElementsContainer.flowElements.Remove(id);
            flowElement = null;
        }

        FlowElementXmlConvertor.Import(elementNode, sequenceFlow);
        return sequenceFlow;
    }
}