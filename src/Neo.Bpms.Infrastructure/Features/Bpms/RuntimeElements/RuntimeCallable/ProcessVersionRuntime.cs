using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Extensions.BusinessProcesses;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Cmmn.Entities;
using Neo.Bpms.Domain.Models.Cmmn.Fields;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catch;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

public class ProcessVersionRuntime(Process definition, string versionNo) : CallableRunTime
{
    public BusinessProcessVersion BusinessProcessVersion;
    public readonly string VersionNo = versionNo;
    public long DbId { get; set; }

    private long? _processDbId;
    public Repository repository => (Repository)((BpmsEngine)DependencyInjectionHolder.Instance.BpmsEngine).BpmsRepository;
    public Process definition = definition;
    public string ProcessId => definition.Id;
    public readonly Dictionary<string, FlowNodeRunTime> nodes = [];
    public Dictionary<string, UserTaskRuntime> UserTasks { get; set; } = [];

    public Entity Entity => definition.Entity;

    public long ProcessDbId
    {
        get
        {
            _processDbId ??= repository.GetProcessRuntime(ProcessId)?.DbId;
            return _processDbId ?? 0;
        }
    }

    public bool TryGetFlowNodeRuntime(string id, out FlowNodeRunTime flowNodeRunTime)
    {
        if (id != null)
        {
            if (nodes.TryGetValue(id, out flowNodeRunTime) && flowNodeRunTime != null)
            {
                return true;
            }
        }

        flowNodeRunTime = null;
        return false;
    }

    public bool TryGetCatchRuntime(string id, out CatchRuntime catchRuntime)
    {
        _ = TryGetFlowNodeRuntime(id, out FlowNodeRunTime flowNodeRunTime);
        catchRuntime = (CatchRuntime)flowNodeRunTime;
        return catchRuntime != null;
    }
    public bool TryGetActivityRuntime(string id, out ActivityRuntime activityRuntime)
    {
        _ = TryGetFlowNodeRuntime(id, out FlowNodeRunTime flowNodeRunTime);
        activityRuntime = (ActivityRuntime)flowNodeRunTime;
        return activityRuntime != null;
    }

    public FlowNodeRunTime GetFlowNodeRuntime(string id)
    {
        _ = TryGetFlowNodeRuntime(id, out FlowNodeRunTime flowNodeRunTime);
        return flowNodeRunTime;
    }

    public IEnumerable<CatchRuntime> GetStarts()
    {
        return GetNotIncoming()
            .OfType<CatchRuntime>()
            .Where(crt =>
                crt.CatchEvent.TriggerType == Event.eEventType.None &&
                crt.CatchEvent.Location == CatchEventLocation.Start);
    }

    public IEnumerable<CatchRuntime> GetCatches()
    {
        return nodes.Values.OfType<CatchRuntime>();
    }

    public IEnumerable<FlowNodeRunTime> GetNotIncoming()
    {
        return nodes.Values.Where(rt => !rt.HasIncoming);
    }

    public IEnumerable<ActivityRuntime> GetActivities()
    {
        return nodes.Values.OfType<ActivityRuntime>();
    }

    public IEnumerable<GatewayRuntime> GetGateways()
    {
        return nodes.Values.OfType<GatewayRuntime>();
    }

    internal ProcessInstance CreateInstance(ExecutionInstance execution, LocalParameters inputData,
        string entityPkv, long? parentAiId, List<DataOutput> dataOutputs)
    {
        ProcessInstance pi = new(0, ProcessInstanceStateId.Activated,
            this, entityPkv, parentAiId, execution)
        {
            CreationTime = DateTime.Now,
            CreatorUserId = execution.AuditTrail.User?.Id
        };
        //pi.AllowedActiveTime = definition.AllowedActiveTime;
        SetInputData(pi, inputData, dataOutputs);
        DataStorage.SaveProcessInstance(pi, null, "PVR.1", true, DataStorage.LockChangeRequest.Lock);
        pi.AddAuditDetail(BPMNAuditDetailTypeId.CreateInstance, $"Create Process({definition.Id}_V{VersionNo})");
        return pi;
    }

    private static void SetInputData(ProcessInstance pi, LocalParameters inputData,
        List<DataOutput> dataOutputs)
    {
        if (inputData == null)
        {
            return;
        }

        foreach (InputSet inputSet in pi.Process?.ioSpecification?.inputSets ?? Enumerable.Empty<InputSet>())
        {
            foreach (DataInputRef input in inputSet.dataInputRefs)
            {
                if (inputData.TryGetValue(input.dataInput.Name, out object value))
                {
                    _ = pi.SetData(input.dataInput.Name, value);
                }
            }
        }

        foreach (DataInput dataInput in pi.Process?.ioSpecification?.dataInputs ?? Enumerable.Empty<DataInput>())
        {
            if (inputData.TryGetValue(dataInput.Name, out object value))
            {
                _ = pi.SetData(dataInput.Name, value);
            }
        }
        if (dataOutputs != null)
        {
            foreach (DataOutput dataOutput in dataOutputs)
            {
                if (inputData.TryGetValue(dataOutput.Name, out object val))
                {
                    _ = pi.SetData(dataOutput.fieldId, val);
                }
                //if (!FlowNodeRunTime.FetchTransformationValue(dataOutput, pi, null, inputData, out var value))
                //	continue;
                //pi.SetData(dataOutput.targetRef.Name, value);
            }
        }
    }

    internal string FetchEntityPkv(LocalParameters messageParams)
    {
        if (messageParams == null)
        {
            return null;
        }

        Entity entity = Entity;
        EntityField keyField = entity?.KeyFields?.FirstOrDefault();
        string entityPkv = "";
        object entityPkvObj;
        Property entityPkvProperty = definition?.EntityPkProperty(entity);
        if (entityPkvProperty != null)
        {
            if (messageParams.TryGetValue(entityPkvProperty.Name, out entityPkvObj))
            {
                entityPkv = entityPkvObj?.ToString();
            }
            else if (messageParams.TryGetValue(entityPkvProperty.fieldId, out entityPkvObj))
            {
                entityPkv = entityPkvObj?.ToString();
            }
        }

        if (string.IsNullOrEmpty(entityPkv) || entityPkv == "undefined")
        {
            if (keyField != null && messageParams.TryGetValue(keyField.Id ?? "Ids", out entityPkvObj))
            {
                entityPkv = entityPkvObj?.ToString();
            }
            else if (messageParams.TryGetValue("Ids", out entityPkvObj))
            {
                entityPkv = entityPkvObj?.ToString();
            }
            else if (messageParams.TryGetValue("ids", out entityPkvObj))
            {
                entityPkv = entityPkvObj?.ToString();
            }
        }

        return entityPkv;
    }

    public void AddError(string text, string @for, string code, string generalText)
    {
        BusinessProcessVersion.BpmnDefinitions.ErrorInfos.AddError(text, @for, code, generalText);
    }

    public void AddFatal(string text, string @for, string code, string generalText)
    {
        BusinessProcessVersion.BpmnDefinitions.ErrorInfos.AddFatal(text, @for, code, generalText);
    }
}
