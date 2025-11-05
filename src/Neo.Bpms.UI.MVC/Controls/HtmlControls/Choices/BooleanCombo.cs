using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices.Boolean;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public class BooleanCombo(IFormLogicHelper formLogicHelper, InputFieldDefinition field, 
    ControlsRendererData controlsRendererData, ISBVRRenderer sbvrRenderer) 
    : Combo(formLogicHelper, field, controlsRendererData, sbvrRenderer)
{
    public override NeoStringBuilder Render()
    {
        ElasticObject record = ControlsRendererData.Record;
        string value = record?.GetString(Field.FieldName) ??
                        $"{(ControlsRendererData.Options.IsFilter ? BooleanItem.All : BooleanItem.False):D}";
        Domain.Entities.Cmmn.Entities.Entity entity = ProjectDefinition.Project.GetEntity(ControlsRendererData.Structure.NamespaceId, ControlsRendererData.Structure.EntityId);
        string[] fieldIds = Field.FieldName.Split('.');
        EntityField field = entity?.GetField(fieldIds[0]);
        if (fieldIds.Length > 1)
        {
            for (int ii = 1; ii < fieldIds.Length; ii++)
                field = field?.AssociationEntity?.Entity()?.GetField(fieldIds[ii]);
        }
        NeoStringBuilder result = new();
        RenderHeader(result, false, false, [value], false, ((int)BooleanItem.All).ToString());
        RenderOptions(result, value);
        RenderFooter(result);
        return result;
    }


    private NeoStringBuilder RenderOptions(NeoStringBuilder result, object value)
    {
        if (value == null && ControlsRendererData.Options.IsFilter)
            value = (int)BooleanItem.All;
        BooleanItem valueItem = BooleanEntityField.GetValueItem((ControlsRendererData.Record?.Attributes?.Count() ?? 0) == 0, value);
        BooleanTitles titles = new(Field);
        if (ControlsRendererData.Options.IsFilter)
        {
            result = AddOption(result, valueItem, titles.All, BooleanItem.All);
        }

        result = AddOption(result, valueItem, titles.True, BooleanItem.True);

        result = AddOption(result, valueItem, titles.False, BooleanItem.False);

        if (ControlsRendererData.Options.IsFilter)
        {
            //todo optionName = ProjectDefinition.Project.DefaultCalendar == "shamsi" ? "تهی" : "Null";
            string nullTitle = titles.Null;
            if (!string.IsNullOrEmpty(nullTitle) && nullTitle != (titles.False ?? "")) //todo
                result = AddOption(result, valueItem, nullTitle, BooleanItem.Null);
        }
        return result;
    }

    private static NeoStringBuilder AddOption(NeoStringBuilder result, BooleanItem valueItem, string optionName, BooleanItem optionValue)
    {
        result += "<option";
        if (valueItem == optionValue)
            result += " selected=\"selected\"";
        result += " value=\"" + (int)optionValue + "\"";
        result += ">" + optionName + "</option>";
        return result;
    }
}
