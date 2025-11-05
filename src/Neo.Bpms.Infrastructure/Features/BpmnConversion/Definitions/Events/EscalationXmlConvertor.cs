using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class EscalationXmlConvertor
{
    internal static void Export(dynamic bpmnElement)
    {
        foreach (var escalation in ProjectDefinition.Project.Escalations ?? [])
        {
            var element = bpmnElement.escalation();
            RootElementXmlConvertor.Export(element, escalation);
            element.name = escalation.Name;
            element.escalationCode = escalation.escalationCode;
            ItemDefinitionXmlConvertor.ExportStructureRef(element, escalation);
        }
    }

    internal static void Import(ElasticObject bpmnElement)
    {
        foreach (var element in bpmnElement.GetElements("escalation") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var escalation = ProjectDefinition.Project.GetEscalation(id);
            if (escalation != null) continue;

            var name = element.GetString("name");
            var escalationCode = element.GetString("escalationCode");
            escalation = new Escalation(ProjectDefinition.Project.BpmnDefinitions,
                id, escalationCode, name);
            ItemDefinitionXmlConvertor.ImportStructureRef(ProjectDefinition.Project.BpmnDefinitions,
                escalation, element);
            RootElementXmlConvertor.Import(element, escalation);
            ProjectDefinition.Project.AddEscalation(escalation);
        }
    }
}
