namespace Neo.Bpms.UI.MVC.PartialModels.Common;

public class PaginationModel
{
    public int pageNo { get; set; }
    public int maxPageLinksToShow { get; set; }
    public int recordsPerPage { get; set; }
    public int thisPageRowsCount { get; set; }
    public long totalRecordsCount { get; set; }
}