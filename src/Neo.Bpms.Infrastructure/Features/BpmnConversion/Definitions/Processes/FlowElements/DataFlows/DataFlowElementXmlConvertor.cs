using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.DataStores;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.DataFlows;

internal static class DataFlowElementXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic node, DataFlowElement dataFlowElement)
    {
        dynamic element = null;
        switch (dataFlowElement.flowElementType)
        {
            case FlowElement.eFlowElementType.DataStoreRef:
                element = DataStoreReferenceXmlConvertor.Export(node, dataFlowElement as DataStoreReference);
                break;
            case FlowElement.eFlowElementType.DataObject:
                element = ExportDataObject(node, dataFlowElement as DataObject);
                break;
            case FlowElement.eFlowElementType.DataObjectRef:
                element = ExportDataObjectRef(node, dataFlowElement as DataObjectReference);
                break;
        }
        if (element != null)
            BasicExport(bpmnElement, element, dataFlowElement);
    }

    internal static DataFlowElement Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IFlowElementsContainer flowElementsContainer, string elementName,
        ElasticObject element, ref FlowElement flowElement, string id)
    {
        DataFlowElement dataFlowTypeCheckElement = null;
        var dataFlow = flowElement as DataFlowElement;
        var dataFlowDef = ImportDataFlow(flowElementsContainer, elementName, element,
            dataFlow, ref dataFlowTypeCheckElement);
        if (dataFlowTypeCheckElement == null && dataFlow != null)
        {
            flowElementsContainer.flowElements.Remove(id);
            flowElement = null;
        }
        if (dataFlowDef != null)
            BasicImport(bpmnDefinitions, bpmnElement, element, dataFlowDef);
        return dataFlowDef;
    }

    private static void BasicExport(ElasticObject bpmnElement, dynamic element, DataFlowElement dataFlowElement)
    {
        FlowElementXmlConvertor.Export(element, dataFlowElement);
        ItemAwareElementXmlConvertor.Export(bpmnElement, element, dataFlowElement);
    }

    private static void BasicImport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement, ElasticObject element, DataFlowElement dataFlowElement)
    {
        FlowElementXmlConvertor.Import(element, dataFlowElement);
        ItemAwareElementXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, dataFlowElement);
    }

    private static DataFlowElement ImportDataFlow(IFlowElementsContainer flowElementsContainer, string elementsKey, ElasticObject element,
        DataFlowElement dataFlowElement, ref DataFlowElement dataFlowTypeCheckElement)
    {
        DataFlowElement dataFlowDef = null;
        switch (elementsKey)
        {
            case "dataStoreReference":
                dataFlowTypeCheckElement = dataFlowElement as DataStoreReference;
                dataFlowDef = DataStoreReferenceXmlConvertor.Import(flowElementsContainer, element, dataFlowElement as DataStoreReference);
                break;
            case "dataObject":
                dataFlowTypeCheckElement = dataFlowElement as DataObject;
                dataFlowDef = ImportDataObject(flowElementsContainer, element, dataFlowElement as DataObject);
                break;
            case "dataObjectReference":
                dataFlowTypeCheckElement = dataFlowElement as DataObjectReference;
                dataFlowDef = ImportDataObjectRef(flowElementsContainer, element, dataFlowElement as DataObjectReference);
                break;
        }
        return dataFlowDef;
    }

    private static dynamic ExportDataObject(dynamic process, DataObject dataObject)
    {
        var node = process.dataObject();
        node.isCollection = dataObject.isCollection;
        return node;
    }

    private static DataObject ImportDataObject(IFlowElementsContainer flowElementsContainer, ElasticObject element, DataObject dataObject)
    {
        dataObject ??= new DataObject(flowElementsContainer, "", "", null);
        dataObject.isCollection = element.GetBool("isCollection");
        return dataObject;
    }

    private static dynamic ExportDataObjectRef(dynamic process, DataObjectReference dataObjectReference)
    {
        var node = process.dataObjectReference();
        node.dataObjectRef = dataObjectReference.dataObject?.Id;
        return node;
    }

    private static DataObjectReference ImportDataObjectRef(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        DataObjectReference dataObjectRef)
    {
        dataObjectRef ??= new DataObjectReference(flowElementsContainer, "", "", null);
        dataObjectRef.dataObject = flowElementsContainer.GetFlowElement(element.GetString("dataObjectRef")) as DataObject;
        return dataObjectRef;
    }
}