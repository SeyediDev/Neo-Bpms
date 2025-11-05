using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class DataStoreReferenceXmlConvertor
{
    internal static dynamic Export(dynamic node, DataStoreReference dataStoreReference)
    {
        var element = node.dataStoreReference();
        element.dataStoreRef = dataStoreReference.dataStore?.Id;
        element.saveOnPrimaryKeyProperty = dataStoreReference.SaveOnPrimaryKeyProperty;
        var extentionsNode = BaseElementXmlConvertor.ExportExtensionElements(element);
        foreach (var filterMapping in dataStoreReference.FilterMappings ?? Enumerable.Empty<DataStoreReference.FilterMapping>())
        {
            var filterMappingElement = extentionsNode.filterMapping();
            filterMappingElement.dataStoreField = filterMapping.DataStoreField;
            filterMappingElement.processProperty = filterMapping.ProcessProperty;
        }
        FormalExpressionXmlConvertor.Export(element, dataStoreReference.FilterExpression);
        return element;
    }
    internal static DataStoreReference Import(IFlowElementsContainer flowElementsContainer, ElasticObject element, DataStoreReference dataStoreReference)
    {
        dataStoreReference ??= new DataStoreReference(flowElementsContainer, "", "", null);
        var dataStoreRef = element.GetString("dataStoreRef");
        dataStoreReference.dataStore =
            ProjectDefinition.Project.GetDataStore(dataStoreRef);
        dataStoreReference.SaveOnPrimaryKeyProperty = element.GetString("saveOnPrimaryKeyProperty");
        dataStoreReference.FilterMappings = null;
        foreach (var filterMappingElement in BaseElementXmlConvertor.ImportExtensionItem(element, "filterMapping") ?? Enumerable.Empty<ElasticObject>())
        {
            dataStoreReference.FilterMappings ??= [];
            dataStoreReference.FilterMappings.Add(new DataStoreReference.FilterMapping
            {
                DataStoreField = filterMappingElement.GetString("dataStoreField"),
                ProcessProperty = filterMappingElement.GetString("processProperty")
            });
        }
        dataStoreReference.FilterExpression = FormalExpressionXmlConvertor.Import(element);
        return dataStoreReference;
    }
}
