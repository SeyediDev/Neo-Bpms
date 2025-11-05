using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.FlowElements.DataFlows;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.RootElements;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Events;

internal static class ErrorXmlConvertor
{
    internal static void Export(dynamic node)
    {
        foreach (var error in ProjectDefinition.Project.Errors ?? [])
        {
            var element = node.error();
            RootElementXmlConvertor.Export(element, error);
            element.name = error.Name;
            element.errorCode = error.errorCode;
            ItemDefinitionXmlConvertor.ExportStructureRef(element, error);
        }
    }

    internal static void Import(ElasticObject bpmnElement)
    {
        foreach (var element in bpmnElement.GetElements("error") ?? Enumerable.Empty<ElasticObject>())
        {
            var id = element.GetString("id");
            var error = ProjectDefinition.Project.GetError(id);
            if (error != null) continue;

            var name = element.GetString("name");
            var errorCode = element.GetString("errorCode");
            error = new Error(ProjectDefinition.Project.BpmnDefinitions,
                id, name, errorCode, null);
            ItemDefinitionXmlConvertor.ImportStructureRef(ProjectDefinition.Project.BpmnDefinitions,
                error, element);
            RootElementXmlConvertor.Import(element, error);
            ProjectDefinition.Project.AddError(error);
        }
    }
}
