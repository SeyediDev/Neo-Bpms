namespace Neo.Bpms.Domain.Models.Cmmn.Data.DDL;

public class ForeignKeyItem
{
    public string FKName { get; set; }
    public string ForeignTable { get; set; }
    public string ForeignTableSchema { get; set; }
    public string BaseTable { get; set; }
    public string BaseTableSchema { get; set; }
    public string DELETE_RULE { get; set; }
    public string UPDATE_RULE { get; set; }
    public string ForeignKeyColumn { get; set; }
    public string BaseKeyColumn { get; set; }
}