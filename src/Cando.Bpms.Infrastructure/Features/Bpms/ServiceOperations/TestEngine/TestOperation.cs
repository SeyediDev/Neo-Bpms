using Neo.Bpms.Domain.Entities.Service.Internal;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;
using Neo.Bpms.Domain.Modeling.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;

namespace Neo.Bpms.Infrastructure.Features.Bpms.ServiceOperations.TestEngine;

public class TestOperationRuntime : InternalServiceOperationRuntime<TestOperationInput, TestOperationOutput>
{
    protected override Task<TestOperationOutput> RunOperationAsync(IAuditTrail auditHeader, TestOperationInput input)
    {
        auditHeader.AddDetail((long)ServiceDetailTypeId.RunOperation, $"RunOperation of TestOperation. input:{input}", null, null, null, null);
        return null;
    }
}
