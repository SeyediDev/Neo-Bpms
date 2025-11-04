namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

//todo document in advanced meta design notes
//todo review check 
/// <summary>
/// Trigger Parameter Attribute
/// 
/// Each trigger attribute can have multiple following Trigger Parameter Attributes to define parameter passing behavior
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EAttr_TriggerParameter : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EAttr_TriggerParameter"/> class.
    /// </summary>
    /// <param name="trigFieldId">The trigerring entity field identifier.</param>
    /// <param name="sourceFieldId">The source field identifier.</param>
    public EAttr_TriggerParameter(string trigFieldId, string sourceFieldId)
    {
        Formula = null;
        SourceFieldId = sourceFieldId;
        TrigFieldId = trigFieldId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EAttr_TriggerParameter"/> class.
    /// </summary>
    /// <param name="trigFieldId">The trigerring entity field identifier.</param>
    public EAttr_TriggerParameter(string trigFieldId)
    {
        TrigFieldId = trigFieldId;
    }

    public string Formula { get; set; }
    public string TrigFieldId { get; set; }
    public string SourceFieldId { get; set; }
}
