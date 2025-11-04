namespace Neo.Bpms.Domain.Entities.Service.ServiceOperation;

public interface IServiceOperationRuntime
{
    Type InputStructure { get; }
    Type OutputStructure { get; }
}
