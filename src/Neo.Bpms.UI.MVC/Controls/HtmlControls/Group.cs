using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class Group(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRendererData, IControlsRenderer controlsRenderer, ISBVRRenderer sbvrRenderer)
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        NeoStringBuilder result = new();
        List<LinkDefinition> linkItems = [.. GetChildrenOfType(eControlTypeId.Link)
            .Select(ifd => new LinkDefinition(ifd, ControlsRendererData.Url))
            .Where(l => l.IsAccessibleFor(ControlsRendererData.User))];

        // استایل inline برای اطمینان از عرض کامل و نمایش زیر هم - بدون تداخل با Bootstrap
        result += $@"<div data-id=""{Field.FieldName}"" id=""{Field.FieldName}""
                        class=""neo-group-control {ControlsRendererData.ControlsClassString} {CommonProperties.ShowHideRelatedClass}""
                        style=""width: 100%; display: block; flex: 0 0 100%; max-width: 100%; box-sizing: border-box; padding: 0 15px; margin-bottom: 0;"">";
        RenderDesignIcons(result);
        
        // Container with modern styling - beautiful card design
        result += @"<div class=""neo-group-card"">";
        
        // Optional label/header if LabelName property exists
        if (Field.HasProperty(eControlPropertyId.LabelName))
        {
            result += $@"<div class=""neo-group-header"">";
            RenderIcon(result, "float-end");
            result += $"<span class=\"neo-group-title\">{Field.Alias}</span>";
            result += @"</div>";
        }

        // Render child controls in a row - they will flow horizontally next to each other
        // Child controls have their own col-md-* classes, so they'll be side by side in this row
        result += @"<div class=""row m-0"">";
        result += controlsRenderer.CreateControls(ControlsRendererData, Field.FieldName);
        result += @"</div>";
        
        // Render links if any
        if (linkItems.Count != 0)
        {
            result += @"<div class=""p-1"">";
            foreach (LinkDefinition link in linkItems)
            {
                result = RenderGroupAction(result, link);
            }
            result += @"</div>";
        }

        result += "</div></div>";
        return result;
    }

    private static NeoStringBuilder RenderGroupAction(NeoStringBuilder result, LinkDefinition link)
    {
        CommonProperties properties = new(link.ControlDefinition.GetProperties());
        result += $@"<a id=""{link.Id}"" class=""btn {properties.ButtonStyleClass(ContextualStyle.Dark)} btn-sm"" href=""{link.GetUrl()}"">{link.Title}</a>";
        return result;
    }
}

