using Neo.Bpms.Domain.Models.Cmmn.Fields;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Reports;

public class ReferField
{
    public string Id => AssociationPrefix + Field.Id;
    public EntityField Field { get; set; }
    public string AssociationPrefix { get; set; }
}