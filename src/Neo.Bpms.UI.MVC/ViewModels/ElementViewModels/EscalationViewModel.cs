using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.UI.MVC.ViewModels.ElementViewModels;

public class EscalationViewModel
{
    public EscalationViewModel()
    {

    }

    public EscalationViewModel(Escalation e)
    {
        id = e.Id;
        name = e.Name;
        escalationCode = e.escalationCode;
        namespaceId = e.structure?.NamespaceId;
        entityId = e.structure?.Id;
    }

    public string id { get; set; }
    public string name { get; set; }
    public string escalationCode { get; set; }
    /// <summary>
    /// structureRef
    /// </summary>
    public string namespaceId { get; set; }
    public string entityId { get; set; }

    public Escalation ToEscalation()
    {
        return new Escalation(ProjectDefinition.Project.BpmnDefinitions, id, escalationCode, name,
            ProjectDefinition.Project.BpmnDefinitions.AddOrGetItemDefinition(false,
                ProjectDefinition.Project.GetEntity(namespaceId, entityId)))
        {
            Id = id,
            Name = name,
            escalationCode = escalationCode
        };
    }
}