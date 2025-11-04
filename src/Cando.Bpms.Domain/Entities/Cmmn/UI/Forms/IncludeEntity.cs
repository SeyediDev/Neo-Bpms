namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

/// <summary>
/// Include Entity to include related entity fields in the form
/// </summary>
public class IncludeEntity
{
    public string AssociationId { get; set; }
    public Report.ReportFields reportFields { get; set; } = [];
    public IncludeEntity() { }
}