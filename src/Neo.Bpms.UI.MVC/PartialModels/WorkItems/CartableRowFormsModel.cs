using Neo.Bpms.Infrastructure.Features.Bpms.Processes.Managers.Dto;

namespace Neo.Bpms.UI.MVC.PartialModels.WorkItems;

public class CartableRowFormsModel
{
    public WorkItemViewModel item { get; set; }
    public string caller { get; set; }
}