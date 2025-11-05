global using Neo.Bpms.Domain.Features.Definitions.Entities.Processes;
global using Neo.Bpms.Domain.Features.MetaDefinitions.Entities;


namespace Neo.Bpms.Domain.Features.Definitions.Entities;

/// <summary>
/// Base class and utilities for all modeling definition helpers
/// </summary>
public abstract partial class BaseModelingDefinition
{
    protected BaseModelClass currentBaseElement { get; set; }

    protected IEnumerable<Type> ExtractSubs<T>()
    {
        return GetType().ExtractSubs<T>();
    }

    protected IEnumerable<T?> ExtractSubsInstances<T>()
    {
        return GetType().ExtractSubsInstances<T>();
    }
}
