namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Components;

public class UIComponentProperty
{
    public UIComponentProperty()
    {

    }

    public UIComponentProperty(eControlPropertyId id, object value)
    {
        PropertyId = id;
        Value = value;
    }
    public eControlPropertyId PropertyId { get; set; }
    public eControlPropertyId Id => PropertyId;
    public object Value { get; set; }
    public string PropertyQueryName => $"_prop_{PropertyId.ToString()}";

    public bool GetValueAsBoolean()
    {
        return !string.IsNullOrEmpty(Value?.ToString()) &&
                 (Value.ToString() == "on" ||
                  Value.ToString().ToLower() == "true" ||
                  ToBoolean());
    }

    private bool ToBoolean()
    {
        try
        {
            return string.IsNullOrEmpty(Value?.ToString())
                ? false
                : Value.ToString() == "1" || Value.ToString().ToLower() == "true" ? true : Convert.ToBoolean(Value);
        }
        catch (Exception)
        {
            // Logger.LogCritical(e,e.Message);
            return false;
        }
    }
}
