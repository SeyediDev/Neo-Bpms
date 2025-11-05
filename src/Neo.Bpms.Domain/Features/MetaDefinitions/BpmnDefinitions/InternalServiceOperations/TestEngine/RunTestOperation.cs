using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine;
public class RunTestOperation :
    InternalServiceOperationModel<RunTestOperationInput, RunTestOperationOutput>
{
    public RunTestOperation() : base("RunTest")
    {
    }
}
