using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Entities.ProcessModel;
using Neo.Bpms.Domain.Models.Bpmn.Core.Services;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.WorkManagement;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;

public static partial class DataStorage
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    public static void SetStartedActivityInstancesToCreated()
    {
        //TODO Improvement مدیریت جاب که انجام بشه دیگه نیازی به این نیست
        ElasticObject air = new() { [nameof(ActivityInstanceRecord.userTaskStateId)] = (int)UserTaskInstanceStateId.Created };
        ApplyUtility au = ApplyUtility<ActivityInstanceRecord>.New();
        au.AddSubQuery(eJoinType.InnerJoin,
            QueryUtility<ProcessInstanceRecord>.Where(
                $"{nameof(ProcessInstanceRecord.BPMNEngineId)}=={DependencyInjectionHolder.Instance.BpmsEngine.BPMNEngineId}"),
            nameof(ActivityInstanceRecord.ProcessInstanceId), nameof(ProcessInstanceRecord.Id));
        bool b = au.UpdateWithFilter(
            $"{nameof(ActivityInstanceRecord.userTaskStateId)}=={(int)UserTaskInstanceStateId.Started}",
            air);
        Logger.LogTrace("{0} SetStartedTempTrace {1}", b, au.CommandTxt);
    }

    public static void UnLockInstancesOfThisBPMNEngine()
    {
        //TODO Improvement مدیریت جاب که انجام بشه دیگه نیازی به این نیست
        ElasticObject pir = new() { [nameof(ProcessInstanceRecord.Locked)] = false };
        ApplyUtility au = ApplyUtility<ProcessInstanceRecord>.New();
        bool b = au.UpdateWithFilter(
            $"({nameof(ProcessInstanceRecord.BPMNEngineId)}=={DependencyInjectionHolder.Instance.BpmsEngine.BPMNEngineId}) And " +
            $"({nameof(ProcessInstanceRecord.Locked)}==true)",
            pir);
        Logger.LogTrace("{0} UnLockInstancesOfThisBPMNEngine {1}", b, au.CommandTxt);
    }

    public static IList<FlowNodeAddressing> FetchQueuedTasks(string interfaceName, string operationName, int count,
        string machineId, bool waitingForItsOwnMachine)
    {
        QueryUtility qAi = EstablishQuery(machineId, waitingForItsOwnMachine, count);
        QueryUtility qFlowNode = qAi.Join(nameof(ActivityInstanceRecord.BPMNFlowNode));
        QueryUtility qOperation = qFlowNode.Join(nameof(BPMNFlowNode.Operation))
            .Where($"{nameof(BPMNOperation.OperationId)}=='{Operation.GenerateId(interfaceName, operationName)}'");
        _ = qOperation.Join(nameof(BPMNOperation.Interface))
            .Where($"{nameof(BPMNInterface.InterfaceId)}=='{interfaceName}'");
        return [.. qAi.Select<ActivityInstanceRecord, FlowNodeAddressing>(null, air =>
                new FlowNodeAddressing
                {
                    FlowNodeId = air.BPMNFlowNodeId,
                    ProcessInstanceId = air.ProcessInstanceId,
                    ActivityInstanceId = air.Id
                })];
    }

    private static QueryUtility EstablishQuery(string machineId, bool waitingForItsOwnMachine, int count)
    {
        QueryUtility qAi = QueryUtility.New<ActivityInstanceRecord>()
            .SelectFields(nameof(ActivityInstanceRecord.Id), nameof(ActivityInstanceRecord.ProcessInstanceId),
                nameof(ActivityInstanceRecord.BPMNFlowNodeId))
            .Where($"IsNull({nameof(ActivityInstanceRecord.Closed)},0)==0")
            .Where($"{nameof(ActivityInstanceRecord.StateId)}<{ActivityInstanceStateId.Completed:D}")
            .Where($"{nameof(ActivityInstanceRecord.userTaskStateId)}=='{(int)UserTaskInstanceStateId.Created}'")
            .SetPage(1, count);
        QueryUtility qFlowNode = qAi.Include(nameof(ActivityInstanceRecord.BPMNFlowNode))
            .Where($"{nameof(BPMNFlowNode.StateId)}==1");
        QueryUtility qProcessVersion = qFlowNode.Include(nameof(BPMNFlowNode.ProcessVersion))
            .Where($"{nameof(BPMNProcessVersion.StateId)}==1");
        _ = qProcessVersion.Include(nameof(BPMNProcessVersion.Process))
            .Where($"{nameof(BPMNProcess.StateId)}==1");
        if (machineId != null)
        {
            _ = qAi.Where(
                $"(IsNull({nameof(ActivityInstanceRecord.MachineId)},'')=='') || {nameof(ActivityInstanceRecord.MachineId)}=='{machineId}'");
            _ = qAi.OrderBy(nameof(ActivityInstanceRecord.MachineId), SortType.Descending);
        }
        else
        {
            if (waitingForItsOwnMachine)
            {
                _ = qAi.Where($"IsNull({nameof(ActivityInstanceRecord.MachineId)}, '')==''");
            }

            _ = qAi.OrderBy(nameof(ActivityInstanceRecord.Priority), SortType.Descending);
            _ = qAi.OrderBy(nameof(ActivityInstanceRecord.Id));
        }

        _ = qAi.Include(nameof(ActivityInstanceRecord.ProcessInstance))
            .Where($"{nameof(ProcessInstanceRecord.StateId)}<{ProcessInstanceStateId.Completed:D}")
            .Where($"IsNull({nameof(ProcessInstanceRecord.Locked)},false)==false");
        return qAi;
    }
}
