namespace Neo.Bpms.Domain.Models.Cmmn.UI.Forms;

/// <summary>
/// Properties of the form fields
/// </summary>
public class FormProperty : UIComponentProperty
{
    public object value => Value;
    public eControlPropertyId id => Id;

    public FormProperty()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormProperty"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="value">The value.</param>
    public FormProperty(eControlPropertyId id, object value)
    {
        PropertyId = id;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FormProperty"/> class.
    /// </summary>
    /// <param name="p">The p.</param>
    public FormProperty(UIComponentProperty p)
    {
        PropertyId = p.PropertyId;
        Value = p.Value;
    }
}
