using Neo.Bpms.Infrastructure.Features.Bpms.Interfaces.Operation;

namespace Neo.Bpms.Infrastructure.Features.Bpms.RuntimeElements.RuntimeOperation;

internal class BPMNRunOperationCallBackParams : IOperationUserParams
{
    public ProcessInstance PI { get; set; }
    public ActivityInstance AI { get; set; }

    public override string ToString()
    {
        return $"Operation {AI?.OperationName}, AI_Id {AI?.Id}, PI_Id {PI?.Id}";
    }

    public long Id => AI.Id;
    public string PreferredMachineId => AI.MachineId;
    public string OperationName => AI.OperationName;
    public AuditTrail AuditTrail => AI.AuditTrail;
}