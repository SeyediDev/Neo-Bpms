using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class PropertyXmlConvertor
{
    internal static void Export(ElasticObject bpmnElement, dynamic node, IPropertyContainer propertyContainer)
    {
        foreach (var property in propertyContainer.properties ?? Enumerable.Empty<Property>())
        {
            var element = node.property();
            DataElementXmlConvertor.Export(bpmnElement, element, property);
        }
    }

    internal static void Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        ElasticObject containerElement, IPropertyContainer container)
    {
        var oldProperties = container.properties;
        container.properties = [];
        foreach (var element in containerElement.GetElements("property") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var property = oldProperties?.FirstOrDefault(p => p.Id == id) ??
                new Property(container, id, "", null);
            DataElementXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, property);
            //			    if (property.itemSubjectRef != null && property.itemSubjectRef.structureRef == null)
            //			    { todo
            //			        property.itemSubjectRef.structureRef = container.entity?.getfield(property.name);
            //			    }
            container.properties ??= [];
            container.properties.Add(property);
        }
    }
}