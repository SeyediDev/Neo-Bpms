using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository
{
    private void LoadProcess(BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion,
        bool forceObsolete, Process process)
    {
        if (process == null)
        {
            return;
        }

        ProcessRunTime processRunTime;
        if (processesRunTimes.TryGetValue(process.Id, out ProcessRunTime value))
        {
            processRunTime = value;
        }
        else
        {
            processRunTime = new ProcessRunTime();
            _ = processesRunTimes.TryAdd(process.Id, processRunTime);
        }

        processRunTime.BusinessProcess = businessProcess;
        if (processRunTime.Versions.TryGetValue(businessProcessVersion.Id, out ProcessVersionRuntime processVersion) &&
            processVersion != null)
        {
            processVersion.BusinessProcessVersion = businessProcessVersion;
            processVersion.definition = process;
            ProcessWithdrawObsolete(forceObsolete, process, processVersion);
        }
        else
        {
            processVersion =
                new ProcessVersionRuntime(process, businessProcessVersion.Id)
                {
                    BusinessProcessVersion = businessProcessVersion
                };
            _ = processRunTime.Versions.TryAdd(businessProcessVersion.Id, processVersion);
        }

        foreach (FlowElement flowElement in process.flowElements.Values)
        {
            AddFlowElement(processVersion, flowElement);
        }

        SetEventBasedGatewayLinks(processVersion);
    }

    private void AddFlowElement(ProcessVersionRuntime processVersion, FlowElement flowElement)
    {
        FlowNodeRunTime result;
        switch (flowElement)
        {
            case CatchEvent catchEvent:
                AddCatchEvent(processVersion, catchEvent);
                return;
            case ThrowEvent throwEvent:
                result = AddThrowEvent(processVersion, throwEvent);
                break;
            case Gateway gateway:
                result = AddGateway(processVersion, gateway);
                break;
            case Activity activity:
                result = AddActivity(processVersion, activity);
                break;
            default:
                return;
        }

        AddFlowElementRuntime(processVersion, flowElement, result);
    }

    private static void AddFlowElementRuntime(ProcessVersionRuntime processVersion,
        FlowElement flowElement, FlowNodeRunTime result)
    {
        if (result != null)
        {
            if (!processVersion.nodes.ContainsKey(result.flowNode.Id))
            {
                processVersion.nodes.Add(result.flowNode.Id, result);
            }
            else
            {
                processVersion.AddFatal($"Duplicate flow element {result.flowNode.Id}, in process {processVersion.definition.Id}",
                    flowElement.Id, "13.0.0", "Duplicate Flow Element Id");
            }
        }
        else
        {
            processVersion.AddFatal($"Could not add flow element {flowElement.Id} in process {processVersion.definition.Id}",
                flowElement.Id, "13.0.1", "Invalid Flow Element");
        }
    }
}
