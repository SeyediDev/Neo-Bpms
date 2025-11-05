using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Model.Project;

public partial class ProjectContext(string projectName) : BaseModelClass(null, projectName, projectName), INameSpaceRepository
{
    public string CustomerName;
    public string ProjectName = projectName;
    public string ProjectCode;

    public string StartDate;
    public string SupportStartDate;
    public string FileMethod { get; set; }
    public string DefaultCalendar { get; set; } = "shamsi";
    public string DefaultCulture { get; set; } = "fa";
    public string DefaultCollation { get; set; } = "Persian_100_CI_AI";
    public bool CaptchaInLoginEnabled { get; set; } = false;
    public bool HasDesignFeatures { get; set; } = false;
    public bool NotifyInLogin { get; set; }

    public bool HasProcess => BusinessProcesses.Count != 0;

    public MenuItem MainMenuItem { get; set; }
}
