using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.ViewModels.ElementViewModels;

public class ErrorViewModel
{
    public ErrorViewModel()
    {

    }

    public ErrorViewModel(Error e)
    {
        id = e.Id;
        name = e.Name;
        errorCode = e.errorCode;
        namespaceId = e.structure?.NamespaceId;
        entityId = e.structure?.Id;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string errorCode { get; set; }
    /// <summary>
    /// structureRef
    /// </summary>
    public string namespaceId { get; set; }
    public string entityId { get; set; }

    public Error ToError()
    {
        return new Error(ProjectDefinition.Project.BpmnDefinitions, id, name, errorCode,
            ProjectDefinition.Project.BpmnDefinitions.AddOrGetItemDefinition(false,
                ProjectDefinition.Project.GetEntity(namespaceId, entityId)));
    }
}