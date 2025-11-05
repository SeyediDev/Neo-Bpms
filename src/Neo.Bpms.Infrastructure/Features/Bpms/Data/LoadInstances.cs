using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;

public static partial class DataStorage
{
    public static ProcessInstance FetchPi(ProcessVersionRuntime processVersionRuntime,
        long processInstanceId, ExecutionInstance execution)
    {
        ProcessInstance pi = null;
        var pir = QueryUtility<ProcessInstanceRecord>.New()
            .SelectFieldsOfEntity()
            .Find($"{nameof(ProcessInstanceRecord.Id)}=={processInstanceId}", null);
        if (pir != null)
        {
            pi = LoadProcessInstance(pir, execution, ref processVersionRuntime);
        }

        return pi != null && pi.EntityPkv == "undefined"
            ? throw new Exception($"ProcessInstance {processInstanceId} EntityPkv set to undefined")
            : pi;
    }

    public static Dictionary<long, ProcessInstance> LoadProcessInstances(
        ProcessVersionRuntime processVersionRuntime, ExecutionInstance execution, string entityPkv = null)
    {
        Dictionary<long, ProcessInstance> pis = [];
        var q = QueryUtility<ProcessInstanceRecord>
            .Where($"{nameof(ProcessInstanceRecord.ProcessVersionId)}=={processVersionRuntime.DbId}")
            .Where($"{nameof(ProcessInstanceRecord.StateId)}<{ProcessInstanceStateId.Completed:D}");
        if (!string.IsNullOrEmpty(entityPkv))
        {
            q.Where($"{nameof(ProcessInstanceRecord.EntityPKV)}=='{entityPkv}'");
        }

        q.ForEach(null, pir =>
        {
            ProcessInstance pi = LoadProcessInstance(pir, execution,
                ref processVersionRuntime);
            pis.Add(pi.Id, pi);
        });
        return pis;
    }

    public static ProcessInstanceRecordDb FetchProcessInstanceRecord(long piId)
    {
        ProcessInstanceRecord pir = QueryUtility<ProcessInstanceRecord>.LoadViewModel<ProcessInstanceRecord>(piId);
        return pir ?? throw new Exception($"Invalid Process Instance Id {piId}.");
    }

    public static List<ProcessInstanceRecord_Attachment> LoadAttachments(long piId)
    {
        var q = QueryUtility<ProcessInstanceRecord_Attachment>
            .Where($"{nameof(ProcessInstanceRecord_Attachment.ProcessInstanceId)}=='{piId}'");
        return q.ToList<ProcessInstanceRecord_Attachment>();
    }

    public static ProcessInstance LoadProcessInstance(ProcessVersionRuntime processVersion,
        ProcessInstanceRecordDb pir, ExecutionInstance execution)
    {
        ProcessInstance pi = new(pir.Id, pir.InstanceStateId,
            processVersion, pir.EntityPKV, pir.ParentActivityInstanceId, execution)
        {
            AllowedActiveTime = pir.AllowedActiveTime,
            CloseTime = pir.CloseTime,
            CreationTime = pir.CreationTime,
            CreatorUserId = pir.CreatorUserId,
            //Weight = pir.Weight
            Locked = pir.Locked
        };
        return pi;
    }

    public static ProcessInstance LockControl(ProcessInstance pi, string traceCode, out bool waited)
    {
        waited = pi.Locked;
        if (pi.Locked)
        {
            long piId = pi.Id;
            pi = WaitForUnlockProcessInstance(pi);
            if (pi == null)
            {
                throw new Exception($"ProcessInstance {piId} was closed.");
            }
        }

        LockProcessInstance(pi, traceCode);
        return pi;
    }

    public static FlowNodeInstance LockControl(FlowNodeInstance flowNodeInstance, string traceCode)
    {
        ProcessInstance pi = null;
        FlowNodeInstance ai = flowNodeInstance;
        try
        {
            pi = LockControl(flowNodeInstance.pi, traceCode, out bool waited);
            if (waited && pi != null)
            {
                ai = LoadFlowNodeInstance(flowNodeInstance.Id, flowNodeInstance.pi.Execution, ref pi);
            }
        }
        catch
        {
            // ignored
        }

        if (pi == null || ai == null || ai.Closed)
        {
            ai = null;
        }

        return ai;
    }

    public static ProcessInstance WaitForUnlockProcessInstance(ProcessInstance pi)
    {
        int counter = 0;
        while (counter < 1000 && pi.Locked)
        {
            Thread.Sleep(500);
            pi = FetchPi(pi.ProcessVersion, pi.Id, pi.Execution);
            counter++;
        }

        if (pi.Locked)
        {
            throw new Exception($"ProcessInstance {pi.Id} was locked. Please try again...");
        }
        else
        {
            if (pi.Closed)
            {
                pi = null;
            }
        }
        return pi;
    }

    public static FlowNodeInstance WaitForUnlockFlowNodeInstance(FlowNodeInstance ai)
    {
        ProcessInstance pi = WaitForUnlockProcessInstance(ai.pi);
        ai = LoadFlowNodeInstance(ai.Id, ai.pi.Execution, ref pi);
        if (ai?.Closed ?? false)
        {
            ai = null;
        }

        return ai;
    }

    public static IList<ProcessInstance> FetchSubProcessInstances(ProcessInstance pi)
    {
        ProcessVersionRuntime processVersion = null;
        return [.. QueryUtility<ProcessInstanceRecord>
            .Where(
                $"({nameof(ProcessInstanceRecord.ParentActivityInstance)}.{nameof(ActivityInstanceRecord.ProcessInstanceId)})=={pi.Id}")
            .ToList()
            .Select(pir => LoadProcessInstance(pir, pi.Execution, ref processVersion))];
    }

    private static ProcessInstance LoadProcessInstance(ElasticObject pir,
        ExecutionInstance execution, ref ProcessVersionRuntime processVersion)
    {
        long processVersionId = pir.GetLong(nameof(ProcessInstanceRecord.ProcessVersionId));
        long processInstanceId = pir.Id;
        if (processVersion == null || processVersion.DbId != processVersionId)
        {
            processVersion = ((Repository)((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).BpmsRepository).GetProcessVersionRuntimeByDbId(processVersionId);
            if (processVersion == null)
            {
                Logger.LogError("Database has pi {0} in invalid process {1}", processInstanceId, processVersionId);
                return null;
            }
        }

        try
        {
            return GetPi(processVersion, pir, execution);
        }
        catch
        {
            Logger.LogError("Database has duplicate pi {0} in process {1}", processInstanceId, processVersionId);
        }

        return null;
    }

    private static ProcessInstance GetPi(ProcessVersionRuntime processVersion, ElasticObject pir,
        ExecutionInstance execution)
    {
        ProcessInstance pi = new(pir.Id,
            pir.GetEnum(nameof(ProcessInstanceRecord.StateId), ProcessInstanceStateId.Cancelled),
            processVersion,
            pir.GetString(nameof(ProcessInstanceRecord.EntityPKV)),
            pir.GetNullableLong(nameof(ProcessInstanceRecord.ParentActivityInstanceId)),
            execution)
        {
            AllowedActiveTime = pir.GetTimeSpan(nameof(ProcessInstanceRecord.AllowedActiveTime)),
            CloseTime = pir.GetDateTime(nameof(ProcessInstanceRecord.CloseTime)),
            CreationTime = pir.GetDateTime(nameof(ProcessInstanceRecord.CreationTime)),
            CreatorUserId = pir.GetString(nameof(ProcessInstanceRecord.CreatorUserId)),
            //Weight = pir.GetDouble("Weight")
            Locked = pir.GetBool(nameof(ProcessInstanceRecord.Locked)),
            ParentAiId = pir.GetLong(nameof(ProcessInstanceRecord.ParentActivityInstanceId))
        };
        return pi;
    }
}
