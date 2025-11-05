namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class FlowNodeDefinition<TFlowNode> : FlowElementDefinition<TFlowNode>, IFlowNodeDefinition
    where TFlowNode : FlowNode
{
    protected virtual LaneDefinition Lane => null;
    protected virtual object OutputStateId => null;

    protected virtual object InputStateId => null;

    protected override void PreDefinitions()
    {
        base.PreDefinitions();
        if (OutputStateId != null)
            Element.outputStateId = Convert.ToInt32(OutputStateId);
        if (InputStateId != null)
            Element.inputStateId = Convert.ToInt32(InputStateId);
    }

    protected void Outgoing<TFlowNodeDefinition>()
        where TFlowNodeDefinition : IFlowNodeDefinition
    {
        OutgoingElements.Add(typeof(TFlowNodeDefinition).Name);
    }

    public List<string> OutgoingElements = [];

    protected void Incoming<TFlowNodeDefinition>()
    where TFlowNodeDefinition : IFlowNodeDefinition
    {
        IncomingElements.Add(typeof(TFlowNodeDefinition).Name);
    }

    public List<string> IncomingElements = [];
}
public interface IFlowNodeDefinition { }

public abstract partial class ProcessDefinition
{
    private FlowNode _currentFlowNode;

    /// <summary>
    /// Sets the input state identifier.
    /// </summary>
    /// <param name="inputStateId">The input state identifier.</param>
    /// <returns></returns>
    protected bool SetInputStateId(object inputStateId)
    {
        if (_currentFlowNode == null) return false;
        _currentFlowNode.inputStateId = Convert.ToInt32(inputStateId);
        return true;
    }

    /// <summary>
    /// Adds the element.
    /// </summary>
    /// <param name="flowNode">The flow node.</param>
    /// <param name="lane"></param>
    /// <param name="outputStateId">The output state identifier.</param>
    private void AddFlowNode(FlowNode flowNode, Lane lane, object outputStateId)
    {
        flowNode.outputStateId = outputStateId == null ? 0 : Convert.ToInt32(outputStateId);
        _currentFlowNode = flowNode;
        lane?.addFlowElement(_currentFlowNode.Id);
        AddFlowElement(flowNode);
    }

    /// <summary>
    /// Finds the element.
    /// </summary>
    /// <param name="mainElementId">The main element identifier.</param>
    /// <returns></returns>
    private FlowElement FindElement(string mainElementId)
    {
        var flowElements = currentSubProcess != null
            ? currentSubProcess.flowElements
            : process.flowElements;
        return flowElements.TryGetValue(mainElementId, out var fe) ? fe : null;
    }
}
