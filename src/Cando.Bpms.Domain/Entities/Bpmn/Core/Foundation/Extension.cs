namespace Neo.Bpms.Domain.Entities.Bpmn.Core.Foundation;

/// <summary>
/// The Extension element binds/imports an ExtensionDefinition and its attributes to a BPMN model definition.
/// </summary>
public class Extension
{
    /// <summary>
    /// if the semantics defined by the extension definition and its attribute definition MUST be understood by the BPMN adopter in order to process the BPMN model correctly.
    /// </summary>
    public bool mustUnderstand;
    public ExtensionDefinition definition;
    public Extension(ExtensionDefinition definition)
    {
        this.definition = definition;
        mustUnderstand = false;
    }
    public Extension(ExtensionDefinition definition, bool mustUnderstand)
    {
        this.definition = definition;
        this.mustUnderstand = mustUnderstand;
    }
}
