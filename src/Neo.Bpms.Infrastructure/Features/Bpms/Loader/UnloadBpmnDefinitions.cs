using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Entities.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.GlobalTasks;
using Neo.Bpms.Infrastructure.Features.Bpms.Loader.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeGlobalTask;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Loader;

public partial class Repository
{
    public bool UnloadBpmnDefinitions(BusinessProcessVersion businessProcessVersion)
    {
        BpmnDefinitions definitions = businessProcessVersion.BpmnDefinitions;
        foreach (RootElement rootEl in definitions.GetRootElements())
        {
            switch (rootEl)
            {
                case Process _ when !processesRunTimes.ContainsKey(rootEl.Id):
                    continue;
                case Process _:
                    {
                        ProcessRunTime process = processesRunTimes[rootEl.Id];
                        if (!process.Versions.ContainsKey(businessProcessVersion.Id))
                        {
                            continue;
                        }

                        ProcessVersionRuntime processVersion = process.Versions[businessProcessVersion.Id];
                        deleteCatchEvents(processVersion);
                        foreach (KeyValuePair<string, FlowNodeRunTime> item in processVersion.nodes)
                        {
                            item.Value.WithdrawObsolete();
                        }
                        processVersion.nodes.Clear();
                        _ = process.Versions.TryRemove(businessProcessVersion.Id, out _);
                        if (process.Versions.IsEmpty)
                        {
                            _ = processesRunTimes.TryRemove(rootEl.Id, out _);
                        }

                        break;
                    }

                case GlobalTask _ when !globalTasks.ContainsKey(rootEl.Id):
                    continue;
                case GlobalTask _:
                    {
                        GlobalTaskRunTime globalTask = globalTasks[rootEl.Id];
                        if (!globalTask.versions.ContainsKey(businessProcessVersion.Id))
                        {
                            continue;
                        }

                        globalTask.versions[businessProcessVersion.Id].WithdrawObsolete();
                        _ = globalTasks.Remove(businessProcessVersion.Id);
                        if (globalTask.versions.Count == 0)
                        {
                            _ = globalTasks.Remove(rootEl.Id);
                        }

                        break;
                    }
            }

        }
        return true;
    }
    private void ProcessWithdrawObsolete(bool forceObsolete, Process process, ProcessVersionRuntime processVersion)
    {
        deleteCatchEvents(processVersion);
        if (forceObsolete)
        {
            foreach (KeyValuePair<string, FlowNodeRunTime> item in processVersion.nodes)
            {
                item.Value.WithdrawObsolete();
            }

            processVersion.nodes.Clear();
        }
        else
        {
            foreach (KeyValuePair<string, FlowNodeRunTime> item in processVersion.nodes)
            {
                WithdrawObsoleteIfRemoved(process, processVersion, item);
            }

            foreach (FlowElement fel in process.flowElements.Values)
            {
                if (processVersion.TryGetFlowNodeRuntime(fel.Id, out _))
                {
                    _ = processVersion.nodes.Remove(fel.Id);
                }
            }
        }
    }
    private static void WithdrawObsoleteIfRemoved(Process process,
        ProcessVersionRuntime processVersion, KeyValuePair<string, FlowNodeRunTime> item)
    {
        if (FlowElementsContainsItem(process, item.Key))
        {
            return;
        }

        item.Value.WithdrawObsolete();
        _ = processVersion.nodes.Remove(item.Key);
    }
    private void deleteCatchEvents(ProcessVersionRuntime processVersion)
    {
        foreach (KeyValuePair<string, MessageCatches> messageCatch in MessagesCatches)
        {
            List<MessageCatchRuntimeLink> messageCatchesItems = messageCatch.Value.Catches.ToList();
            foreach (MessageCatchRuntimeLink item in messageCatchesItems)
            {
                if (item.MessageCatchRuntime.ProcessVersion == processVersion)
                {
                    _ = messageCatch.Value.Catches.Remove(item);
                }
            }
        }

        processVersion.UserTasks.Clear();
        List<KeyValuePair<string, List<SignalCatchRuntime>>> signalCatchesItems = signalCatches.ToList();
        foreach (KeyValuePair<string, List<SignalCatchRuntime>> item in signalCatchesItems)
        {
            _ = item.Value.RemoveAll(c => c.ProcessVersion == processVersion);
        }
    }
}
