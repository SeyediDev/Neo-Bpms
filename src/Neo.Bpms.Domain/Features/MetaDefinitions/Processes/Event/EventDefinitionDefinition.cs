using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Events;
using Neo.Bpms.Domain.Models.Bpmn.Core.Services;
using Neo.Bpms.Domain.Models.Bpmn.Processes.Events.EventDefinition;

namespace Neo.Bpms.Domain.Features.Definitions.Entities.Processes;

public abstract partial class ProcessDefinition
{
    /// <summary>
    /// Add the error event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <param name="name">name</param>
    /// <param name="namespaceId">namespace Id</param>
    /// <param name="entityId">entity Id</param>
    /// <returns></returns>
    protected ErrorEventDefinition AddErrorEventDefinition(string code, string name, string namespaceId = null,
        string entityId = null)
    {
        _ = AddError(code, name, namespaceId, entityId);
        return AddErrorEventDefinition(code);
    }

    /// <summary>
    /// Add the error event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <returns></returns>
    protected ErrorEventDefinition AddErrorEventDefinition(string code)
    {
        Error item = ProjectDefinition.Project.GetErrorByCode(code) ?? throw new Exception($"Invalid error {code}.");
        string id = $"{item.Id}.ErrorEventDefinition";
        ErrorEventDefinition eventDefinition = new(definitions, id, item);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Add the signal event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <param name="name">The name.</param>
    /// <param name="namespaceId">namespace Id</param>
    /// <param name="entityId">entity Id</param>
    /// <returns></returns>
    protected SignalEventDefinition AddSignalEventDefinition(string code, string name, string namespaceId,
        string entityId)
    {
        _ = AddSignal(code, name, namespaceId, entityId);
        return AddSignalEventDefinition(code);
    }

    /// <summary>
    /// Add the signal event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <returns></returns>
    protected SignalEventDefinition AddSignalEventDefinition(string code)
    {
        Signal item = ProjectDefinition.Project.GetSignal(code) ?? throw new Exception($"Invalid signal {code}.");
        string id = $"{item.Id}.SignalEventDefinition";
        SignalEventDefinition eventDefinition = new(definitions, id, item);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Add the message event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <param name="name">The name.</param>
    /// <param name="operation">operation</param>
    /// <param name="messageEventDefinitionId"></param>
    /// <returns></returns>
    protected MessageEventDefinition AddMessageEventDefinition<TEntity>(string code, string name,
        Operation operation = null, string messageEventDefinitionId = null)
    {
        _ = AddMessage<TEntity>(code, name);
        return AddMessageEventDefinition(code, operation, messageEventDefinitionId);
    }

    /// <summary>
    /// Add the message event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <param name="operation">operation</param>
    /// <param name="messageEventDefinitionId"></param>
    /// <returns></returns>
    protected MessageEventDefinition AddMessageEventDefinition(string code, Operation operation = null, string messageEventDefinitionId = null)
    {
        Message item = ProjectDefinition.Project.GetMessageByCode(code) ?? throw new Exception($"Invalid message {code}.");
        if (string.IsNullOrEmpty(messageEventDefinitionId))
        {
            messageEventDefinitionId = $"{item.Id}.MessageEventDefinition";
        }

        MessageEventDefinition eventDefinition = new(definitions, messageEventDefinitionId, item, operation);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Add the escalation event definition.
    /// </summary>
    /// <typeparam name="TEntity">entity</typeparam>
    /// <param name="code">code</param>
    /// <param name="name">name</param>
    /// <returns></returns>
    protected EscalationEventDefinition AddEscalationEventDefinition<TEntity>(string code, string name)
    {
        _ = AddEscalation<TEntity>(code, name);
        return AddEscalationEventDefinition(code);
    }

    /// <summary>
    /// Add the escalation event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <param name="name">name</param>
    /// <returns></returns>
    protected EscalationEventDefinition AddEscalationEventDefinition(string code, string name)
    {
        _ = AddEscalation(code, name);
        return AddEscalationEventDefinition(code);
    }

    /// <summary>
    /// Add the escalation event definition.
    /// </summary>
    /// <param name="code">code</param>
    /// <returns></returns>
    protected EscalationEventDefinition AddEscalationEventDefinition(string code)
    {
        Escalation escalation = ProjectDefinition.Project.GetEscalationByCode(code) ?? throw new Exception($"Invalid escalation {code}.");
        string id = $"{escalation.Id}.EscalationEventDefinition";
        EscalationEventDefinition eventDefinition = new(definitions, id);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Adds the terminate throw event definition.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    protected TerminateEventDefinition AddTerminateEventDefinition(string id)
    {
        //todo id
        TerminateEventDefinition eventDefinition = new(definitions, id);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Add the cancel event definition.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    protected CancelEventDefinition AddCancelEventDefinition(string id)
    {
        //todo id
        CancelEventDefinition eventDefinition = new(definitions, id);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Adds the compensation throw event definition.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="waitForCompletion">if set to <c>true</c> [wait for completion].</param>
    /// <param name="activityRef">The activity reference.</param>
    /// <returns></returns>
    protected CompensateEventDefinition AddCompensationEventDefinition(string id, bool waitForCompletion = true,
        string activityRef = null)
    {
        //todo id
        CompensateEventDefinition eventDefinition = new(definitions, id, activityRef, waitForCompletion);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }

    /// <summary>
    /// Add the link event definition.
    /// </summary>
    /// <param name="target">The name.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    protected LinkEventDefinition AddLinkTo(string target, string name)
    {
        //todo ایراد در تعریف تابع وجود دارد
        return currentIntermediateThrowEvent == null
            ? throw new Exception("AddLinkTo not used correctly")
            : AddLinkEventDefinition(currentIntermediateThrowEvent.Id, target, name);
    }

    /// <summary>
    /// Add the link event definition.
    /// </summary>
    /// <param name="source">The name.</param>
    /// <param name="name"></param>
    /// <returns></returns>
    protected LinkEventDefinition AddLinkFrom(string source, string name)
    {
        //todo ایراد در تعریف تابع وجود دارد
        return currentIntermediateCatchEvent == null
            ? throw new Exception("AddLinkFrom not used correctly")
            : AddLinkEventDefinition(source, currentIntermediateCatchEvent.Id, name);
    }

    protected LinkEventDefinition AddLinkEventDefinition(string source, string target, string name)
    {
        string id = $"LinkTo.{target}";
        if (definitions.GetRootElement(id) is not LinkEventDefinition link)
        {
            link = new LinkEventDefinition(definitions, id, name)
            {
                Name = name,
                target = target,
                sources = []
            };
            definitions.AddRootElement(link);
            //todo ایراد فکر کنم نباید به روت ها اضافه شود و فقط موقع استفاده تعریف شود
        }
        link.sources.Add(source);

        currentBaseElement = link;
        return link;
    }

    /// <summary>
    /// Adds the conditional catch event definition.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="condition">The condition.</param>
    /// <returns></returns>
    protected ConditionalEventDefinition AddConditionalEventDefinition(string id, string condition)
    {
        //todo id
        FormalExpression expCondition = new(id + ".Condition", Parser.ParseTree(condition));
        if (expCondition.Expression == null)
        {
            throw new Exception($"Can not parse {condition} in Element {_currentFlowNode.Id}");
        }

        ConditionalEventDefinition eventDefinition = new(definitions, id, expCondition);
        definitions.AddRootElement(eventDefinition);
        currentBaseElement = eventDefinition;
        return eventDefinition;
    }
}
