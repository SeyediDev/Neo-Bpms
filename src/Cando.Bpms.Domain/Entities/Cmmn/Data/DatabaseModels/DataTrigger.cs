namespace Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;

public class DataTrigger
{
    public string Name { get; set; }
    public string Owner { get; set; }
    public string Schema { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsDelete { get; set; }
    public bool IsInsert { get; set; }
    public bool IsAfter { get; set; }
    public string IsInsteadOf { get; set; }
    public bool Disabled { get; set; }
}