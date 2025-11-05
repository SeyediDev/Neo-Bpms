using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.Domain.Models.Cmmn.UI.Forms;
using Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.FieldDefinitions;

public class TableDefinition(string alias) : InputFieldDefinition(alias)
{
    public FormField TableDef { get; set; }
    public bool Editable => PropertyBoolean(eControlPropertyId.Editable);
    public List<string> KeyFields { get; set; } = [];
    public List<InputFieldDefinition> Columns { get; set; }= [];
    public List<ColumnFieldDefinition> ColumnInfos => [.. Columns.OfType<ColumnFieldDefinition>()];
    public List<TableDefinition> Tables => [.. Columns.OfType<TableDefinition>()];
    public List<IndexFormSubjectId> Subjects { get; set; }
    public FormLogicDefinition Logic { get; set; } = new();
    public IDictionary<string, ComboData> CombosData = new Dictionary<string, ComboData>();

    public InputFieldDefinition MultipleForeignKeyField { get; set; }
}
