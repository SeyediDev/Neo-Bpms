namespace Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

public class ElementViewModel
{
    public string Id;
    public string Name;
    public string Type;
    public string ProcessLink;
    public List<FlowElementViewModel> Inputs = [];
    public List<FlowElementViewModel> Outputs = [];
    public int ColorIndex { get; set; }
    public int Colspan => Math.Max(Inputs.Count, Outputs.Count);
}
