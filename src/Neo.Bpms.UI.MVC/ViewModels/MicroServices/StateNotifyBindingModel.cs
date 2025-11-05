using Neo.Bpms.Infrastructure.Features.Bpms.MicroServices;

namespace Neo.Bpms.UI.MVC.ViewModels.MicroServices;

public class StateNotifyBindingModel
{
    public string funcName { get; set; }
    public int resourceIndex { get; set; }
    public int requestId { get; set; }
    public string detail { get; set; }
    public long? code { get; set; }
    public string data { get; set; }
    public StateNotifyStatus status { get; set; }
    public int? progress { get; set; }
    public DateTime? time { get; set; }
}
