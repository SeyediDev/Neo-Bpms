namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Healper;

/// <summary>
/// Interface for rendering SBVR (Semantics of Business Vocabulary and Business Rules) icons
/// </summary>
public interface ISBVRRenderer
{
    /// <summary>
    /// Renders SBVR icon HTML for the given field
    /// </summary>
    /// <param name="field">The input field definition</param>
    /// <returns>HTML string containing SBVR icon</returns>
    string RenderSBVRIcon(InputFieldDefinition field);

    /// <summary>
    /// Renders SBVR icon HTML for the given entity (single icon with count and modal)
    /// </summary>
    /// <param name="entityId">The entity ID to render SBVR for</param>
    /// <param name="entityName">The display name of the entity</param>
    /// <returns>HTML string containing SBVR icon</returns>
    string RenderEntitySBVR(string entityId, string entityName);
}
