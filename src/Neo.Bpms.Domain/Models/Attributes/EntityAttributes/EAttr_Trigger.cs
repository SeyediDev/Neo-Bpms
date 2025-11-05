namespace Neo.Bpms.Domain.Models.Attributes.EntityAttributes;

/// <summary>
/// Trigger Attribute
/// 
/// Trigger is one of the advanced features of the entity.
/// Entity can have multiple trigger definitions.
/// After Trigger Attribute, Trigger Parameter Attributes will define the details of parameter passing.
/// Entity can trigger another entity data operations when it is changing and the condition met.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EAttr_Trigger"/> class.
/// </remarks>
/// <param name="triggeredEntityId">The triggered entity identifier.</param>
/// <param name="triggeredOperationId">The triggered operation identifier.</param>
/// <param name="triggeredEntityNamespace">The triggered entity namespace.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EAttr_Trigger(string triggeredEntityId, string triggeredOperationId,
    string triggeredEntityNamespace = null) : Attribute
{
    public string TriggeredEntityId { get; set; } = triggeredEntityId;
    public string TriggeredOperationId { get; set; } = triggeredOperationId;
    public string TriggeredEntityNamespace { get; set; } = triggeredEntityNamespace;

    public string Condition { get; set; }
    ///// <summary>
    ///// Gets or sets the query definition.
    ///// </summary>
    ///// <value>
    ///// The query definition. it is used if the trigger must be repeated for each query row
    ///// </value>
    //public QueryDefinition Query { get; set; }
    ////string condition = null, QueryDefinition query = null
}
