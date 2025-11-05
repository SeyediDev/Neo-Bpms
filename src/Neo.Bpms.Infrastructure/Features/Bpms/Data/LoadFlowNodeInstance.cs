using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Modeling.Entities.ProcessData;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;
public static partial class DataStorage
{
    public static Dictionary<long, FlowNodeInstance> FetchFlowNodeInstancesOfFlowNode(
        FlowNodeRunTime flowNodeRunTime, ExecutionInstance execution, ProcessInstance pi)
    {
        return FetchFlowNodeInstances(execution, pi, flowNodeRunTime, null, 0, null, null);
    }

    public static Dictionary<long, FlowNodeInstance> FetchFlowNodeInstancesOfProcessInstance(
        ExecutionInstance execution, long processInstanceId, FlowNodeRunTime flowNodeRunTime = null,
        string filter = null)
    {
        filter = $"({nameof(ActivityInstanceRecord.ProcessInstanceId)}=={processInstanceId})" +
                 (string.IsNullOrEmpty(filter) ? "" : $" And ({filter})");
        return FetchFlowNodeInstances(execution, null, flowNodeRunTime, filter, 0, null, null);
    }

    public static IEnumerable<ActivityInstance> FetchActivityInstancesOfProcessInstance(ProcessInstance pi,
        ActivityRuntime activityRuntime = null, string filter = null)
    {
        return FetchFlowNodeInstancesOfProcessInstance(pi,
                activityRuntime, filter)
            ?.Values.OfType<ActivityInstance>();
    }

    public static Dictionary<long, FlowNodeInstance> FetchFlowNodeInstancesOfProcessInstance(ProcessInstance pi,
        FlowNodeRunTime flowNodeRunTime = null, string filter = null)
    {
        filter = $"({nameof(ActivityInstanceRecord.ProcessInstanceId)}=={pi.Id})" +
                 (string.IsNullOrEmpty(filter) ? "" : $" And ({filter})");
        return FetchFlowNodeInstances(pi.Execution, pi, flowNodeRunTime, filter, 0, null, null);
    }

    public static IEnumerable<FlowNodeInstance> CorrelateFlowNodeInstances(ExecutionInstance execution,
        FlowNodeRunTime flowNodeRunTime, LocalParameters localParameters, params string[] entityFilters)
    {
        return FetchFlowNodeInstances(execution, null, flowNodeRunTime,
                null, 1, nameof(ActivityInstanceRecord.Id),
                localParameters, entityFilters).Values;
    }

    public static Dictionary<long, FlowNodeInstance> FetchFlowNodeInstances(ExecutionInstance execution,
        ProcessInstance inPi, FlowNodeRunTime flowNodeRunTime, string filter, int maxRecordCount,
        string orderBy, LocalParameters localParameters, params string[] entityFilters)
    {
        List<ActivityInstanceRecord> airs = FetchActivityInstanceRecords(flowNodeRunTime, filter, maxRecordCount, orderBy, localParameters,
            entityFilters);
        if (airs == null)
        {
            return null;
        }

        ProcessInstance pi = inPi;
        Dictionary<long, FlowNodeInstance> flowNodeInstances = [];
        foreach (ActivityInstanceRecord air in airs)
        {
            FlowNodeInstance flowNodeInstance =
                LoadFlowNodeInstance(execution, air, ref flowNodeRunTime, ref pi);
            if (flowNodeInstance == null)
            {
                continue;
            }

            flowNodeInstances.Add(flowNodeInstance.Id, flowNodeInstance);
        }

        return flowNodeInstances;
    }

    public static ActivityInstanceRecordDb FetchFlowInstanceRecord(long aiId)
    {
        ActivityInstanceRecord air = EstablishFlowNodeInstanceQuery().Find<ActivityInstanceRecord>($"Id='{aiId}'");
        //if (air == null)
        //	throw new Exception($"Invalid Activity Instance Id {aiId}.");
        return air;
    }

    public static ActivityInstance FetchActivityInstance(long activityInstanceId, ExecutionInstance execution)
    {
        ProcessInstance pi = null;
        return FetchFlowNodeInstance(activityInstanceId, execution, ref pi) as ActivityInstance;
    }

    public static FlowNodeInstance LoadFlowNodeInstance(long activityInstanceId, ExecutionInstance execution,
        ref ProcessInstance pi)
    {
        return FetchFlowNodeInstance(activityInstanceId, execution, ref pi);
    }

    public static List<ActivityInstanceRecord> FetchActivityInstanceRecords(FlowNodeRunTime flowNodeRunTime,
        string filter, int maxRecordCount, string orderBy,
        LocalParameters localParameters, params string[] entityFilters)
    {
        QueryUtility qAi = EstablishActiveFlowNodeInstanceQuery().Where(filter);
        if (maxRecordCount > 0)
        {
            _ = qAi.SetPage(1, maxRecordCount);
        }

        if (flowNodeRunTime != null)
        {
            _ = qAi.Where($"{nameof(ActivityInstanceRecord.BPMNFlowNodeId)}=={flowNodeRunTime.DbId}");
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            _ = qAi.OrderBy(orderBy);
        }

        if (entityFilters != null &&
            flowNodeRunTime?.ProcessVersion?.definition?.Entity?.KeyFields?.FirstOrDefault() != null)
        {
            QueryUtility qPi = qAi.Join(nameof(ActivityInstanceRecord.ProcessInstance));
            Entity entity = flowNodeRunTime.ProcessVersion.definition.Entity;
            QueryUtility qEntity = qPi?.Join(entity.NamespaceId, entity.Id, entity.KeyFields?.FirstOrDefault()?.Id,
                    nameof(ProcessInstanceRecord.EntityPKV))
                ?.Query;
            foreach (string entityFilter in entityFilters)
            {
                _ = (qEntity?.Where(entityFilter));
            }
        }

        List<ActivityInstanceRecord> airs = qAi.OrderBy(nameof(ActivityInstanceRecord.ProcessInstanceId))
            .ToList<ActivityInstanceRecord>(localParameters);
        return airs;
    }

    private static QueryUtility EstablishFlowNodeInstanceQuery()
    {
        return QueryUtility<ActivityInstanceRecord>.New();
    }

    private static QueryUtility EstablishActiveFlowNodeInstanceQuery()
    {
        return EstablishFlowNodeInstanceQuery()
            .Where($"IsNull({nameof(ActivityInstanceRecord.Closed)},0)==0")
            .Where($"{nameof(ActivityInstanceRecord.StateId)} < {ActivityInstanceStateId.Completed:D}");
    }

    private static FlowNodeInstance FetchFlowNodeInstance(long flowNodeInstanceId, ExecutionInstance execution,
        ref ProcessInstance pi)
    {
        ActivityInstanceRecordDb flowInstanceRecord = FetchFlowInstanceRecord(flowNodeInstanceId);
        if (flowInstanceRecord == null)
        {
            Logger.LogError("Invalid flow node instance id {0}", flowNodeInstanceId);
            return null;
        }

        if (flowInstanceRecord.Closed || flowInstanceRecord.StateId >= ActivityInstanceStateId.Completed.ToInt())
        {
            Logger.LogError("Fetch closed flow node instance id {0} in flow node {1}",
                flowInstanceRecord.Id, flowInstanceRecord.BPMNFlowNodeId);
        }

        FlowNodeRunTime flowNodeRunTime = null;
        return LoadFlowNodeInstance(execution, flowInstanceRecord,
            ref flowNodeRunTime, ref pi);
    }

    private static FlowNodeInstance LoadFlowNodeInstance(ExecutionInstance execution,
        ActivityInstanceRecordDb flowNodeInstanceRecord, ref FlowNodeRunTime flowNodeRunTime, ref ProcessInstance pi)
    {
        long bpmnFlowNodeId = flowNodeInstanceRecord.BPMNFlowNodeId;
        if (flowNodeRunTime == null || flowNodeRunTime.DbId != bpmnFlowNodeId)
        {
            flowNodeRunTime = ((Repository)((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).BpmsRepository).GetFlowNodeRunTime(bpmnFlowNodeId);
            if (flowNodeRunTime == null)
            {
                Logger.LogError("Database has ai {0} in invalid flow node {1}", flowNodeInstanceRecord.Id, bpmnFlowNodeId);
                return null;
            }
        }

        if (pi == null || pi.Id != flowNodeInstanceRecord.ProcessInstanceId)
        {
            pi = FetchPi(flowNodeRunTime.ProcessVersion, flowNodeInstanceRecord.ProcessInstanceId, execution);
            if (pi == null)
            {
                Logger.LogError("Database has ai {0} in invalid pi {1} in process {2}", flowNodeInstanceRecord.Id,
                    flowNodeInstanceRecord.ProcessInstanceId, flowNodeRunTime.ProcessVersion.definition.Id);
                return null;
            }
        }

        return flowNodeRunTime switch
        {
            UserTaskRuntime userTaskRuntime => new UserTaskInstance(flowNodeInstanceRecord, userTaskRuntime, pi),
            ActivityRuntime activityRuntime => new ActivityInstance(flowNodeInstanceRecord, activityRuntime, pi),
            CatchRuntime catchRuntime => new EventWaitingInstance(flowNodeInstanceRecord, catchRuntime, pi),
            GatewayRuntime gatewayRuntime => new GatewaySyncWaitingInstance(flowNodeInstanceRecord, gatewayRuntime, pi),
            _ => null,
        };
    }
}
