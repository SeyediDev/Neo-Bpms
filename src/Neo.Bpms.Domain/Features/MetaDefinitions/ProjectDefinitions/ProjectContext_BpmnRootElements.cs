using Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;
using Neo.Bpms.Domain.Models.Bpmn.Core.Services;
using Neo.Bpms.Domain.Models.Bpmn.Processes.DataItems.DataFlow;

namespace Neo.Bpms.Domain.Model.Project;

public partial class ProjectContext
{
    public BpmnDefinitions BpmnDefinitions { get; set; } =
        new BpmnDefinitions("neoTotalBpmn", "neo-sjs", "www.bmi.ir/xmlns/e-bpmn2.xsd", "NeoBpms", "1.0");

    public IEnumerable<Resource> Resources => BpmnRootElements?.OfType<Resource>();
    public IEnumerable<Interface> Interfaces => BpmnRootElements?.OfType<Interface>();
    public IEnumerable<DataStore> DataStores => BpmnRootElements?.OfType<DataStore>();
    public IEnumerable<Message> Messages => BpmnRootElements?.OfType<Message>();
    public IEnumerable<Error> Errors => BpmnRootElements?.OfType<Error>();
    public IEnumerable<Escalation> Escalations => BpmnRootElements?.OfType<Escalation>();
    public IEnumerable<Signal> Signals => BpmnRootElements?.OfType<Signal>();

    public ItemDefinition AddOrGetItemDefinition(bool isCollation, string namespaceId, string entityId)
    {
        return BpmnDefinitions.AddOrGetItemDefinition(isCollation, GetEntity(namespaceId, entityId));
    }

    private IEnumerable<RootElement> BpmnRootElements => BpmnDefinitions.GetRootElements();

    public Resource AddResource(string resourceId, string resourceName, string namespaceId, string entityId)
    {
        var resource = GetResource(resourceId);
        if (resource == null)
        {
            HumanResourceType? type = null;
            switch (namespaceId)
            {
                case "UserAndOrganization":
                    switch (entityId)
                    {
                        case "SystemUser":
                            type = HumanResourceType.User;
                            break;
                        case "SystemUserGroup":
                            type = HumanResourceType.UserGroup;
                            break;
                    }

                    break;
            }

            return AddResource(type.HasValue
                ? new HumanResource(null, resourceId, resourceName, type.Value)
                {
                    NamespaceId = namespaceId,
                    EntityId = entityId
                }
                : new Resource(null, resourceId, resourceName)
                {
                    NamespaceId = namespaceId,
                    EntityId = entityId
                });
        }

        if (string.IsNullOrEmpty(resource.NamespaceId))
            resource.NamespaceId = namespaceId;
        if (string.IsNullOrEmpty(resource.EntityId))
            resource.EntityId = entityId;
        if (string.IsNullOrEmpty(resource.Name))
            resource.Name = resourceName;
        return resource;
    }

    public Resource AddResource(Resource item, bool replace = false)
    {
        return AddRootElement(item, replace) as Resource;
    }

    public Interface AddInterface(Interface item, bool replace = false)
    {
        return AddRootElement(item, replace) as Interface;
    }

    public DataStore AddDataStore(DataStore item, bool replace = false)
    {
        return AddRootElement(item, replace) as DataStore;
    }

    public Message AddMessage(Message item, bool replace = false)
    {
        AddRootElement(item, replace);
        AddCorrelationProperties(item);
        return item;
    }

    public Error AddError(Error item, bool replace = false)
    {
        return AddRootElement(item, replace) as Error;
    }

    public Escalation AddEscalation(Escalation item, bool replace = false)
    {
        return AddRootElement(item, replace) as Escalation;
    }

    public Signal AddSignal(Signal item, bool replace = false)
    {
        return AddRootElement(item, replace) as Signal;
    }

    public bool DeleteResource(string elementId)
    {
        return BpmnDefinitions.RemoveRootElement(elementId);
    }

    public bool DeleteInterface(string elementId)
    {
        return BpmnDefinitions.RemoveRootElement(elementId);
    }

    public void DeleteOperation(string elementId)
    {
        var operation = GetOperation(elementId);
        operation?.Interface.operations.Remove(operation.Name);
    }

    public bool DeleteDataStore(string elementId)
    {
        return BpmnDefinitions.RemoveRootElement(elementId);
    }

    public bool DeleteMessage(string elementId)
    {
        return BpmnDefinitions.RemoveRootElement(elementId);
    }

    public bool DeleteError(string elementId)
    {
        return BpmnDefinitions.RemoveRootElement(elementId);
    }

    public bool DeleteEscalation(string elementId)
    {
        return BpmnDefinitions.RemoveRootElement(elementId);
    }

    public bool DeleteSignal(string elementId)
    {
        return BpmnDefinitions.RemoveRootElement(elementId);
    }

    public Resource GetResource(string resourceId)
    {
        if (string.IsNullOrEmpty(resourceId)) return null;
        return BpmnDefinitions.GetRootElement(resourceId) as Resource;
    }

    public Interface GetInterface(string interfaceId)
    {
        if (string.IsNullOrEmpty(interfaceId)) return null;
        return BpmnDefinitions.GetRootElement(interfaceId) as Interface;
    }

    public Operation GetOperation(string operationId)
    {
        return (Interfaces ?? [])
            .SelectMany(@interface => @interface.operations.Values)
            .FirstOrDefault(operation => operation.Id == operationId);
    }

    public DataStore GetDataStore(string dataStoreId)
    {
        if (string.IsNullOrEmpty(dataStoreId)) return null;
        return BpmnDefinitions.GetRootElement(dataStoreId) as DataStore;
    }

    public DataStore GetDataStoreByName(string code)
    {
        return DataStores.FirstOrDefault(dataStore => dataStore.Name == code);
    }

    public Message GetMessage(string messageId)
    {
        if (string.IsNullOrEmpty(messageId)) return null;
        return BpmnDefinitions.GetRootElement(messageId) as Message;
    }

    public Message GetMessageByCode(string code)
    {
        return GetMessage(GetMessageIdByCode(code));
    }

    public static string GetMessageIdByCode(string code)
    {
        return $"Message.{code}";
    }

    public Error GetError(string errorId)
    {
        if (string.IsNullOrEmpty(errorId)) return null;
        return BpmnDefinitions.GetRootElement(errorId) as Error;
    }

    public Error GetErrorByCode(string errorCode)
    {
        return string.IsNullOrEmpty(errorCode)
            ? null
            : Errors?.FirstOrDefault(item => item.errorCode == errorCode);
    }

    public Escalation GetEscalation(string escalationId)
    {
        if (string.IsNullOrEmpty(escalationId)) return null;
        return BpmnDefinitions.GetRootElement(escalationId) as Escalation;
    }

    public Escalation GetEscalationByCode(string escalationCode)
    {
        return string.IsNullOrEmpty(escalationCode)
            ? null
            : Escalations?.FirstOrDefault(item => item.escalationCode == escalationCode);
    }

    public static string GetEscalationIdByCode(string code)
    {
        return $"Escalation.{code}";
    }

    public static string GetSignalIdByCode(string code)
    {
        return $"Signal.{code}";
    }

    public Signal GetSignal(string signalId)
    {
        if (string.IsNullOrEmpty(signalId)) return null;
        return BpmnDefinitions.GetRootElement(signalId) as Signal;
    }

    private RootElement AddRootElement(RootElement item, bool replace)
    {
        var rootElement = BpmnDefinitions.GetRootElement(item.Id);
        if (rootElement != null)
        {
            if (!replace)
                return rootElement;
            BpmnDefinitions.RemoveRootElement(item.Id);
        }

        BpmnDefinitions.AddRootElement(item);
        return item;
    }

    public void RemoveCorrelationProperties(Message message)
    {
        var correlationProperties = BpmnDefinitions.GetRootElements()
            ?.OfType<CorrelationProperty>()
            .Where(cp =>
                cp.retrievalExpression?.Any(re => Equals(re.messageRef, message)) ?? false)
            .ToList();
        foreach (var correlationProperty in correlationProperties ?? Enumerable.Empty<CorrelationProperty>())
            BpmnDefinitions.RemoveRootElement(correlationProperty.Id);
        var collaboration = BpmnDefinitions.GetRootElements().OfType<Collaboration>().FirstOrDefault();
        if (collaboration?.correlationKeys == null) return;
        var correlationKeys = collaboration.correlationKeys.Where(ck =>
                ck.correlationPropertyRef?.Any(cp =>
                    cp?.retrievalExpression?.Any(re => Equals(re.messageRef, message)) ?? false) ?? false)
            .ToList();
        foreach (var correlationKey in correlationKeys)
            collaboration.correlationKeys.Remove(correlationKey);
    }

    public void AddCorrelationProperties(Message message)
    {
        if (message?.itemRef?.structure?.entityFields == null) return;
        var collaboration = BpmnDefinitions.GetRootElements().OfType<Collaboration>().FirstOrDefault();
        var correlationKey = new CorrelationKey(collaboration, $"{collaboration?.Id}.correlationKey.{message.Id}",
            $"keys of message {message.Name}");
        foreach (var entityField in message.itemRef.structure.entityFields.Values)
        {
            var correlationPropertyId = $"{message.Id}.{entityField.Id}";
            var cp = new CorrelationProperty(BpmnDefinitions, $"CorrelationProperty.{correlationPropertyId}",
                entityField.Id, entityField.GetTypeName())
            {
                retrievalExpression = []
            };
            cp.retrievalExpression.Add(
                new CorrelationPropertyRetrievalExpression(cp, $"CorrelationPropertyRetrieval.{correlationPropertyId}",
                    new FormalExpression($"CorrelationPropertyRetrievalExpression.{correlationPropertyId}",
                        new ExpressionTree
                        { ExpressionString = entityField.Id, Root = new VariableNameExpressionNode(entityField.Id) }),
                    message)
            );
            if (entityField.IncludeInPkv)
            {
                correlationKey.correlationPropertyRef ??= [];
                correlationKey.correlationPropertyRef.Add(cp);
            }

            BpmnDefinitions.AddRootElement(cp);
        }

        if (collaboration == null) return;
        collaboration.correlationKeys ??= [];
        collaboration.correlationKeys.Add(correlationKey);
    }
}
