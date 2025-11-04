using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

internal static class SignalXmlConvertor
{
    internal static void Export(dynamic node)
    {
        foreach (var signal in ProjectDefinition.Project.Signals ?? [])
        {
            var element = node.signal();
            RootElementXmlConvertor.Export(element, signal);
            element.name = signal.Name;
            ItemDefinitionXmlConvertor.ExportStructureRef(node, signal);
        }
    }

    internal static void Import(ElasticObject bpmnElement)
    {
        foreach (var element in bpmnElement.GetElements("signal") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var signal = ProjectDefinition.Project.GetSignal(id);
            if (signal != null) continue;
            signal = new Signal(ProjectDefinition.Project.BpmnDefinitions, id,
                element.GetString("name"), null);
            ItemDefinitionXmlConvertor.ImportStructureRef(ProjectDefinition.Project.BpmnDefinitions, signal, element);
            RootElementXmlConvertor.Import(element, signal);
            ProjectDefinition.Project.AddSignal(signal);
        }
    }
}
