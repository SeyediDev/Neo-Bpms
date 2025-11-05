using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.Expressions;
using Neo.Bpms.Domain.Entities.Bpmn.Core.CommonElements.ItemAware;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Activities.LoopCharacteristic;
using Neo.Bpms.Domain.Entities.Bpmn.Processes.Events.ThrowEvent;

namespace Neo.Bpms.Domain.Modeling.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    protected LoopCharacteristics currentLoopCharacteristics;

    /// <summary>
    /// Sets the standard loop.
    /// </summary>
    /// <param name="loopCondition">The loop condition.</param>
    /// <param name="testBefore">if set to <c>true</c> [test before].</param>
    /// <param name="loopMaximum">The loop maximum.</param>
    /// <returns></returns>
    protected bool SetStandardLoop(string loopCondition, bool testBefore = false, int loopMaximum = 0)
    {
        if (_currentActivity == null)
        {
            return false;
        }

        currentLoopCharacteristics = new StandardLoopCharacteristics(_currentActivity,
            _currentActivity.Id + ".StandardLoopCharacteristics",
            new FormalExpression(_currentActivity.Id, Parser.ParseTree(loopCondition)), testBefore, loopMaximum);
        return SetLoopCharacteristics(currentLoopCharacteristics);
    }

    /// <summary>
    /// Sets the multi instance loop.
    /// </summary>
    /// <param name="isSequential">if set to <c>true</c> [is sequential].</param>
    /// <param name="loopCardinality">The loop cardinality.</param>
    /// <param name="completionCondition">The completion condition.</param>
    /// <returns></returns>
    protected bool SetMultiInstanceLoopCardinality(bool isSequential, string loopCardinality,
        string completionCondition = null)
    {
        if (_currentActivity == null)
        {
            return false;
        }

        currentLoopCharacteristics = new MultiInstanceLoopCharacteristics(_currentActivity,
            _currentActivity.Id + ".MultiInstanceLoopCharacteristics")
        {
            isSequential = isSequential,
            loopCardinality = new FormalExpression(_currentActivity.Id, Parser.ParseTree(loopCardinality)),
            completionCondition = !string.IsNullOrEmpty(completionCondition)
                ? new FormalExpression(_currentActivity.Id, Parser.ParseTree(completionCondition))
                : null
        };
        return SetLoopCharacteristics(currentLoopCharacteristics);
    }

    /// <summary>
    /// Sets Multi Instance Loop By Data Input
    /// </summary>
    /// <param name="isSequential">is Sequential</param>
    /// <param name="loopDataInputRef">loop Data Input Reference</param>
    /// <param name="completionCondition">The completion Condition</param>
    /// <returns></returns>
    protected bool SetMultiInstanceLoopByDataInput(bool isSequential, IItemAwareElement loopDataInputRef,
        string completionCondition = null)
    {
        //todo rename to SetMultiInstanceLoopDataInputByDataStore
        if (_currentActivity == null)
        {
            return false;
        }

        MultiInstanceLoopCharacteristics loopCharacteristics = new(_currentActivity,
            _currentActivity.Id + ".MultiInstanceLoopCharacteristics")
        {
            isSequential = isSequential,
            loopDataInputRef = loopDataInputRef,
            completionCondition = !string.IsNullOrEmpty(completionCondition)
                ? new FormalExpression(_currentActivity.Id, Parser.ParseTree(completionCondition))
                : null,
        };
        currentLoopCharacteristics = loopCharacteristics;
        return SetLoopCharacteristics(currentLoopCharacteristics);
    }

    /// <summary>
    /// Sets Multi Instance Loop By Collection Property
    /// </summary>
    /// <param name="isSequential">is Sequential</param>
    /// <param name="collectionProperty">The collection Property</param>
    /// <param name="completionCondition">The completion Condition</param>
    /// <returns></returns>
    protected bool SetMultiInstanceLoopByCollectionProperty(bool isSequential, string collectionProperty,
        string completionCondition = null)
    {
        //todo rename to SetMultiInstanceLoopDataInputByProperty
        if (_currentActivity == null)
        {
            return false;
        }

        MultiInstanceLoopCharacteristics loopCharacteristics = new(_currentActivity,
            _currentActivity.Id + ".MultiInstanceLoopCharacteristics")
        {
            isSequential = isSequential,
            loopDataInputRef = process.GetProperty(collectionProperty),
            completionCondition = !string.IsNullOrEmpty(completionCondition)
                ? new FormalExpression(_currentActivity.Id, Parser.ParseTree(completionCondition))
                : null,
        };
        currentLoopCharacteristics = loopCharacteristics;
        return SetLoopCharacteristics(currentLoopCharacteristics);
    }

    /// <summary>
    /// Adds the multi instance loop complex behavior.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <param name="implicitEventId">The implicit event identifier.</param>
    /// <param name="implicitEventName">Name of the implicit event.</param>
    /// <param name="eventType">Type of the event.</param>
    /// <returns></returns>
    protected bool AddMultiInstanceLoopComplexBehavior(string condition, string implicitEventId,
        string implicitEventName,
        Event.eEventType eventType)
    {
        if (currentLoopCharacteristics is not MultiInstanceLoopCharacteristics loop)
        {
            return false;
        }

        loop.complexBehaviorDefinition ??= [];
        loop.complexBehaviorDefinition.Add(new ComplexBehaviorDefinition(
            loop, loop.Id + ".ComplexBehaviorDefinition",
            new FormalExpression(_currentActivity.Id, Parser.ParseTree(condition)),
            new ImplicitThrowEvent(process, implicitEventId, implicitEventName)));
        return true;
    }

    /// <summary>
    /// Sets the loop characteristics.
    /// </summary>
    /// <param name="lc">The lc.</param>
    /// <returns></returns>
    private bool SetLoopCharacteristics(LoopCharacteristics lc)
    {
        if (_currentActivity == null)
        {
            return false;
        }

        _currentActivity.loopCharacteristics = lc;
        return true;
    }
}
