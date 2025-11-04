namespace Neo.Bpms.Domain.Entities.Cmmn.UI;

public partial class Report
{
    /// <summary>
    /// Possible Sub Report
    /// 
    /// Possible Sub Report may be from this entity or related entity, in the second situation, 
    /// an association in the related entity (main entity) of the sub report has association to the report entity
    /// </summary>
    public class PossibleSubReport
    {
        public string Name { get; set; }
        public SubReportType Type { get; set; }
        public string NamespaceId { get; set; }
        public string EntityId { get; set; }
        public string SubReportId { get; set; }
        public string AssociationName { get; set; }
    }
}
