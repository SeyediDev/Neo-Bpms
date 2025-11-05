namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class Card(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
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
                        class=""{ControlsRendererData.ControlsClassString} {CommonProperties.NarrowColumnClasses} {CommonProperties.ShowHideRelatedClass}"">";
        RenderDesignIcons(result);
        result += $@"<div class=""card {CommonProperties.BgAndTextStyleClass}"">";
        if (Field.HasProperty(eControlPropertyId.LabelName))
        {
            result += @"<div class=""card-header"">";

            RenderIcon(result, "float-end");

            result += Field.Alias;
            result += @"</div>";
        }

        // if(HasAnyBodyControls())
        // {
        //     result += @"<div class=""card-body"">";
        result += controlsRenderer.CreateControls(ControlsRendererData, Field.FieldName);
        //     result += @"</div>";
        // }
        if (linkItems.Count != 0)
        {
            result += @"<div class=""p-1"">";

            foreach (LinkDefinition link in linkItems)
            {
                result = RenderCardAction(result, link);
            }
            result += @"</div>";
        }

        result += "</div></div>";
        return result;
    }

    private static NeoStringBuilder RenderCardAction(NeoStringBuilder result, LinkDefinition link)
    {
        CommonProperties properties = new(link.ControlDefinition.GetProperties());

        result += $@"<a id=""{link.Id}"" class=""btn {properties.ButtonStyleClass(ContextualStyle.Dark)} btn-sm"" href=""{link.GetUrl()}"">{link.Title}</a>";
        return result;
    }

    private bool HasAnyBodyControls()
    {
        return ControlsRendererData.Structure.Fields.Any(
            f => f.parentControlId == Field.FieldName &&
                 f.ControlType != eControlTypeId.Link);
    }
}
