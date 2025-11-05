namespace Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine;

public class TestEngineInterface : InternalServiceGroupDefinition
{
    public TestEngineInterface() : base("TestEngineInterface")
    {
    }

    public override void AddOperations()
    {
        AddOperation(new TestOperation());
        AddOperation(new RunTestOperation());
        AddOperation(new CatchTestSendTask());
    }
}
