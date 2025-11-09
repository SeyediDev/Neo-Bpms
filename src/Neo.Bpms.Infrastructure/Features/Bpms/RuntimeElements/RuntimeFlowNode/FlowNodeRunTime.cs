using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeCallable;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeGateway.RuntimeEventBasedGateway;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
public abstract partial class FlowNodeRunTime(ProcessVersionRuntime processVersion, FlowNode flowNode)
{
    public long DbId { get; set; }
    public ProcessVersionRuntime ProcessVersion { get; set; } = processVersion;

    public EventBasedGatewayRuntime IncomingEventBasedGatewayRuntime { get; set; }
    public FlowNode flowNode { get; set; } = flowNode;
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;

    #region instance
    internal virtual void WithdrawObsolete()
    {
        //var auditTrail = new AuditTrail(TriggerTypeId.Revision, bpmnDefinitions.name, null);
        //DataStorage.SaveAudit(auditTrail);
    }
    #endregion instance

    public virtual void Free(FlowNodeInstance ai)
    {
    }

    protected static object GetPropertyValue(string name, ProcessInstance pi, FlowNodeInstance ai)
    {
        object value;
        if (ai != null)
            ai.GetData(name, out value);
        else
            pi.GetData(name, out value);
        return value;
    }

    protected void AuditTrace(string text, ProcessInstance pi, FlowNodeInstance ai)
    {
        if (!IsLogActive(pi, ai)) return;
        pi.LogTrace(GetLogText(text));
    }

    protected void LogDebug(string text, ProcessInstance pi, FlowNodeInstance ai)
    {
        if (!IsLogActive(pi, ai)) return;
        pi.LogDebug(GetLogText(text));
    }

    protected void LogError(string text, ProcessInstance pi, FlowNodeInstance ai)
    {
        if (!IsLogActive(pi, ai)) return;
        pi.LogError(GetLogText(text));
    }

    private static bool IsLogActive(ProcessInstance pi, FlowNodeInstance ai)
    {
        return ai?.IsActiveLog ?? pi.IsActiveLog;
    }

    private string GetLogText(string text)
    {
        return $"{GetType().Name}.{flowNode.Id}:{text}";
    }

    public FlowNodeTypeId FlowNodeTypeId
    {
        get
        {
            try
            {
                switch (flowNode.nodeType)
                {
                    case FlowNode.eFlowNodeType.Gateway:
                        var gateway = (Gateway)flowNode;
                        switch (gateway?.gatewayType)
                        {
                            case Gateway.eGatewayType.Exclusive:
                                return FlowNodeTypeId.ExclusiveGateway;
                            case Gateway.eGatewayType.Inclusive:
                                return FlowNodeTypeId.InclusiveGateway;
                            case Gateway.eGatewayType.Parallel:
                                return FlowNodeTypeId.ParallelGateway;
                            case Gateway.eGatewayType.Complex:
                                return FlowNodeTypeId.ComplexGateway;
                            case Gateway.eGatewayType.EventBased:
                                return FlowNodeTypeId.EventBased;
                        }

                        return FlowNodeTypeId.GatewayWaitingInstance;
                    case FlowNode.eFlowNodeType.Event:
                        var @event = (Event)flowNode;
                        switch (@event)
                        {
                            case BoundaryEvent _:
                                return FlowNodeTypeId.Boundary;
                            case EndEvent _:
                                return FlowNodeTypeId.End;
                            case ImplicitThrowEvent _:
                                return FlowNodeTypeId.Implicit;
                            case IntermediateCatchEvent _:
                                return FlowNodeTypeId.IntermediateCatch;
                            case IntermediateThrowEvent _:
                                return FlowNodeTypeId.IntermediateThrow;
                            case StartEvent _:
                                return FlowNodeTypeId.Start;
                        }

                        return FlowNodeTypeId.EventWaitingInstance;
                    case FlowNode.eFlowNodeType.Activity:
                        var activity = (Activity)flowNode;
                        switch (activity?.ActivityType)
                        {
                            case Activity.eActivityType.UserTask:
                                return FlowNodeTypeId.UserTask;
                            case Activity.eActivityType.ManualTask:
                                return FlowNodeTypeId.ManualTask;
                            case Activity.eActivityType.ServiceTask:
                                return FlowNodeTypeId.ServiceTask;
                            case Activity.eActivityType.SendTask:
                                return FlowNodeTypeId.SendTask;
                            case Activity.eActivityType.ScriptTask:
                                return FlowNodeTypeId.ScriptTask;
                            case Activity.eActivityType.BusinessRuleTask:
                                return FlowNodeTypeId.BusinessRuleTask;
                            case Activity.eActivityType.Task:
                                return FlowNodeTypeId.Task;
                            case Activity.eActivityType.ReceiveTask:
                                return FlowNodeTypeId.ReceiveTask;
                            case Activity.eActivityType.EmbeddedSubProcess:
                                return FlowNodeTypeId.EmbeddedSubProcess;
                            case Activity.eActivityType.EventSubProcess:
                                return FlowNodeTypeId.EventSubProcess;
                            case Activity.eActivityType.AdhocSubProcess:
                                return FlowNodeTypeId.AdhocSubProcess;
                            case Activity.eActivityType.TransactionSubProcess:
                                return FlowNodeTypeId.TransactionSubProcess;
                            case Activity.eActivityType.CallActivitySubProcess:
                                return FlowNodeTypeId.CallActivitySubProcess;
                            case Activity.eActivityType.CallActivityGlobalTask:
                                return FlowNodeTypeId.CallActivityGlobalTask;
                        }

                        return FlowNodeTypeId.ObsoleteActivityInstance;
                    case FlowNode.eFlowNodeType.ChoreographyActivity:
                        return FlowNodeTypeId.ObsoleteActivityInstance;
                }
            }
            catch //(Exception e)
            {
                // ignored
            }

            return FlowNodeTypeId.ObsoleteActivityInstance;
        }
    }

    public bool HasIncoming => flowNode.HasIncoming;
    public bool IsStarting => !HasIncoming ||
                              flowNode.incoming.Any(i => i.sourceRef is StartEvent);
}
