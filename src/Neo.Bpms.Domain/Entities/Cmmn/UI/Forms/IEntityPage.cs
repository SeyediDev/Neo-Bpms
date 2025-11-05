namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

public interface IEntityItem
{
    public string NamespaceId { get; }
    public string EntityId { get; }
    public string Id { get; }
}
public interface IEntityPage
{
    UiEntity Entity { get; }
    long DbId { get; set; }
    string Id { get; set; }
    string Name { get; set; }
}
