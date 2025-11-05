using Neo.Bpms.Domain.Entities.Cmmn.Entities;

namespace Neo.Bpms.Domain.Entities.Cmmn.DataSynchronization;

public interface IEntityReference
{
    string Name { get; }
    EntityAddress EntityAddress { get; }
}