using Neo.Bpms.Domain.Entities.Cmmn.UI.Resources;

namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Components;

public enum FilterParameter
{
    [SuitableControls(eControlTypeId.TextInput)]
    [Title(nameof(IsEqualTo), typeof(Texts))]
    IsEqualTo,

    [SuitableControls(eControlTypeId.TextInput)]
    [Title(nameof(IsNotEqualTo), typeof(Texts))]
    IsNotEqualTo,

    [SuitableControls(eControlTypeId.TextInput,
        eControlTypeId.ComboBox)]
    [Title(nameof(Contains), typeof(Texts))]
    Contains,

    [SuitableControls(eControlTypeId.ComboBox)]
    [Title(nameof(DoesNotContain), typeof(Texts))]
    DoesNotContain,

    [SuitableControls(eControlTypeId.TextInput)]
    [Title(nameof(StartsWith), typeof(Texts))]
    StartsWith,

    [SuitableControls(eControlTypeId.TextInput)]
    [Title(nameof(EndsWith), typeof(Texts))]
    EndsWith,

    [SuitableControls(eControlTypeId.NumberInput)]
    [Title(nameof(GreaterThan), typeof(Texts))]
    GreaterThan,

    [SuitableControls(eControlTypeId.NumberInput)]
    [Title(nameof(GreaterThanOrEqual), typeof(Texts))]
    GreaterThanOrEqual,

    [SuitableControls(eControlTypeId.NumberInput)]
    [Title(nameof(LessThan), typeof(Texts))]
    LessThan,

    [SuitableControls(eControlTypeId.NumberInput)]
    [Title(nameof(LessThanOrEqual), typeof(Texts))]
    LessThanOrEqual,

    [SuitableControls(eControlTypeId.TextInput, eControlTypeId.ComboBox, eControlTypeId.DatePicker, eControlTypeId.DateTime, eControlTypeId.NumberInput)]
    [Title(nameof(IsNull), typeof(Texts))]
    IsNull = 1000,

    [SuitableControls(eControlTypeId.TextInput, eControlTypeId.ComboBox, eControlTypeId.DatePicker, eControlTypeId.DateTime, eControlTypeId.NumberInput)]
    [Title(nameof(IsNotNull), typeof(Texts))]
    IsNotNull = 1001
}

public static class FilterParameterMethods
{
    public static string FilterParameterName(string formFieldId)
    {
        return $"{formFieldId}__FilterParameter";
    }
    public static bool IsSuitableFor(this FilterParameter filterParameter, eControlTypeId control)
    {
        return filterParameter
            .GetAttribute<SuitableControlsAttribute>()
            .Controls
            .Contains(control);
    }

    public static IEnumerable<FilterParameter> GetSuitableParameters(eControlTypeId controlType)
    {
        return Enum.GetValues(typeof(FilterParameter))
            .Cast<FilterParameter>()
            .Where(fp => fp.IsSuitableFor(controlType));
    }
}
