using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Base;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Core.Infrastructure;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Domain.Models.WorkManagement;
using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.Loader.Dto;
using Neo.Bpms.Infrastructure.Features.Bpms.MetaDataPart;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeActivity.RuntimeTask;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Engine;

public class BpmsEngine: IBpmsEngine
{
    public IBpmsRepository BpmsRepository {  get; set; }
    public Repository Repository => (Repository)BpmsRepository;
    private BPMNJobTimer BpmnJobTimer { get; set; }
    private OperationsMaintainerTimer OperationsMaintainerTimer { get; set; }

    private ILogger<BpmsEngine> Logger { get; set; }
    public BpmsEngine(ILogger<BpmsEngine> logger, IBpmsRepository bpmsRepository)
    {
        BpmsRepository = bpmsRepository;
        Logger = logger;
        Logger.LogTrace("Load bpmnDefinitions...");
    }
    
    public void Load()
    { 
        LoadBaseBpmnDefinitions();
        foreach (BusinessProcess businessProcess in ProjectDefinition.Project.BusinessProcesses.Values)
        {
            foreach (BusinessProcessVersion businessProcessVersion in businessProcess.Versions.Values)
            {
                LoadBpmnDefinitions(businessProcess, businessProcessVersion);
            }
        }

        Logger.LogTrace("load SyncBpmnMetaData...");
        SyncBpmnMetaData();
        //TODO مدیریت جاب که انجام بشه دیگه نیازی به این نیست
        //DataStorage.UnLockInstancesOfThisBPMNEngine();
        //TODO Improvement مدیریت جاب که انجام بشه دیگه نیازی به این نیست
        //DataStorage.SetStartedActivityInstancesToCreated();
        OperationsMaintainerTimer = new OperationsMaintainerTimer([.. Repository.FlowNodeRunTimes.Values.OfType<IBPMNOperationRuntime>()]);

        Logger.LogTrace("BpmnEngine is ready...");
    }

    private static long? _bpmnEngineId { get; set; }
    public long BPMNEngineId
    {
        get
        {
            if (_bpmnEngineId is null)
            {
                string machineName = Environment.MachineName;
                string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                BPMNEngine bpmnEngineRecord = QueryUtility<BPMNEngine>.FirstOrDefault(
                        $"(MachineName=='{machineName}') And (ProcessName=='{processName}')");
                ApplyUtility au = ApplyUtility<BPMNEngine>.New();
                if (bpmnEngineRecord == null)
                {
                    _ = au.Insert(bpmnEngineRecord = new BPMNEngine
                    {
                        MachineName = machineName,
                        ProcessName = processName,
                        StartTime = DateTime.Now
                    });
                }
                else
                {
                    bpmnEngineRecord.StartTime = DateTime.Now;
                    _ = au.Update(bpmnEngineRecord);
                }

                _bpmnEngineId = bpmnEngineRecord.Id;
            }
            return _bpmnEngineId.Value;
        }
        set
        {
            _bpmnEngineId = value;
        }
    }

    private void LoadBaseBpmnDefinitions()
    {
        BusinessProcess businessProcess = new("Base", "Base");
        BusinessProcessVersion businessProcessVersion = new(businessProcess, "Base",
            ProjectDefinition.Project.BpmnDefinitions);
        businessProcess.Versions.Add(businessProcessVersion.Id, businessProcessVersion);
        LoadBpmnDefinitions(businessProcess, businessProcessVersion);
    }

    private void LoadBpmnDefinitions(BusinessProcess businessProcess, BusinessProcessVersion businessProcessVersion)
    {
        _ = Repository.LoadBpmnDefinitions(businessProcess, businessProcessVersion, true);
        ReportBpmnDefinitionErrors(businessProcess, businessProcessVersion);
    }

    public ActivityInstanceRecordDb FetchActivityInstanceRecord(long workItemId)
    {
        return DataStorage.FetchFlowInstanceRecord(workItemId);
    }
    public ProcessInstanceRecordDb FetchProcessInstanceRecord(long processInstanceId)
    {
        return DataStorage.FetchProcessInstanceRecord(processInstanceId);
    }

    private void SyncBpmnMetaData()
    {
        if (ProjectDefinition.DontSyncMetaData)
        {
            return;
        }

        try
        {
            BPMNMetaDataManager metaDataManager = new(Repository);
            metaDataManager.SaveMetadataToDatabase();
            //todo check errors
        }
        catch
        {
            // ignored
        }
    }

    private void ReportBpmnDefinitionErrors(BusinessProcess businessProcess,
        BusinessProcessVersion businessProcessVersion)
    {
        BpmnDefinitions bpmnDefinitions = businessProcessVersion?.BpmnDefinitions;
        if (bpmnDefinitions == null)
        {
            Logger.LogError("Can not load bpmnDefinitions for process {0} version {1}", businessProcess?.Name,
                businessProcessVersion?.Id);
            return;
        }

        if (bpmnDefinitions.ErrorInfos == null)
        {
            return;
        }

        foreach (ErrorInformation errorInfo in bpmnDefinitions.ErrorInfos)
        {
            string errorText = $"{errorInfo.Code} {errorInfo.Text} for {errorInfo.For}";
            switch (errorInfo.Type)
            {
                case eErrorLevel.FatalError:
                    Logger.LogCritical(errorText);
                    break;
                case eErrorLevel.Error:
                    Logger.LogError(errorText);
                    break;
                case eErrorLevel.WarningLevel0:
                case eErrorLevel.WarningLevel1:
                case eErrorLevel.WarningLevel2:
                    Logger.LogWarning(errorText);
                    break;
                case eErrorLevel.Info:
                    Logger.LogInformation(errorText);
                    break;
            }
        }
    }
    public bool DistributeSignal(ExecutionInstance execution, string signalName, LocalParameters signal)
    {
        if (Repository.signalCatches.TryGetValue(signalName, out List<SignalCatchRuntime> signalRuntimeList))
        {
            foreach (SignalCatchRuntime signalRuntime in signalRuntimeList)
            {
                _ = signalRuntime.SignalReceived(execution, signal);
            }
        }

        return true;
    }
    #region Message
    public bool MessageReceived(string messageCode, LocalParameters messageParams, AuditTrail auditTrail)
    {
        Message message = ProjectDefinition.Project.GetMessageByCode(messageCode);
        return message != null && MessageReceivedByMessage(message, messageParams, auditTrail);
    }

    internal bool MessageReceivedByMessage(Message message, LocalParameters messageParams, AuditTrail auditTrail)
    {
        if (!Repository.MessagesCatches.TryGetValue(message.Id, out MessageCatches messageCatches))
        {
            return false;
        }

        ExecutionInstance execution = new(auditTrail, null);
        foreach (MessageCatchRuntimeLink @catch in messageCatches.Catches)
        {
            if (@catch.MessageCatchRuntime.MessageReceived(execution,
                @catch.MessageEventDefinition, messageParams))
            {
                break;
            }
        }

        return true;
    }
    #endregion
    #region Message
    private const int TimeoutMaxWait = 1000; // 1 second
    private const int MachineTimeoutMaxWait = 1200000; // 20 minutes!

    public void RunServiceTasksFromQueue(string interfaceName, string operationName, int count,
                                                    string machineId, bool waitingForItsOwnMachine)
    {
        if (count == 0)
        {
            return;
        }

        int timeout = string.IsNullOrEmpty(machineId) ? TimeoutMaxWait : MachineTimeoutMaxWait;
        if (Monitor.TryEnter(string.Intern($"RunServiceTasksFromQueue-{interfaceName}-{operationName}"), timeout))
        {
            try
            {
                IList<FlowNodeAddressing> tasks = DataStorage.FetchQueuedTasks(interfaceName, operationName, count, machineId,
                                                                        waitingForItsOwnMachine);
                //if(tasks.Count > 0)
                Logger.LogDebug("RunServiceTasksFromQueue operationName:{0}  tasksCount:{1} pm: {2} w: {3} cnt:{4}",
                                operationName, tasks.Count, machineId, waitingForItsOwnMachine, count);
                RunTasks(tasks, machineId);
            }
            finally
            {
                Monitor.Exit(string.Intern($"RunServiceTasksFromQueue-{interfaceName}-{operationName}"));
            }
        }
        else
        {
            Logger.LogDebug("failed to enter {0} {1} {2}", operationName, machineId, count);
        }
    }
    private void RunTasks(IList<FlowNodeAddressing> tasks, string machineId)
    {
        TaskRuntime taskRuntime = null;
        foreach (FlowNodeAddressing task in tasks ?? Enumerable.Empty<FlowNodeAddressing>())
        {
            if (taskRuntime == null || taskRuntime.DbId != task.FlowNodeId)
            {
                taskRuntime = (TaskRuntime)Repository.GetFlowNodeRunTime(task.FlowNodeId);
            }

            if (taskRuntime == null)
            {
                Logger.LogCritical("Doesn't make sense. {0} ", task.FlowNodeId);
                continue;
            }

            Logger.LogInformation("Run task taskId {0} in process {1} form ai {2}", taskRuntime.Activity.Id,
                            taskRuntime.ProcessVersion.definition.Id, task.ActivityInstanceId);
            AuditTrail auditTrail = new(TriggerTypeId.CompleteTaskByMachine,
                                                         taskRuntime.ProcessVersion.definition.Id);
            ExecutionInstance execution = new(auditTrail, null);
            ProcessInstance pi = DataStorage.FetchPi(taskRuntime.ProcessVersion, task.ProcessInstanceId, execution);
            if (pi == null)
            {
                Logger.LogError("pi {0} is null in repository and can not load it.", task.ProcessInstanceId);
                continue;
            }

            if (pi.Locked)
            {
                continue;
            }

            ActivityInstance ai;
            try
            {
                ai = DataStorage.FetchActivityInstance(task.ActivityInstanceId, execution);
                if (ai == null)
                {
                    Logger.LogError("ai {0} is null in repository", task.ActivityInstanceId);
                    continue;
                }
            }
            catch
            {
                Logger.LogError("ai {0} not found in repository", task.ActivityInstanceId);
                continue;
            }

            if (ai.userTaskState != UserTaskInstanceStateId.Created)
            {
                Logger.LogError("ai {0} is not Created and is {1} {2} and pi is {3}", task.ActivityInstanceId,
                                ai.userTaskState, ai.Closed ? "and Closed" : "", ai.pi.state);
            }

            if (ai.Closed)
            {
                Logger.LogTrace("ha ha ha gereftamesh ai {0} is {1} and pi is {2}", task.ActivityInstanceId,
                                ai.userTaskState, ai.pi.state);
            }

            if (taskRuntime is IBPMNOperationRuntime operationRuntime)
            {
                OperationResourceRuntime resourceRuntime = operationRuntime.BPMNOperationRuntime.GetAllocatedResource(ai.Id);
                if (resourceRuntime?.IsWorking() ?? false)
                {
                    Logger.LogCritical("Trying to allocate work in working ai {0}", ai.Id);
                    continue;
                }
            }

            DataStorage.LockProcessInstance(pi, "RSTFQ.0");
            ai.ReBorn();
            ai.MachineId = machineId;
            taskRuntime.CheckDataInputAvailabilityAndStart(ai, true, null);
            execution.DoJobs();
            DataStorage.UnlockProcessInstance(pi, "RSTFQ.1");
            DataStorage.SaveAudit(auditTrail);
        }
    }
    #endregion
    #region UserEvents
    public bool CompleteTaskByUser(AuditTrail auditTrail,
        string processId, string processVersion, string taskId, long workItemId, string workDescription)
    {
        UserTaskRuntime userTask = FetchUserTaskRunTime(processId, processVersion, taskId, workItemId, null);
        LocalParameters localParameters = new(auditTrail?.User);
        return userTask.CompleteWorkItemOfUserTask(auditTrail, workItemId, workDescription, localParameters);
    }

    public bool SetWorkItemStartTime(string taskId, long workItemId, Form form, IdentityUser user, long userGroupId)
    {
        UserTaskRuntime userTask = FetchUserTaskRuntime(taskId, workItemId);
        return userTask.SetWorkItemStartTime(workItemId, form, user, userGroupId);
    }

    public bool CreateProcessInstanceAndCompleteUserTask(AuditTrail auditTrail,
        string processId, string processVersion, string taskId,
        string entityPkv, LocalParameters inputData, string workDescription,
        TimeSpan executionDuration, out long? wid)
    {
        UserTaskRuntime userTask = FetchUserTaskRunTime(processId, processVersion, taskId, null, null);
        return userTask.CreateProcessInstanceAndCompleteUserTask(auditTrail, entityPkv, workDescription,
            executionDuration, inputData, out wid);
    }

    /// <summary>
    /// for save form of start userTask
    /// </summary>
    public bool CreateProcessInstanceAndCreateUserTask(AuditTrail auditTrail,
        string processId, string processVersion, string taskId,
        out long? aiId, string entityPkv, LocalParameters inputData, string workDescription)
    {
        UserTaskRuntime userTask = FetchUserTaskRunTime(processId, processVersion, taskId, null, null);
        ExecutionInstance execution = CreateExecution(auditTrail, workDescription);
        UserTaskInstance wi = userTask.CreateProcessInstanceAndCreateUserTask(execution, entityPkv, inputData);
        execution.DoJobs();
        DataStorage.UnlockProcessInstance(wi.pi, "UE.Unlock.0");
        aiId = wi.Id;
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="processId"></param>
    /// <param name="processVersion"></param>
    /// <param name="user"></param>
    /// <param name="userGroupId"></param>
    /// <param name="inputData"></param>
    /// <param name="workDescription"></param>
    /// <param name="dataOutputs"></param>
    /// <param name="pi"></param>
    /// <returns></returns>
    public bool CreateProcessInstanceAndStartIt(string processId, string processVersion,
        IdentityUser user, long userGroupId, ElasticObject inputData, string workDescription,
        out ElasticObject dataOutputs, out ProcessInstance pi)
    {
        ProcessVersionRuntime versionRuntime = FetchProcessVersionRunTime(processId, processVersion, null, null);
        AuditTrail auditTrail = new(TriggerTypeId.CreateInstanceByUser, user.Id, user, userGroupId);
        ExecutionInstance execution = CreateExecution(auditTrail, workDescription);
        LocalParameters localParameters = inputData?.ToLocalParameters() ?? new LocalParameters(user);
        string entityPkv = versionRuntime.FetchEntityPkv(localParameters);
        FlowNodeRunTime flowNodeRunTime = versionRuntime.GetStarts().FirstOrDefault()
                              ?? versionRuntime.GetNotIncoming().FirstOrDefault();
        pi = versionRuntime.CreateInstance(execution, localParameters, entityPkv, null,
            (flowNodeRunTime as IDataOutputContainer)?.dataOutputs);
        flowNodeRunTime?.ReceiveToken(pi, localParameters);
        execution.DoJobs();
        DataStorage.SaveProcessInstance(pi, null, "UE.1", true, DataStorage.LockChangeRequest.Unlock);
        dataOutputs = new ElasticObject();
        if (pi.Process?.ioSpecification?.dataOutputs != null)
        {
            foreach (DataOutput dataOutput in pi.Process?.ioSpecification?.dataOutputs)
            {
                _ = pi.GetData(dataOutput.Name, out object value);
                _ = dataOutputs.SetField(dataOutput.Name, value);
            }
        }

        DataStorage.SaveAudit(auditTrail);
        return true;
    }

    public bool ChangeUser(string processId, string processVersion, string taskId, long workItemId,
        UserTaskInstanceStateId newState, IdentityUser newUser, long userGroupId, ExecutionInstance execution)
    {
        if (!ChangeAllocatedUser(processId, processVersion, taskId, workItemId,
            newState, newUser, userGroupId, execution))
        {
            return false;
        }

        DataStorage.SaveAudit(execution.AuditTrail);
        return true;
    }

    public bool ChangeUser(AuditTrail auditTrail, string processId, string processVersion, string taskId,
        long workItemId, UserTaskInstanceStateId newState, IdentityUser newUser, long userGroupId)
    {
        ExecutionInstance execution = CreateExecution(auditTrail, null);
        return ChangeAllocatedUser(processId, processVersion, taskId, workItemId,
            newState, newUser, userGroupId, execution);
    }

    private bool ChangeAllocatedUser(string processId, string processVersion,
        string taskId, long workItemId, UserTaskInstanceStateId newState,
        IdentityUser newUser, long userGroupId, ExecutionInstance execution)
    {
        UserTaskRuntime userTask = FetchUserTaskRunTime(processId, processVersion, taskId, workItemId, null);
        return userTask.ChangeUser(execution, workItemId, newState, newUser, userGroupId);
    }

    public bool ChangeStateByUser(AuditTrail auditTrail, string processId, string processVersion, string taskId,
        long workItemId, string workDescription, UserTaskInstanceStateId newState)
    {
        UserTaskRuntime userTask = FetchUserTaskRunTime(processId, processVersion, taskId, workItemId, null);
        //todo check security
        return userTask.ChangeStateByUser(auditTrail, workItemId, workDescription, newState);
    }

    public bool ChangeStateByUser(string processId, string processVersion, string taskId,
        long workItemId, string workDescription, IdentityUser user, long userGroupId, UserTaskInstanceStateId newState)
    {
        AuditTrail auditTrail = new(TriggerTypeId.ChangeStateByUser, user.Id, user, userGroupId);
        if (!ChangeStateByUser(auditTrail, processId, processVersion, taskId,
            workItemId, workDescription, newState))
        {
            return false;
        }

        DataStorage.SaveAudit(auditTrail);
        return true;
    }

    public bool RestoreTask(TaskAddressing task, IdentityUser user, long userGroupId)
    {
        if (task == null)
        {
            return false;
        }

        UserTaskRuntime userTask = FetchUserTaskRunTime(task.ProcessId, task.ProcessVersion, task.TaskId, task.ActivityInstanceId,
            task.ProcessInstanceId);
        LocalParameters localParameters = new(user);
        ActivityInstanceRecordDb air = DataStorage.FetchFlowInstanceRecord(task.ActivityInstanceId);
        ProcessInstanceRecordDb pir = DataStorage.FetchProcessInstanceRecord(task.ProcessInstanceId);
        if (air.ProcessInstanceId != pir.Id)
        {
            throw new Exception("Invalid work item. Error 17.1");
        }

        if (pir.StateId >= (long)ProcessInstanceStateId.Completed)
        {
            if (pir.ParentActivityInstanceId is null or 0)
            {
                throw new Exception("This work item can not restored. Error 17.0");
            }
        }

        if (air.StateId != (long)ActivityInstanceStateId.Completed)
        {
            throw new Exception("Invalid work item. Error 17.2");
        }

        if (air.userTaskStateId != UserTaskInstanceStateId.Completed)
        {
            throw new Exception("Invalid work item. Error 17.3");
        }

        _ = localParameters.AddOrUpdate("_CompletionTime", air.CompletionTime);
        List<ActivityInstanceRecord> nextAirs = QueryUtility<ActivityInstanceRecord>
            .Where($"{nameof(ActivityInstanceRecord.ProcessInstanceId)}=='{pir.Id}'")
            .Where($"{nameof(ActivityInstanceRecord.Id)}!='{air.Id}'")
            .Where($"{nameof(ActivityInstanceRecord.CreationTime)}>=_CompletionTime")
            .Where($"{nameof(ActivityInstanceRecord.StateId)}>=" + (long)ActivityInstanceStateId.Completed)
            .ToList<ActivityInstanceRecord>(localParameters);
        if (nextAirs.Count > 0)
        {
            throw new Exception("This work item can not restored. Error 17.4");
        }

        AuditTrail auditTrail = new(TriggerTypeId.RestoreTask, user?.Id, user, userGroupId);
        ExecutionInstance execution = CreateExecution(auditTrail, null);
        ProcessInstance pi = DataStorage.LoadProcessInstance(userTask.ProcessVersion, pir, execution);
        pi.Activate();
        pi.CloseTime = DateTime.MinValue;
        UserTaskInstance wi = new(air, userTask, pi);
        if (wi == null)
        {
            throw new Exception("This work item can not restored. Error 17.5");
        }

        DataStorage.SaveProcessInstance(pi, wi, "UE.2", false, DataStorage.LockChangeRequest.Lock);
        wi.Activate();
        wi.AllocatedToASingleResource(air.ActualOwnerId, air.UserGroupId);
        wi.Save();
        Dictionary<long, FlowNodeInstance> instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(pi);
        if (instances != null)
        {
            foreach (FlowNodeInstance instance in instances.Values.Where(i => !i.Closed && i.CreationTime >= air.CompletionTime))
            {
                if (instance is ActivityInstance aI)
                {
                    aI.Withdrawn();
                    aI.Save();
                }
                else
                {
                    instance.Cancel();
                    instance.Save();
                }
            }
        }

        execution.DoJobs();
        DataStorage.UnlockProcessInstance(pi, "UE.Unlock.1");
        DataStorage.SaveAudit(auditTrail);
        return true;
    }

    private UserTaskRuntime FetchUserTaskRunTime(string processId, string processVersion,
        string taskId, long? aiId, long? piId)
    {
        ProcessVersionRuntime versionRuntime = FetchProcessVersionRunTime(processId, processVersion, aiId, piId);
        UserTaskRuntime userTask = versionRuntime.UserTasks.GetItem(taskId);
        return userTask ?? throw new Exception(
                $"Invalid Task {taskId} In Process {versionRuntime.BusinessProcessVersion.Id} Version {versionRuntime.VersionNo}.");
    }

    private UserTaskRuntime FetchUserTaskRuntime(string taskId, long? aiId)
    {
        ProcessVersionRuntime versionRuntime = FetchProcessVersionRuntime(aiId, 0);
        UserTaskRuntime userTask = versionRuntime.UserTasks.GetItem(taskId);
        return userTask ?? throw new Exception(
                $"Invalid Task {taskId} In Process {versionRuntime.BusinessProcessVersion.Id} Version {versionRuntime.VersionNo}.");
    }

    private ProcessVersionRuntime FetchProcessVersionRunTime(string processId,
        string processVersion, long? aiId, long? piId)
    {
        ProcessRunTime processRuntime = Repository.GetProcessRuntime(processId);
        if (processRuntime == null && !string.IsNullOrEmpty(processId))
        {
            throw new Exception($"Invalid Process {processId}.");
        }

        ProcessVersionRuntime versionRuntime;
        if (processRuntime == null || string.IsNullOrEmpty(processVersion))
        {
            versionRuntime = FetchProcessVersionRuntime(aiId, piId);
            if (versionRuntime == null && processRuntime != null && string.IsNullOrEmpty(processVersion))
            {
                versionRuntime = processRuntime.ActiveVersion;
            }
        }
        else
        {
            versionRuntime = processRuntime.GetVersion(processVersion);
        }

        return versionRuntime ?? throw new Exception($"Invalid Version {processVersion} In Process {processId}.");
    }

    private ProcessVersionRuntime FetchProcessVersionRuntime(long? aiId, long? piId)
    {
        if ((aiId ?? 0) > 0)
        {
            ActivityInstanceRecordDb air = DataStorage.FetchFlowInstanceRecord(aiId.Value);
            FlowNodeRunTime flowNodeRunTime = Repository.GetFlowNodeRunTime(air.BPMNFlowNodeId);
            if (flowNodeRunTime == null)
            {
            }

            ProcessVersionRuntime versionRuntime = flowNodeRunTime?.ProcessVersion;
            if (versionRuntime != null)
            {
                return versionRuntime;
            }
        }
        else if ((piId ?? 0) > 0)
        {
            ProcessInstanceRecordDb pir = DataStorage.FetchProcessInstanceRecord(piId.Value);
            ProcessVersionRuntime versionRuntime = Repository.GetProcessVersionRuntimeByDbId(pir.ProcessVersionId);
            if (versionRuntime != null)
            {
                return versionRuntime;
            }
        }

        return null;
    }

    private static ExecutionInstance CreateExecution(AuditTrail auditTrail, string workDescription)
    {
        return new ExecutionInstance(auditTrail, workDescription);
    }
    
    #endregion
}
