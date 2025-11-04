namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.FilterModels;

public class EntityFilterInfo(CommonFormStructure structure, Form form, ElasticObject filterValues, string sortFields)
{
    public CommonFormStructure Structure { get; } = structure;
    public Form Form { get; } = form;
    public ElasticObject FilterValues { get; } = filterValues;
    public string SortFields { get; } = sortFields;
}
