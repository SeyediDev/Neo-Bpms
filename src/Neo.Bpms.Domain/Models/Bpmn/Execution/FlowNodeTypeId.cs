using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.CatchEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.ThrowEvent;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;

namespace Neo.Bpms.Domain.Models.Bpmn.Execution;

public enum FlowNodeTypeId
{
    Task = Activity.eActivityType.Task,
    UserTask = Activity.eActivityType.UserTask,
    ManualTask = Activity.eActivityType.ManualTask,
    ServiceTask = Activity.eActivityType.ServiceTask,
    SendTask = Activity.eActivityType.SendTask,
    ScriptTask = Activity.eActivityType.ScriptTask,
    BusinessRuleTask = Activity.eActivityType.BusinessRuleTask,

    ReceiveTask = Activity.eActivityType.ReceiveTask,

    EmbeddedSubProcess = Activity.eActivityType.EmbeddedSubProcess,
    EventSubProcess = Activity.eActivityType.EventSubProcess,
    AdhocSubProcess = Activity.eActivityType.AdhocSubProcess,
    TransactionSubProcess = Activity.eActivityType.TransactionSubProcess,

    CallActivitySubProcess = Activity.eActivityType.CallActivitySubProcess,
    CallActivityGlobalTask = Activity.eActivityType.CallActivityGlobalTask,

    ObsoleteActivityInstance = 200,
    EventWaitingInstance = 300,
    Start = EventWaitingInstance + CatchEventLocation.Start,
    IntermediateCatch = EventWaitingInstance + CatchEventLocation.IntermediateCatch,
    Boundary = EventWaitingInstance + CatchEventLocation.Boundary,
    IntermediateThrow = EventWaitingInstance + ThrowEventLocation.IntermediateThrow,
    End = EventWaitingInstance + ThrowEventLocation.End,
    Implicit = EventWaitingInstance + ThrowEventLocation.Implicit,

    GatewayWaitingInstance = 400,
    ExclusiveGateway = GatewayWaitingInstance + Gateway.eGatewayType.Exclusive,
    InclusiveGateway = GatewayWaitingInstance + Gateway.eGatewayType.Inclusive,
    ParallelGateway = GatewayWaitingInstance + Gateway.eGatewayType.Parallel,
    ComplexGateway = GatewayWaitingInstance + Gateway.eGatewayType.Complex,
    EventBased = GatewayWaitingInstance + Gateway.eGatewayType.EventBased
}
