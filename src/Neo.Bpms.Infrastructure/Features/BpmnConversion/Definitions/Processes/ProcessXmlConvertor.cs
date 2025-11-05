using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Bpmn.Collaborations;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Correlations;
using Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes.Resources;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.Processes;

/// <summary>
/// 14.9
/// </summary>
internal static class ProcessXmlConvertor
{
    internal static void Export(dynamic bpmnElement, BpmnDefinitions bpmnDefinitions)
    {
        foreach (var process in bpmnDefinitions?.GetRootElements()?.OfType<Process>() ?? [])
        {
            var element = bpmnElement.process();
            CallableElementXmlConvertor.Export(bpmnElement, element, process);
            element.processType = process.processType;
            element.isClosed = process.isClosed;
            element.isExecutable = process.isExecutable;
            element.status = process.Status.ToInt();
            element.namespaceId = process.EntityNamespaceId;
            element.entityId = process.EntityId;
            element.stateProperty = process.StateProperty;

            //element.administratorLock = process.AdministratorLock;
            //element.checkInUserId = process.CheckInUserId;
            element.displayFields = process.DisplayFields;
            if (process.definitionalCollaborationRef != null)
                element.definitionalCollaborationRef = process.definitionalCollaborationRef?.Id;

            AuditingXmlConvertor.Export(element, process);
            MonitoringXmlConvertor.Export(element, process);
            PropertyXmlConvertor.Export(bpmnElement, element, process);
            FlowElementsContainerXmlConvertor.Export(bpmnDefinitions, bpmnElement, element, process);
            ArtifactXmlConvertor.Export(element, process);
            ResourceRoleXmlConvertor.Export(bpmnDefinitions, element, process);
            CorrelationSubscriptionXmlConvertor.Export(element, process);
            //supports

            Validate(bpmnDefinitions, process);
        }
    }

    internal static void Import(ElasticObject bpmnElement, BpmnDefinitions bpmnDefinitions)
    {
        foreach (var element in bpmnElement.GetElements("process") ?? Enumerable.Empty<ElasticObject>())
        {
            var process = AddOrGetProcess(bpmnDefinitions, element);
            CallableElementXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, process);
            process.isExecutable = element.GetBool("isExecutable");
            process.isClosed = element.GetBool("isClosed");
            process.processType = element.GetEnumText("processType", Process.ProcessType.None);
            //process.ForDocumentation = element.GetBool("ForDocumentation");
            process.StateProperty = element.GetString("stateProperty") ?? "StateId";
            process.DisplayFields = element.GetString("displayFields");
            process.Status = element.GetEnumText("status", Process.ProcessStatus.Draft);

            ImportDefinitionalCollaborationRef(bpmnDefinitions, element, process);
            AuditingXmlConvertor.Import(element, process);
            MonitoringXmlConvertor.Import(element, process);
            PropertyXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, process);
            FlowElementsContainerXmlConvertor.Import(bpmnDefinitions, bpmnElement, process, element);
            ArtifactXmlConvertor.Import(element, process);
            ResourceRoleXmlConvertor.Imports(bpmnDefinitions, element, process);
            CorrelationSubscriptionXmlConvertor.Import(element, process);
            //supports

            Validate(bpmnDefinitions, process);
        }
    }

    private static void ImportDefinitionalCollaborationRef(BpmnDefinitions bpmnDefinitions, ElasticObject element, Process process)
    {
        var definitionalCollaborationRef = element.GetString("definitionalCollaborationRef");
        if (!string.IsNullOrEmpty(definitionalCollaborationRef))
            process.definitionalCollaborationRef =
                bpmnDefinitions.GetRootElement(definitionalCollaborationRef) as Collaboration;
        else
            process.definitionalCollaborationRef = bpmnDefinitions.GetRootElements().OfType<Collaboration>()
                .FirstOrDefault(c => c.participants.Any(p => p.processRef?.Id == process.Id));
    }

    private static Process AddOrGetProcess(BpmnDefinitions bpmnDefinitions, ElasticObject element)
    {
        var id = element.GetString("id");
        var rootElement = bpmnDefinitions.GetRootElement(id);
        var namespaceId = element.GetString("namespaceId");
        var entityId = element.GetString("entityId");
        var entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        var process = rootElement as Process
                      ?? new Process(bpmnDefinitions, id, bpmnDefinitions.Name, entity);
        if (rootElement is not Process)
        {
            if (rootElement != null)
                bpmnDefinitions.RemoveRootElement(rootElement.Id);
            bpmnDefinitions.AddRootElement(process);
        }
        else
            process.Entity = entity;
        return process;
    }

    private static void Validate(BpmnDefinitions bpmnDefinitions, Process process)
    {
        if (process.Entity == null)
            bpmnDefinitions.ErrorInfos.AddWarning($"Entity is not specified for the process {process.Name}.", process.Name, "14.9.0", "",
                eWarningLevel.WarningLevel0);
    }
}