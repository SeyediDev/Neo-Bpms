using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Gateways;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Lanes;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract class GatewayDefinition<TGateway> : FlowNodeDefinition<TGateway>
    where TGateway : Gateway
{
}

public abstract class ExclusiveGatewayDefinition : GatewayDefinition<ExclusiveGateway>
{
    protected override ExclusiveGateway FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new ExclusiveGateway(flowElementsContainer, ElementId, Name);
    }
}

public abstract class InclusiveGatewayDefinition : GatewayDefinition<InclusiveGateway>
{
    protected override InclusiveGateway FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new InclusiveGateway(flowElementsContainer, ElementId, Name);
    }
}

public abstract class ParallelGatewayDefinition : GatewayDefinition<ParallelGateway>
{
    protected override ParallelGateway FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new ParallelGateway(flowElementsContainer, ElementId, Name);
    }
}

public abstract class ComplexGatewayDefinition : GatewayDefinition<ComplexGateway>
{
    protected abstract string ActivationCondition { get; }

    protected override ComplexGateway FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        FormalExpression activationConditionExp = null;
        if (!string.IsNullOrEmpty(ActivationCondition))
        {
            activationConditionExp =
                new FormalExpression($"{ElementId}.ActivationCondition", Parser.ParseTree(ActivationCondition));
            if (activationConditionExp.Expression == null)
                throw new Exception(
                    $"Can not parse {ActivationCondition} in activationCondition in ComplexGateway {ElementId}");
        }

        return new ComplexGateway(flowElementsContainer, ElementId, Name, activationConditionExp);
    }
}

public abstract class EventBasedGatewayDefinition : GatewayDefinition<EventBasedGateway>
{
    protected virtual bool Initiate => false;

    protected virtual EventBasedGateway.eEventBasedGatewayType EventGatewayType =>
        EventBasedGateway.eEventBasedGatewayType.Exclusive;

    protected override EventBasedGateway FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        return new EventBasedGateway(flowElementsContainer, ElementId, Name, Initiate, EventGatewayType);
    }
}

public abstract partial class ProcessDefinition
{
    protected Gateway currentGateway;

    /// <summary>
    /// Adds the exclusive gateway.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected ExclusiveGateway AddExclusiveGateway(string actionId, string name, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var gw = new ExclusiveGateway(process, actionId, name);
        AddGateway(gw, _currentLane, outputStateId);
        return gw;
    }

    /// <summary>
    /// Adds the inclusive gateway.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected InclusiveGateway AddInclusiveGateway(string actionId, string name, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var gw = new InclusiveGateway(process, actionId, name);
        AddGateway(gw, _currentLane, outputStateId);
        return gw;
    }

    /// <summary>
    /// Adds the parallel gateway.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    protected ParallelGateway AddParallelGateway(string actionId, string name, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var gw = new ParallelGateway(process, actionId, name);
        AddGateway(gw, _currentLane, outputStateId);
        return gw;
    }

    /// <summary>
    /// Adds the complex gateway.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="activationCondition">The activation condition.</param>
    /// <returns></returns>
    protected ComplexGateway AddComplexGateway(string actionId, string name,
        FormalExpression activationCondition, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var gw = new ComplexGateway(process, actionId, name, activationCondition);
        AddGateway(gw, _currentLane, outputStateId);
        return gw;
    }

    //bool SetMinimumAmountOfRequiredFinishedParallelBranches(string MinimumNumberOfRequiredDoneOptionalBranchs) { }
    /// <summary>
    /// Adds the event based gateway.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <param name="initiate">if set to <c>true</c> [initiate].</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    protected EventBasedGateway AddEventBasedGateway(string actionId, string name, bool initiate,
        EventBasedGateway.eEventBasedGatewayType type, object outputStateId = null)
    {
        if (definitions == null || process == null) return null;
        var gw = new EventBasedGateway(process, actionId, name, initiate, type);
        AddGateway(gw, _currentLane, outputStateId);
        return gw;
    }

    /// <summary>
    /// Adds the gateway.
    /// </summary>
    /// <param name="gw">The gw.</param>
    /// <param name="lane">The lane.</param>
    /// <param name="outputStateId">The output state identifier.</param>
    /// <returns></returns>
    private void AddGateway(Gateway gw, Lane lane, object outputStateId)
    {
        currentGateway = gw;
        currentBaseElement = currentGateway;
        AddFlowNode(gw, lane, outputStateId);
    }
}