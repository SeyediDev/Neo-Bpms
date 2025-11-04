using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.ResourceAssignment;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

/// <summary>
/// 14.7
/// </summary>
internal static class ResourceParameterBindingXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions, dynamic node,
        ResourceRole resourceRole, string @namespace)
    {
        foreach (var resourceParameterBinding in resourceRole.resourceParameterBindings ??
                                                 Enumerable.Empty<ResourceParameterBinding>())
        {
            var element = resourceRole.Parent is Lane ? node.performerParameterBinding() : node.resourceParameterBinding();
            BaseElementXmlConvertor.Export(element, resourceParameterBinding, @namespace,
                @namespace != "teta");
            element.parameterRef = resourceParameterBinding.parameterRef?.Id;
            if (resourceRole.Parent is Lane)
                element.parameterBindingExpression = (resourceParameterBinding.expression as FormalExpression)?.Expression?.ExpressionString;
            else
            {
                var expressionElement = element.expression();
                element.parameterBindingHelper = resourceParameterBinding.parameterRef?.Name ?? resourceParameterBinding.parameterRef?.Id;
                FormalExpressionXmlConvertor.Export(expressionElement, resourceParameterBinding.expression, @namespace);
            }
        }
        Validate(bpmnDefinitions, resourceRole);
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, IResourceRoleContainer resourceRoleContainer, ElasticObject node, ResourceRole resourceRole)
    {
        resourceRole.resourceParameterBindings = null;
        var resourceParameterBindingsElements = resourceRoleContainer is Lane
            ? node.GetElements("performerParameterBinding")
            : node.GetElements("resourceParameterBinding");
        foreach (var element in resourceParameterBindingsElements ?? Enumerable.Empty<ElasticObject>())
        {
            resourceRole.resourceParameterBindings ??= [];
            var id = element.GetString("id");
            var parameterRef = element.GetString("parameterRef");
            var resourceParameter = resourceRole.resourceRef?.resourceParameters?.FirstOrDefault(p => p.Id == parameterRef);
            var parameterBindingExpression = System.Net.WebUtility.HtmlDecode(element.GetString("parameterBindingExpression"));
            var f = resourceRoleContainer is Lane
                ? new FormalExpression(element.GetString("id") + ".AssignmentExpression.expersion", Parser.ParseTree(parameterBindingExpression))
                : FormalExpressionXmlConvertor.Import(element.GetElement("expression"));
            var resourceParameterBinding = new ResourceParameterBinding(resourceRole, id, resourceParameter, f);
            BaseElementXmlConvertor.Import(element, resourceParameterBinding);
            resourceRole.resourceParameterBindings.Add(resourceParameterBinding);
        }
        Validate(bpmnDefinitions, resourceRole);
    }

    internal static void Validate(BpmnDefinitions bpmnDefinitions, ResourceRole resourceRole)
    {
        var p1 = resourceRole.resourceRef?.resourceParameters?.Count(p => p.isRequired) ?? 0;
        var p2 = resourceRole.resourceParameterBindings?.Count(b => b.parameterRef?.isRequired ?? false) ?? 0;
        if (p1 != p2)
            bpmnDefinitions.ErrorInfos.AddWarning(
                "Some required parameters are not specified.",
                $"{resourceRole.Name ?? resourceRole.resourceRef?.Name}", "14.7.0", "", eWarningLevel.WarningLevel0);
        foreach (var resourceParameterBinding in resourceRole.resourceParameterBindings ?? Enumerable.Empty<ResourceParameterBinding>())
        {
            if (resourceParameterBinding.parameterRef == null)
                bpmnDefinitions.ErrorInfos.AddWarning(
                    "Parameter not defined.",
                    $"{resourceRole.Name ?? resourceRole.resourceRef?.Name}", "14.7.1", "", eWarningLevel.WarningLevel0);
            if (resourceParameterBinding.FormalExpression?.Expression?.ExpressionString == null)
                bpmnDefinitions.ErrorInfos.AddWarning(
                    "Expression of parameterBinding can not be null.",
                    $"{resourceRole.Name ?? resourceRole.resourceRef?.Name}", "14.7.2", "", eWarningLevel.WarningLevel0);
        }
    }
}
