using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;

namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine;
public class TestOperation : InternalServiceOperationModel<TestOperationInput, TestOperationOutput>
{
    public TestOperation() : base("Test")
    {
    }
}
