using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Models.Bpmn.Execution;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities.LoopCharacteristic;

namespace Neo.Bpms.Infrastructure.Features.Bpms;

public abstract partial class ActivityRuntime
{
    private bool IsCompletedMultiInstanceLoop(ActivityInstance ai, LocalParameters inputData)
    {
        if (Activity.loopCharacteristics is not MultiInstanceLoopCharacteristics multiInstanceLoop)
            return true;
        var instances = DataStorage.FetchActivityInstancesOfProcessInstance(ai.pi, this)
            .Where(i => i.state == ActivityInstanceStateId.Active)
            .ToList();
        if (CheckMultiInstanceCompletionCondition(multiInstanceLoop, ai, inputData))
        {
            foreach (var instance in instances)
            {
                CancelOtherInstances(instance);
            }
            return true;
        }
        //todo behavior
        if (instances.Count != 0)
        {
            if (multiInstanceLoop.isSequential)
            {
                var firstInstance = instances.Where(i => i.LoopCounter > ai.LoopCounter)
                    .OrderBy(i => i.LoopCounter)
                    .FirstOrDefault();
                if (firstInstance != null)
                    StartInstanceOfLoop(new LoopInstance
                    {
                        Ai = firstInstance,
                        InputData = inputData
                    });
            }

            return false;
        }

        return true;
    }

    private static void CancelOtherInstances(ActivityInstance instance)
    {
        instance.Withdrawn();
        instance.Save();
    }

    private void ReceiveTokenOnMultiInstanceLoop(ProcessInstance pi, LocalParameters inputData)
    {
        if (Activity.loopCharacteristics is not MultiInstanceLoopCharacteristics multiInstanceLoop)
            return;
        List<LoopInstance> instances = null;
        if (multiInstanceLoop.loopCardinality is FormalExpression loopCardinality)
            CreateMultiInstancesOnLoopCardinality(pi, inputData, multiInstanceLoop, loopCardinality,
                out instances);
        else if (multiInstanceLoop.loopDataInputRef != null)
            CreateMultiInstancesOnLoopDataInput(pi, inputData, multiInstanceLoop, out instances);
        if (instances != null && instances.Count > 0)
        {
            if (multiInstanceLoop.isSequential)
                StartInstanceOfLoop(instances.FirstOrDefault());
            else
            {
                foreach (var loopInstance in instances)
                    StartInstanceOfLoop(loopInstance);
            }
            return;
        }

        SendTokenToOutgoing(pi, inputData, TokenPattern.Inclusive);
    }

    private bool CheckMultiInstanceCompletionCondition(MultiInstanceLoopCharacteristics multiInstanceLoop,
        ActivityInstance ai, LocalParameters inputData)
    {
        if (ai == null)
            throw new ArgumentNullException(nameof(ai));
        if (multiInstanceLoop.completionCondition is not FormalExpression condition)
            return false;
        inputData ??= new LocalParameters(ai.pi.AuditTrail?.User);
        inputData.Add("LoopCounter", ai.LoopCounter);
        ai.SetData("LoopCounter", ai.LoopCounter);
        var expressionInInstance = new ExpressionInInstance(ai.pi, ai, inputData, condition.Expression);
        return expressionInInstance.CheckCondition("Standard Loop Condition : ");
    }

    private void CreateInstanceOfLoop(ProcessInstance pi,
        ICollection<LoopInstance> instances, long loopCounter, ElasticObject item)
    {
        var loopInstance = CreateInstanceOfLoop(pi, loopCounter, item?.ToLocalParameters());
        loopInstance.Ai.Activate();
        loopInstance.Ai.Save();
        instances.Add(loopInstance);
    }
}

internal class LoopInstance
{
    internal ActivityInstance Ai { get; set; }
    internal LocalParameters InputData { get; set; }
}
