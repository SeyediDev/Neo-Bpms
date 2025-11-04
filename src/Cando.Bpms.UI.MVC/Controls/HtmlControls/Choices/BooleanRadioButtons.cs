using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices.Boolean;

namespace Neo.Bpms.UI.MVC.Controls.HtmlControls.Choices;

public class BooleanRadioButtons(IFormLogicHelper formLogicHelper, 
    InputFieldDefinition field, ControlsRendererData controlsRenderer, ISBVRRenderer sbvrRenderer) 
    : RadioButtons(formLogicHelper, field, controlsRenderer, sbvrRenderer)
{
    protected override CandoStringBuilder RenderOptions(CandoStringBuilder result,
        List<string> idsList)
    {
        object value = idsList?.FirstOrDefault();

        if (value == null && ControlsRendererData.Options.IsFilter)
            value = (int)BooleanItem.All;
        BooleanItem valueItem = BooleanEntityField.GetValueItem((ControlsRendererData.Record?.Attributes?.Count() ?? 0) == 0, value);
        BooleanTitles titles = new(Field);
        result += RenderOption(idsList, titles.True, BooleanItem.True.ToString("D"), valueItem.ToString("D"));
        if (ControlsRendererData.Options.IsFilter)
        {
            result += RenderOption(idsList, titles.All, BooleanItem.All.ToString("D"), valueItem.ToString("D"), true);
        }
        result += RenderOption(idsList, titles.False, BooleanItem.False.ToString("D"), valueItem.ToString("D"));

        if (ControlsRendererData.Options.IsFilter)//todo can set null value in forms? 
        {
            //todo optionName = ProjectDefinition.Project.DefaultCalendar == "shamsi" ? "تهی" : "Null";
            string nullTitle = titles.Null;
            if (!string.IsNullOrEmpty(nullTitle) && nullTitle != (titles.False ?? "")) //todo
                result += RenderOption(idsList, nullTitle, BooleanItem.Null.ToString("D"), valueItem.ToString("D"));
        }
        return result;
    }

    protected override List<string> GatherIdsList(ElasticObject record)
    {
        string selected = record?.GetString(Field.FieldName);

        return selected == null ? null : [selected];
    }
}
