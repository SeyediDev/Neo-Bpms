namespace Neo.Bpms.Domain.Models.Service.ServiceOperation;

public interface IServiceOperationRuntime
{
    Type InputStructure { get; }
    Type OutputStructure { get; }
}
