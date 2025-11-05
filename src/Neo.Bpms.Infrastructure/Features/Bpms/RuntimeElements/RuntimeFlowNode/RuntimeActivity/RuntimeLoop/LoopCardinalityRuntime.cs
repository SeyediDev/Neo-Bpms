using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.LoopCharacteristic;

namespace Neo.Bpms.Infrastructure.Features.Bpms;
public abstract partial class ActivityRuntime
{
    private void CreateMultiInstancesOnLoopCardinality(ProcessInstance pi, LocalParameters inputData,
        MultiInstanceLoopCharacteristics multiInstanceLoop,
        FormalExpression loopCardinality, out List<LoopInstance> instances)
    {
        instances = [];
        var instanceCount = CalcLoopCardinality(pi, inputData, loopCardinality);
        for (var loopCounter = 0; loopCounter < instanceCount; loopCounter++)
        {
            var instanceInputData = new ElasticObject { ["user"] = pi.AuditTrail?.User };
            if (inputData != null)
                instanceInputData.Merge(inputData);
            instanceInputData.SetField("LoopCounter", loopCounter);
            CreateInstanceOfLoop(pi, instances, loopCounter, instanceInputData);
        }
    }

    private int CalcLoopCardinality(ProcessInstance pi, LocalParameters inputData, FormalExpression lc)
    {
        var expressionInInstance = new ExpressionInInstance(pi, null, inputData, lc.Expression);
        var value = expressionInInstance.EvalExpression("LoopCardinality =");
        var instanceCount = 0;
        if (value != null && value as string != "undefined")
            instanceCount = Convert.ToInt32(value);
        return instanceCount;
    }
}