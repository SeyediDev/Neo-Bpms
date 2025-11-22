namespace Neo.Bpms.Domain.Features.Definitions.Entities;

public abstract partial class ModelDefinition<TNamespace> : ModelDefinition
{
    public override void DefineEntitiesDefinition()
    {
        var definitionsTypes = typeof(TNamespace).Assembly.GetLoadableTypes().Where(t => !t.IsAbstract).ToList();
        foreach (Type definitionsType in definitionsTypes.Where(ReflectionTools.IsInBaseInterface<IEntityDefinition>))
        {
            if (!definitionsType.IsAbstract)
            {
                Type[] types = [];
                ConstructorInfo cons = definitionsType.GetConstructor(types);
                object[] parameters = [];
                if (cons?.Invoke(parameters) is not EntityDefinition entityDefinition)
                {
                    continue;
                }
                if (entityDefinition.DefinitionEntity == null)
                {
                    continue;
                }
                var entity = (UiEntity)ProjectDefinition.Project.GetEntity(entityDefinition.DefinitionEntity);
                if (entity == null)
                {
                    continue;
                }
                if (entity.Defined)
                {
                    continue;
                }
                entityDefinition.Model = model;
                entityDefinition.Entity = entity;
                _ = entityDefinition.DefineFieldStructure();
                entityDefinition.DefineUI();
            }
        }
    }
}
/// <summary>
/// Base class to define Namespaces and their details. All Namespaces(Models) in the business and meta models are sub classes of this object.
/// Namespaces are the main classification and container tool for the business entities.
/// </summary>
public abstract partial class ModelDefinition : BaseModelingDefinition
{
    /// <summary>
    /// Identifies the entities.
    /// </summary>
    /// <returns></returns>
    public ModelNamespace IdentifyEntities()
    {
        _ = Identify();
        return model;
    }

    public void IdentifyDetail()
    {
        if (model == null)
        {
            return;
        }

        Partitions();
        _ = DefineEnumerations();
        Enumerations();
        _ = DefineEntities();
        Entities();
    }

    public virtual void DefineEntitiesDefinition() { }

    /// <summary>
    ///     Determines whether the current type is or implements the specified generic interface, and determines that
    ///     interface's generic type parameters.</summary>
    /// <returns>
    ///     True if the current type is or implements the specified generic interface.</returns>
    protected abstract bool Identify();

    //public abstract bool DefineFields();
    /// <summary>
    /// Defines Entities
    /// </summary>
    /// <returns></returns>
    public virtual bool DefineEntities()
    {
        return true;
    }

    /// <summary>
    /// Defines Entities
    /// </summary>
    /// <returns></returns>

    protected virtual void Entities()
    {
    }

    /// <summary>
    /// Defines the model(Namespace).
    /// Namespace is the main classification and container tool for the business entities.
    /// </summary>
    /// <param name="enName">Name of the en.</param>
    /// <param name="name">The name.</param>
    /// <param name="schema">The abstract prefix.</param>
    /// <param name="providerId">The abstract prefix.</param>
    /// <param name="dontMap">The abstract prefix.</param>
    /// <returns></returns>
    protected bool DefineModel(string enName, string name, string schema,
        string providerId, bool dontMap = false)
    {
        model = new ModelNamespace(name, enName, schema)
        {
            Provider = providerId,
            DontSync = dontMap
        };
        currentBaseElement = model;
        return true;
    }

    public ModelNamespace model;

    /// <summary>
    /// Defines Entities
    /// </summary>
    /// <returns></returns>
    public bool DefineEntity<TEntity>()
    {
        return DefineEntity(typeof(TEntity));
    }

    protected static INameSpaceRepository GetNameSpaceRepository()
    {
        return ProjectDefinition.Project;
    }
    /// <summary>
    /// Defines the entity.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    private bool DefineEntity(Type type)
    {
        Entity entity = new CSharpObjectToEntity(GetNameSpaceRepository()).DefineEntity(model, type, out _);
        return entity != null;
    }

    public void DefineEntities<TInterface>(Assembly asm)
    {
        IEnumerable<Type> types = asm.GetLoadableTypes().Where(t => !t.IsAbstract);
        foreach (Type type in types.Where(ReflectionTools.IsInBaseInterface<TInterface>))
        {
            if (!type.IsAbstract)
            {
                _ = DefineEntity(type);
            }
            if (type.IsEnum)
            {
                DefineEnumeration(type);
            }
        }
    }

}
