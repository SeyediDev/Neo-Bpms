using Neo.Bpms.Domain.Models.Cmmn.Fields;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Excel.Import;

public class HeaderColumn
{
    public int Index { get; set; }
    public EntityField Field { get; set; }
    public string FieldName { get; set; }
}