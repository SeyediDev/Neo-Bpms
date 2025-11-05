using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Correlation;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class CorrelationPropertyRetrievalExpressionXmlConvertor
{
    internal static void Export(dynamic correlationPropertyElement, CorrelationProperty correlationProperty)
    {
        foreach (var retrievalExpression in correlationProperty.retrievalExpression ??
                                            Enumerable.Empty<CorrelationPropertyRetrievalExpression>())
        {
            var element = correlationPropertyElement.correlationPropertyRetrievalExpression();
            BaseElementXmlConvertor.Export(element, retrievalExpression);
            element.messageRef = retrievalExpression.messageRef?.Id;
            FormalExpressionXmlConvertor.Export(element.messagePath(), retrievalExpression.messagePath);
        }
    }

    internal static void Import(ElasticObject correlationPropertyElement, CorrelationProperty correlationProperty)
    {
        correlationProperty.retrievalExpression = null;
        foreach (var element in correlationPropertyElement.GetElements("correlationPropertyRef") ??
                                Enumerable.Empty<ElasticObject>())
        {
            var correlationPropertyRetrievalExpression = new CorrelationPropertyRetrievalExpression(correlationProperty, "",
                FormalExpressionXmlConvertor.Import(element.GetElement("messagePath")),
                ProjectDefinition.Project.GetMessage(element.GetString("messageRef")));
            BaseElementXmlConvertor.Import(element, correlationPropertyRetrievalExpression);

            correlationProperty.retrievalExpression ??= [];
            correlationProperty.retrievalExpression.Add(correlationPropertyRetrievalExpression);
        }
    }
}
