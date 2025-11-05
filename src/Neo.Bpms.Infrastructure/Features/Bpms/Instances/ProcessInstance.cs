using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

public class ProcessInstance(long instanceId, ProcessInstanceStateId instanceState,
    ProcessVersionRuntime processVersion, string entityPkv, long? parentAiId, ExecutionInstance execution) : ScopeInstance(instanceId, instanceState)
{
    private ActivityInstance _parentAi;

    private ElasticObject _data;

    public sealed override AuditTrail AuditTrail => Execution.AuditTrail;
    public ExecutionInstance Execution { get; set; } = execution;
    public ProcessVersionRuntime ProcessVersion { get; set; } = processVersion;
    public Process Process => ProcessVersion?.definition;
    public string ModelId => Process.EntityNamespaceId;
    public string EntityId => Process.EntityId;
    public string CreatorUserId { get; set; }
    public bool Locked { get; set; }
    public long? ParentAiId { get; set; } = parentAiId;

    public bool Closed => state >= ProcessInstanceStateId.Completed;

    public ActivityInstance ParentAi
    {
        get => ParentAiId != null && ParentAiId.Value > 0
                ? (_parentAi ??= DataStorage.FetchActivityInstance(ParentAiId.Value, Execution))
                : null;
        set
        {
            _parentAi = value;
            ParentAiId = value?.Id;
        }
    }

    protected override ElasticObject data
    {
        get
        {
            if (_data == null)
            {
                InitData();
            }

            return _data;
        }
        set => _data = value;
    }

    public override bool GetData(string fieldName, out object value, IList<string> fields = null)
    {
        if (Process.flowElements.TryGetValue(fieldName, out FlowElement flowElement))
        {
            if (flowElement.flowElementType == FlowElement.eFlowElementType.DataStoreRef)
            {
                value = flowElement is DataStoreReference dataStoreRef
                    ? GetDataStoreValue(dataStoreRef, data, fields)
                    : null;
                return true;
            }
        }

        if (data.GetField(fieldName, out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    public override bool SetData(string fieldName, object value)
    {
        // ReSharper disable once InvertIf
        if (IsActiveLog)
        {
            _ = data.GetField(fieldName, out object o);
            if (!Equals(o, value))
            {
                LogTrace($"pi.SetField({fieldName},{value})");
            }
        }

        return data.SetField(fieldName, value);
    }

    public string EntityPkv
    {
        get
        {
            if (!string.IsNullOrEmpty(entityPkv))
            {
                return entityPkv;
            }

            Entity entity = ProjectDefinition.Project.GetEntity(Process.EntityNamespaceId, Process.EntityId);
            Property pkProperty = Process.EntityPkProperty(entity);
            if (pkProperty != null && data != null && data.GetField(pkProperty.Name, out object oIds) &&
                !string.IsNullOrEmpty(oIds?.ToString()))
            {
                if (oIds.ToString() == "undefined")
                {
                    throw new Exception($"key property {pkProperty.Name} can not set to undefined");
                }

                entityPkv = oIds.ToString();
            }

            return entityPkv;
        }
        set
        {
            if (value == "undefined")
            {
                return;
            }

            entityPkv = value;
            if (value == null)
            {
                return;
            }

            ElasticObject cloneData = _data?.Clone();
            DataStorage.LoadDataRecord(ProcessVersion, entityPkv, this);
            if (cloneData != null)
            {
                FetchData(false, cloneData, Process);
            }
        }
    }

    internal void ExtractEntityPkv(ElasticObject content, IPropertyContainer propertyContainer)
    {
        BaseElement element = propertyContainer as BaseElement;
        while (element is not null and not Domain.Model.BPMN.Processes.Process)
        {
            element = (BaseElement)element.Parent;
        }

        Entity entity = (element as Process)?.Entity;
        object oIds;
        string entityPkv = entity != null && content != null
            ? string.Join(",", entity.KeyFields.Select(keyField =>
                {
                    Property property = propertyContainer.properties?.FirstOrDefault(p => p.fieldId == keyField.Id);
                    string v = string.Empty;
                    oIds = ExtractEntityPkvFromElasticObject(content, property, keyField, ref v);

                    if (string.IsNullOrEmpty(v) && data != null)
                    {
                        oIds = ExtractEntityPkvFromElasticObject(data, property, keyField, ref v);
                    }

                    return v;
                })
                .ToList())
            : null;
        if (string.IsNullOrEmpty(entityPkv) && content != null)
        {
            if (content.GetField("Ids", out oIds) && !string.IsNullOrEmpty(oIds?.ToString()))
            {
                entityPkv = oIds.ToString();
            }
            else if (content.GetField("ids", out oIds) && !string.IsNullOrEmpty(oIds?.ToString()))
            {
                entityPkv = oIds.ToString();
            }
        }

        if (!string.IsNullOrEmpty(entityPkv))
        {
            if (string.IsNullOrEmpty(EntityPkv))
            {
                EntityPkv = entityPkv;
            }
            else if (EntityPkv != entityPkv)
            {
                LogError($"entity pkv different ids {EntityPkv} {entityPkv} In Process {Process?.Id}");
            }
        }
    }

    private static object ExtractEntityPkvFromElasticObject(ElasticObject content, Property property, EntityField keyField,
        ref string v)
    {
        object oIds;
        if (property != null)
        {
            if (content.GetField(property.Name, out oIds) &&
                !string.IsNullOrEmpty(oIds?.ToString()))
            {
                v = oIds.ToString();
            }
            else if (content.GetField(keyField.Id, out oIds) &&
                     !string.IsNullOrEmpty(oIds?.ToString()))
            {
                v = oIds.ToString();
            }
        }
        else if (content.GetField(keyField.Id, out oIds) &&
                 !string.IsNullOrEmpty(oIds?.ToString()))
        {
            v = oIds.ToString();
        }

        return oIds;
    }

    internal sealed override bool IsActiveLog => CheckIsActiveLog(Process, data, null);

    internal void ChangeDataState(int stateId)
    {
        _ = data.SetField(Process.StateProperty, stateId);
    }

    internal int DataStateId => (int)data.GetLong(Process.StateProperty);

    internal bool IsCorrelated(LocalParameters content)
    {
        foreach (KeyValuePair<string, object> fv in content)
        {
            bool b = GetData(fv.Key, out object val);
            if (b && val != fv.Value)
            {
                return false;
            }
        }

        return true;
    }

    internal override void Cancel()
    {
        CancelFlowNodeInstances();
        base.Cancel();
    }

    internal override void Complete()
    {
        Dictionary<long, FlowNodeInstance> instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(Execution, Id);
        if (instances == null)
        {
            return;
        }

        foreach (FlowNodeInstance flowNodeInstance in instances.Values)
        {
            flowNodeInstance.CompleteAndSave();
        }

        base.Complete();
    }

    internal void CloseProcessInstance()
    {
        AddAuditDetail(BPMNAuditDetailTypeId.TerminateProcess,
            $"Close Process Instance of process {Process.Id}");
        base.Complete();
        DataStorage.SaveProcessInstance(this, null, "PI.1", false, DataStorage.LockChangeRequest.Unlock);
    }

    internal void TerminateProcessAndActivityInstances()
    {
        AddAuditDetail(BPMNAuditDetailTypeId.TerminateProcessAndActivityInstances,
            $"Close Process Instance of process {Process.Id}");
        if (state == ProcessInstanceStateId.InCancel)
        {
            Cancel();
        }
        else
        {
            CancelFlowNodeInstances();
            base.Complete();
        }

        Execution.DoJobs();
        DataStorage.SaveProcessInstance(this, null, "PI.0", false, DataStorage.LockChangeRequest.Unlock);
        CompleteParentActivityInstance();
    }

    internal override void Close()
    {
        Dictionary<long, FlowNodeInstance> instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(Execution, Id);
        if (instances != null)
        {
            foreach (FlowNodeInstance flowNodeInstance in instances.Values)
            {
                flowNodeInstance.Close();
                flowNodeInstance.Save();
            }
        }

        base.Close();
    }

    internal void CheckAndClosePiAndParentAiIfNotExecuting()
    {
        if (!Closed)
        {
            Dictionary<long, FlowNodeInstance> instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(Execution, Id);
            if (instances == null || instances.Count == 0)
            {
                CloseProcessInstance();
            }
        }

        if (Closed && ParentAiId > 0)
        {
            CompleteParentActivityInstance();
        }
    }

    internal void CancelFlowNodeInstances()
    {
        Dictionary<long, FlowNodeInstance> instances = DataStorage.FetchFlowNodeInstancesOfProcessInstance(Execution, Id);
        if (instances == null || instances.Count == 0)
        {
            return;
        }

        foreach (FlowNodeInstance flowNodeInstance in instances.Values)
        {
            flowNodeInstance.CancelFlowNodeInstance();
        }

        CancelSubProcessInstance();
    }

    private void CancelSubProcessInstance()
    {
        IList<ProcessInstance> subPis = DataStorage.FetchSubProcessInstances(this);
        foreach (ProcessInstance subPi in subPis)
        {
            subPi.InCancel();
            subPi.TerminateProcessAndActivityInstances();
            subPi.CancelSubProcessInstance();
        }
    }

    private void CompleteParentActivityInstance()
    {
        ActivityInstance parentAi = ParentAi;
        if (parentAi?.state is not ActivityInstanceStateId.Active and
            not ActivityInstanceStateId.Ready)
        {
            return;
        }

        AddAuditDetail(BPMNAuditDetailTypeId.CompleteParent,
            $"Complete Parent Activity Instance of Activity {parentAi.FlowNode.Id}");
        Execution.AddJob(new CompleteParentActivityInstanceJob
        {
            ParentAi = parentAi,
            ProcessOutputData = FetchProcessOutputData(),
            Description = Execution.Description /*todo sjfs*/
        });
    }

    private LocalParameters FetchProcessOutputData()
    {
        LocalParameters processOutputData = [];
        foreach (DataOutput dataOutput in ProcessVersion.definition?.ioSpecification?.dataOutputs ??
                                   Enumerable.Empty<DataOutput>())
        {
            if (GetData(dataOutput.Name, out object value))
            {
                _ = processOutputData.AddOrUpdate(dataOutput.Name, value);
            }
        }

        return processOutputData;
    }

    private object GetDataStoreValue(DataStoreReference element, ElasticObject data1, IList<string> fields)
    {
        if (!data1.GetField(element.Id, out object obj))
        {
            obj = InitDataStore(element, fields);
            _ = data1.SetField(element.Id, obj);
        }

        return obj;
    }

    private object InitDataStore(DataStoreReference dataStoreReference, IList<string> fields)
    {
        if (dataStoreReference?.dataStore?.itemSubjectRef == null)
        {
            return null;
        }

        return dataStoreReference.dataStore.itemSubjectRef.isCollection
            ? DataStorage.ReadDataStoreList(dataStoreReference, this, fields)
            : DataStorage.ReadDataStore(dataStoreReference, this, fields);
    }

    private void InitData()
    {
        if (!string.IsNullOrEmpty(entityPkv))
        {
            _data = new ElasticObject();
            DataStorage.LoadDataRecord(ProcessVersion, entityPkv, this);
        }
        else
        {
            _data = new ElasticObject();
        }
    }
}
