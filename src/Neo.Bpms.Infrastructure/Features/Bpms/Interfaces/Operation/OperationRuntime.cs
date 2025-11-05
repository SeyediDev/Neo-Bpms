namespace Neo.Bpms.Infrastructure.Features.Bpms.Interfaces.Operation;

public class OperationRuntime(InterfaceRuntime interfaceRuntime, string operationImplementationRef)
{
    protected InterfaceRuntime InterfaceRuntime { get; set; } = interfaceRuntime;
    protected string OperationImplementationRef { get; set; } = operationImplementationRef;
    protected ConcurrentDictionary<long, OperationResourceRuntime> AllocatedResources { get; set; } = new ConcurrentDictionary<long, OperationResourceRuntime>();
    protected readonly object _allocationLock = new();

    public virtual async Task Run(object obj, LocalParameters inputData)
    {
        RunOperationCallBackParams callbackParams = new(OperationImplementationRef, 0, "", null);
        if (InterfaceRuntime != null)
        {
            bool isWorking = await InterfaceRuntime.RunOperation(callbackParams, inputData,
                OperationImplementationRef, Done, Fail, TryAllocateResource, TakeBackToQueue,
                StartedCallBack);
            if (isWorking)
            {
            }
        }
        else
        {
            Fail("NoOperator", inputData, callbackParams, true);
        }
    }

    public OperationResourceRuntime GetAllocatedResource(long aiId)
    {
        lock (_allocationLock)
        {
            return AllocatedResources.GetItem(aiId);
        }
    }

    public void AsyncRunTaskFromQueue(bool waitingForItsOwnMachine)
    {
        Task.Factory.StartNew(() => RunTaskFromQueue(waitingForItsOwnMachine), CancellationToken.None,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public virtual void RunTaskFromQueue(bool waitingForItsOwnMachine)
    {
    }

    protected void Free(long allocatedResourceId)
    {
        lock (_allocationLock)
        {
            if (AllocatedResources.TryRemove(allocatedResourceId, out OperationResourceRuntime allocatedResource))
            {
                allocatedResource?.Free();
            }
        }
    }

    protected virtual void Done(LocalParameters receivedMessage, IOperationUserParams callBackParams,
        bool immediate)
    {
        if (callBackParams is not RunOperationCallBackParams)
        {
            return;
        }
        //...
        AsyncRunTaskFromQueue(true);
    }

    protected virtual void Fail(string errorCode, LocalParameters receivedMessage,
        IOperationUserParams callBackParams, bool immediate)
    {
        if (callBackParams is not RunOperationCallBackParams)
        {
            return;
        }
        //...
        AsyncRunTaskFromQueue(true);
    }

    protected virtual void TakeBackToQueue(IOperationUserParams callBackParams)
    {
        if (callBackParams is not RunOperationCallBackParams)
        {
            return;
        }
        //...
        AsyncRunTaskFromQueue(true);
    }

    protected virtual void StartedCallBack(IOperationUserParams callBackParams)
    {
    }

    protected virtual bool TryAllocateResource(IOperationUserParams callBackParams, OperationResourceRuntime resource)
    {
        return callBackParams is RunOperationCallBackParams;
    }
}
