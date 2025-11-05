using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Execution;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Auditing;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems;
using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Instances;

/// <summary>
/// A scope describes the context in which execution of an Activity happens. This consists of the set of:
/// • Data Objects available (including DataInput and DataOutput)
/// • Events available for catching or throwing triggers
/// • Conversations going on in that scope
/// In general, a scope contains exactly one main flow of Activities which is started, when the scope gets activated. 
/// Vice versa, all Activities are enclosed by a scope. Scopes are hierarchically nested.
/// Scopes can have several scope instances at runtime. They are also hierarchically nested according to their generation. In a scope instance several tokens can be active.
/// 
/// BPMN has the following model elements with scope characteristics:
/// • Choreography
/// • Pool
/// • Sub-Process
/// • Task
/// • Activity
/// • Multi-instances body
/// 
/// Scopes are used to define the semantics of: 
/// • Visibility of Data Objects (including DataInput and DataOutput)
/// • Event resolution
/// • Starting/stopping of token execution
/// The Data Objects, Events, and correlation keys described by a scope can be explicitly modeled or implicitly defined.
/// </summary>
public abstract class ScopeInstance : IPropertyValueContainer
{
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    public long Id { get; private set; }
    public ProcessInstanceStateId state { get; private set; }
    public DateTime CreationTime;
    public DateTime? CloseTime;
    public TimeSpan AllowedActiveTime;
    public abstract AuditTrail AuditTrail { get; }

    protected ScopeInstance(long instanceId, object instanceState)
    {
        Id = instanceId;
        CreationTime = DateTime.Now;
        // ReSharper disable once VirtualMemberCallInConstructor
        SetState(instanceState);
    }

    public void SetId(long instanceId)
    {
        Id = instanceId;
    }

    public virtual TimeSpan ActiveTime => (IsActive() ? DateTime.Now : CloseTime ?? DateTime.Now) - CreationTime;
    public virtual TimeSpan RemainingActiveTime => CreationTime + AllowedActiveTime - DateTime.Now;

    public double PerformanceRatio => AllowedActiveTime.Milliseconds > 0
        ? ActiveTime.Milliseconds * 100 / AllowedActiveTime.Milliseconds
        : 0;

    public ElasticObject Data => data;
    protected abstract ElasticObject data { get; set; }
    public abstract bool SetData(string name, object value);
    public abstract bool GetData(string name, out object value, IList<string> fields = null);
    internal abstract bool IsActiveLog { get; }

    public string FetchData(string name)
    {
        return GetData(name, out object value) && value != null ? value.ToString() : string.Empty;
    }

    internal void FetchData(bool isContentFromEntityLayer, ElasticObject content,
        IPropertyContainer propertyContainer)
    {
        if (content == null)
        {
            return;
        }

        foreach (Property property in propertyContainer?.properties ?? Enumerable.Empty<Property>())
        {
            FetchDataFromItemAware(isContentFromEntityLayer, content, property);
        }

        IFlowElementsContainer flowElementContainer = propertyContainer as IFlowElementsContainer;
        foreach (DataObject dataObject in flowElementContainer?.flowElements.Values.OfType<DataObject>() ??
                                   [])
        {
            FetchDataFromItemAware(isContentFromEntityLayer, content, dataObject);
        }

        ProcessInstance pi = this as ProcessInstance;
        pi?.ExtractEntityPkv(content, propertyContainer);
    }

    public bool IsActive()
    {
        return state == ProcessInstanceStateId.Activated;
    }

    public void LogError(string message)
    {
        if (!IsActiveLog)
        {
            return;
        }

        AddAuditDetail(BPMNAuditDetailTypeId.Error, message);
        Logger.LogError(GetNLogMessage(message));
    }

    public void LogDebug(string message)
    {
        if (!IsActiveLog)
        {
            return;
        }

        AddAuditDetail(BPMNAuditDetailTypeId.Debug, message);
        Logger.LogDebug(GetNLogMessage(message));
    }

    public void LogTrace(string message)
    {
        if (!IsActiveLog)
        {
            return;
        }

        AddAuditDetail(BPMNAuditDetailTypeId.Trace, message);
        Logger.LogTrace(GetNLogMessage(message));
    }

    internal virtual void Activate()
    {
        state = ProcessInstanceStateId.Activated;
    }

    internal virtual void Cancel()
    {
        state = ProcessInstanceStateId.Cancelled;
        Close();
    }

    internal virtual void InCancel()
    {
        state = ProcessInstanceStateId.InCancel;
    }

    internal virtual void Complete()
    {
        state = ProcessInstanceStateId.Completed;
        Close();
    }

    internal virtual void Close()
    {
        CloseTime = DateTime.Now;
    }

    internal void AddAuditDetail(BPMNAuditDetailTypeId typeId, string text)
    {
        ActivityInstance ai = this as ActivityInstance;
        ProcessInstance pi = this as ProcessInstance ?? ai?.pi;
        AuditTrail?.AddDetail((long)typeId, $"{ai?.FlowNodeRunTime.GetType().Name} {text}",
            pi?.ProcessVersion.DbId, ai?.FlowNodeRunTime.DbId, pi?.Id, ai?.Id);
    }

    protected bool CheckIsActiveLog(IAuditingContainer container, ElasticObject piData, ElasticObject aiData)
    {
        if (container?.auditing == null)
        {
            return false;
        }

        if (!container.auditing.generateTraceLog)
        {
            return false;
        }

        if (string.IsNullOrEmpty(container.auditing.logCondition))
        {
            return true;
        }

        ExpressionTree expression = Parser.ParseTree(container.auditing.logCondition);
        LocalParameters lc = [];
        if (aiData != null)
        {
            _ = lc.Set(aiData);
        }

        if (piData != null)
        {
            _ = lc.Set(piData);
        }

        ExpressionInInstance expressionInInstance = new(null, null, lc, expression);
        object value = expressionInInstance.EvalExpression(null);
        return ConvUtill.ToBoolean(value);
    }

    protected virtual void SetState(object instanceState)
    {
        state = (ProcessInstanceStateId)Convert.ToInt16(instanceState);
    }

    private string GetNLogMessage(string message)
    {
        ActivityInstance ai = this as ActivityInstance;
        ProcessInstance pi = this as ProcessInstance ?? ai?.pi;
        return $"{message}, PI:{pi?.Id}" + (ai != null ? $", AI:{ai.Id}" : "") +
               (pi?.AuditTrail != null ? ", AT:" + pi.AuditTrail.Id : "");
    }

    private void FetchDataFromItemAware(bool isContentFromEntityLayer, ElasticObject content,
        IFieldAwareElement itemAware)
    {
        if (isContentFromEntityLayer && string.IsNullOrEmpty(itemAware.fieldId))
        {
            return;
        }

        if (content.GetField(itemAware.Name, out object value))
        {
            if (itemAware.field?.FieldType == TVariableTypes.BOOL)
            {
                value = ConvUtill.ToBoolean(value);
            }

            _ = SetData(itemAware.Name, value);
        }
        else if (itemAware.field?.AssociationEntity?.Maps != null)
        {
            string keyFieldValue = "";
            bool first = true;
            foreach (EntityRelationMap map in itemAware.field.AssociationEntity.Maps)
            {
                if (!first)
                {
                    keyFieldValue += "#";
                }

                if (content.GetField(map.SourceField, out value))
                {
                    keyFieldValue += value;
                }

                first = false;
            }

            if (!first && !string.IsNullOrEmpty(keyFieldValue))
            {
                _ = SetData(itemAware.Name, keyFieldValue);
            }
            else if (isContentFromEntityLayer)
            {
                _ = SetData(itemAware.Name, null);
            }
        }
        else if (isContentFromEntityLayer)
        {
            _ = SetData(itemAware.Name, null);
        }
    }
}
