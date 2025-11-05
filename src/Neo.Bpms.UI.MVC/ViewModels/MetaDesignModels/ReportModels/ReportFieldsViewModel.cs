namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class ReportFieldsViewModel
{
    public ReportFieldsViewModel()
    {

    }
    public ReportFieldsViewModel(Report report)
    {
        associationId = "";
        treeAssociationId = "";
        FillFields(report.reportFields.Values);
        FillAssociations(report);
    }

    public ReportFieldsViewModel(Report report, IncludeEntity association, string associationId, string treeAssociationId, bool addFields)
    {
        this.associationId = associationId;
        this.treeAssociationId = treeAssociationId;
        if (addFields)
            FillFields(association.reportFields.Values);
        FillAssociations(report);
    }

    public string associationId { get; set; }
    public string treeAssociationId { get; set; }
    public List<ReportColumnViewModel> fields { get; set; }
    public List<ReportFieldsViewModel> associations { get; set; }

    private void FillFields(ICollection<Report.Field> reportFields)
    {
        fields = [.. reportFields.Select(reportField => new ReportColumnViewModel(reportField))];
    }

    private void FillAssociations(Report report)
    {
        associations = [];
        foreach (IncludeEntity includeEntity in report.Includes ?? Enumerable.Empty<IncludeEntity>())
        {
            if (!includeEntity.AssociationId.StartsWith(treeAssociationId) ||
                includeEntity.AssociationId == treeAssociationId)
                continue;
            string[] ids = includeEntity.AssociationId[associationId.Length..].Split('.');
            string treeId = treeAssociationId;
            for (int i = 0; i < ids.Length; i++)
            {
                string id = ids[i];
                treeId = (string.IsNullOrEmpty(treeId) ? "" : treeId + ".") + id;
                if (associations.Any(a => a.treeAssociationId == treeId))
                    continue;
                associations.Add(new ReportFieldsViewModel(report, includeEntity, id, treeId, i == ids.Length - 1));
            }
        }
    }
}
