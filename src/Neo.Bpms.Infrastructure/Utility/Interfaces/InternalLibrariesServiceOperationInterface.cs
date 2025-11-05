using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Infrastructure.Utility.Interfaces;
public class InternalLibrariesServiceOperationInterface : InterfaceRuntime
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    private static IServiceProvider _serviceProvider;
    // todo set in ConfigureServices in ContentPool and bpms projects!
    public static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public override bool JsonFittingRequirement => false;

    public override async Task<bool> RunOperation(IOperationUserParams userParams,
        LocalParameters inputData,
        string operationImplementationRef,
        Action<LocalParameters, IOperationUserParams, bool> done,
        Action<string, LocalParameters, IOperationUserParams, bool> fail,
        Func<IOperationUserParams, OperationResourceRuntime, bool> tryAllocateResource,
        Action<IOperationUserParams> takeBackToQueue,
        Action<IOperationUserParams> startedCallBack)
    {
        var isWorking = false;
        var outParams = new LocalParameters();
        var runSucceed = true;
        var errorCode = "";
        try
        {
            var name = operationImplementationRef;
            if (ProjectDefinition.Project.ServiceInterfaces.GetUsingOperation("bpms://",
                ServiceInterfaceProtocol.InternalLibrary, name) is not IInternalServiceOperation op)
                throw new Exception($"Invalid Operation {name}");
            if (_serviceProvider == null || _serviceProvider.GetService(op.RunTimeType) is not IInternalServiceOperationRuntime operationRuntime)
            {
                throw new Exception($"{op.RunTimeType} is not configured or is of wrong type");
            }
            outParams = await operationRuntime.RunOperation(userParams.AuditTrail, inputData);
        }
        catch (ServiceOperationException ex)
        {
            errorCode = ex.ErrorCode;
            outParams.AddOrUpdate("___ErrorDetails", ex.Message);
            runSucceed = false;
        }
        catch (ArgumentException ex)
        {
            errorCode = "ArgumentException";
            outParams.AddOrUpdate("___ErrorDetails", ex.Message);
            runSucceed = false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            errorCode = "Failed";
            outParams.AddOrUpdate("___ErrorDetails", ex.Message);
            runSucceed = false;
        }

        if (!runSucceed)
        {
            fail($"library.{errorCode}", outParams, userParams, true);
        }
        else
            done(outParams, userParams, true);
        await Task.CompletedTask;
        return isWorking;
    }

    public override int IdleResourcesCount(string operationName)
    {
        return 1; // todo!
    }

    protected override OperationResourceRuntime GetReadyResource(IOperationUserParams userParams)
        => throw new NotImplementedException();
}
