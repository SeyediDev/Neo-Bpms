namespace Neo.Bpms.Domain.Entities.Service.ServiceOperation;

public interface IServiceOperation
{
    Type InputStructure { get; }
    Type OutputStructure { get; }
}
