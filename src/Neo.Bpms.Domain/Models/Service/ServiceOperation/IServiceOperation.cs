namespace Neo.Bpms.Domain.Models.Service.ServiceOperation;

public interface IServiceOperation
{
    Type InputStructure { get; }
    Type OutputStructure { get; }
}
