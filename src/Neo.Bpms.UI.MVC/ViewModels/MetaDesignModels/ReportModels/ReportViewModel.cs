namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class ReportViewModel
{
    public ReportViewModel()
    {
    }
    public ReportViewModel(Report report)
    {
        namespaceId = report.NamespaceId;
        entityId = report.EntityId;
        id = report.Id;
        name = report.Name;
        reportFields = new ReportFieldsViewModel(report);
        subReports = report.PossibleSubReports?.Select(rf => new SubReportViewModel(rf));
        filters = report.InputRecordsFilters?.Select(f => new FilterViewModel(f));
    }

    public string namespaceId { get; set; }
    public string entityId { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public ReportFieldsViewModel reportFields { get; set; }
    public IEnumerable<SubReportViewModel> subReports { get; set; }
    public IEnumerable<FilterViewModel> filters { get; set; }


    public void ModifyReport(Report report)
    {
        report.Id = id;
        report.Name = name;
        report.PossibleSubReports = subReports?.Select(sr => sr.ToPossibleSubReport()).ToList();
        report.reportFields = SetReportFields(report, reportFields);
        report.Includes = SetIncluds(report, report.Includes, "", reportFields);
        report.InputRecordsFilters = filters?.Select(f => f.ToFilter()).ToList();
    }

    private static List<IncludeEntity> SetIncluds(Report report,
            List<IncludeEntity> oldIncludes, string associationPrefix,
            ReportFieldsViewModel parent)
    {
        if (parent.associations == null || parent.associations.Count == 0) return null;
        List<IncludeEntity> includes = [];
        foreach (ReportFieldsViewModel reportFieldsViewModel in parent.associations)
        {
            string associationId =
                $"{associationPrefix}{(string.IsNullOrEmpty(associationPrefix) ? "" : ".")}{reportFieldsViewModel.associationId}";
            IncludeEntity include = oldIncludes?.FirstOrDefault(i => i.AssociationId == associationId)
                          ?? new IncludeEntity { AssociationId = associationId };
            include.reportFields = SetReportFields(report, reportFieldsViewModel);
            includes.Add(include);
            List<IncludeEntity> subs = SetIncluds(report, oldIncludes, associationId, reportFieldsViewModel);
            if (subs != null)
                includes.AddRange(subs);
        }
        return includes;
    }

    private static Report.ReportFields SetReportFields(Report report, ReportFieldsViewModel reportFields)
    {
        Report.ReportFields fields = [];
        foreach (ReportColumnViewModel col in reportFields.fields ?? Enumerable.Empty<ReportColumnViewModel>())
        {
            if (fields.TryGetValue(col.id, out _))
                fields.TryRemove(col.id, out _);
            fields.TryAdd(col.id, col.ToReportField(report));
        }
        return fields;
    }
}
