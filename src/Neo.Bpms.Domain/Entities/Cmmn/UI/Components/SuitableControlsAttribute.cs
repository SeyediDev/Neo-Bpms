namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Components;

public class SuitableControlsAttribute(params eControlTypeId[] controls) : Attribute
{
    public readonly eControlTypeId[] Controls = controls;
}
