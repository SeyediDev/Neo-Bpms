using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class MessageXmlConvertor
{
    internal static void Export(dynamic bpmnElement)
    {
        foreach (var message in ProjectDefinition.Project.Messages ?? [])
        {
            var element = bpmnElement.message();
            RootElementXmlConvertor.Export(element, message);
            element.name = message.Name;
            if (message.itemRef == null) continue;
            element.itemRef = message.itemRef.Id;
            ItemDefinitionXmlConvertor.Export(bpmnElement as ElasticObject, message.itemRef);
        }
    }

    internal static void Import(ElasticObject bpmnElement)
    {
        foreach (var element in bpmnElement.GetElements("message") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var message = ProjectDefinition.Project.GetMessage(id);
            if (message != null) continue;
            message = new Message(ProjectDefinition.Project.BpmnDefinitions,
                id, element.GetString("name"), null)
            {
                itemRef = ItemDefinitionXmlConvertor.Import(ProjectDefinition.Project.BpmnDefinitions,
                    bpmnElement, element.GetString("itemRef"))
            };
            RootElementXmlConvertor.Import(element, message);
            ProjectDefinition.Project.AddMessage(message);
        }
    }
}
