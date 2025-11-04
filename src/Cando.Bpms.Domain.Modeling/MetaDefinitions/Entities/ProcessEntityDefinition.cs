using Neo.Bpms.Domain.Modeling.CodeFirst;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.Entities;

public class ProcessEntityDefinition
{
    public static string ProcessEntitiesNamespace = "ProcessEntities";

    public static INameSpaceRepository GetNameSpaceRepository()
    {
        return ProjectDefinition.Project;
    }

    /// <summary>
    /// Defines Entity
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <exception cref="Exception">Entity not defined :  + sType</exception>
    /// <returns></returns>
    public static Entity GetOrDefineEntity<T>()
    {
        Type tType = typeof(T);
        return GetOrDefineEntity(tType);
    }

    public static Entity GetOrDefineEntity(Type entityType)
    {
        Entity entity = ProjectDefinition.Project.GetEntityFromType(entityType)
                     ?? DefineProcessEntity(entityType);
        return entity;
    }

    /// <summary>
    /// Defines the entity.
    /// </summary>
    /// <param name="entityType">The type.</param>
    /// <returns></returns>
    public static Entity DefineProcessEntity(Type entityType)
    {
        INameSpaceRepository repository = GetNameSpaceRepository();
        string[] namespaceIds = entityType.Namespace?.Split('.');
        string namespaceId = namespaceIds?[^1] ?? "";
        ModelNamespace processEntitiesNs = repository.GetModel(namespaceId) ??
                                repository.AddNamespace(namespaceId, namespaceId, nameof(BpmsSchema.ProcessModel));
        Entity entity = new CSharpObjectToEntity(repository).DefineEntity(processEntitiesNs, entityType, out _) ?? throw new Exception($"Can not define process entity {entityType.Name} in Namespace {namespaceId}");
        entity.NotMapped = true;
        new CSharpObjectToEntityField(repository, entityType, entity, DependencyInjectionHolder.Instance.Logger)
            .DefineEntityFields();

        DefineSubTablesEntities(entityType);

        return entity;
    }

    private static void DefineSubTablesEntities(Type entityType)
    {
        List<MemberInfo> members = ReflectionField.Members(entityType).ToList();
        List<MemberInfo> genericListItems = members.Where(m =>
                ReflectionTools.IsGenericList(ReflectionField.FetchMemberType(m)))
            .ToList();
        foreach (MemberInfo tableItem in genericListItems)
        {
            Type listTableType = ReflectionField.FetchMemberType(tableItem);
            Type tableType = listTableType.GenericTypeArguments[0];
            Entity tableEntity = ProjectDefinition.Project.GetEntityFromType(tableType);
            if (tableEntity == null)
            {
                _ = DefineProcessEntity(tableType);
            }
        }
    }
}
