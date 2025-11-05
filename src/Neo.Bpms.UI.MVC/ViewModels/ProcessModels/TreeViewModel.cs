// ReSharper disable UnusedAutoPropertyAccessor.Global

using Neo.Bpms.Domain.Entities.CmmnConfig;

namespace Neo.Bpms.UI.MVC.ViewModels.ProcessModels;

public class TreeViewModel
{
    public TreeViewModel(TreeConfig processTree, long dbId)
    {
        Id = processTree.Id.ToString();
        DbId = dbId;
        ParentId = processTree.ParentId?.ToString() ?? "#";
        OrderId = processTree.OrderId;
        Name = processTree.Name;
        NodeTypeId = processTree.NodeTypeId;
        StandardCode = processTree.StandardCode;
    }

    public TreeViewModel()
    {
    }

    public string Id { get; set; }
    public long DbId { get; set; }
    public string ParentId { get; set; }
    public double OrderId { get; set; }
    public string Name { get; set; }
    public TreeNodeTypeId NodeTypeId { get; set; }
    public string StandardCode { get; set; }
    public object data { get; set; }
}