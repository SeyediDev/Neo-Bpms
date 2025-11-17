using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class LinkList(IFormLogicHelper formLogicHelper, InputFieldDefinition field,
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer)
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        NeoStringBuilder result = new();
        List<LinkDefinition> linkItems = [.. GetChildrenOfType(eControlTypeId.LinkListItem)
            .Select(ifd => new LinkDefinition(ifd, ControlsRendererData.Url))
            .Where(l => l.IsAccessibleFor(ControlsRendererData.User))];
        if (linkItems.Count == 0)
            return result;

        result += $@"<div data-id=""{Field.FieldName}"" id=""{Field.FieldName}""";
        result += $@" class=""{ControlsRendererData.ControlsClassString} {CommonProperties.NarrowColumnClasses} {CommonProperties.ShowHideRelatedClass}"">";
        RenderDesignIcons(result);
        result += $@"<div class=""card {CommonProperties.BgAndTextStyleClass}"">
                        <div class=""card-header"" ";
        result = AddPropertiesEffect(result, CommonProperties);
        result += ">";
        RenderIcon(result, "float-end");

        result += Field.Alias;
        result += @"</div>
                        <div class=""list-group list-group-flush"">";
        foreach (LinkDefinition linkItem in linkItems)
        {
            CommonProperties properties = new(linkItem.ControlDefinition.GetProperties());

            result +=
                $@"<a data-id=""{linkItem.Id}"" id=""{linkItem.Id}"" 
                          class=""list-group-item list-group-item-action {properties.ListGroupItemStyleClass}"" 
                          href=""{linkItem.GetUrl()}""";
            result = AddPropertiesEffect(result, properties);
            result += $@">{linkItem.Title}</a>";
        }

        result += "</div></div></div>";
        return result;
    }

    private static NeoStringBuilder AddPropertiesEffect(NeoStringBuilder result, CommonProperties properties)
    {
        if (!string.IsNullOrEmpty(properties.Color))
            result += $" style=\"color:{properties.Color}\"";
        if (!string.IsNullOrEmpty(properties.BackgroundColor))
            result += $" style=\"background-color:{properties.BackgroundColor}\"";
        return result;
    }
}
