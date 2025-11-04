using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class FormalExpressionXmlConvertor
{
    internal static void Export(dynamic element, BpmnExpression formalExpression, string @namespace = null)
    {
        Export(element, formalExpression as FormalExpression, @namespace);
    }

    internal static void Export(dynamic element, FormalExpression formalExpression, string @namespace = null)
    {
        if (formalExpression == null) return;
        BaseElementXmlConvertor.Export(element, formalExpression, @namespace, false);
        if (string.IsNullOrEmpty(formalExpression.language))
            element.language = formalExpression.language;
        //todo evaluatesToTypeRef = ItemK
        /*if (@namespace == "teta")
			{
				element["type"] = new ElasticObject("","bpmn2:tExpression");
				element["type"].Namespace = "http://www.w3.org/2001/XMLSchema-instance";
				//element.type = "bpmn2:tExpression";
			}*/
        element.InternalValue = formalExpression.Expression?.ExpressionString;
    }

    internal static FormalExpression Import(ElasticObject element)
    {
        if (element == null) return null;
        var formula = element.InternalValue?.ToString();
        if (formula != null)
            formula = System.Net.WebUtility.HtmlDecode(formula);
        return new FormalExpression(element.GetString("id"), Parser.ParseTree(formula))
        {
            language = element.GetString("language"),
            //todo evaluatesToTypeRef = ItemK
        };
    }
}
