using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class DataElementDefinition<TDataElement> : BaseElementDefinition<TDataElement>
    where TDataElement : DataElement
{
}

public class PropertyDefinition
{
}

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds the process property. for detail descriptions please refer to the model.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="isCollection">if set to <c>true</c> [is collection].</param>
    /// <param name="entityFieldId">The entity field Identifier</param>
    /// <returns></returns>
    protected bool AddProcessProperty(string name, bool isCollection = false, string entityFieldId = null)
    {
        if (process == null)
        {
            return false;
        }

        string id = "Property." + name;
        if (string.IsNullOrEmpty(entityFieldId))
        {
            entityFieldId = name;
        }

        Property property = new(process, id, name,
            definitions.AddOrGetItemDefinition(isCollection, _processEntity?.GetField(entityFieldId)));
        process.properties ??= [];
        process.properties.Add(property);
        return true;
    }

    /// <summary>
    /// Adds the process property. for detail descriptions please refer to the model.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="isCollection">if set to <c>true</c> [is collection].</param>
    /// <param name="entityFieldId">The entity field Identifier</param>
    /// <returns></returns>
    protected bool AddProperty(string name, bool isCollection = false, string entityFieldId = null)
    {
        return AddProcessProperty(name, isCollection, entityFieldId);
    }

    /// <summary>
    /// Sets Process Display Fields
    /// </summary>
    /// <param name="displayFields">The display fields</param>
    /// <returns></returns>
    protected bool SetDisplayFields(string displayFields)
    {
        if (process == null)
        {
            return false;
        }

        process.DisplayFields = displayFields;
        return true;
    }

    /// <summary>
    /// Sets State Property
    /// </summary>
    /// <param name="stateProperty">state property</param>
    /// <returns></returns>
    protected bool SetStateProperty(string stateProperty)
    {
        if (process == null)
        {
            return false;
        }

        process.StateProperty = stateProperty;
        return true;
    }

    protected bool SetDescriptionProperty(string descriptionProperty)
    {
        return true;
    }
}
