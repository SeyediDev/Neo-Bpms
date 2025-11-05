using Neo.Bpms.UI.MVC.ViewModels.Forms;

namespace Neo.Bpms.UI.MVC.ViewModels.ProcessModels;

public class BulkWorkItemFormViewModel
{
    public WorkItemsFilter Filter { get; set; }
    public string FormId { get; set; }
    public BulkQueryType QueryType { get; set; }
    public List<string> AiList { get; set; }
    public ElasticObject Record { get; set; }
    public CommonFormStructure IndexStructure { get; set; }
    public string IndexFormFilterValues { get; set; }
    public string Caller { get; set; }
}