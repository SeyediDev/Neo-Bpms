using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.ViewModels.ElementViewModels;

public class SignalViewModel
{
    public SignalViewModel()
    {

    }
    public SignalViewModel(Signal signal)
    {
        id = signal.Id;
        name = signal.Name;
        namespaceId = signal.structure?.NamespaceId;
        entityId = signal.structure?.Id;
    }

    public string id { get; set; }
    public string name { get; set; }
    /// <summary>
    /// structureRef
    /// </summary>
    public string namespaceId { get; set; }
    public string entityId { get; set; }

    public Signal ToSignal()
    {
        return new Signal(ProjectDefinition.Project.BpmnDefinitions, id, name,
            ProjectDefinition.Project.BpmnDefinitions.AddOrGetItemDefinition(false,
                ProjectDefinition.Project.GetEntity(namespaceId, entityId)));
    }
}