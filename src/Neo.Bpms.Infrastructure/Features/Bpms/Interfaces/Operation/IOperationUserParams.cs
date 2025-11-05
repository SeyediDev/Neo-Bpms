using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Interfaces.Operation;
public interface IOperationUserParams
{
    long Id { get; }
    string PreferredMachineId { get; }
    string OperationName { get; }
    AuditTrail AuditTrail { get; }
}
