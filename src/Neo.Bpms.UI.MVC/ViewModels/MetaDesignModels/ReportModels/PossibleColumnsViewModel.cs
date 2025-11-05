namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.ReportModels;

public class PossibleColumunsViewModel
{
    public IEnumerable<PossibleFieldViewModel> fields { get; set; }

    public IEnumerable<PossibleAssociationsViewModel> associations { get; set; }
}
