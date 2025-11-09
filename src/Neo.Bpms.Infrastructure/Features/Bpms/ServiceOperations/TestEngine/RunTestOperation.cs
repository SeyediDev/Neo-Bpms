using Neo.Bpms.Domain.Features.MetaDefinitions.BpmnDefinitions.InternalServiceOperations.TestEngine.ProcessEntities;
using Neo.Bpms.Domain.Models.Service.Internal;
using Neo.Bpms.Domain.Models.Service.ServiceOperation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.ServiceOperations.TestEngine;

public class RunTestOperationRuntime :
    InternalServiceOperationRuntime<RunTestOperationInput, RunTestOperationOutput>
{
    protected override Task<RunTestOperationOutput> RunOperationAsync(IAuditTrail auditHeader, RunTestOperationInput input)
    {
        auditHeader.AddDetail((long)ServiceDetailTypeId.RunOperation, $"RunOperation of RunTestOperation. input:{input}", null, null, null, null);
        var generateException = input?.GenerateException ?? false;
        var errorCode = input?.ErrorCode ?? "library.ErrorOccurred";
        return generateException ? throw new Exception(errorCode) :
            Task.FromResult(new RunTestOperationOutput { ResultOperation = true });
    }
}
