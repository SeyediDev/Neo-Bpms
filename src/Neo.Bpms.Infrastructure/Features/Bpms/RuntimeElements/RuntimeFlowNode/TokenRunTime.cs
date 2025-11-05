using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.FlowElements;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.EventDefinition;
using Neo.Bpms.Infrastructure.Features.Bpms.Jobs.ExecutionJob;
using Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeFlowNode.RuntimeEvent.Catching;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
public abstract partial class FlowNodeRunTime
{
    internal abstract void ReceiveToken(ProcessInstance pi, LocalParameters inputData, SequenceFlow seq = null);

    internal void CompleteAndSavePi(FlowNodeInstance ai, LocalParameters inputData, TokenPattern pattern)
    {
        var data = FetchInputData(ai.pi, ai, inputData);//todo
        Complete(ai, data, pattern);
        DataStorage.SaveProcessInstance(ai.pi, ai, "TRT.1", true, DataStorage.LockChangeRequest.Unlock);
    }

    internal void Complete(FlowNodeInstance ai, LocalParameters outputData,
        TokenPattern pattern, string defaultSeqFlowId = null)
    {
        RunDataOutputAssociations(ai.pi, ai, outputData);
        if (!ai.Closed)
            ai.CompleteAndSave();

        if (!IsCompleted(ai, outputData))
            return;
        SendTokenToOutgoing(ai.pi, outputData, pattern, defaultSeqFlowId);
    }

    protected virtual bool IsCompleted(FlowNodeInstance ai, LocalParameters outputData)
    {
        return true;
    }

    internal virtual void SendTokenToOutgoing(ProcessInstance pi, LocalParameters inputData,
        TokenPattern pattern, string defaultSeqFlowId = null)
    {
        CheckAndChangeState(pi, flowNode.outputStateId);
        var sent = false;
        foreach (var sequenceFlow in flowNode.outgoing)
        {
            if (!ProcessVersion.TryGetFlowNodeRuntime(sequenceFlow.targetRef.Id, out var outgoingFlowNodeRuntime))
                continue;
            if (pattern.In(TokenPattern.Exclusive, TokenPattern.Inclusive))
            {
                if (!CheckSequenceFlowCondition(pi, inputData, sequenceFlow, outgoingFlowNodeRuntime))
                    continue;
            }

            var passed = SendTokenToOutput(pi, inputData, sequenceFlow, outgoingFlowNodeRuntime);
            if (!passed)
                continue;
            sent = true;
            if (pattern == TokenPattern.Exclusive)
                break;
        }

        if (!sent && !string.IsNullOrEmpty(defaultSeqFlowId) &&
            pattern.In(TokenPattern.Exclusive, TokenPattern.Inclusive))
        {
            sent = SendTokenToDefaultSequenceFlow(pi, defaultSeqFlowId, inputData);
        }

        if (!sent)
        {
            if (flowNode.InSubProcess && !flowNode.SubProcess.triggeredByEvent)
            {
                //todo fetch parent sub process activity
                //continue of embedded sub process
                if (ProcessVersion.TryGetFlowNodeRuntime(flowNode.SubProcess.Id, out var subProcessRuntime))
                {
                    subProcessRuntime.SendTokenToOutgoing(pi, inputData, TokenPattern.Parallel);
                    sent = true;
                }
            }
        }

        if (!sent)
            pi.CheckAndClosePiAndParentAiIfNotExecuting();
    }

    protected void CheckAndChangeState(ProcessInstance pi, int? stateId)
    {
        if (stateId == null || stateId <= 0) return;
        AuditTrace($"Change state to {stateId}", pi, null);
        pi.ChangeDataState(stateId.Value);
        DataStorage.SaveDataRecord(pi, null, "ChangeState", out var recordsAffected);
        if (recordsAffected > 0)
            TryCatchEvent<ConditionalEventDefinition>.Try(null, null, pi, null);
    }

    private bool CheckSequenceFlowCondition(ProcessInstance pi,
        LocalParameters localParameters, SequenceFlow output, FlowNodeRunTime flowNodeRunTime)
    {
        var expressionInInstance = new ExpressionInInstance(pi, null, localParameters, output.conditionExpression);
        if (!expressionInInstance.CheckCondition($"Flow {flowNodeRunTime.flowNode.Id}"))
        {
            AuditTrace($"Flow {flowNodeRunTime.flowNode.Id} condition is not true.", pi, null);
            return false;
        }

        return true;
    }

    private bool SendTokenToOutput(ProcessInstance pi,
        LocalParameters inputData, SequenceFlow sequenceFlow, FlowNodeRunTime nodeRunTime)
    {
        if (nodeRunTime.flowNode.inputStateId > 0)
        {
            if (nodeRunTime.flowNode.inputStateId != pi.DataStateId)
            {
                AuditTrace($"Waiting for state {nodeRunTime.flowNode.inputStateId}. current state is {pi.DataStateId}",
                    pi, null);
                return false;
            }
        }

        AuditTrace($"continue by sequence {nodeRunTime.flowNode.Id}", pi, null);
        CheckAndChangeState(pi, sequenceFlow.StateId);
        pi.Execution.AddJob(new SendToken
        {
            FlowNodeRunTime = nodeRunTime,
            ProcessInstance = pi,
            SequenceFlow = sequenceFlow,
            InputData = inputData?.Clone()
        });
        return true;
    }

    private bool SendTokenToDefaultSequenceFlow(ProcessInstance pi,
        string defaultSeqFlowId, LocalParameters inputData)
    {
        if (!ProcessVersion.definition.flowElements.TryGetValue(defaultSeqFlowId, out var flowElement))
            return false;
        var defaultSeqFlow = flowElement as SequenceFlow;
        if (defaultSeqFlow?.targetRef?.Id == null)
            return false;
        if (!ProcessVersion.TryGetFlowNodeRuntime(defaultSeqFlow.targetRef.Id, out var flowNodeRuntime))
            return false;
        AuditTrace($"continue by sequence {flowNodeRuntime.flowNode.Id}", pi, null);
        return SendTokenToOutput(pi, inputData, defaultSeqFlow, flowNodeRuntime);
    }

    internal enum TokenPattern
    {
        Exclusive,
        Inclusive,
        Parallel
    }
}
