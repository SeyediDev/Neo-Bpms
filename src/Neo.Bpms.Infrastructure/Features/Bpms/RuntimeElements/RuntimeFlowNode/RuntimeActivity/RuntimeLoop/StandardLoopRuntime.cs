using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public abstract partial class ActivityRuntime
{
    private bool IsCompletedStandardLoop(ActivityInstance ai, LocalParameters inputData)
    {
        if (Activity.loopCharacteristics is not StandardLoopCharacteristics standardLoop)
            return true;
        var loopCounter = ai.LoopCounter + 1;
        if (CheckLoopMaximum(standardLoop, loopCounter) &&
            CheckStandardLoopCondition(standardLoop, ai.pi, inputData, loopCounter))
        {
            CreateAndStartLoopInstance(ai.pi, loopCounter, inputData);
            return false;
        }

        return true;
    }

    private void ReceiveTokenOnStandardLoop(ProcessInstance pi, LocalParameters inputData)
    {
        if (Activity.loopCharacteristics is not StandardLoopCharacteristics standardLoop)
            return;
        const long loopCounter = 0;
        if (CheckLoopMaximum(standardLoop, loopCounter) &&
            standardLoop.testBefore && CheckStandardLoopCondition(standardLoop, pi, inputData, loopCounter)
            || !standardLoop.testBefore)
            CreateAndStartLoopInstance(pi, loopCounter, inputData);
        else
            SendTokenToOutgoing(pi, inputData, TokenPattern.Inclusive);
    }

    private bool CheckStandardLoopCondition(StandardLoopCharacteristics standardLoop,
        ProcessInstance pi, LocalParameters inputData, long loopCounter)
    {
        if (standardLoop.loopCondition is not FormalExpression condition)
            return true;
        inputData.Add("LoopCounter", loopCounter);
        var expressionInInstance = new ExpressionInInstance(pi, null, inputData, condition.Expression);
        return expressionInInstance.CheckCondition("Standard Loop Condition : ");
    }

    private static bool CheckLoopMaximum(StandardLoopCharacteristics standardLoop, long loopCounter)
    {
        return loopCounter < standardLoop.loopMaximum || standardLoop.loopMaximum == 0;
    }
}
