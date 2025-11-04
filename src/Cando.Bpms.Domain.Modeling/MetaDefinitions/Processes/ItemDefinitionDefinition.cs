using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    protected ItemDefinition AddItemDefinition<T>(bool isCollection, string parentId, bool addToRootElement)
    {
        return definitions.AddOrGetItemDefinition(isCollection,
            ProjectDefinition.Project.GetEntity<T>());
    }

    protected ItemDefinition AddItemDefinition(bool isCollection,
        string namespaceId, string entityId)
    {
        var entity = ProjectDefinition.Project.GetEntity(namespaceId, entityId);
        return entity == null
            ? null
            : definitions.AddOrGetItemDefinition(isCollection, entity);
    }
}
