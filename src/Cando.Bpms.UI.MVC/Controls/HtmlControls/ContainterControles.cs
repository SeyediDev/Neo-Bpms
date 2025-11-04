namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class ContainersControls(
    IFormLogicHelper formLogicHelper, InputFieldDefinition field, ControlsRendererData controlsRendererData,
    IControlsRenderer controlsRenderer, ISBVRRenderer sbvrRenderer)
    : BaseCandoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override CandoStringBuilder Render()
    {
        throw new NotImplementedException();
    }

    public CandoStringBuilder RenderAccordion()
    {
        CandoStringBuilder stringBuilder = new();
        stringBuilder.Append("<div class=\"w-100\"></div>"); // Force new line
        stringBuilder.Append($"<div data-id=\"{Field.FieldName}\" class=\"{CommonProperties.WideColumnClasses} {ControlsClassString}\">");
        RenderDesignIcons(stringBuilder);
        stringBuilder.Append($"<div class=\"modern-accordion\" id=\"{Field.FieldName}-collapse\" role=\"tablist\"" +
                              " aria-multiselectable=\"true\">");
        bool firstAccordionIteration = true;
        foreach (InputFieldDefinition tab in ControlsRendererData.Structure.Fields.Where(f => f.parentControlId == Field.FieldName &&
                                                                        f.ControlType == eControlTypeId.AccordionItem))
        {
            stringBuilder.Append("<div class=\"card\">");
            stringBuilder.Append(
                $"<div class=\"card-header\" role=\"tab\" id=\"{Field.FieldName}-collapse-heading-{tab.FieldName}\">");
            stringBuilder.Append("<h5 class=\"m-0\">");
            stringBuilder.Append(
                $"<a role=\"button\" data-toggle=\"collapse\" data-parent=\"#{Field.FieldName}-collapse\"" +
                $" href=\"#{Field.FieldName}-collapse-{tab.FieldName}\" aria-expanded=\"{(firstAccordionIteration ? "true" : "false")}\" " +
                $"aria-controls=\"{Field.FieldName}-collapse-{tab.FieldName}\">");
            string collapseTitle = tab.PropertyValue(eControlPropertyId.LabelName);
            stringBuilder.Append(collapseTitle + "</a></h5></div>");

            stringBuilder.Append(
                $"<div id=\"{Field.FieldName}-collapse-{tab.FieldName}\" class=\"collapse {(firstAccordionIteration ? "show" : "")}\"" +
                $" role=\"tabpanel\" aria-labelledby=\"{Field.FieldName}-collapse-heading-{tab.FieldName}\">");
            stringBuilder.Append("<div class=\"card-body row m-0\">");
            stringBuilder.Append(controlsRenderer.CreateControls(ControlsRendererData, tab.FieldName));
            stringBuilder.Append("</div></div></div>");
            firstAccordionIteration = false;
        }

        stringBuilder.Append("</div>");
        stringBuilder.Append("</div>");
        return stringBuilder;
    }

    public CandoStringBuilder RenderMultiTab()
    {
        CandoStringBuilder stringBuilder = new();
        stringBuilder.Append("<div class=\"w-100\"></div>"); // Force new line
        stringBuilder.Append(
            $"<div data-id=\"{Field.FieldName}\" class=\"{ControlsClassString} {CommonProperties.WideColumnClasses}\" >");
        RenderDesignIcons(stringBuilder);
        stringBuilder.Append("<div class=\"row\"><div class=\"col-lg-12\">");
        IEnumerable<InputFieldDefinition> data = Structure.Fields.Where(f => f.parentControlId == Field.FieldName &&
                                                                        f.ControlType == eControlTypeId.MultiTabItem);
        string isOneTab = data.Count() == 1 ? "d-none" : string.Empty;
        stringBuilder.Append($"<ul class=\"nav nav-tabs  {isOneTab}  \" >");
        bool firstUlIteration = true;
        foreach (InputFieldDefinition tab in data)
        {
            string tabTitle = tab.Alias;
            stringBuilder.Append($"<li id=\"{tab.FieldName}\" class=\"{(firstUlIteration ? "active" : "")} nav-item\"><a class=\"nav-link {(firstUlIteration ? "active" : "")}\" data-toggle=\"tab\" " +
                                  $"href=\"#tabcontent-{tab.FieldName}\">{tabTitle}</a></li>");
            firstUlIteration = false;
        }

        stringBuilder.Append("</ul>");
        isOneTab = isOneTab != string.Empty ? "style='padding-top: 10px;'" : string.Empty;
        stringBuilder.Append($"<div class=\"tab-content\" {isOneTab}>");
        bool firstContentIteration = true;
        foreach (InputFieldDefinition tab in Structure.Fields.Where(f =>
            f.parentControlId == Field.FieldName && f.ControlType == eControlTypeId.MultiTabItem))
        {
            stringBuilder.Append(
                $"<div data-id=\"{tab.FieldName}\" id=\"tabcontent-{tab.FieldName}\" class=\"sortable-container tab-pane {(firstContentIteration ? "active" : "")}\"><div class=\"row m-0\">");
            stringBuilder.Append(controlsRenderer.CreateControls(ControlsRendererData, tab.FieldName));
            stringBuilder.Append("</div></div>");
            firstContentIteration = false;
        }

        stringBuilder.Append("</div>");
        stringBuilder.Append("</div></div>");
        stringBuilder.Append("</div>");
        return stringBuilder;
    }
    public CandoStringBuilder RenderFieldSet()
    {
        CandoStringBuilder stringBuilder = new();
        stringBuilder += "<div class=\"w-100\"></div>"; // Force new line
        stringBuilder +=
            $"<div id=\"{Field.FieldName}\" " +
            $"class=\"{ControlsClassString} {CommonProperties.WideColumnClasses} teta-fieldSet\"";
        stringBuilder = AddPropertiesEfect(stringBuilder, CommonProperties);
        stringBuilder += $">";
        RenderDesignIcons(stringBuilder);

        stringBuilder += $"<fieldset class=\"sortable-container row m-0\" id=\"{Field.FieldName}\" ";
        stringBuilder = AddPropertiesEfect(stringBuilder, CommonProperties);
        stringBuilder += $">";
        string legend = Field.PropertyValue(eControlPropertyId.LabelName);
        if (!string.IsNullOrEmpty(legend))
        {
            stringBuilder += "<legend class=\"w-auto py-1 px-3 m-2\">" + legend + "</legend>";
        }

        stringBuilder += controlsRenderer.CreateControls(ControlsRendererData, Field.FieldName);
        stringBuilder += "</fieldset></div>";
        return stringBuilder;
    }
    private static CandoStringBuilder AddPropertiesEfect(CandoStringBuilder result, CommonProperties properties)
    {
        if (!string.IsNullOrEmpty(properties.Color))
            result += $" style=\"color:{properties.Color}\"";
        if (!string.IsNullOrEmpty(properties.BackgroundColor))
            result += $" style=\"background-color:{properties.BackgroundColor}\"";
        return result;
    }
}
