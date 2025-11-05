namespace Neo.Bpms.Domain.Models.Cmmn.UI.Components;

public class SuitableControlsAttribute(params eControlTypeId[] controls) : Attribute
{
    public readonly eControlTypeId[] Controls = controls;
}
