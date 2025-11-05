namespace Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;

public interface IEntityLoader<TProjectMetaDefinition>
{
    void LoadEntity(TProjectMetaDefinition projectMetaDefinition, bool loadUi);
    void DefineAllNamespaces(TProjectMetaDefinition projectMetaDefinition);
}
public class EntityLoader<TProjectMetaDefinition>(ISpecificEntitiesLoader specificEntitiesLoader)
    : IEntityLoader<TProjectMetaDefinition>
    where TProjectMetaDefinition : ProjectMetaDefinition, new()
{
    public void LoadEntity(TProjectMetaDefinition projectMetaDefinition, bool loadUi)
    {
        DefineAndLoadNamespaceDetails(projectMetaDefinition, loadUi);
    }

    public void DefineAllNamespaces(TProjectMetaDefinition projectMetaDefinition)
    {
        projectMetaDefinition.DefineNamespaceNames();
        foreach (ModelDefinition nsp in projectMetaDefinition.Namespaces.Values)
        {
            ModelNamespace ns = nsp.IdentifyEntities();
            _ = ProjectDefinition.Project.AddNamespace(ns);
        }
    }

    private void DefineAndLoadNamespaceDetails(TProjectMetaDefinition projectMetaDefinition, bool loadUi)
    {
        foreach (ModelDefinition nsp in projectMetaDefinition.Namespaces.Values)
        {
            nsp.IdentifyDetail();
        }
        foreach (ModelNamespace model in ProjectDefinition.Project.Namespaces.Values)
        {
            DefineEntityStructures(model, ProjectDefinition.Project);
        }
        specificEntitiesLoader.LoadSpecificEntities();
        ReconfigureRelations();
        ExtractEntityField.CalcFieldsFromHierarchiesAndRelationships(ProjectDefinition.Project);

        foreach (ModelDefinition nsp in projectMetaDefinition.Namespaces.Values)
        {
            nsp.DefineEntitiesDefinition();
        }
        foreach (ModelNamespace model in ProjectDefinition.Project.Namespaces.Values)
        {
            DefineEntitiesDefinitions(model, loadUi);
        }

        if (loadUi)
        {
            specificEntitiesLoader.LoadSpecificUi();
        }
    }

    /// <summary>
    /// Defines the entity structures.
    /// </summary>
    /// <returns></returns>
    private static void DefineEntityStructures(ModelNamespace model, INameSpaceRepository repository)
    {
        foreach (Entity entity in model.GetEntities().Values)
        {
            if (entity is not UiEntity uiEntity)
            {
                continue;
            }

            DefineEntityStructure(model, uiEntity, repository);
        }
    }

    /// <summary>
    /// Defines the entity structure.
    /// </summary>
    /// <param name="modelDef"></param>
    /// <param name="entity">The entity.</param>
    /// <param name="repository"></param>
    /// <returns></returns>
    private static void DefineEntityStructure(ModelNamespace model, UiEntity entity, INameSpaceRepository repository)
    {
        Type modelType = model.GetType();
        Type entityType = entity.EntityType ??
            modelType.GetNestedType(entity.Id)
                   ?? Assembly.GetAssembly(model.GetType()).GetType(modelType.Namespace + "." + entity.Id);

        Type definitionsType =
            modelType.GetNestedType(entity.Id)
                    ?? Assembly.GetAssembly(entityType.GetType()).GetType(modelType.Namespace + "." + entity.Id + "Definitions")
                    ?? Assembly.GetAssembly(model.GetType()).GetType(modelType.Namespace + "." + entity.Id + "Definitions");
        if (entityType == null)
        {
            if (definitionsType == null)
            {
                System.Diagnostics.Debug.Assert(true, "Entity definition not found : " + entity.Id);
                return;
            }
        }

        if (entityType != null)
        {
            new CSharpObjectToEntityField(repository, entityType, entity, DependencyInjectionHolder.Instance.Logger)
                .DefineEntityFields();
        }
        else if (definitionsType?.IsSubclassOf(typeof(EntityDefinition)) ?? false)
        {
            Type[] types = [];
            ConstructorInfo cons = definitionsType.GetConstructor(types);
            object[] parameters = [];
            if (cons?.Invoke(parameters) is not EntityDefinition entityDefinition)
            {
                return;
            }

            entityDefinition.Model = model;
            entityDefinition.Entity = entity;
            _ = entityDefinition.DefineFieldStructure();
        }

    }

    /// <summary>
    /// Defines the entities definitions.
    /// </summary>
    /// <returns></returns>
    private static void DefineEntitiesDefinitions(ModelNamespace model, bool loadUi)
    {
        Dictionary<string, UiEntity> entities = [];
        bool again = true;
        while (again)
        {
            again = false;
            foreach (Entity entity in model.GetEntities().Values.ToList())
            {
                if (entity is not UiEntity uiEntity)
                {
                    continue;
                }

                if (entities.ContainsKey(uiEntity.Id))
                {
                    continue;
                }

                again = true;
                entities.Add(uiEntity.Id, uiEntity);
                DefineEntityDefinitions(model, uiEntity, loadUi);
            }
        }
    }

    /// <summary>
    /// Defines the entity additional information.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="entity">The entity.</param>
    /// <param name="loadUi"></param>
    /// <returns></returns>
    private static void DefineEntityDefinitions(ModelNamespace model, UiEntity entity, bool loadUi)
    {
        Type thisType = model.GetType();
        Assembly assembly = Assembly.GetAssembly(model.GetType());
        Type entityType = entity.EntityType ?? thisType.GetNestedType(entity.Id) ??
                    assembly.GetType(thisType.Namespace + "." + entity.Id);
        if (entityType == null || entityType.IsAbstract)
        {
            return;
        }

        Assembly entityTypeAssembly = Assembly.GetAssembly(entityType);

        string sType = entity.Id + "Definitions";
        Type entityDefinitionType = thisType.GetNestedType(sType) ?? GetEntityDefinitionType(model, sType, entityTypeAssembly, entityType);
        if (entityDefinitionType == null)
        {
            foreach (ModelNamespace @namespace in ProjectDefinition.Project.Namespaces.Values)
            {
                entityDefinitionType = GetEntityDefinitionType(@namespace, sType, entityTypeAssembly, entityType);
                if (entityDefinitionType != null)
                {
                    break;
                }
            }
        }
        if (entityDefinitionType == null && entity.IsEnum)
        {
            entityDefinitionType = typeof(StringListDefinitions);
        }

        if (entityDefinitionType == null)
        {
            entityDefinitionType = typeof(DefaultCRUDDefinition);
        }

        if (entityDefinitionType == null)
        {
            return;
        }

        if (!entityDefinitionType.IsSubclassOf(typeof(EntityDefinition)))
        {
            return;
        }

        Type[] types = [];
        ConstructorInfo cons = entityDefinitionType.GetConstructor(types);
        object[] parameters = [];
        if (cons?.Invoke(parameters) is not EntityDefinition entityDefinition)
        {
            return;
        }

        entityDefinition.Model = model;
        entityDefinition.Entity = entity;
        entityDefinition.DefineOperations();
        if (loadUi)
        {
            entityDefinition.DefineUI();
        }
    }

    private static Type GetEntityDefinitionType(ModelNamespace model, string sType, Assembly entityTypeAssembly, Type entityType)
    {
        Type thisType = model.GetType();
        Assembly assembly = Assembly.GetAssembly(model.GetType());

        return entityTypeAssembly.GetType(entityType.Namespace + "." + sType) ??
            assembly.GetType(thisType.Namespace + "." + sType) ??
            entityTypeAssembly.GetType(thisType.Namespace + "." + sType) ??
            assembly.GetType(entityType.Namespace + "." + sType);
    }

    private static void ReconfigureRelations()
    {
        foreach (ModelNamespace @namespace in ProjectDefinition.Project.Namespaces.Values)
        {
            foreach (Entity entity in @namespace.GetEntities().Values)
            {
                foreach (EntityField entityField in entity.entityFields.Values)
                {
                    if (entityField.Association != null)
                    {
                        entityField.Association.DestEntity = ProjectDefinition.Project.GetEntity(entityField.Association.DestNamespaceId, entityField.Association.DestEntityId);
                    }
                }
            }
        }
    }
}
