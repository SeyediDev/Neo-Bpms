namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

public class LaneViewModel
{
    public string Id;
    public string Name;
    public string ParentId;
    public ResourceViewModel Performer = new();
    public List<ElementViewModel> Elements = [];
    public int Colspan => Elements.Sum(element => 1 + element.Colspan);
}
