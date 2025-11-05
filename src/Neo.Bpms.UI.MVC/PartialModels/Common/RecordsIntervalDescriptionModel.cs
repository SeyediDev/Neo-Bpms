namespace Neo.Bpms.UI.MVC.PartialModels.Common;

public class RecordsIntervalDescriptionModel
{
    public int pageNo { get; set; }
    public int recordsPerPage { get; set; }
    public int thisPageRowsCount { get; set; }
    public long totalRecordsCount { get; set; }
}