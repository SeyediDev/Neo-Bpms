namespace Neo.Bpms.Infrastructure.Utility.Interfaces;
public class ExternalLibrariesServiceOperationInterface : InterfaceRuntime
{
    public override bool JsonFittingRequirement => true;

    public override Task<bool> RunOperation(IOperationUserParams userParams, LocalParameters inputData,
        string operationImplementationRef,
        Action<LocalParameters, IOperationUserParams, bool> done,
        Action<string, LocalParameters, IOperationUserParams, bool> fail,
        Func<IOperationUserParams, OperationResourceRuntime, bool> tryAllocateResource,
        Action<IOperationUserParams> takeBackToQueue, Action<IOperationUserParams> startedCallBack)
    {
        return Task.FromResult(false);
    }

    public override int IdleResourcesCount(string operationName)
    {
        return 0;
    }

    protected override OperationResourceRuntime GetReadyResource(IOperationUserParams userParams)
    {
        return null;
    }
}
