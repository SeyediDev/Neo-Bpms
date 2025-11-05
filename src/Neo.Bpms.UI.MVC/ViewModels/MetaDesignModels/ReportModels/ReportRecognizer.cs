namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class ReportRecognizer
{
    public ReportRecognizer()
    {

    }

    public ReportRecognizer(Report report)
    {
        id = report.Id;
        name = report.Name;
        entityId = report.EntityId;
        namespaceId = report.NamespaceId;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string entityId { get; set; }
    public string namespaceId { get; set; }
}
