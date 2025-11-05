namespace Neo.Bpms.Domain.Model.BPMN.Core.Foundation;

public partial class Element
{
    public Type type { get { return (Type)value; } set { this.value = value; } }
}
