using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataAssociation;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.DataStores;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.DataFlows;

/// <summary>
/// 14.6
/// </summary>
internal static class DataAssociationXmlConvertor
{
    internal static void ExportOutputs(BpmnDefinitions bpmnDefinitions, dynamic node, IDataOutputAssociationContainer container)
    {
        foreach (var association in container.dataOutputAssociations ?? Enumerable.Empty<DataOutputAssociation>())
        {
            var associationElement = node.dataOutputAssociation();
            Export(bpmnDefinitions, associationElement, association);
        }
    }

    internal static void ExportInputs(BpmnDefinitions bpmnDefinitions, dynamic node, IDataInputAssociationContainer container)
    {
        foreach (var association in container.dataInputAssociations ?? Enumerable.Empty<DataInputAssociation>())
        {
            var associationElement = node.dataInputAssociation();
            Export(bpmnDefinitions, associationElement, association);
        }
    }

    internal static void ImportOutputs(BpmnDefinitions bpmnDefinitions, IDataOutputAssociationContainer container, ElasticObject node)
    {
        container.dataOutputAssociations = Import<DataOutputAssociation>(bpmnDefinitions, container, node, "dataOutputAssociation");
    }

    internal static void ImportInputs(BpmnDefinitions bpmnDefinitions, IDataInputAssociationContainer container, ElasticObject node)
    {
        container.dataInputAssociations = Import<DataInputAssociation>(bpmnDefinitions, container, node, "dataInputAssociation");
    }

    private static void Export(BpmnDefinitions bpmnDefinitions, dynamic element, DataAssociation association)
    {
        BaseElementXmlConvertor.Export(element, association);
        if (association.targetRef != null)
        {
            ItemAwareElementXmlConvertor.ExportElementRef(element.targetRef(), association.targetRef);
            element.tetaTargetRef = association.targetRef?.Name ?? (association.targetRef as BaseElement)?.Id;
        }
        else
            bpmnDefinitions.ErrorInfos.AddError($"The targetRef is null in the data association {association.Id}.", association.Id, "14.6.0", "Null Reference");
        foreach (var sourceRef in association.sourceRef ?? Enumerable.Empty<IItemAwareElement>())
            ItemAwareElementXmlConvertor.ExportElementRef(element.sourceRef(), sourceRef);
        if (association.transformation != null)
        {
            var transformationNode = element.transformation();
            FormalExpressionXmlConvertor.Export(transformationNode, association.transformation);
        }
        foreach (var assignment in association.assignment ?? Enumerable.Empty<Assignment>())
        {
            var assignmentNode = element.assignment();
            AssignmentXmlConvertor.Export(assignmentNode, assignment);
        }
    }

    private static List<TDataAssociation> Import<TDataAssociation>(BpmnDefinitions bpmnDefinitions,
        IDataAssociationContainer container, ElasticObject node, string elementName)
        where TDataAssociation : DataAssociation
    {
        List<TDataAssociation> dataAssociations = null;
        foreach (var associationElement in node.GetElements(elementName) ?? Enumerable.Empty<ElasticObject>())
        {
            dataAssociations ??= [];
            var id = associationElement.GetString("id");
            var dataAssociation = typeof(TDataAssociation) == typeof(DataInputAssociation)
                ? (DataAssociation)new DataInputAssociation(container as IDataInputAssociationContainer, id)
                : new DataOutputAssociation(container as IDataOutputAssociationContainer, id);
            var targetRefElement = associationElement.GetElement("targetRef");
            var fromInputItems = dataAssociation is DataInputAssociation;//todo
            dataAssociation.targetRef = GetItemAwareElement(bpmnDefinitions, container, targetRefElement, fromInputItems)
                                        ?? //todo its ampool
                                        GetItemAwareElement(bpmnDefinitions, container, targetRefElement, !fromInputItems);
            FillSourceRef(bpmnDefinitions, container, associationElement, dataAssociation);
            FillTransformation(associationElement, dataAssociation);
            FillAssignment(associationElement, dataAssociation);
            dataAssociations.Add((TDataAssociation)dataAssociation);
        }

        return dataAssociations;
    }

    private static void FillAssignment(ElasticObject associationElement, DataAssociation dataAssociation)
    {
        dataAssociation.assignment = null;
        foreach (var element in associationElement.GetElements("assignment") ?? Enumerable.Empty<ElasticObject>())
        {
            dataAssociation.assignment ??= [];
            var assignment = AssignmentXmlConvertor.Import(dataAssociation, element);
            dataAssociation.assignment.Add(assignment);
        }
    }

    private static void FillTransformation(ElasticObject associationElement, DataAssociation dataAssociation)
    {
        dataAssociation.transformation = null;
        var transformationElement = associationElement.GetElement("transformation");
        if (transformationElement != null && transformationElement.Attributes.Any())//todo
            dataAssociation.transformation = FormalExpressionXmlConvertor.Import(transformationElement);
    }

    private static void FillSourceRef(BpmnDefinitions bpmnDefinitions,
        IItemAwareContainer itemAwareContainer, ElasticObject associationElement, DataAssociation dataAssociation)
    {
        dataAssociation.sourceRef = null;
        foreach (var element in associationElement.GetElements("sourceRef") ?? Enumerable.Empty<ElasticObject>())
        {
            dataAssociation.sourceRef ??= [];
            var fromInputItems = dataAssociation is DataInputAssociation;//todo
            var dataElement = GetItemAwareElement(bpmnDefinitions, itemAwareContainer, element, fromInputItems)
                              ?? //todo its ampool
                              GetItemAwareElement(bpmnDefinitions, itemAwareContainer, element, !fromInputItems);
            if (dataElement != null)
                dataAssociation.sourceRef.Add(dataElement);
        }
    }

    private static IItemAwareElement GetItemAwareElement(BpmnDefinitions bpmnDefinitions,
        IItemAwareContainer itemAwareContainer, ElasticObject element, bool fromInputItems)
    {
        return ItemAwareElementXmlConvertor.ImportElementRef(bpmnDefinitions,
            itemAwareContainer, element, fromInputItems);
    }

    public static void Validate(BpmnDefinitions bpmnDefinitions, IDataAssociationContainer dataAssociationContainer)
    {
        if (dataAssociationContainer is IDataInputAssociationContainer)
        {
            //todo data store can not save in input
        }
        if (dataAssociationContainer is IDataOutputAssociationContainer)
        {
        }
    }
}
