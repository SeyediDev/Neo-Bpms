using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.Auditing;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion.Definitions.BaseElements;

internal static class AuditingXmlConvertor
{
    internal static void Export(dynamic containerElement, IAuditingContainer auditingContainer)
    {
        if (auditingContainer.auditing == null) return;
        var element = containerElement.auditing();
        BaseElementXmlConvertor.Export(element, auditingContainer.auditing);
        element.saveInstances = auditingContainer.auditing.saveInstances;
        element.generateTraceLog = auditingContainer.auditing.generateTraceLog;
        element.logCondition = auditingContainer.auditing.logCondition;
        element.breakPointCondition = auditingContainer.auditing.breakPointCondition;
    }

    internal static void Import(ElasticObject containerElement, IAuditingContainer container)
    {
        var element = containerElement.GetElement("auditing");
        if (element == null) return;
        container.auditing = new Auditing(container, element.GetString("id"));
        BaseElementXmlConvertor.Import(element, container.auditing);
        container.auditing.saveInstances = element.GetBool("saveInstances", true);
        container.auditing.generateTraceLog = element.GetBool("generateTraceLog");
        container.auditing.logCondition = element.GetString("logCondition");
        container.auditing.breakPointCondition = element.GetString("breakPointCondition");
    }
}
