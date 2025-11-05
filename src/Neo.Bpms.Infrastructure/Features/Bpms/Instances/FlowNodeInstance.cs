using Neo.Bpms.Domain.Entities.ProcessData;
using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.ioSpecification;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

public abstract class FlowNodeInstance(long instanceId, FlowNodeRunTime flowNodeRunTime, ProcessInstance pi,
    object instanceState) : ScopeInstance(instanceId, instanceState)
{
    public override AuditTrail AuditTrail => pi?.AuditTrail;

    protected FlowNodeInstance(ElasticObject flowNodeInstanceRecord, FlowNodeRunTime flowNodeRunTime,
        ProcessInstance pi)
        : this(flowNodeInstanceRecord.Id, flowNodeRunTime, pi,
            (ProcessInstanceStateId)flowNodeInstanceRecord.GetLong("StateId"))
    {
        AllowedActiveTime = flowNodeInstanceRecord.GetTimeSpan(nameof(ActivityInstanceRecord.AllowedActiveTime));
        Closed = flowNodeInstanceRecord.GetBool(nameof(ActivityInstanceRecord.Closed));
        CloseTime = flowNodeInstanceRecord.GetNullableDateTime(nameof(ActivityInstanceRecord.CloseTime));
        CreationTime = flowNodeInstanceRecord.GetDateTime(nameof(ActivityInstanceRecord.CreationTime));
    }

    protected FlowNodeInstance(ActivityInstanceRecordDb flowNodeInstanceRecord, FlowNodeRunTime flowNodeRunTime,
        ProcessInstance pi)
        : this(flowNodeInstanceRecord.Id, flowNodeRunTime, pi, (ProcessInstanceStateId)flowNodeInstanceRecord.StateId)
    {
        AllowedActiveTime = flowNodeInstanceRecord.AllowedActiveTime;
        Closed = flowNodeInstanceRecord.Closed;
        CloseTime = flowNodeInstanceRecord.CloseTime;
        CreationTime = flowNodeInstanceRecord.CreationTime;
    }

    public FlowNodeRunTime FlowNodeRunTime { get; } = flowNodeRunTime;
    public FlowNode FlowNode => FlowNodeRunTime.flowNode;
    public ProcessInstance pi { get; } = pi;

    /// <summary>
    /// The process ends
    /// </summary>
    public bool Closed;

    protected override ElasticObject data { get; set; }

    public override bool GetData(string name, out object value, IList<string> fields = null)
    {
        return data != null && data.GetField(name, out value) || pi.GetData(name, out value, fields);
    }

    public override bool SetData(string fieldName, object value)
    {
        data ??= new ElasticObject();
        _ = data.SetField(fieldName, value);
        return true;
    }

    internal override bool IsActiveLog
    {
        get
        {
            bool isActiveLog = true;
            if (FlowNode?.auditing != null)
            {
                isActiveLog = CheckIsActiveLog(FlowNode, pi.Data, data);
            }

            return isActiveLog && pi.IsActiveLog;
        }
    }

    internal override void Close()
    {
        Closed = true;
        Free();
        base.Close();
    }

    protected void Free()
    {
        FlowNodeRunTime.Free(this);
    }

    protected virtual Property GetProperty(string name)
    {
        return null;
    }

    protected Property GetProperty(IPropertyContainer container, string name)
    {
        return container.properties?.FirstOrDefault(prop => prop.Name == name);
    }

    internal void CancelFlowNodeInstance()
    {
        Cancel();
        AddAuditDetail(BPMNAuditDetailTypeId.CancelActivity,
            $"Cancel {FlowNode.GetType().Name} Instance of {FlowNode.Id}");
        Save();
    }

    internal void CompleteAndSave()
    {
        Complete();
        Save();
    }

    internal void Save(string description = null)
    {
        DataStorage.SaveFlowNodeInstance(this, description);
    }
}

public abstract class FlowNodeInstance2 : FlowNodeInstance
{
    protected FlowNodeInstance2(long instanceId, FlowNodeRunTime flowNodeRunTime, ProcessInstance pi,
        object instanceState) : base(instanceId, flowNodeRunTime, pi, instanceState)
    {
    }

    protected FlowNodeInstance2(ElasticObject flowNodeInstanceRecord, FlowNodeRunTime flowNodeRunTime,
        ProcessInstance pi) : base(flowNodeInstanceRecord, flowNodeRunTime, pi)
    {
    }

    protected FlowNodeInstance2(ActivityInstanceRecordDb flowNodeInstanceRecord, FlowNodeRunTime flowNodeRunTime,
        ProcessInstance pi) : base(flowNodeInstanceRecord, flowNodeRunTime, pi)
    {
    }

    internal InputSet SelectedInputSet;
}
