namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

public class ProcessViewModel
{
    public string Id;
    public string Name;
    public string EntityId;
    public List<string> InputParams = [];
    public List<string> OuputParams = [];
    public List<string> Properties = [];

    public List<ResourceViewModel> Managers = [];
    public List<LaneViewModel> Lanes = [];

    public ElementViewModel GetElement(string elementId)
    {
        return Lanes.SelectMany(laneViewModel => laneViewModel.Elements).FirstOrDefault(e => e.Id == elementId);
    }
}
