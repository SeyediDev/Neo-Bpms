using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;

namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

public partial class Report : Form
{
    public List<PossibleSubReport> PossibleSubReports { get; set; }
    public List<ReportHaving> InputRecordsHavings { get; set; }
    [XmlIgnore]
    public Dictionary<string, ConfiguredReport> MetaConfigures { get; set; } = [];

    public ReportFields reportFields { get; set; } = [];

    /// <summary>
    /// Report
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="id">id</param>
    /// <param name="name">name</param>
    /// <param name="enName"></param>
    /// <returns></returns>
    public Report(UiEntity entity, string id, string name, string enName /*,
			eReportPosition position = eReportPosition.PageContent*/) /*, Report.eFlags Flags = Report.eFlags.vfPortrait*/
        : base(entity, id, name, enName, eFormType.Report)
    {
        //this.position = position;
    }

    public Report()
    {
        //this.position = eReportPosition.PageContent;
    }

    /// <summary>
    /// Add Column
    /// </summary>
    /// <param name="reportFieldsPrm"></param>
    /// <param name="fieldId">field</param>
    /// <returns></returns>
    public static Field AddColumn(ReportFields reportFieldsPrm, string fieldId, bool asGroupBy, bool asAggregation)
    {
        if (reportFieldsPrm.TryGetValue(fieldId, out Field field))
        {
            field.asColumn = true;
        }
        else
        {
            field = new Field
            {
                fieldId = fieldId,
                asAggregation = asAggregation,
                asGroupBy = asGroupBy,
                asColumn = true
            };
            reportFieldsPrm.TryAdd(fieldId, field);
        }
        return field;
    }

    /// <summary>
    /// Add Group By
    /// </summary>
    /// <param name="reportFieldsPrm"></param>
    /// <param name="fieldId">field</param>
    /// <returns></returns>
    public static Field AddGroupBy(ReportFields reportFieldsPrm, string fieldId)
    {
        if (reportFieldsPrm.TryGetValue(fieldId, out Field field))
        {
            field.asGroupBy = true;
        }
        else
        {
            field = new Field
            {
                fieldId = fieldId,
                asAggregation = false,
                asColumn = false,
                asGroupBy = true
            };
            reportFieldsPrm.TryAdd(fieldId, field);
        }
        return field;
    }

    /// <summary>
    /// Add Aggregation
    /// </summary>
    /// <param name="reportFieldsPrm"></param>
    /// <param name="fieldId">field</param>
    /// <returns></returns>
    public static Field AddAggregation(ReportFields reportFieldsPrm, string fieldId)
    {
        if (reportFieldsPrm.TryGetValue(fieldId, out Field field))
        {
            field.asAggregation = true;
        }
        else
        {
            field = new Field
            {
                fieldId = fieldId,
                asAggregation = true,
                asColumn = false,
                asGroupBy = false
            };
            reportFieldsPrm.TryAdd(fieldId, field);
        }
        return field;
    }

    /// <summary>
    /// Add Possible Sub Report
    /// </summary>
    /// <param name="entityName">entity Name</param>
    /// <param name="associationName">association Name</param>
    /// <param name="subReportId">sub Report</param>
    /// <returns></returns>
    public PossibleSubReport AddPossibleSubReport(string entityName, string associationName, string subReportId)
    {
        PossibleSubReport psr = new()
        {
            Name = Name,
            EntityId = entityName,
            AssociationName = associationName,
            SubReportId = subReportId
        };
        PossibleSubReports ??= [];
        PossibleSubReports.Add(psr);
        return psr;
    }

    public void AddInputRecordsHaving(ReportHaving having)
    {
        InputRecordsHavings ??= [];
        InputRecordsHavings.Add(having);
    }
}
