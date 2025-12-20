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

        result += $@"<div data-id=""{Field.FieldName}"" id=""{Field.FieldName}""
                        class=""col-12 col-md-12 col-sm-12 col-lg-12 {ControlsRendererData.ControlsClassString} {CommonProperties.ShowHideRelatedClass}"">";
        RenderDesignIcons(result);
        
        // Container with border and full width - items will be in a row next to each other
        result += @"<div class=""border border-secondary rounded p-3 mb-3"">";
        
        // Optional label/header if LabelName property exists
        if (Field.HasProperty(eControlPropertyId.LabelName))
        {
            result += $@"<div class=""mb-2 pb-2 border-bottom"">";
            RenderIcon(result, "float-end");
            result += $"<strong>{Field.Alias}</strong>";
            result += @"</div>";
        }

        // Render child controls in a row - they will flow horizontally next to each other
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

