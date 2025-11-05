using Neo.Bpms.Domain.Models.Cmmn.Partitions;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels;

public class PartitionSchemeViewModel
{
    public string id { get; set; }
    public string function { get; set; }
    public string preFix { get; set; }
    public IList<string> fileGroups { get; set; }
    public string namespaceId { get; set; }
    public PartitionSchemeViewModel()
    {

    }
    public PartitionSchemeViewModel(PartitionScheme partitionScheme)
    {
        id = partitionScheme.Id;
        function = partitionScheme.PartitionFunction?.Id;
        preFix = partitionScheme.FileGroupPrefix;
        fileGroups = partitionScheme.FileGroups;
        namespaceId = (partitionScheme.Parent as ModelNamespace)?.Id;
    }

    public void Modify(PartitionScheme partitionScheme)
    {
        partitionScheme.Id = id;
        partitionScheme.PartitionFunction = (partitionScheme.Parent as ModelNamespace)?.GetPartitionFunction(function);
        partitionScheme.FileGroupPrefix = preFix;
        partitionScheme.FileGroups = fileGroups?.ToList();
    }
}
