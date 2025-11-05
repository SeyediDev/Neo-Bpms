using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.GlobalTasks;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.SubProcess;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeGlobalTask;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository
{
    public bool LoadBpmnDefinitions(BusinessProcess businessProcess,
        BusinessProcessVersion businessProcessVersion, bool forceObsolete = false)
    {
        BpmnDefinitions bpmnDefinitions = businessProcessVersion.BpmnDefinitions;
        if (bpmnDefinitions == null)
        {
            return false;
        }

        foreach (RootElement rootElement in bpmnDefinitions.GetRootElements())
        {
            LoadRootElement(businessProcess, businessProcessVersion, rootElement, forceObsolete);
        }

        return true;
    }

    private void LoadRootElement(BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        RootElement rootElement, bool forceObsolete)
    {
        switch (rootElement)
        {
            case Process process:
                LoadProcess(businessProcess, businessProcessVersion, forceObsolete, process);
                break;
            case GlobalTask globalTask:
                LoadGlobalTask(businessProcessVersion, globalTask);
                break;
        }
    }

    private void LoadGlobalTask(BusinessProcessVersion businessProcessVersion, GlobalTask globalTask)
    {
        GlobalTaskRunTime globalTaskRunTime;
        if (globalTasks.ContainsKey(globalTask.Id))
        {
            globalTaskRunTime = globalTasks[globalTask.Id];
        }
        else
        {
            globalTaskRunTime = new GlobalTaskRunTime();
            globalTasks.Add(globalTask.Id, globalTaskRunTime);
        }

        if (globalTaskRunTime.versions.ContainsKey(businessProcessVersion.Id))
        {
            globalTaskRunTime.versions[businessProcessVersion.Id].WithdrawObsolete();
            globalTaskRunTime.versions[businessProcessVersion.Id] = GetNewGlobalTask(globalTask);
        }
        else
        {
            GlobalTaskVersionRuntime globalTaskVersion =
                GetNewGlobalTask(globalTask);
            globalTaskRunTime.versions.Add(businessProcessVersion.Id, globalTaskVersion);
        }
    }

    private static bool FlowElementsContainsItem(IFlowElementsContainer container, string itemId)
    {
        return (container.flowElements?.ContainsKey(itemId) ?? false) || (container.flowElements.Values?.OfType<SubProcess>() ?? [])
            .Any(subProcess => FlowElementsContainsItem(subProcess, itemId));
    }

    private GlobalTaskVersionRuntime GetNewGlobalTask(GlobalTask globalTask)
    {
        return globalTask.taskType switch
        {
            GlobalTask.eGlobalTaskType.Script => new GlobalScriptTaskRuntime { taskDefinition = globalTask as GlobalScriptTask, repository = this },
            GlobalTask.eGlobalTaskType.BusinessRule => new GlobalBusinessRuleTaskRuntime
            {
                taskDefinition = globalTask as GlobalBusinessRuleTask,
                repository = this
            },
            GlobalTask.eGlobalTaskType.User => new GlobalUserTaskRuntime { taskDefinition = globalTask as GlobalUserTask, repository = this },
            //case GlobalTask.eGlobalTaskType.Manual:
            _ => new GlobalManualTaskRuntime { taskDefinition = globalTask as GlobalManualTask, repository = this },
        };
    }
}
