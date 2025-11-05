using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Services;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

public class BPMNOperationRuntime : OperationRuntime
{
    internal BPMNOperationRuntime(ActivityRuntime runtime)
        : base(
            ((Interface)(runtime.flowNode as IOperationContainer)?.operationRef?.Parent)?.implementation as
            InterfaceRuntime,
            (runtime.flowNode as IOperationContainer)?.operationRef?.implementationRef)
    {
        Runtime = runtime;
    }

    private ActivityRuntime Runtime { get; set; }
    private IOperationContainer OperationContainer => Runtime.flowNode as IOperationContainer;

    public override async System.Threading.Tasks.Task Run(object obj, LocalParameters inputData)
    {
        if (obj is not ActivityInstance ai)
        {
            return;
        }

        BPMNRunOperationCallBackParams callbackParams = new() { PI = ai.pi, AI = ai };
        if (InterfaceRuntime != null)
        {
            LocalParameters inputParams = CreateInputData(ai, inputData,
                OperationContainer.operationRef?.inMessageRef?.itemRef,
                InterfaceRuntime.JsonFittingRequirement);
            bool isWorking = await InterfaceRuntime.RunOperation(callbackParams, inputParams,
                OperationImplementationRef, Done, Fail, TryAllocateResource, TakeBackToQueue,
                StartedCallBack);
            if (isWorking)
            {
                if (ai.userTaskState != UserTaskInstanceStateId.Started)
                {
                    ai.LogError($"is working but userTask is wrong {ai.userTaskState} {ai.Id}");
                }

                ai.Save("Run");
            }
        }
        else
        {
            Fail("NoOperator", inputData, callbackParams, true);
        }
    }

    public override void RunTaskFromQueue(bool waitingForItsOwnMachine)
    {
        Interface @interface = (Interface)OperationContainer.operationRef?.Parent;
        if (@interface == null)
        {
            return;
        }

        ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).RunServiceTasksFromQueue(@interface.Name, OperationContainer.operationRef.Name,
            InterfaceRuntime?.IdleResourcesCount(OperationContainer.operationRef.Name) ?? 1,
            null, waitingForItsOwnMachine);
    }

    internal void Free(FlowNodeInstance ai)
    {
        Free(ai.Id);
    }

    internal void RunOperationFromQueue(ActivityInstance ai)
    {
        ai.pi.Execution.AddJob(new RunOperationFromQueueJob
        {
            BPMNOperationRuntime = this
        });
    }

    protected override void Done(LocalParameters receivedMessage, IOperationUserParams callBackParams,
        bool immediate)
    {
        if (callBackParams is not BPMNRunOperationCallBackParams cbParams)
        {
            return;
        }

        ActivityInstance ai = cbParams.AI;
        if (IsCompleted(ai))
        {
            ai.LogError(
                $"Done operation s: {ai.state} uts: {ai.userTaskState} c: {ai.Closed} ps:{ai.pi.state}");
        }

        Runtime.Complete(ai, receivedMessage);
        ai.pi.Execution.DoJobs();
        AsyncRunTaskFromQueue(true);
    }

    protected override void Fail(string errorCode, LocalParameters receivedMessage,
        IOperationUserParams callBackParams, bool immediate)
    {
        if (callBackParams is not BPMNRunOperationCallBackParams cbParams)
        {
            return;
        }

        ActivityInstance ai = cbParams.AI;
        if (IsCompleted(ai))
        {
            ai.LogError(
                $"fail operation s: {ai.state} uts: {ai.userTaskState} c: {ai.Closed} ps:{ai.pi.state}");
        }

        _ = receivedMessage.AddOrUpdate("___Failed", true);
        ai.Fail();
        ai.Save($"Fail : {errorCode} {receivedMessage.GetString("___ErrorDetails")}");
        TryCatchEvent<ErrorEventDefinition>.Try(errorCode, receivedMessage, ai.pi, ai);
        ai.pi.Execution.DoJobs();
        AsyncRunTaskFromQueue(true);
    }

    protected override void TakeBackToQueue(IOperationUserParams callBackParams)
    {
        if (callBackParams is not BPMNRunOperationCallBackParams cbParams)
        {
            return;
        }

        ActivityInstance ai = cbParams.AI;
        if (IsCompleted(ai))
        {
            ai.LogError($"tbtq {ai.Id} s: {ai.state} uts: {ai.userTaskState} c: {ai.Closed} ps:{ai.pi.state}");
        }

        ai.ReBorn();
        ai.Save("Returned to Queue");
        ai.pi.Execution.DoJobs();
        AsyncRunTaskFromQueue(true);
    }

    protected override void StartedCallBack(IOperationUserParams callBackParams)
    {
        if (callBackParams is not BPMNRunOperationCallBackParams cbParams)
        {
            return;
        }

        ActivityInstance ai = cbParams.AI;
        ai.Start();
    }

    protected override bool TryAllocateResource(IOperationUserParams callBackParams,
        OperationResourceRuntime resource)
    {
        if (callBackParams is not BPMNRunOperationCallBackParams cbParams)
        {
            return false;
        }

        ActivityInstance ai = cbParams.AI;
        if (ai == null)
        {
            return false;
        }

        lock (_allocationLock)
        {
            if (AllocatedResources.ContainsKey(ai.Id))
            {
                return false;
            }

            if (!AllocatedResources.TryAdd(ai.Id, resource))
            {
                return false;
            }

            ai.AllocateToASingleResource();
            ai.MachineId = resource.GetMachineId();
            return true;
        }
    }

    private static bool IsCompleted(ActivityInstance ai)
    {
        return ai.state >= ActivityInstanceStateId.Completed || ai.Closed ||
               ai.pi.state >= ProcessInstanceStateId.Completed ||
               ai.userTaskState >= UserTaskInstanceStateId.Completed;
    }

    private static LocalParameters CreateInputData(IPropertyValueContainer ai,
        LocalParameters inputData, IStructureDefinition structure, bool jsonFitting)
    {
        LocalParameters inParams;
        Entity inMessageElement = structure?.structure;
        if (inMessageElement != null)
        {
            inParams = [];
            foreach (KeyValuePair<string, EntityField> item in inMessageElement.entityFields)
            {
                if (inputData != null && inputData.ContainsKey(item.Key))
                {
                    _ = inParams.AddOrUpdate(item.Key,
                        jsonFitting ? GetJsonFittingValue(inputData[item.Key]) : inputData[item.Key]);
                }
                else
                {
                    if (ai.GetData(item.Key, out object value))
                    {
                        _ = inParams.AddOrUpdate(item.Key,
                            value != null ? jsonFitting ? GetJsonFittingValue(value) : value : null);
                    }
                }
            }
        }
        else
        {
            if (jsonFitting)
            {
                inParams = [];
                foreach (KeyValuePair<string, object> input in inputData)
                {
                    _ = inParams.AddOrUpdate(input.Key, GetJsonFittingValue(input.Value));
                }
            }
            else
            {
                inParams = inputData;
            }
        }

        return inParams;
    }

    private static object GetJsonFittingValue(object obj)
    {
        return string.IsNullOrEmpty(obj?.ToString())
            ? null
            : obj;
    }
}
