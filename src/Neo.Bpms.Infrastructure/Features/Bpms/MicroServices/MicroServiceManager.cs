global using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.Definitions;
global using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices.MonitoringInfo;
using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces;
using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces.Operation;
using RestSharp;

namespace Neo.Bpms.Infrastructure.Features.Bpms.MicroServices;

public class MicroServiceManager : InterfaceRuntime
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    private readonly MicroservicesDefinitions _microServicesDefinitions;
    private long _currentRequestId;
    private Timer _statusCheckerTimer;
    private Action<IOperationUserParams> StartedCallBack;
    private readonly object _theLock = new();
    public override bool JsonFittingRequirement => true;

    public MicroServiceManager() //: base(null)
    {
        _microServicesDefinitions = new MicroservicesDefinitions();
        _currentRequestId = 1;
        InstantiateTimers();
    }

    public IEnumerable<SingleResourceInfo> GetResourcesInfo()
    {
        return _microServicesDefinitions.GetResourcesInfo();
    }

    public override Task<bool> RunOperation(IOperationUserParams userParams,
        LocalParameters inputData,
        string operationImplementationRef,
        Action<LocalParameters, IOperationUserParams, bool> done,
        Action<string, LocalParameters, IOperationUserParams, bool> fail,
        Func<IOperationUserParams, OperationResourceRuntime, bool> tryAllocateResource,
        Action<IOperationUserParams> takeBackToQueue,
        Action<IOperationUserParams> startedCallBack)
    {
        StartedCallBack = startedCallBack;
        //	        if (BpmnInterface == null)
        //	            BpmnInterface = (Interface) operationRef.parent; //for default constructor
        bool isWorking = false;

        ServiceResource resource;
        lock (_theLock)
        {
            resource = (ServiceResource)GetReadyResource(userParams);
            if (resource == null)
            {
                LoggerError("Input Data", inputData); // todo why!?
                return Task.FromResult(isWorking);
            }

            ChangeRequestId();
            if (!tryAllocateResource(userParams, resource))
            {
                Logger.LogInformation("already allocated {0}", userParams.ToString());
                return Task.FromResult(isWorking);
            }

            resource.AssignWork(_currentRequestId, done, fail, takeBackToQueue, userParams);
        }

        Request(inputData, resource, userParams, out isWorking);
        return Task.FromResult(isWorking);
    }

    public ServiceResource GetResource(string machineId, string funcName, int resourceIndex)
    {
        return _microServicesDefinitions.GetSpecificResource(machineId, funcName, resourceIndex);
    }

    public bool TryRegister(string machineId, IEnumerable<FunctionInMachineDefinition> functions,
        bool reRegister)
    {
        lock (string.Intern("ServiceManagerRegister-" + machineId))
        {
            if (reRegister && _microServicesDefinitions.HasMachine(machineId))
            {
                return false;
            }

            _microServicesDefinitions.Register(machineId, functions);
            return true;
        }
    }

    public void Notify(ServiceResource resource, string detail, long? code, string data,
        StateNotifyStatus status, int? progress)
    {
        if (progress != null)
        {
            resource.UpdateProgress(progress.Value, detail);
        }

        LocalParameters output = [];
        //TODO if (!string.IsNullOrEmpty(data))
        //    JsonBinder.JsonBinder.Bind(data, output);
        switch (status)
        {
            case StateNotifyStatus.error:
                LogNotifyError(resource, detail, code, data);
                _ = output.AddOrUpdate("___ErrorDetails", detail);
                resource.FailWork(CalculateErrorCode(code), output, false);
                break;
            case StateNotifyStatus.done:
                LogNotifyDone(resource, data);
                resource.FinishWork(output, false);
                break;
        }
    }

    public bool TryUpdateAlivedTime(string machineId)
    {
        if (!(_microServicesDefinitions.MachineDefinitions?.TryGetValue(machineId, out ServiceMachineDefinition machine) ?? false))
        {
            return false;
        }

        machine.UpdateAlivedTime();
        return true;
    }

    public override int IdleResourcesCount(string operationName)
    {
        return _microServicesDefinitions.IdleResourcesCount(operationName);
    }

    public void RunFromQueue(string machineId)
    {
        IEnumerable<FunctionInMachineDefinition> functions = _microServicesDefinitions.GetFunctionsByMachineId(machineId);
        foreach (FunctionInMachineDefinition function in functions)
        {
            ((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).RunServiceTasksFromQueue(GetType().Name, function.FuncName,
                function.IdleResourcesCount(), machineId, false);
        }
    }

    private void InstantiateTimers()
    {
        InstantiateStatusChecker();
    }

    private void InstantiateStatusChecker() // todo think about relationship between these two timers
    {
        if (_statusCheckerTimer != null)
        {
            return;
        }

        MicroservicesStatusChecker statusChecker = new(_microServicesDefinitions);
        _statusCheckerTimer = new Timer(statusChecker.CheckStatus, new AutoResetEvent(false),
            TimeSpan.FromMinutes(1.5), TimeSpan.FromMinutes(1));
    }

    private void Request(LocalParameters Params,
        ServiceResource resource, IOperationUserParams userParams, out bool isWorking)
    {
        isWorking = false;
        LocalParameters output = [];
        RestResponse response = SendHttpRequest(Params, AcquireUrl(resource.Function, resource));
        if (response == null)
        {
            resource.FailWork("runtimeerror", output, true);
            return;
        }

        if (!response.IsSuccessful())
        {
            if (!string.IsNullOrEmpty(response.Content))
            {
                //JsonBinder.JsonBinder.Bind(response.Content, output);
                _ = output.AddOrUpdate("___ErrorDetails", output.GetString("detail"));
            }

            GatherAdditionalResponseOutputs(response, output);
            if (response.WasUnableToConnect())
            {
                resource.HadUnsuccessfulConnection();
                resource.TakeBackToQueue();
            }
            else
            {
                resource.FailWork(CalculateErrorCode(output.Get("code") ?? output.Get("StatusCode")), output,
                    true);
            }

            return;
        }

        if (!resource.Function.Immediate)
        {
            StartedCallBack(userParams);
            isWorking = true;
            return;
        }

        //JsonBinder.JsonBinder.BindData(response.Content, output);
        GatherAdditionalResponseOutputs(response, output);

        resource.FinishWork(output, true);
    }

    protected override OperationResourceRuntime GetReadyResource(IOperationUserParams userParams)
    {
        ServiceMachineDefinition readyMachine =
            _microServicesDefinitions.GetFirstReadyMachine(userParams.OperationName, userParams.PreferredMachineId);
        if (readyMachine == null)
        {
            LogNoResource(userParams.OperationName);
            return null;
        }

        Logger.LogTrace("{0} machine {1}", userParams.ToString(), readyMachine.MachineId);
        return readyMachine.GetFunction(userParams.OperationName).GetFirstReadyResource();
    }

    private static void GatherAdditionalResponseOutputs(RestResponse response, LocalParameters output)
    {
        output["Cookies"] = response.Cookies; //todo are they needed?
        output["ErrorMessage"] = response.ErrorMessage;
        output["Headers"] = response.Headers;
        output["Server"] = response.Server;
        output["StatusCode"] = response.StatusCode;
        output["StatusDescription"] = response.StatusDescription;
        output["ErrorException"] = response.ErrorException;
    }

    private string CalculateErrorCode(object code)
    {
        //			string code = string.Empty;
        //			if (output.ContainsKey("code"))
        //				code = output["code"]?.ToString();
        //			else
        //				code = output["StatusCode"]?.ToString();
        return $"service.{code}";
    }

    private string AcquireUrl(FunctionInMachineDefinition functionInMachine, ServiceResource resource)
    {
        return $"{functionInMachine.Url}/{resource.ResourceIndex + 1}/{resource.CurrentRequestId}/{resource.UserParams?.Id}";
        //	        return $"{functionInMachine.Url}/{resource.ResourceIndex + 1}/{_currentRequestId}";
    }

    private static RestResponse SendHttpRequest(LocalParameters Params, string url)
    {
        RestRequest request = new(url, RestSharp.Method.Post);
        _ = request.AddHeader("cache-control", "no-cache");
        _ = request.AddHeader("Content-type", "application/json");


        if (Params != null)
        {
            request.AddParameter("application/json", Params.ToJson(), ParameterType.RequestBody);
        }
        //			    request.AddJsonBody(Params);							
        try
        {
            RestClient client = new(url);
            LogBeforeRequest(url);
            RestResponse response = client.Execute(request);
            LogResponse(request, client, response, url);
            return response;
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
            return null;
        }
    }

    //private static LocalParameters GatherInputParameters(ActivityInstance ai, LocalParameters inputData,
    //       Operation operationRef)
    //   {
    //   }

    private void ChangeRequestId()
    {
        _ = Interlocked.Increment(ref _currentRequestId);
    }

    private static void LogResponse(RestRequest request, RestClient client, RestResponse response, string url)
    {
        Logger.LogInformation("The request with parameters {0} sent to {1}",
            request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody), url);
        Logger.LogInformation("The response with status code {0} {1} had this content: {2} for {3}", response.StatusCode,
            response.ErrorMessage, response.Content, url);
    }

    private static void LogBeforeRequest(string url)
    {
        Logger.LogInformation("Ready to call {0}", url);
    }

    private static void LogNoResource(string functionName)
    {
        Logger.LogInformation("There is no available machine/resource for {0}", functionName);
    }

    private static void LogNotifyDone(ServiceResource resource, string data)
    {
        Logger.LogInformation("Url {0} finished {1} request output {2}", resource.Function.Url, resource.CurrentRequestId, data);
    }

    private static void LogNotifyError(ServiceResource resource, string detail, long? code, string data)
    {
        Logger.LogInformation("Url {0} notified {1} error detail {2} code {3} data {4}",
            resource.Function.Url, resource.CurrentRequestId, detail, code, data);
    }

    private static void LoggerError(string title, object inputData)
    {
        Logger.LogError("{0} : {1}", title, inputData);
    }
}
