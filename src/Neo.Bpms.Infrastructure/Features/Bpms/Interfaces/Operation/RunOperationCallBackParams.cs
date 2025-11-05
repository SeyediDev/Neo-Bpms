using Neo.Bpms.Domain.Models.Base.Audit;

namespace Neo.Bpms.Infrastructure.Features.Bpms.Interfaces.Operation;

public class RunOperationCallBackParams(string operationImplementationRef, long id, string machineId, AuditTrail auditTrail) : IOperationUserParams
{
    public override string ToString()
    {
        return $"Operation {OperationName}, Id {Id}";
    }

    public long Id { get; } = id;

    public string PreferredMachineId { get; } = machineId;

    public string OperationName { get; } = operationImplementationRef;

    public AuditTrail AuditTrail { get; } = auditTrail;
}