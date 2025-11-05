namespace Neo.Bpms.Domain.Models.Attributes.EntityAttributes;

//todo: check if these feature are supported at all or not, are supported on design tools and then document it in advanced meta model design notes.
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EAttr_DataEvent(string id, string condition) : Attribute
{
    public string Condition { get; set; } = condition;
    public string Id { get; set; } = id;
    public string Name { get; set; }
}
