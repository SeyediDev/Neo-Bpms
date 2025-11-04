using Neo.Bpms.Domain.Entities.Base.Audit;
using Neo.Bpms.Domain.Entities.Service.ServiceOperation;

namespace Neo.Bpms.Domain.Entities.Service.Internal;

public interface IInternalServiceOperationRuntime : IServiceOperationRuntime
{
    Task<LocalParameters> RunOperation(IAuditTrail auditTrail, LocalParameters inData);
}
