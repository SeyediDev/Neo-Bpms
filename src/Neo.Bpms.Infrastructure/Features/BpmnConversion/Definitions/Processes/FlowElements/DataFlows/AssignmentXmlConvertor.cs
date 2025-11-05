using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.DataFlows;

internal static class AssignmentXmlConvertor
{
    internal static void Export(dynamic node, Assignment assignment)
    {
        BaseElementXmlConvertor.Export(node, assignment);
        if (assignment.from != null)
        {
            var fromNode = node.from();
            FormalExpressionXmlConvertor.Export(fromNode, assignment.from as FormalExpression);
        }
        if (assignment.to != null)
        {
            var toNode = node.to();
            FormalExpressionXmlConvertor.Export(toNode, assignment.to as FormalExpression);
        }
    }

    internal static Assignment Import(DataAssociation dataAssociation, ElasticObject element)
    {
        var from = FormalExpressionXmlConvertor.Import(element.GetElement("from"));
        var to = FormalExpressionXmlConvertor.Import(element.GetElement("to"));
        var assignment = new Assignment(dataAssociation, element.GetString("id"), from, to);
        BaseElementXmlConvertor.Import(element, assignment);
        return assignment;
    }
}
