using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.ViewModels.ElementViewModels;

public class MessageViewModel
{
    public MessageViewModel()
    {

    }
    public MessageViewModel(Message m)
    {
        id = m.Id;
        name = m.Name;
        namespaceId = m.itemRef?.structure?.NamespaceId;
        entityId = m.itemRef?.structure?.Id;
    }

    public string id { get; set; }
    public string name { get; set; }
    /// <summary>
    /// structureRef
    /// </summary>
    public string namespaceId { get; set; }
    public string entityId { get; set; }

    public Message ToMessage()
    {
        return new Message(ProjectDefinition.Project.BpmnDefinitions, id, name,
            ProjectDefinition.Project.BpmnDefinitions.AddOrGetItemDefinition(false,
            ProjectDefinition.Project.GetEntity(namespaceId, entityId)));
    }
}
