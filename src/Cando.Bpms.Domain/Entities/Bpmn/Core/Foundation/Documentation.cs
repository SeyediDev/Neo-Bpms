namespace Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

/// <summary>
/// All BPMN elements that inherit from the BaseElement will have the capability, through the Documentation element, to have one (1) or more text descriptions of that element
/// </summary>
public class Documentation(string id, string text, string textFormat = "text/plain")
{
    public string text = text;

    /// <summary>
    /// mime-type format
    /// </summary>
    public string textFormat = textFormat;

    public string id = id;
}
