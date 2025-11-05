using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Domain.Models.Service.Internal;

public interface IInternalServiceOperationRuntime : IServiceOperationRuntime
{
    Task<LocalParameters> RunOperation(IAuditTrail auditTrail, LocalParameters inData);
}
