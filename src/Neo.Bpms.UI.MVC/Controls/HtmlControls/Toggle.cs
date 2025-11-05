using Neo.Bpms.Domain.Extensions;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;
using Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices.Boolean;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls;

public class Toggle(IFormLogicHelper formLogicHelper, InputFieldDefinition field,
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer)
    : BaseNeoHtmlControl(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    private readonly BooleanTitles _titles = new(field);
    public override NeoStringBuilder Render()
    {
        NeoStringBuilder result = new();

        bool bValue = ControlsRendererData.Record.GetBool(Field.FieldName);
        result += $@"
            <div data-id=""{Field.FieldName}"" id=""{Field.FieldName}""
                 class=""{ControlsRendererData.ControlsClassString}{CommonProperties.NarrowColumnClasses}{CommonProperties.ShowHideRelatedClass}"">";
        RenderDesignIcons(result);
        result += $@"
                <div>{RenderBulkEditCheckbox()}
                    <label for=""field-{Field.FieldName}"" style=""margin-bottom:8px;"" class=""input-label"">
                        {Field.Label} {(CommonProperties.IsRequired ? "<span style=\"color:red;\">*</span>" : "")}
                    </label>
                </div>
                <input id=""field-{Field.FieldName}"" associated-hidden-name=""{Field.FieldName}"" type=""checkbox"" 
                       onchange=""checkboxChanged(this)"" {CommonProperties.ReadOnlyRelatedAttribute} {(bValue ? "checked" : "")} 
                       data-toggle=""toggle"" data-size=""small"" data-onstyle=""{OnStyle}"" data-offstyle=""{OffStyle}""
                       data-on=""{OnTitle}"" data-off=""{OffTitle}"" {WidthAttribute} data-style=""min-w-30"" />
                <input type=""hidden"" name=""{Field.FieldName}"" {LogicString}
                       {(bValue ? " checked value =\"true\"" : " value=\"false\"")}
                       {CommonProperties.ReadOnlyRelatedAttribute} />
            </div>";
        return result;
    }

    public string WidthAttribute => Math.Max(OnTitle.Length, OffTitle.Length) > 10 ? "data-width=\"100%\"" : "";

    public string OnTitle => _titles.TrueProperty ?? ViewTexts.TrueTitle;
    public string OffTitle => _titles.False;

    private string OnStyle => Field.PropertyValue(eControlPropertyId.TrueStyle)?.ToLower() ?? "primary";
    private string OffStyle => Field.PropertyValue(eControlPropertyId.FalseStyle)?.ToLower() ?? "light";
}
