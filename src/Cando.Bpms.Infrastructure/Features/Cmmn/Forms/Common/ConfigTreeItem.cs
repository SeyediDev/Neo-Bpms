namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Common;

public class ConfigTreeItem
{
    public string id { get; set; }
    public string parent { get; set; }
    public string text { get; set; }
    public string type { get; set; }
    public dynamic data { get; set; }
}
