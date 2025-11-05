namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

/// <summary>
/// Base class to define processes and their details. All process definitions in the business are sub classes of this object.
/// for detail descriptions please refer to the model.
/// </summary>
public abstract partial class ProcessDefinition : BpmnDefinitionsDefinition
{
    /// <summary>
    /// Defines all definitions.
    /// </summary>
    /// <returns></returns>
    public override BpmnDefinitions DefineAll()
    {
        if (!Identify())
        {
            return null;
        }

        if (process == null)
        {
            return null;
        }

        if (!DefineResources())
        {
            return null;
        }

        Resources();

        if (!DefineOperations())
        {
            return null;
        }

        Operations();

        if (!DefineDataStores())
        {
            return null;
        }

        DataStores();

        if (!DefineEvents())
        {
            return null;
        }

        Events();

        if (!DefineProperties())
        {
            return null;
        }

        Properties();
        ExtractProperties();

        if (!DefineResourceRoles())
        {
            return null;
        }

        ResourceRoles();

        if (!DefineWorkflow())
        {
            return null;
        }

        Workflow();

        Diagrams();
        //if (!DefineBusinessRules()) return null;
        //if (!DefineServices()) return null;
        return definitions;
    }

    private void ExtractProperties()
    {
        Type propertyType = ExtractSubs<PropertyDefinition>()?.FirstOrDefault();
        if (propertyType == null)
        {
            return;
        }

        foreach (FieldInfo info in propertyType.GetFields())
        {
            _ = AddProperty(info.Name, info.FieldType.IsArray,
                info.Attributes.GetAttribute<EntityFieldMap>()?.DBName);
        }

        foreach (PropertyInfo info in propertyType.GetProperties())
        {
            _ = AddProperty(info.Name, info.PropertyType.IsArray,
                info.Attributes.GetAttribute<EntityFieldMap>()?.DBName);
        }
    }

    /// <summary>
    /// Define Human Resources
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineResources()
    {
        return true;
    }

    /// <summary>
    /// Define Human Resources
    /// </summary>
    protected virtual void Resources()
    {
    }

    /// <summary>
    /// Define Operations
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineOperations()
    {
        return true;
    }

    /// <summary>
    /// Define Operations
    /// </summary>
    /// <returns></returns>

    protected virtual void Operations()
    {
    }

    /// <summary>
    /// Define Data Stores
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineDataStores()
    {
        return true;
    }

    /// <summary>
    /// Define Data Stores
    /// </summary>
    /// <returns></returns>

    protected virtual void DataStores()
    {
    }

    /// <summary>
    /// Define Events
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineEvents()
    {
        return true;
    }

    /// <summary>
    /// Define Events
    /// </summary>
    /// <returns></returns>

    protected virtual void Events()
    {
    }

    /// <summary>
    /// Define Resource Roles
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineResourceRoles()
    {
        return true;
    }

    /// <summary>
    /// Define Resource Roles
    /// </summary>
    /// <returns></returns>
    protected virtual void ResourceRoles()
    {
    }

    /// <summary>
    /// Define Properties
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineProperties()
    {
        return true;
    }

    /// <summary>
    /// Define Properties
    /// </summary>
    /// <returns></returns>
    protected virtual void Properties()
    {
    }

    /// <summary>
    /// Define Workflow
    /// </summary>
    /// <returns></returns>
    protected virtual bool DefineWorkflow()
    {
        return true;
    }

    /// <summary>
    /// Define Workflow
    /// </summary>
    /// <returns></returns>
    protected virtual void Workflow()
    {
    }

    protected const int WithoutChangingState = 0;
    protected const int WCS = 0;
    private Process process { get; set; }
    private IFlowElementsContainer _flowElementsContainer;
    protected void DefineProcessEntity<T>()
    {
        _ = ProcessEntityDefinition.GetOrDefineEntity<T>();
    }

}
