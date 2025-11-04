namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

public class ConditionalFormatting
{
    public enum eType
    {
        Column = 1,
        Row
    }
    public eType Type = eType.Row;
    public string ColumnName;
    public string Filter;
    public string ClassName;

    public string QueryName
        => $"_format_{ClassName?.Replace("-", "_")}_{Type}" + (Type == eType.Column ? "_" + ColumnName : "");
}
