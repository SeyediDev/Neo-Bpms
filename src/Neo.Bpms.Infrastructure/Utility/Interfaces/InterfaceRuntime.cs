namespace Neo.Bpms.Infrastructure.Utility.Interfaces;
public abstract class InterfaceRuntime
{
    public abstract bool JsonFittingRequirement { get; }

    public abstract Task<bool> RunOperation(IOperationUserParams userParams, LocalParameters inputData,
        string operationImplementationRef,
        Action<LocalParameters, IOperationUserParams, bool> done,
        Action<string, LocalParameters, IOperationUserParams, bool> fail,
        Func<IOperationUserParams, OperationResourceRuntime, bool> tryAllocateResource,
        Action<IOperationUserParams> takeBackToQueue,
        Action<IOperationUserParams> startedCallBack);

    public abstract int IdleResourcesCount(string operationName);
    protected abstract OperationResourceRuntime GetReadyResource(IOperationUserParams userParams);
}
