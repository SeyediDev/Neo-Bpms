using static Neo.Bpms.Domain.Entities.Cmmn.UI.Report;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class SubReportViewModel
{
    public SubReportViewModel()
    {

    }
    public SubReportViewModel(Report report, string associationName)
    {
        namespaceId = report.NamespaceId;
        entityId = report.EntityId;
        subReportId = report.Id;
        this.associationName = associationName;
        name = report.Name;
        subReportType = SubReportType.SubReport | SubReportType.DrillDown; //todo ?
    }
    public SubReportViewModel(PossibleSubReport rf)
    {
        namespaceId = rf.NamespaceId;
        entityId = rf.EntityId;
        subReportId = rf.SubReportId;
        associationName = rf.AssociationName;
        name = rf.Name;
        subReportType = rf.Type;
    }
    public string namespaceId { get; set; }
    public string entityId { get; set; }
    public string subReportId { get; set; }
    public string associationName { get; set; }
    public string name { get; set; }
    public SubReportType subReportType { get; set; } // todo!

    public PossibleSubReport ToPossibleSubReport()
    {
        return new PossibleSubReport
        {
            NamespaceId = namespaceId,
            EntityId = entityId,
            SubReportId = subReportId,
            AssociationName = associationName,
            Name = name,
            Type = subReportType
        };
    }
}
