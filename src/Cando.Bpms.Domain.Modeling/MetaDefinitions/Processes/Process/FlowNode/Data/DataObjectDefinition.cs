using Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Adds Data Object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">The Identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="isCollection">if set to <c>true</c> [is Collection]</param>
    /// <param name="saveOnPrimaryKeyProperty">Primary Key Property</param>
    /// <returns></returns>
    protected DataObject AddDataObject<T>(string id, string name, bool isCollection, string saveOnPrimaryKeyProperty = null)
    {
        if (definitions == null || process == null) return null;
        //todo 
        var itemDef = AddItemDefinition<T>(isCollection, id, true);
        var currentDataObject = new DataObject(process, id, name, itemDef, isCollection);

        if (currentSubProcess != null)
            currentSubProcess.flowElements.Add(id, currentDataObject);
        else
            process.flowElements.Add(id, currentDataObject);
        return currentDataObject;
    }
}
