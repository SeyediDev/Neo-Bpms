using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Gateways;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public class SequenceFlowDefinition : FlowElementDefinition<SequenceFlow>
{
    protected override string ElementId => $"{SourceRef.Id}_{TargetRef.Id}";
    protected override string Name => SequenceName;

    public string SequenceName { get; set; }
    public bool IsDefault { get; set; }
    public string Condition { get; set; }
    public FlowNode SourceRef { get; set; }
    public FlowNode TargetRef { get; set; }
    public bool IsImmediate { get; set; } = true;

    protected override SequenceFlow FlowElementCreation(IFlowElementsContainer flowElementsContainer)
    {
        if (SourceRef is not InclusiveGateway && SourceRef is not ExclusiveGateway &&
            SourceRef is not ComplexGateway &&
            SourceRef is not Activity)
            throw new Exception(
                $"Element {SourceRef.Id} is not InclusiveGateway or ExclusiveGateway that contains conditional input {TargetRef.Id}");
        FormalExpression exp = null;
        if (!string.IsNullOrEmpty(Condition))
        {
            exp = new FormalExpression($"{ElementId}.Condition", Parser.ParseTree(Condition));
            if (exp.Expression == null)
                throw new Exception(
                    $"Can not parse {Condition} in conditional input {TargetRef.Id} in Element {SourceRef.Id}");
        }

        return new SequenceFlow(flowElementsContainer, ElementId, Name, SourceRef, TargetRef, exp, IsImmediate);
    }

    protected override void PreDefinitions()
    {
        if (IsDefault && SourceRef is IHasDefaultSequenceFlow hasDefaultSequenceFlow)
        {
            hasDefaultSequenceFlow.defaultSequenceFlowId = ElementId;
        }
    }
    protected override void Definitions()
    {
    }
}

public abstract partial class ProcessDefinition
{
    protected SequenceFlow currentSequenceFlow;

    /// <summary>
    /// Adds the input action.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="isDefault">if set to <c>true</c> [is default].</param>
    /// <param name="stateId">The out put state</param>
    /// <returns></returns>
    protected SequenceFlow AddInputAction(string actionId, bool isDefault = false, object stateId = null)
    {
        if (FindElement(actionId) is not FlowNode srcElement)
            throw new Exception($"Input Element {actionId} in {_currentFlowNode.Id} not defined ");
        currentSequenceFlow =
            new SequenceFlow(process, actionId + "_" + _currentFlowNode.Id, "", srcElement, _currentFlowNode)
            {
                StateId = stateId != null ? Convert.ToInt32(stateId) : 0
            };
        process.flowElements.Add(currentSequenceFlow.Id, currentSequenceFlow);
        currentBaseElement = currentSequenceFlow;
        if (isDefault)
            SetDefaultSequence(srcElement);
        return currentSequenceFlow;
    }

    /// <summary>
    /// Adds the conditional input.
    /// </summary>
    /// <param name="actionId">The action identifier.</param>
    /// <param name="condition">The expression.</param>
    /// <param name="name">The name.</param>
    /// <param name="isDefault">if set to <c>true</c> [is default].</param>
    /// <param name="stateId">The out put state</param>
    /// <returns></returns>
    protected SequenceFlow AddConditionalInput(string actionId,
        string condition, string name, bool isDefault = false, object stateId = null)
    {
        if (FindElement(actionId) is not FlowNode srcElement)
            throw new Exception($"Input Element {actionId} in {_currentFlowNode.Id} not defined ");
        if (srcElement is not InclusiveGateway && srcElement is not ExclusiveGateway &&
            srcElement is not ComplexGateway &&
            srcElement is not Activity)
            throw new Exception(
                $"Element {_currentFlowNode.Id} is not InclusiveGateway or ExclusiveGateway that contains conditional input {actionId}");
        FormalExpression exp;
        if (!string.IsNullOrEmpty(condition))
        {
            exp = new FormalExpression(actionId + "_" + _currentFlowNode.Id + ".Condition",
                Parser.ParseTree(condition));
            if (exp.Expression == null)
                throw new Exception(
                    $"Can not parse {condition} in conditional input {actionId} in Element {_currentFlowNode.Id}");
        }
        else
            throw new Exception($"please use none conditional input {actionId} in Element {_currentFlowNode.Id}");

        currentSequenceFlow = new SequenceFlow(process, actionId + "_" + _currentFlowNode.Id, name,
            srcElement, _currentFlowNode, exp)
        {
            StateId = stateId != null ? Convert.ToInt32(stateId) : 0
        };
        process.flowElements.Add(currentSequenceFlow.Id, currentSequenceFlow);

        currentBaseElement = currentSequenceFlow;
        if (isDefault)
            SetDefaultSequence(srcElement);
        return currentSequenceFlow;
    }

    /// <summary>
    /// Sets the default sequence.
    /// </summary>
    /// <param name="srcElement">The source element.</param>
    private void SetDefaultSequence(FlowNode srcElement)
    {
        if (srcElement is IHasDefaultSequenceFlow hasDefaultSequenceFlow)
        {
            hasDefaultSequenceFlow.defaultSequenceFlowId = currentSequenceFlow?.Id;
        }
    }

    /// <summary>
    /// Selects the element.
    /// </summary>
    /// <param name="elId">The el identifier.</param>
    /// <returns></returns>
    protected bool SelectElement(string elId)
    {
        _currentFlowNode = FindElement(elId) as FlowNode;
        if (_currentFlowNode == null)
            throw new Exception("Can not select element " + elId + " in process " + definitions.Id);
        return (_currentFlowNode != null);
    }
}