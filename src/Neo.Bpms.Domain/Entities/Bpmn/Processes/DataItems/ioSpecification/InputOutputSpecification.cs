using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Neo.Bpms.Domain.Entities.Cmmn.Fields;

namespace Neo.Bpms.Domain.Entities.Bpmn.Processes.DataItems.ioSpecification;

// Certain Activities and CallableElements contain a InputOutputSpecification element to describe their data requirements. 
// Execution semantics are defined for the InputOutputSpecification and they apply the same way to all elements that extend it. 
// Not every Activity type defines inputs and outputs, only Tasks, CallableElements (Global Tasks and Processes) MAY define their data requirements
// Embedded Sub-Processes MUST NOT define Data Inputs and Data Outputs directly, however they MAY define them indirectly via MultiInstanceLoopCharacteristics.
public class InputOutputSpecification(IIoSpecificationContainer baseElement, string id) : BaseElement(baseElement as BaseElement, id), IDataInputContainer, IDataOutputContainer
{
    /// <summary>
    /// An optional reference to the Data Inputs of the InputOutputSpecification. If the InputOutputSpecification defines no Data Input, 
    /// it means no data is REQUIRED to start the Activity. This is an ordered set.
    /// ordered list
    /// </summary>
    public List<DataInput> dataInputs { get; set; }

    /// <summary>
    /// An optional reference to the Data Outputs of the InputOutputSpecification. If the InputOutputSpecification defines no Data Output, 
    /// it means no data is REQUIRED to finish the Activity. This is an ordered set.
    /// ordered list
    /// </summary>
    public List<DataOutput> dataOutputs { get; set; }

    /// <summary>
    /// A reference to the InputSets defined by the InputOutputSpecification. Every InputOutputSpecification MUST define at least one InputSet.
    /// </summary>
    public List<InputSet> inputSets { get; set; }

    /// <summary>
    /// A reference to the OutputSets defined by the InputOutputSpecification. Every Data Interface MUST define at least one OutputSet
    /// </summary>
    public List<OutputSet> outputSets { get; set; }

    public void Set(Entity inputStruct, Entity outputStruct)
    {
        if (inputStruct?.entityFields != null)
        {
            dataInputs = [];
            //InputSet inputSet = null;
            //if (inputSets == null || inputSets.Count == 0)
            //{
            //	inputSet = new InputSet(this, id+".InputSet.0", "InputSet");
            //	inputSets = new List<InputSet> { inputSet};
            //}
            foreach (KeyValuePair<string, EntityField> fld in inputStruct.entityFields)
            {
                dataInputs.Add(new DataInput(this, $"{Id}.DataInput.{fld.Key}", fld.Key, null, false));
                //if(inputSet!=null)
                //	inputSet.dataInputRefs.Add(new DataInputRef());
            }
        }
        if (outputStruct?.entityFields != null)
        {
            dataOutputs = [];
            foreach (KeyValuePair<string, EntityField> fld in outputStruct.entityFields)
            {
                dataOutputs.Add(new DataOutput(this, $"{Id}.DataOutput.{fld.Key}", fld.Key, null, false));
            }
        }
    }

    public IItemAwareElement GetItemAwareElement(string itemId, string itemName, bool fromInputItems)
    {
        return ItemAwareContainer.GetItemAwareElement(this, itemId, itemName, fromInputItems);
    }
}

public interface IIoSpecificationContainer : IItemAwareContainer
{
    InputOutputSpecification ioSpecification { get; set; }
}
//Execution Semantics for Data:
//When an element that defines an InputOutputSpecification is ready to begin execution by means of Sequence
//Flow or Event being caught, the inputs of the interface are filled with data coming from elements in the context, such as
//Data Objects or Properties. The way to represent these assignments is the Data Association elements.
//Each defined InputSet element will be evaluated in the order they are included in the InputOutputSpecification.
//For each InputSet, the data inputs it references will be evaluated if it is valid.
//All data associations that define as target the data input will be evaluated, and if any of the sources of the data association
//is “unavailable,” then the InputSet is “unavailable” and the next InputSet is evaluated.
//The first InputSet where all data inputs are “available” (by means of data associations) is used to start the execution of
//the Activity. If no InputSet is “available,” then the execution will wait until this condition is met.
//The time and frequency of when and how often this condition is evaluated is out of scope for this International Standard.
//Implementations will wait for the sources of data associations to become available and then re-evaluate the InputSets.
//In the case of throw and catch Events, given their nature, the execution semantics for data is different.
//When a throw Event is activated, all DataInputAssociations of the event are executed, filling the Data Inputs of
//the Event. Finally, DataInputs are then copied to the elements thrown by the Event (Messages, Signals, etc.).
//Since there are no InputSets defined for Events, the execution will never wait.
//When a catch Event is activated, Data Outputs of the event are filled with the element that triggered the Event. Then
//all DataOutputAssociations of the Event are executed. There are no OutputSets defined for Events.
//To allow invoking a Process from both a Call Activity and via Message Flow, the Start Event and End Event
//support an additional case.
//In the case of a Start Event, the Data Inputs of the enclosing process are available as targets to the
//DataOutputAssociations of the Event. This way the Process Data Inputs can be filled using the elements that
//triggered the Start Event.
//In the case of an End Event, the Data Outputs of the enclosing process are available as sources to the
//DataInputAssociations of the Event. This way the resulting elements of the End Event can use the Process
//Data Outputs as sources.
