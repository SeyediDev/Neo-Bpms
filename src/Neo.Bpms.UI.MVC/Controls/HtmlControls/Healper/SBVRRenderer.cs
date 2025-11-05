using Neo.Common.Attributes;
using Markdig;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Healper;

/// <summary>
/// Service for rendering SBVR (Semantics of Business Vocabulary and Business Rules) icons
/// </summary>
public class SBVRRenderer : ISBVRRenderer
{
    /// <summary>
    /// Renders SBVR icon HTML for the given field
    /// </summary>
    /// <param name="field">The input field definition</param>
    /// <returns>HTML string containing SBVR icon</returns>
    public string RenderSBVRIcon(InputFieldDefinition field)
    {
        if (field.SBVRs == null || field.SBVRs.Count == 0 || field.FormFieldType == Domain.Entities.Cmmn.UI.Forms.FormField.Type.FilterField)
            return string.Empty;

        // Find the highest priority modality (Obligatory > Prohibited > Necessary > Permitted)
        var highestPriorityModality = field.SBVRs
            .Select(s => s.Modality)
            .OrderBy(m => (int)m)
            .First();

        // Get modality color class for the single icon
        string modalityClass = highestPriorityModality switch
        {
            SBVRModality.Obligatory => "sbvr-obligatory",
            SBVRModality.Prohibited => "sbvr-prohibited",
            SBVRModality.Necessary => "sbvr-necessary",
            SBVRModality.Permitted => "sbvr-permitted",
            SBVRModality.Recommended => "sbvr-recommended",
            SBVRModality.Calculated => "sbvr-calculated",
            SBVRModality.Predicted => "sbvr-predicted",
            _ => "sbvr-default"
        };

        // Build tooltip content with all rules as a beautiful list
        var tooltipBuilder = new StringBuilder();
        tooltipBuilder.Append("<div class=\"sbvr-tooltip-content\">");
        
        // Group rules by modality for better organization
        var groupedRules = field.SBVRs
            .GroupBy(s => s.Modality)
            .OrderBy(g => (int)g.Key);

        foreach (var group in groupedRules)
        {
            tooltipBuilder.Append($"<div class=\"sbvr-group\" data-modality=\"{group.Key}\">");
            
            foreach (var sbvr in group)
            {
                // Get CSS class for this modality
                string modalityIconClass = sbvr.Modality switch
                {
                    SBVRModality.Obligatory => "sbvr-icon-obligatory",
                    SBVRModality.Prohibited => "sbvr-icon-prohibited",
                    SBVRModality.Necessary => "sbvr-icon-necessary",
                    SBVRModality.Permitted => "sbvr-icon-permitted",
                    SBVRModality.Recommended => "sbvr-icon-recommended",
                    SBVRModality.Calculated => "sbvr-icon-calculated",
                    SBVRModality.Predicted => "sbvr-icon-predicted",
                    _ => "sbvr-icon-default"
                };
                
                tooltipBuilder.Append("<div class=\"sbvr-rule-item\">");
                tooltipBuilder.Append($"<div class=\"sbvr-rule-header\">");
                tooltipBuilder.Append($"<i class=\"fa fa-shield sbvr-item-icon {modalityIconClass}\" data-modality=\"{sbvr.Modality}\"></i>");
                tooltipBuilder.Append($"<span class=\"sbvr-modality-badge\">{sbvr.Modality.ToName()}</span>");
                tooltipBuilder.Append($"<span class=\"sbvr-subject\">{System.Net.WebUtility.HtmlEncode(sbvr.Subject)}</span>");
                tooltipBuilder.Append("</div>");
                
                // Convert Markdown to HTML with advanced pipeline (tables, code blocks, etc.)
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions() // Enables tables, task lists, etc.
                    .Build();
                
                string verbPhrase = Markdown.ToHtml(sbvr.VerbPhrase ?? string.Empty, pipeline);
                tooltipBuilder.Append($"<div class=\"sbvr-verb\">{verbPhrase}</div>");
                
                if (!string.IsNullOrEmpty(sbvr.Condition))
                {
                    string condition = Markdown.ToHtml(sbvr.Condition, pipeline);
                    tooltipBuilder.Append($"<div class=\"sbvr-condition\"><strong>شرط:</strong> {condition}</div>");
                }
                tooltipBuilder.Append("</div>");
            }
            
            tooltipBuilder.Append("</div>");
        }
        
        tooltipBuilder.Append("</div>");

        // Render single icon with all rules in tooltip
        // Note: We use Replace to escape quotes, not SecurityElement.Escape, 
        // because we need to preserve HTML structure for data-html="true"
        string tooltipHtml = tooltipBuilder.ToString().Replace("\"", "&quot;");
        
        var sb = new StringBuilder();
        
        // Generate unique ID for this field's modal
        string modalId = $"sbvr-modal-{field.FieldName?.Replace(".", "-") ?? Guid.NewGuid().ToString("N").Substring(0, 8)}";
        
        // Always use modal (avoid inline tooltip HTML issues in read-only pages)
        bool useModal = true;
        
        // Span for icon and badge only
        sb.Append("<span class=\"sbvr-icon-container\" style=\"display:inline-block;margin-left:6px;margin-right:6px;vertical-align:middle;\">");
        
        if (useModal)
        {
            // Clickable icon that opens modal
            sb.Append($@"<i class=""fa fa-shield sbvr-icon {modalityClass}"" 
                           style=""cursor:pointer;font-size:14px;margin-right:4px;"" 
                           data-toggle=""modal"" 
                           data-target=""#{modalId}""
                           data-modality=""{highestPriorityModality}""
                           data-rules-count=""{field.SBVRs.Count}""
                           title=""کلیک کنید برای مشاهده جزئیات""></i>");
        }
        else
        {
            // Small content: use regular tooltip
            sb.Append($@"<i class=""fa fa-shield sbvr-icon {modalityClass}"" 
                           style=""cursor:help;font-size:14px;margin-right:4px;"" 
                           data-toggle=""tooltip"" 
                           data-html=""true""
                           data-placement=""auto""
                           data-container=""body""
                           data-modality=""{highestPriorityModality}""
                           data-rules-count=""{field.SBVRs.Count}""
                           title=""{tooltipHtml}""></i>");
        }
        
        // Add badge with count if more than 1 rule
        if (field.SBVRs.Count > 1)
        {
            sb.Append($"<span class=\"sbvr-count-badge\">{field.SBVRs.Count}</span>");
        }
        
        sb.Append("</span>");
        
        // Modal HTML - rendered OUTSIDE the icon container, will be moved to body by Bootstrap
        if (useModal)
        {
            sb.Append($@"
<div class=""modal fade sbvr-modal"" id=""{modalId}"" tabindex=""-1"" role=""dialog"" aria-labelledby=""{modalId}-title"" aria-hidden=""true"">
    <div class=""modal-dialog modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header sbvr-modal-header"">
                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""بستن"">
                    <span aria-hidden=""true"">&times;</span>
                </button>
                <h4 class=""modal-title"" id=""{modalId}-title"">
                    <i class=""fa fa-shield {modalityClass}"" style=""margin-left:8px;""></i>
                    {System.Net.WebUtility.HtmlEncode(field.Label ?? field.FieldName ?? "فیلد")}
                </h4>
            </div>
            <div class=""modal-body sbvr-modal-body"">
                <div class=""sbvr-tooltip-content"">
                    {tooltipBuilder}
                </div>
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">بستن</button>
            </div>
        </div>
    </div>
</div>");
        }
        return sb.ToString();
    }

    /// <summary>
    /// Renders SBVR icon HTML for the given entity (single icon with count and modal)
    /// </summary>
    /// <param name="entityId">The entity ID to render SBVR for</param>
    /// <param name="entityName">The display name of the entity</param>
    /// <returns>HTML string containing SBVR icon</returns>
    public string RenderEntitySBVR(string entityId, string entityName)
    {
        if (string.IsNullOrEmpty(entityId)) return string.Empty;

        // Find entity type by name
        var entityType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .FirstOrDefault(p => p.Name == entityId);

        if (entityType == null) return string.Empty;

        // Get SBVR attributes from the class
        var sbvrAttributes = entityType.GetCustomAttributes(typeof(SBVRAttribute), false)
            .Cast<SBVRAttribute>()
            .ToList();
        
        if (sbvrAttributes.Count == 0) return string.Empty;

        // Find the highest priority modality (Obligatory > Prohibited > Necessary > Permitted)
        var highestPriorityModality = sbvrAttributes
            .Select(s => s.Modality)
            .OrderBy(m => (int)m)
            .First();

        // Get modality color class for the single icon
        string modalityClass = highestPriorityModality switch
        {
            SBVRModality.Obligatory => "sbvr-obligatory",
            SBVRModality.Prohibited => "sbvr-prohibited",
            SBVRModality.Necessary => "sbvr-necessary",
            SBVRModality.Permitted => "sbvr-permitted",
            SBVRModality.Recommended => "sbvr-recommended",
            SBVRModality.Calculated => "sbvr-calculated",
            SBVRModality.Predicted => "sbvr-predicted",
            _ => "sbvr-default"
        };

        // Build tooltip content with all rules as a beautiful list
        var tooltipBuilder = new StringBuilder();
        tooltipBuilder.Append("<div class=\"sbvr-tooltip-content\">");
        
        // Group rules by modality for better organization
        var groupedRules = sbvrAttributes
            .GroupBy(s => s.Modality)
            .OrderBy(g => (int)g.Key);

        foreach (var group in groupedRules)
        {
            tooltipBuilder.Append($"<div class=\"sbvr-group\" data-modality=\"{group.Key}\">");
            
            foreach (var sbvr in group)
            {
                // Get CSS class for this modality
                string modalityIconClass = sbvr.Modality switch
                {
                    SBVRModality.Obligatory => "sbvr-icon-obligatory",
                    SBVRModality.Prohibited => "sbvr-icon-prohibited",
                    SBVRModality.Necessary => "sbvr-icon-necessary",
                    SBVRModality.Permitted => "sbvr-icon-permitted",
                    SBVRModality.Recommended => "sbvr-icon-recommended",
                    SBVRModality.Calculated => "sbvr-icon-calculated",
                    SBVRModality.Predicted => "sbvr-icon-predicted",
                    _ => "sbvr-icon-default"
                };
                
                tooltipBuilder.Append("<div class=\"sbvr-rule-item\">");
                tooltipBuilder.Append($"<div class=\"sbvr-rule-header\">");
                tooltipBuilder.Append($"<i class=\"fa fa-shield sbvr-item-icon {modalityIconClass}\" data-modality=\"{sbvr.Modality}\"></i>");
                tooltipBuilder.Append($"<span class=\"sbvr-modality-badge\">{sbvr.Modality.ToName()}</span>");
                tooltipBuilder.Append($"<span class=\"sbvr-subject\">{System.Net.WebUtility.HtmlEncode(sbvr.Subject)}</span>");
                tooltipBuilder.Append("</div>");
                
                // Convert Markdown to HTML with advanced pipeline (tables, code blocks, etc.)
                var pipeline = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions() // Enables tables, task lists, etc.
                    .Build();
                
                string verbPhrase = Markdown.ToHtml(sbvr.VerbPhrase ?? string.Empty, pipeline);
                tooltipBuilder.Append($"<div class=\"sbvr-verb\">{verbPhrase}</div>");
                
                if (!string.IsNullOrEmpty(sbvr.Condition))
                {
                    string condition = Markdown.ToHtml(sbvr.Condition, pipeline);
                    tooltipBuilder.Append($"<div class=\"sbvr-condition\"><strong>شرط:</strong> {condition}</div>");
                }
                
                tooltipBuilder.Append("</div>");
            }
            
            tooltipBuilder.Append("</div>");
        }
        
        tooltipBuilder.Append("</div>");

        // Generate unique IDs for this entity
        string modalId = $"sbvr-modal-{entityId.ToLower()}";

        // Create single SBVR icon with count and modal
        var sb = new StringBuilder();
        
        // Single icon with count badge and modal trigger
        sb.Append($@"
<span class=""sbvr-icon-container"" data-toggle=""modal"" data-target=""#{modalId}"" style=""cursor: pointer; margin-right: 5px; position: relative;"">
    <i class=""fa fa-shield sbvr-icon {modalityClass}"" data-modality=""{highestPriorityModality}"" title=""مشاهده قوانین کسب‌وکار""></i>
    <span class=""sbvr-count-badge"">{sbvrAttributes.Count}</span>
</span>");

        // Modal HTML
        sb.Append($@"
<div class=""modal fade sbvr-modal"" id=""{modalId}"" tabindex=""-1"" role=""dialog"" aria-labelledby=""{modalId}-title"" aria-hidden=""true"">
    <div class=""modal-dialog modal-lg"" role=""document"">
        <div class=""modal-content"">
            <div class=""modal-header sbvr-modal-header"">
                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""بستن"">
                    <span aria-hidden=""true"">&times;</span>
                </button>
                <h4 class=""modal-title"" id=""{modalId}-title"">
                    <i class=""fa fa-shield {modalityClass}"" style=""margin-left:8px;""></i>
                    {System.Net.WebUtility.HtmlEncode(entityName)}
                </h4>
            </div>
            <div class=""modal-body sbvr-modal-body"">
                <div class=""sbvr-tooltip-content"">
                    {tooltipBuilder}
                </div>
            </div>
            <div class=""modal-footer"">
                <button type=""button"" class=""btn btn-default"" data-dismiss=""modal"">بستن</button>
            </div>
        </div>
    </div>
</div>");

        return sb.ToString();
    }
}
