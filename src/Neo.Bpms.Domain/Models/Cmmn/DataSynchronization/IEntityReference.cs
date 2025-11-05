namespace Neo.Bpms.Domain.Models.Cmmn.DataSynchronization;

public interface IEntityReference
{
    string Name { get; }
    EntityAddress EntityAddress { get; }
}