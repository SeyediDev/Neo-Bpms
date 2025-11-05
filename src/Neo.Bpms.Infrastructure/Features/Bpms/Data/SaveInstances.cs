using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Domain.Modeling.Entities.ProcessData;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Data;

public static partial class DataStorage
{
    public enum LockChangeRequest
    {
        NoChange,
        Lock,
        Unlock,
        Undetermined
    }

    public static void SaveProcessInstance(ProcessInstance pi, FlowNodeInstance ai, string code,
        bool bSaveRecord, LockChangeRequest lockChangeRequest)
    {
        bool isNew = pi.Id <= 0;
        SetLockState(pi, lockChangeRequest, code);
        int recordsAffected = 0;
        if (bSaveRecord)
        {
            SaveDataRecord(pi, ai, code, out recordsAffected);
        }

        ProcessInstanceRecordDb pir = GetPiRecord(pi);
        ApplyUtility apply = ApplyUtility<ProcessInstanceRecord>.NewInTrail(pi.AuditTrail);
        if (isNew)
        {
            if (!apply.Insert(pir))
            {
                pi.LogError(
                    $"can not save process instance code {code}. process:{pi.Process.Id}, pi:{pi.Id} command : {apply.CommandTxt}");
            }

            pi.LogTrace($"pi {pi.Id} created. process:{pi.Process.Id} code {code}");
            pi.SetId(pir.Id);
        }
        else if (!apply.Update(pir))
        {
            pi.LogError(
                $"can not save process instance code {code}. process:{pi.Process.Id}, pi:{pi.Id} command : {apply.CommandTxt}");
        }
        if (recordsAffected > 0)
        {
            TryCatchEvent<ConditionalEventDefinition>.Try(null, null, pi, ai as FlowNodeInstance2);
        }

        if (lockChangeRequest == LockChangeRequest.Unlock)
        {
            pi.Execution.DoJobs();
        }
    }

    public static void LockProcessInstance(ProcessInstance pi, string code)
    {
        pi.AddAuditDetail(BPMNAuditDetailTypeId.LockProcessInstance,
            $"Lock Process {pi.Process.Name}, code {code}");
        ChangeProcessInstanceLockState(pi, true);
    }

    public static void UnlockProcessInstance(ProcessInstance pi, string code)
    {
        pi.Execution.DoJobs();
        pi.AddAuditDetail(BPMNAuditDetailTypeId.UnlockProcessInstance,
            $"Unlock Process {pi.Process.Name}, code {code}");
        ChangeProcessInstanceLockState(pi, false);
    }

    private static void SetLockState(ProcessInstance pi, LockChangeRequest lockChangeRequest, string code)
    {
        switch (lockChangeRequest)
        {
            case LockChangeRequest.NoChange:
                break;
            case LockChangeRequest.Lock:
                pi.Locked = true;
                pi.AddAuditDetail(BPMNAuditDetailTypeId.LockProcessInstance,
                    $"Lock Process {pi.Process.Name}, code {code}");
                break;
            case LockChangeRequest.Unlock:
                pi.Locked = false;
                pi.AddAuditDetail(BPMNAuditDetailTypeId.UnlockProcessInstance,
                    $"Unlock Process {pi.Process.Name}, code {code}");
                break;
        }
    }

    public static bool DeleteProcessInstance(long pid, AuditTrail auditTrail)
    {
        ApplyUtility auAi = ApplyUtility<ActivityInstanceRecord>.NewInTrail(auditTrail);
        _ = auAi.DeleteWithFilter("ProcessInstanceId==" + pid);
        ApplyUtility auPi = ApplyUtility<ProcessInstanceRecord>.NewInTrail(auditTrail);
        _ = auPi.DeleteWithFilter("Id==" + pid);
        return true;
    }

    private static void ChangeProcessInstanceLockState(ProcessInstance pi, bool lockState)
    {
        pi.Locked = lockState;
        ElasticObject pir = new() { ["Id"] = pi.Id, ["Locked"] = lockState };
        ApplyUtility apply = ApplyUtility<ProcessInstanceRecord>.NewInTrail(pi.AuditTrail);
        if (!apply.Update(pir))
        {
            pi.LogError(
                $"can not change lock state of process instance. lock:{lockState} process:{pi.Process.Id}, pi:{pi.Id} command : {apply.CommandTxt}");
        }
    }

    private static ProcessInstanceRecordDb GetPiRecord(ProcessInstance pi)
    {
        if (pi.EntityPkv == "undefined")
        {
            throw new Exception("EntityPkv can not set to undefined");
        }

        ProcessInstanceRecord pir = new()
        {
            Id = pi.Id,
            ProcessVersionId = pi.ProcessVersion.DbId,
            EntityPKV = pi.EntityPkv,
            StateId = getScopeState(pi.state),
            AllowedActiveTime = pi.AllowedActiveTime,
            CloseTime = pi.CloseTime,
            CreationTime = pi.CreationTime,
            CreatorUserId = pi.CreatorUserId,
            Locked = pi.Locked,
            BPMNEngineId = DependencyInjectionHolder.Instance.BpmsEngine.BPMNEngineId,
            ParentActivityInstanceId = pi.ParentAiId
            //Weight = pi.Weight,
        };
        if (string.IsNullOrEmpty(pir.EntityPKV))
        {
            pi.LogError($"pi can not saved by entity pkv null. {pi.Data} process:{pi.Process.Id}, pi:{pi.Id}");
        }

        return pir;
    }
}
