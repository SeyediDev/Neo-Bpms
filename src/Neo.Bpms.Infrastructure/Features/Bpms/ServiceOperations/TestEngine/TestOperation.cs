using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;
using Neo.Bpms.Domain.Models.Service.Internal;
using Neo.Bpms.Domain.Models.Service.ServiceOperation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.ServiceOperations.TestEngine;

public class TestOperationRuntime : InternalServiceOperationRuntime<TestOperationInput, TestOperationOutput>
{
    protected override Task<TestOperationOutput> RunOperationAsync(IAuditTrail auditHeader, TestOperationInput input)
    {
        auditHeader.AddDetail((long)ServiceDetailTypeId.RunOperation, $"RunOperation of TestOperation. input:{input}", null, null, null, null);
        return null;
    }
}
