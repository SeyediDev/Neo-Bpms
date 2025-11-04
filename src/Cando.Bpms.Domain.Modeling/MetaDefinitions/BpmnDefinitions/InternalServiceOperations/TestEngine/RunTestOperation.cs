using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine;
public class RunTestOperation :
    InternalServiceOperationModel<RunTestOperationInput, RunTestOperationOutput>
{
    public RunTestOperation() : base("RunTest")
    {
    }
}
