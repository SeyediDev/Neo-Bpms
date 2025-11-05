using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.SubProcess;
using Task = Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.Task;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

/// <summary>
/// 14.2.
/// </summary>
internal static class ActivityXmlConvertor
{
    internal static void Export(BpmnDefinitions bpmnDefinitions,
        ElasticObject bpmnElement, dynamic process, FlowElement flowElement)
    {
        if (flowElement is not Activity activity) return;
        dynamic activityNode = null;
        switch (activity.ActivityType)
        {
            case Activity.eActivityType.UserTask:
            case Activity.eActivityType.ManualTask:
            case Activity.eActivityType.ServiceTask:
            case Activity.eActivityType.SendTask:
            case Activity.eActivityType.ScriptTask:
            case Activity.eActivityType.BusinessRuleTask:
            case Activity.eActivityType.ReceiveTask:
            case Activity.eActivityType.Task:
                if (flowElement is Task task)
                    activityNode = TaskXmlConvertor.Export(process, task);
                break;
            case Activity.eActivityType.EmbeddedSubProcess:
            case Activity.eActivityType.EventSubProcess:
            case Activity.eActivityType.AdhocSubProcess:
            case Activity.eActivityType.TransactionSubProcess:
                if (flowElement is SubProcess subProcess)
                    activityNode = SubProcessXmlConvertor.Export(bpmnDefinitions, bpmnElement, process, subProcess);
                break;
            case Activity.eActivityType.CallActivitySubProcess:
            case Activity.eActivityType.CallActivityGlobalTask:
                activityNode = CallActivityXmlConvertor.Export(process, activity);
                break;
        }
        if (activityNode != null)
            BasicExport(bpmnDefinitions, bpmnElement, activityNode, activity);
    }

    internal static FlowElement Import(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        IFlowElementsContainer flowElementsContainer, string elementName,
        ElasticObject elementNode, ref FlowElement flowElement, string id)
    {
        Activity activityTypeCheckElement = null;
        var activity = flowElement as Activity;
        var activityDef = TaskXmlConvertor.Import(bpmnDefinitions, flowElementsContainer, elementName, elementNode, activity,
                              ref activityTypeCheckElement)
                          ?? SubProcessXmlConvertor.Import(bpmnDefinitions, bpmnElement, flowElementsContainer, elementName,
                              elementNode, activity, ref activityTypeCheckElement)
                          ?? CallActivityXmlConvertor.Import(flowElementsContainer, elementName, elementNode, activity,
                              ref activityTypeCheckElement);
        if (activityTypeCheckElement == null && activity != null)
        {
            flowElementsContainer.flowElements.Remove(id);
            flowElement = null;
        }
        if (activityDef != null)
            BasicImport(bpmnDefinitions, bpmnElement, elementNode, activityDef);
        return activityDef;
    }

    internal static void Validate(BpmnDefinitions bpmnDefinitions, FlowElement flowElement)
    {
        if (flowElement is not Activity activity)
        {
            bpmnDefinitions.ErrorInfos.AddFatal($"The FlowElement {flowElement?.Id}, is not an activity.", flowElement?.Id, "14.2.0",
                "Internal error");
            return;
        }
        switch (activity.ActivityType)
        {
            case Activity.eActivityType.Task:
            case Activity.eActivityType.UserTask:
            case Activity.eActivityType.ManualTask:
            case Activity.eActivityType.ServiceTask:
            case Activity.eActivityType.SendTask:
            case Activity.eActivityType.ScriptTask:
            case Activity.eActivityType.BusinessRuleTask:
            case Activity.eActivityType.ReceiveTask:
                TaskXmlConvertor.Validate(bpmnDefinitions, activity);
                break;
            case Activity.eActivityType.EmbeddedSubProcess:
            case Activity.eActivityType.EventSubProcess:
            case Activity.eActivityType.AdhocSubProcess:
            case Activity.eActivityType.TransactionSubProcess:
                break;
            case Activity.eActivityType.CallActivitySubProcess:
            case Activity.eActivityType.CallActivityGlobalTask:
                break;
        }
        DataAssociationXmlConvertor.Validate(bpmnDefinitions, activity);
    }

    private static void BasicExport(BpmnDefinitions bpmnDefinitions,
        ElasticObject bpmnElement, dynamic element, Activity activity)
    {
        FlowNodeXmlConvertor.Export(element, activity);
        element.completionQuantity = activity.completionQuantity;
        element.isForCompensation = activity.isForCompensation;
        element.startQuantity = activity.startQuantity;
        element.EmergencyTimeToDo = activity.EmergencyTimeToDo.ToString();
        element.CriticalTimeToDo = activity.CriticalTimeToDo.ToString();
        if (!string.IsNullOrEmpty(activity.defaultSequenceFlowId))
        {
            element["default"] = activity.defaultSequenceFlowId;
        }

        DueTimeDurationXmlConvertor.Export(element, activity);
        element.notificationToOwnerOnAllocate = activity.notificationToOwnerOnAllocate;
        element.notificationToOwnerOnExpiration = activity.notificationToOwnerOnExpiration;
        element.notificationToManagerOnExpiration = activity.notificationToManagerOnExpiration;

        DataAssociationXmlConvertor.ExportInputs(bpmnDefinitions, element, activity);
        DataAssociationXmlConvertor.ExportOutputs(bpmnDefinitions, element, activity);
        IoSpecificationXmlConvertor.Export(bpmnElement, element, activity);
        LoopCharacteristicsXmlConvertor.Export(bpmnDefinitions, bpmnElement, element, activity);
        ResourceRoleXmlConvertor.Export(bpmnDefinitions, element, activity);
        PropertyXmlConvertor.Export(bpmnElement, element, activity);
        Validate(bpmnDefinitions, activity);
    }

    private static void BasicImport(BpmnDefinitions bpmnDefinitions, ElasticObject bpmnElement,
        ElasticObject element, Activity activity)
    {
        FlowNodeXmlConvertor.Import(element, activity);
        activity.isForCompensation = element.GetBool("isForCompensation");
        activity.startQuantity = element.GetInteger("startQuantity", 1);
        activity.completionQuantity = element.GetInteger("completionQuantity", 1);
        activity.defaultSequenceFlowId = element.GetString("default");
        if (TimeSpan.TryParse(element.GetString("EmergencyTimeToDo"), out var et))
            activity.EmergencyTimeToDo = et;
        if (TimeSpan.TryParse(element.GetString("CriticalTimeToDo"), out var ct))
            activity.CriticalTimeToDo = ct;

        DueTimeDurationXmlConvertor.Import(element, activity, element.GetString("timeDuration"));//todo کاربردش چیست ؟
        activity.notificationToOwnerOnAllocate = element.GetBool("notificationToOwnerOnAllocate");
        activity.notificationToOwnerOnExpiration = element.GetBool("notificationToOwnerOnExpiration");
        activity.notificationToManagerOnExpiration = element.GetBool("notificationToManagerOnExpiration");

        DataAssociationXmlConvertor.ImportInputs(bpmnDefinitions, activity, element);
        DataAssociationXmlConvertor.ImportOutputs(bpmnDefinitions, activity, element);
        IoSpecificationXmlConvertor.Import(bpmnDefinitions, bpmnElement, activity, element);
        LoopCharacteristicsXmlConvertor.Import(bpmnDefinitions, bpmnElement, activity, element);
        ResourceRoleXmlConvertor.Imports(bpmnDefinitions, element, activity);
        activity.ActivityAndLaneResources = null;
        PropertyXmlConvertor.Import(bpmnDefinitions, bpmnElement, element, activity);
        Validate(bpmnDefinitions, activity);
    }
}
