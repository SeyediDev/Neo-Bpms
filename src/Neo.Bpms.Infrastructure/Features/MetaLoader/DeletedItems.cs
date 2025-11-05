namespace Neo.Bpms.Infrastructure.Features.MetaLoader;

public class DeletedItems
{
    public List<DeletedItem> Ids { get; set; } = [];
}

public class DeletedItem
{
    public string Id { get; set; }
    public string Name { get; set; }
}
