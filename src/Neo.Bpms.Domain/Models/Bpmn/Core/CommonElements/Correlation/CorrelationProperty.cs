namespace Neo.Bpms.Domain.Models.Bpmn.Core.CommonElements.Correlation;

/// <summary>
/// Key-based Correlation:
/// Key-based correlation is a simple and efficient form of correlation, where one or more keys are used to identify a Conversation.
/// Any incoming Message can be matched against the CorrelationKey by extracting the CorrelationProperties from the Message according to the corresponding CorrelationPropertyRetrievalExpression and comparing the resulting composite key with the CorrelationKey instance for this Conversation.
/// At runtime the first Send Task or Receive Task in a Conversation MUST populate at least one of the CorrelationKey instances by extracting the values of the CorrelationProperties according to the CorrelationPropertyRetrievalExpression from the initially sent or received Message. 
/// Later in the Conversation, the populated CorrelationKey instances are used for the described matching procedure where from incoming Messages a composite key is extracted and used to identify the associated Conversation. 
/// Where these noninitiating Messages derive values for CorrelationKeys, associated with the Conversation but not yet populated, then the derived value will be associated with the Conversation instance.
/// </summary>
public class CorrelationProperty(BpmnDefinitions parent, string id, string name, string type) : RootElement(parent, id, name)
{
    //		public string name;
    public string type = type;

    /// <summary>
    /// representing the associations of FormalExpressions (extraction paths) to specific Messages occurring in this Conversation.
    /// </summary>
    public List<CorrelationPropertyRetrievalExpression> retrievalExpression;

    public override void Copy(RootElement newRootElement)
    {
        if (newRootElement is not CorrelationProperty correlationProperty) return;
        Name = correlationProperty.Name;
        type = correlationProperty.type;
        retrievalExpression = correlationProperty.retrievalExpression;
        CloneBase(newRootElement);
    }
}
