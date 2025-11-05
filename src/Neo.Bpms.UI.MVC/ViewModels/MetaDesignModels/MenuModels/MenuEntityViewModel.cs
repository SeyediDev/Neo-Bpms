using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.Tasks.HumanTasks;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.MenuModels;

//weired name!
public class MenuEntityViewModel
{
    public MenuEntityViewModel()
    {

    }

    public MenuEntityViewModel(UiEntity entity)
    {
        id = entity.Id;
        name = entity.Name;
        namespaceId = entity.NamespaceId;
        dashboards = entity.GetDashboards()?.Values.Select(d => new MenuViewModel(d)) ?? [];
        reports = entity.GetReports()?.Select(report => new MenuViewModel(report)) ?? [];
        forms = entity.getForms()?.Where(f => f.FormType == Form.eFormType.Index).Select(form => new MenuViewModel(form)) ?? [];
        processCreateForms = GetProcessCreateForms(entity);
    }

    private static IEnumerable<MenuViewModel> GetProcessCreateForms(UiEntity entity)
    {
        List<Domain.Entities.Bpmn.Core.Infrastructure.BpmnDefinitions> processes = ProjectDefinition.Project.BusinessProcesses?.Values.SelectMany(p => p.Versions)
            .Select(pv => pv.Value.BpmnDefinitions)
            .Where(b => b.Process.EntityId == entity.Id).ToList();
        IEnumerable<UserTask> createProcessTasks = processes?.SelectMany(p => p.Process.flowElements.Values)
            .Select(f => f as UserTask).Where(f => f != null)
            .Where(f => entity.getForm(f.FormId)?.FormType == Form.eFormType.ProcessCreate);

        return createProcessTasks?
                .Select(task => new MenuViewModel(entity.getForm(task.FormId), task.Process, task.Id))
               ?? [];
    }

    public string id { get; set; }
    public string name { get; set; }
    public string namespaceId { get; set; }
    public IEnumerable<MenuViewModel> dashboards { get; set; }
    public IEnumerable<MenuViewModel> reports { get; set; }
    public IEnumerable<MenuViewModel> forms { get; set; }
    public IEnumerable<MenuViewModel> processCreateForms { get; set; }
}
