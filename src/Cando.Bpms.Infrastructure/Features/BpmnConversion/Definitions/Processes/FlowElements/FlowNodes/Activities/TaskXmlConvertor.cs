using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Extensions;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;
using Neo.Bpms.Domain.Entities.WorkManagement;
using Task = Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.Task;

namespace Neo.Bpms.Infrastructure.Features.BpmnConversion;

/// <summary>
/// 14.1.
/// </summary>
public class TaskXmlConvertor
{
    internal static dynamic Export(dynamic containerElement, Task task)
    {
        dynamic node = null;
        switch (task.ActivityType)
        {
            case Activity.eActivityType.Task:
                node = containerElement.task();
                break;
            case Activity.eActivityType.UserTask:
                var userTask = task as UserTask;
                if (userTask == null) break;
                node = containerElement.userTask();
                node.implementation = userTask.implementation;

                node.formId = userTask.FormId;
                node.indexFormId = userTask.IndexFormId;
                node.associationFieldId = userTask.AssociationFieldId;
                RenderingXmlConvertor.Export(node, userTask);

                node.policy = userTask.WorkDistributionPolicy?.policy ?? eWorkAllocationPolicy.AllocateToUser;
                node.algorithm = userTask.WorkDistributionPolicy?.algorithm ?? eAllocationOrOfferingAlgorithm.Rotational;
                node.rankingFormula = userTask.WorkDistributionPolicy?.rankingFormula;
                node.needsAllocationBeforePerform = userTask.WorkDistributionPolicy?.needsAllocationBeforePerform;
                node.maxGroupOfferedUsers = userTask.WorkDistributionPolicy?.maxGroupOfferedUsers;
                break;
            case Activity.eActivityType.ManualTask:
                node = containerElement.manualTask();
                node.controlBySystem = (task as ManualTask)?.ControlBySystem ?? false;
                break;
            case Activity.eActivityType.ServiceTask:
                var serviceTask = task as ServiceTask;
                if (serviceTask == null) break;
                node = containerElement.serviceTask();
                node.implementation = serviceTask.implementation;
                node.interfaceRef = serviceTask.operationRef?.Interface.Id;
                node.operationRef = serviceTask.operationRef?.Id;
                break;
            case Activity.eActivityType.SendTask:
                var sendTask = task as SendTask;
                if (sendTask == null) break;
                node = containerElement.sendTask();
                node.implementation = sendTask.implementation;
                node.interfaceRef = sendTask.operationRef?.Interface.Id;
                node.operationRef = sendTask.operationRef?.Id;
                node.messageRef = sendTask.messageRef?.Id;
                break;
            case Activity.eActivityType.ScriptTask:
                var scriptTask = task as ScriptTask;
                if (scriptTask == null) break;
                node = containerElement.scriptTask();
                node.script = scriptTask.script;
                node.scriptFormat = scriptTask.scriptFormat;
                node.scriptLanguage = scriptTask.scriptLanguage;
                break;
            case Activity.eActivityType.BusinessRuleTask:
                var businessRuleTask = task as BusinessRuleTask;
                if (businessRuleTask == null) break;
                node = containerElement.businessRuleTask();
                node.implementation = businessRuleTask.implementation;
                break;
            case Activity.eActivityType.ReceiveTask:
                var receiveTask = task as ReceiveTask;
                if (receiveTask == null) break;
                node = containerElement.receiveTask();
                node.implementation = receiveTask.implementation;
                node.instantiate = receiveTask.instantiate;
                node.messageRef = receiveTask.messageRef?.Id;
                node.interfaceRef = receiveTask.operationRef?.Interface.Id;
                node.operationRef = receiveTask.operationRef?.Id;
                break;
        }

        if (node != null)
        {
            node.priorityLevel = task.priorityLevel;
            node.priorityLevelProperty = task.priorityLevelProperty;
        }

        return node;
    }

    internal static Activity Import(BpmnDefinitions bpmnDefinitions,
        IFlowElementsContainer flowElementsContainer, string elementName,
        ElasticObject element, Activity activity, ref Activity activityTypeCheckElement)
    {
        Task task = null;
        switch (elementName)
        {
            case "task":
                activityTypeCheckElement = activity as Task;
                task = activity as Task ?? new Task(flowElementsContainer, element.GetString("id"),
                    element.GetString("name"), Activity.eActivityType.Task);
                break;
            case "userTask":
                activityTypeCheckElement = activity as UserTask;
                task = ImportUserTask(flowElementsContainer, element, activity as UserTask);
                break;
            case "businessRuleTask":
                activityTypeCheckElement = activity as BusinessRuleTask;
                task = ImportBusinessRuleTask(flowElementsContainer, element, activity as BusinessRuleTask);
                break;
            case "scriptTask":
                activityTypeCheckElement = activity as ScriptTask;
                task = ImportScriptTask(flowElementsContainer, element, activity as ScriptTask);
                break;
            case "manualTask":
                activityTypeCheckElement = activity as ManualTask;
                task = ImportManualTask(flowElementsContainer, element, activity as ManualTask);
                break;
            case "receiveTask":
                activityTypeCheckElement = activity as ReceiveTask;
                task = ImportReceiveTask(flowElementsContainer, element, activity as ReceiveTask);
                break;
            case "serviceTask":
                activityTypeCheckElement = activity as ServiceTask;
                task = ImportServiceTask(flowElementsContainer, element, activity as ServiceTask);
                break;
            case "sendTask":
                activityTypeCheckElement = activity as SendTask;
                task = ImportSendTask(flowElementsContainer, element, activity as SendTask);
                break;
        }

        if (task != null)
        {
            task.priorityLevel = element.GetDouble("priorityLevel");
            task.priorityLevelProperty = element.GetString("priorityLevelProperty");
        }

        return task;
    }

    internal static void Validate(BpmnDefinitions bpmnDefinitions, Activity activity)
    {
        if (activity is not Task task)
        {
            bpmnDefinitions.ErrorInfos.AddFatal($"The activity {activity?.Id}, is not a task.",
                activity?.Id, "14.1.2", "Internal error.");
            return;
        }

        switch (task.ActivityType)
        {
            case Activity.eActivityType.UserTask:
                var userTask = task as UserTask;
                if (string.IsNullOrEmpty(userTask?.FormId))
                    bpmnDefinitions.ErrorInfos.AddError(
                        $"In the UserTask {task.Name ?? task.Id}, \"rendering form\" is not specified.",
                        task.Name, "14.1.3", "UserTask must have rendering form.");
                break;
            case Activity.eActivityType.ManualTask:
                break;
            case Activity.eActivityType.ServiceTask:
                var serviceTask = task as ServiceTask;
                if (serviceTask?.operationRef == null)
                    bpmnDefinitions.ErrorInfos.AddError(
                        $"The ServiceTask {task.Name ?? task.Id}, doesn't have OperationRef.",
                        task.Name, "14.1.4", "ServiceTask must have OperationRef.");
                else if (serviceTask.operationRef.inMessageRef == null)
                    bpmnDefinitions.ErrorInfos.AddError(
                        $"In the ServiceTask {task.Name ?? task.Id}, the operation {serviceTask.operationRef.Name} has not inMessageRef.",
                        task.Name, "14.1.5", "Operation must have inMessageRef.");
                break;
            case Activity.eActivityType.SendTask:
                break;
            case Activity.eActivityType.ScriptTask:
                break;
            case Activity.eActivityType.BusinessRuleTask:
                break;
            case Activity.eActivityType.Task:
                bpmnDefinitions.ErrorInfos.AddWarning($"Please specify type of the task {task.Id} {task.Name}.",
                    $"Task {task.Id}", "14.1.1", "Argument must be specified.",
                    eWarningLevel.WarningLevel2);
                break;
            case Activity.eActivityType.ReceiveTask:
                break;
        }
    }

    private static ScriptTask ImportScriptTask(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        ScriptTask scriptTask)
    {
        scriptTask ??= new ScriptTask(flowElementsContainer, "", "");

        scriptTask.script = element.GetString("script");
        scriptTask.scriptFormat = element.GetString("scriptFormat");
        scriptTask.scriptLanguage = element.GetEnumText("scriptLanguage", ScriptTask.eScriptLanguage.CSharp);
        return scriptTask;
    }

    private static ReceiveTask ImportReceiveTask(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        ReceiveTask receiveTask)
    {
        receiveTask ??= new ReceiveTask(flowElementsContainer, "", "");

        receiveTask.implementation = element.GetString("implementation");
        receiveTask.instantiate = element.GetBool("instantiate");
        var messageRef = element.GetString("messageRef");
        if (receiveTask.messageRef?.Id != messageRef)
            receiveTask.messageRef = ProjectDefinition.Project.GetMessage(messageRef);
        var operationRef = element.GetString("operationRef");
        if (receiveTask.operationRef?.Id != operationRef)
            receiveTask.operationRef = ProjectDefinition.Project.GetOperation(operationRef);
        return receiveTask;
    }

    private static BusinessRuleTask ImportBusinessRuleTask(IFlowElementsContainer flowElementsContainer,
        ElasticObject element, BusinessRuleTask businessRuleTask)
    {
        var implementation = element.GetString("implementation");

        businessRuleTask ??= new BusinessRuleTask(flowElementsContainer, "", "");

        if (!string.IsNullOrEmpty(implementation))
            businessRuleTask.implementation = implementation;
        return businessRuleTask;
    }

    private static UserTask ImportUserTask(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        UserTask userTask)
    {
        var formId = element.GetString("formId");
        var implementation = element.GetString("implementation");
        if (userTask == null)
            userTask = new UserTask(flowElementsContainer, "", "", formId, null, null);
        else
        {
            if (!string.IsNullOrEmpty(implementation))
                userTask.implementation = implementation;
        }

        RenderingXmlConvertor.Import(element, userTask);
        userTask.WorkDistributionPolicy = new WorkDistributionPolicy
        {
            policy = element.GetEnumText("policy",
                                                            eWorkAllocationPolicy.AllocateToUser),
            algorithm = element.GetEnumText("algorithm",
                                                            eAllocationOrOfferingAlgorithm.Rotational),
            rankingFormula = element.GetString("rankingFormula"),
            needsAllocationBeforePerform =
                                                            element.GetBool("needsAllocationBeforePerform"),
            maxGroupOfferedUsers = element.GetInteger("maxGroupOfferedUsers", 5)
        };
        return userTask;
    }

    private static ManualTask ImportManualTask(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        ManualTask manualTask)
    {
        manualTask ??= new ManualTask(flowElementsContainer, "", "");
        manualTask.ControlBySystem = element.GetBool("controlBySystem", false);
        return manualTask;
    }

    private static ServiceTask ImportServiceTask(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        ServiceTask serviceTask)
    {
        serviceTask ??= new ServiceTask(flowElementsContainer, "", "", null);
        var imp = element.GetString("implementation");
        if (!string.IsNullOrEmpty(imp))
            serviceTask.implementation = imp;

        var operationRef = element.GetString("operationRef");
        if (!string.IsNullOrEmpty(operationRef) && serviceTask.operationRef?.Id != operationRef)
            serviceTask.operationRef = ProjectDefinition.Project.GetOperation(operationRef);
        return serviceTask;
    }

    private static SendTask ImportSendTask(IFlowElementsContainer flowElementsContainer, ElasticObject element,
        SendTask sendTask)
    {
        sendTask ??= new SendTask(flowElementsContainer, "", "");
        sendTask.implementation = element.GetString("implementation");
        var operationRef = element.GetString("operationRef");
        if (sendTask.operationRef?.Id != operationRef)
            sendTask.operationRef = ProjectDefinition.Project.GetOperation(operationRef);
        var messageRef = element.GetString("messageRef");
        if (sendTask.messageRef?.Id != messageRef)
            sendTask.messageRef = ProjectDefinition.Project.GetMessage(messageRef);
        return sendTask;
    }
}

internal class RenderingXmlConvertor
{
    public static void Export(dynamic containerElement, UserTask userTask)
    {
        foreach (var rendering in userTask?.rendering.OfType<RenderingForm>() ?? [])
        {
            var element = containerElement.rendering();
            BaseElementXmlConvertor.Export(element, rendering);
            element.formId = rendering.formId;
            element.associationFieldId = rendering.associationFieldId;
            element.indexFormId = rendering.indexFormId;
        }
    }

    public static void Import(ElasticObject containerElement, UserTask userTask)
    {
        userTask.rendering = null;
        foreach (var element in containerElement?.GetElements("rendering") ?? Enumerable.Empty<ElasticObject>())
        {
            var formId = element.GetString("formId");
            var associationFieldId = element.GetString("associationFieldId");
            var indexFormId = element.GetString("indexFormId");
            var rendering = new RenderingForm(userTask, "", formId, associationFieldId, indexFormId);
            BaseElementXmlConvertor.Import(element, rendering);
            userTask.rendering ??= [];
            userTask.rendering.Add(rendering);
        }

        if (userTask.rendering == null)
        {
            userTask.rendering = [];
            var rendering = new RenderingForm(userTask, "", null, null, null);
            userTask.rendering.Add(rendering);
        }
    }
}
