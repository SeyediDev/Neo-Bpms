using Neo.Bpms.Domain.Models.Cmmn.Partitions;

namespace Neo.Bpms.Domain.Features.Definitions.Entities;

public abstract partial class ModelDefinition
{
    protected virtual void Partitions()
    {
    }

    public void AddPartitionFunction(string id, Type valueType, PartitionFunctionType functionType,
        PartitionFunctionBoundaryType boundaryType, string startOfRange, string endOfRange, params string[] values)
    {
        model.AddPartitionFunction(id, valueType, functionType, boundaryType, startOfRange, endOfRange, values);
    }

    public void AddPartitionScheme(string id, string partitionFunction, FileGroupSelectionType fileGroupSelectionType,
        string fileGroupPrefix, params string[] fileGroups)
    {
        model.AddPartitionScheme(id, partitionFunction, fileGroupSelectionType, fileGroupPrefix, fileGroups);
    }
}
