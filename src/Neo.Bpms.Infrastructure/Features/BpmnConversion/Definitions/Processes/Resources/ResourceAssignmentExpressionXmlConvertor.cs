using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class ResourceAssignmentExpressionXmlConvertor
{
    internal static void Export(dynamic node, ResourceRole resourceRole, string @namespace = null)
    {
        if (resourceRole.resourceAssignmentExpression == null) return;
        if (resourceRole.Parent is Lane)
        {
            node.performerAssignmentExpression = (resourceRole.resourceAssignmentExpression.expression as FormalExpression)
                ?.Expression?.ExpressionString;
        }
        else
        {
            var element = node.resourceAssignmentExpression();
            BaseElementXmlConvertor.Export(element, resourceRole.resourceAssignmentExpression, @namespace);
            var expression = element.expression();
            FormalExpressionXmlConvertor.Export(expression,
                resourceRole.resourceAssignmentExpression.expression as FormalExpression, @namespace);
        }
    }

    internal static ResourceAssignmentExpression Import(IResourceRoleContainer resourceRoleContainer, ElasticObject node)
    {
        if (resourceRoleContainer is Lane)
        {
            var formula = node.GetString("performerAssignmentExpression");
            if (formula != null)
                formula = System.Net.WebUtility.HtmlDecode(formula);
            return new ResourceAssignmentExpression(null, node.GetString("id") + ".AssignmentExpression", new FormalExpression(node.GetString("id") + ".AssignmentExpression.expersion", Parser.ParseTree(formula)));
        }
        var element = node.GetElement("resourceAssignmentExpression");
        if (element == null) return null;

        var expressionElement = element.GetElement("expression");
        var formalExpression =
            expressionElement != null ? FormalExpressionXmlConvertor.Import(expressionElement) : null;
        var id = element.GetString("id");
        var resourceAssignmentExpression = new ResourceAssignmentExpression(null, id, formalExpression);
        BaseElementXmlConvertor.Import(element, resourceAssignmentExpression);
        return resourceAssignmentExpression;
    }
}
