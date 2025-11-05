using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class BpmnDefinitionsDefinition
{
    protected Error AddError(string code, string name, string namespaceId, string entityId)
    {
        var item = ProjectDefinition.Project.GetErrorByCode(code);
        if (item == null)
        {
            string errorId = $"Error.{code}";
            item = new Error(ProjectDefinition.Project.BpmnDefinitions, errorId, name, code,
                ProjectDefinition.Project.AddOrGetItemDefinition(false, namespaceId, entityId));
            ProjectDefinition.Project.AddError(item);
        }

        currentBaseElement = item;
        return item;
    }

    protected Escalation AddEscalation<TEntity>(string code, string name)
    {
        Entity entity = ProcessEntityDefinition.GetOrDefineEntity<TEntity>();
        return AddEscalation(code, name, entity);
    }

    protected Escalation AddEscalation(string code, string name, Entity entity = null)
    {
        var item = ProjectDefinition.Project.GetEscalationByCode(code);
        if (item != null)
        {
            return item;
        }

        var id = ProjectContext.GetEscalationIdByCode(code);
        item = new Escalation(ProjectDefinition.Project.BpmnDefinitions, id, code, name,
            ProjectDefinition.Project.AddOrGetItemDefinition(false, entity?.NamespaceId, entity?.Id));
        ProjectDefinition.Project.AddEscalation(item);
        currentBaseElement = item;
        return item;
    }

    protected Message AddMessage<TEntity>(string code, string name)
    {
        var entity = ProjectDefinition.Project.GetEntity<TEntity>() ?? ProcessEntityDefinition.DefineProcessEntity(typeof(TEntity));
        var item = AddMessage2RootElements(entity, code, name);
        currentBaseElement = item;
        return item;
    }

    protected static Message AddMessage2RootElements(IModelEntity entity, string code, string name)
    {
        if (entity == null)
        {
            throw new Exception("can not find structure of message " + code +
                                ". please define the entity for this message structure.");
        }

        var item = ProjectDefinition.Project.GetMessageByCode(code);
        if (item != null)
        {
            return item;
        }

        var id = ProjectContext.GetMessageIdByCode(code);
        item = new Message(ProjectDefinition.Project.BpmnDefinitions, id, name,
            ProjectDefinition.Project.AddOrGetItemDefinition(false, ((Entity)entity)?.NamespaceId, entity.Id));
        ProjectDefinition.Project.AddMessage(item);
        return item;
    }

    protected Signal AddSignal(string code, string name, string namespaceId, string entityId)
    {
        var item = ProjectDefinition.Project.GetSignal(code);
        if (item == null)
        {
            string signalId = code; //todo ProjectContext.GetSignalIdByCode(code);
            item = new Signal(ProjectDefinition.Project.BpmnDefinitions, signalId, name,
                ProjectDefinition.Project.AddOrGetItemDefinition(false, namespaceId, entityId));
            ProjectDefinition.Project.AddSignal(item);
        }

        currentBaseElement = item;
        return item;
    }
}
