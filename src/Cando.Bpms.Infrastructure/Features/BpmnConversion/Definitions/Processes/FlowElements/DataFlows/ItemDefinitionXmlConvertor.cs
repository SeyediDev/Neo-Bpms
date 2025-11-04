using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class ItemDefinitionXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, ItemDefinition itemDefinition)
    {
        if (itemDefinition == null) return;
        var element = bpmnElement.GetElements("itemDefinition")
            ?.FirstOrDefault(node => node.GetString("id") == itemDefinition.Id);
        if (element != null) return;
        element = new ElasticObject("itemDefinition");
        bpmnElement.AddElement(element);
        BaseElementXmlConvertor.Export(element, itemDefinition);
        element["isCollection"] = itemDefinition.isCollection;
        element["itemKind"] = itemDefinition.itemKind;
        ExportStructureRef(element, itemDefinition);
    }

    internal static ItemDefinition Import(BpmnDefinitions bpmnDefinitions,
        ElasticObject bpmnElement, string itemDefinitionId)
    {
        if (string.IsNullOrEmpty(itemDefinitionId)) return null;
        if (bpmnDefinitions?.GetRootElement(itemDefinitionId) is ItemDefinition itemDefinition)
            return itemDefinition;
        var element = bpmnElement.GetElements("itemDefinition")
                                        ?.FirstOrDefault(node => node.GetString("id") == itemDefinitionId);
        if (element == null) return null;
        var isCollection = element.GetBool("isCollection");
        var id = element.GetString("id");
        itemDefinition = new ItemDefinition(bpmnDefinitions, id, isCollection);
        ImportStructureRef(bpmnDefinitions, itemDefinition, element, id);
        if (bpmnDefinitions != null)
            bpmnDefinitions.AddRootElement(itemDefinition);
        else //todo
            ProjectDefinition.Project.BpmnDefinitions.AddRootElement(itemDefinition);
        return itemDefinition;
    }

    internal static void ExportStructureRef(dynamic element, IStructureDefinition structureDefinition)
    {
        var id = FetchStructureRef(structureDefinition);
        if (!string.IsNullOrEmpty(id))
            element.structureRef = id;
    }

    internal static void ImportStructureRef(BpmnDefinitions bpmnDefinitions,
        IStructureDefinition structureDefinition, ElasticObject element, string itemDefinitionId = null)
    {
        var structureRef = element.GetString("structureRef");
        var structureRefItems = structureRef?.Split('.').ToList() ?? itemDefinitionId?.Split('.').ToList();
        if (structureRef == null && structureRefItems != null && structureRefItems.Count > 1)
            structureRefItems.RemoveAt(0);
        if (structureRefItems != null && structureRefItems.Count >= 3 &&
            (structureRefItems[0] == "teta:EntityField" || structureRefItems[0] == "teta:Entity")
            )
        {
            var entity = ProjectDefinition.Project.GetEntity(structureRefItems[1], structureRefItems[2]);
            if (structureRefItems[0] == "teta:Entity")
                structureDefinition.structureRef = entity;
            else if (structureRefItems.Count > 3)
                structureDefinition.structureRef = entity?.GetField(structureRefItems[3]);
        }
        else
            structureDefinition.structureRef = structureRef != null ? bpmnDefinitions?.GetRootElement(structureRef) : null;
    }

    private static string FetchStructureRef(IStructureDefinition structureDefinition)
    {
        if (structureDefinition.field != null)
            return $"teta:EntityField.{structureDefinition.structure?.NamespaceId}.{structureDefinition.structure?.Id}.{structureDefinition.field.Id}";
        if (structureDefinition.structure != null)
            return $"teta:Entity.{structureDefinition.structure.NamespaceId}.{structureDefinition.structure.Id}";
        if (structureDefinition.structureRef is BaseElement element)
            return element.Id;
        return structureDefinition.structureRef?.ToString();
    }
}
