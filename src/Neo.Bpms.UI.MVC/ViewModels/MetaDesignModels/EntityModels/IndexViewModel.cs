namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EntityModels;

public class IndexViewModel
{
    public IndexViewModel()
    {

    }
    public IndexViewModel(EntityIndex index)
    {
        name = index.Id;
        isUnique = index.IsUnique;
        isCluster = index.Clustered;
        fields = index.Fields.Select(f => new IndexFieldViewModel(f));
    }

    public string name { get; set; }
    public bool isUnique { get; set; }
    public bool isCluster { get; set; }
    public IEnumerable<IndexFieldViewModel> fields { get; set; }

    public EntityIndex ToEntityIndex()
    {
        return new EntityIndex
        {
            Id = name,
            EnName = name,
            Name = name,
            IsUnique = isUnique,
            Clustered = isCluster,
            Fields = [.. fields.Select(f => f.ToIndexField())]
        };
    }
}
