namespace Neo.Bpms.Infrastructure.Utility.Interfaces.Operation;
public interface IOperationUserParams
{
    long Id { get; }
    string PreferredMachineId { get; }
    string OperationName { get; }
    AuditTrail AuditTrail { get; }
}
