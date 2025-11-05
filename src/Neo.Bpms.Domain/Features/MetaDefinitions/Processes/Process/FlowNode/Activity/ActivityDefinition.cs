using Neo.Bpms.Domain.Models.Bpmn.Processes.Activities;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract class ActivityDefinition<TActivity> : FlowNodeDefinition<TActivity>
    where TActivity : Activity
{
}

public abstract partial class ProcessDefinition
{
    private Activity _currentActivity;
    //isForCompensation = false;
    //startQuantity = 1;
    //completionQuantity = 1;
    //bool SetQuantity(int StartQuantity, int CompletionQuantity, string ActivationCondition) { }

    //	//public SequenceFlow defaultSequence = null;

    //	//public InputOutputSpecification ioSpecification = null;
    //	//public List<DataInputAssociation> dataInputAssociations = new List<DataInputAssociation>();
    //	//public List<DataOutputAssociation> dataOutputAssociations = new List<DataOutputAssociation>();


    /// <summary>
    /// Adds the activity property.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="isCollection">if set to <c>true</c> [is collection].</param>
    /// <param name="entityFieldId">The entity field Id</param>
    /// <returns></returns>
    protected bool AddActivityProperty(string name, bool isCollection = false, string entityFieldId = null)
    {
        if (_currentActivity == null)
        {
            return false;
        }

        string id = _currentActivity.Id + ".Property." + name;
        if (string.IsNullOrEmpty(entityFieldId))
        {
            entityFieldId = name;
        }

        Property property = new(_currentActivity, id, name,
            definitions.AddOrGetItemDefinition(isCollection, _processEntity?.GetField(entityFieldId)));
        _currentActivity.addProperty(property);
        return true;
    }

    /// <summary>
    /// Adds the activity
    /// </summary>
    /// <param name="lane">The lane</param>
    /// <param name="outputStateId">The output state Id</param>
    /// <param name="activity">The Activity</param>
    /// <returns></returns>
    private Activity AddActivity(Lane lane, object outputStateId, Activity activity)
    {
        if (definitions == null || process == null)
        {
            return null;
        }

        AddFlowNode(activity, lane, outputStateId);
        _currentActivity = activity;
        currentBaseElement = activity;
        return activity;
    }

    /// <summary>
    /// Set Activity StartQuantity
    /// </summary>
    /// <param name="startQuantity">The startQuantity</param>
    protected void SetActivityStartQuantity(int startQuantity)
    {
        if (definitions == null || process == null || _currentActivity == null)
        {
            throw new Exception("Invalid Method sequence");
        }

        _currentActivity.startQuantity = startQuantity;
    }

    /// <summary>
    /// Set Activity CompletionQuantity
    /// </summary>
    /// <param name="completionQuantity">The completionQuantity</param>
    protected void SetActivityCompletionQuantity(int completionQuantity)
    {
        if (definitions == null || process == null || _currentActivity == null)
        {
            throw new Exception("Invalid Method sequence");
        }

        _currentActivity.completionQuantity = completionQuantity;
    }
    /// <summary>
    /// Set Activity Critical Time To Do
    /// </summary>
    /// <param name="duration"></param>
    protected void SetActivityCriticalTimeToDo(TimeSpan duration)
    {
        if (definitions == null || process == null || _currentActivity == null)
        {
            throw new Exception("Invalid Method sequence");
        }

        _currentActivity.CriticalTimeToDo = duration;
    }
    /// <summary>
    /// Set Activity Emergency Time To Do
    /// </summary>
    /// <param name="duration"></param>
    protected void SetActivityEmergencyTimeToDo(TimeSpan duration)
    {
        if (definitions == null || process == null || _currentActivity == null)
        {
            throw new Exception("Invalid Method sequence");
        }

        _currentActivity.EmergencyTimeToDo = duration;
    }
}
