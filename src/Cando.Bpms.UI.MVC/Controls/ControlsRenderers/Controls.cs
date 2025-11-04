using Neo.Bpms.Domain.Entities.Cmmn.UI.Forms;

namespace Neo.Bpms.UI.MVC.Controls.ControlsRenderers;

public partial class ControlsRenderer
{
    internal static readonly HtmlEncoder Encoder = HtmlEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Arabic);

    public string CreateControls(ControlsRendererData data, string parentControlId, FormField.Type? formFieldType = null)
    {
        Dictionary<string, InputFieldDefinition> fields = [];
        foreach (InputFieldDefinition fieldInfo in data.Structure.Fields)
        {
            if (fieldInfo == null || (fieldInfo.parentControlId ?? "") != (parentControlId ?? ""))
                continue;
            if (formFieldType != null && formFieldType != fieldInfo.FormFieldType)
                continue;
            string k = fieldInfo.ControlType + fieldInfo.FieldName;
            fields.TryAdd(k, fieldInfo);
        }

        StringWriter html = new();
        foreach (InputFieldDefinition fieldInfo in fields.Values)
        {
            html.Write(CreateControl(data, parentControlId, fieldInfo));
        }
        return html.ToString();
    }

    public CandoStringBuilder CreateControl(ControlsRendererData data, string parentControlId, InputFieldDefinition fieldInfo)
    {
        if (fieldInfo == null || (fieldInfo.parentControlId ?? "") != (parentControlId ?? ""))
            return new CandoStringBuilder();

        CommonProperties commonProperties = new(fieldInfo.GetProperties(),
            data.Options.IsReadOnly || (data.IsBulk && !fieldInfo.PropertyBoolean(eControlPropertyId.Required)));

        object value = GetValue(data.Record, fieldInfo, data.Options.CalendarType, commonProperties.IsReadOnly, out bool isNull);
        if (isNull)
            value = commonProperties.DefaultValue;

        eControlTypeId controlType = DetermineControlType(data, fieldInfo);
        switch (controlType)
        {
            case eControlTypeId.Accordion:
                return new ContainersControls(formLogicHelper, fieldInfo, data, this, _sbvrRenderer)
                    .RenderAccordion();
            case eControlTypeId.MultiTab:
                return new ContainersControls(formLogicHelper, fieldInfo, data, this, _sbvrRenderer)
                    .RenderMultiTab();
            case eControlTypeId.FieldSet:
                return new ContainersControls(formLogicHelper, fieldInfo, data, this, _sbvrRenderer)
                    .RenderFieldSet();
            case eControlTypeId.Card:
                return new Card(formLogicHelper, fieldInfo, data, this, _sbvrRenderer).Render();
            case eControlTypeId.BusinessDefinedControl:
                {
                    string controlId = fieldInfo.PropertyValue(eControlPropertyId.BusinessControlId)
                                         ?? throw new Exception("BusinessDefinedControls must have BusinessControlId property.");
                    data.AddIncludeNeed(BusinessDefinedControls[controlId]);
                    return BusinessDefinedControls[controlId].RenderHtml(fieldInfo, data);
                }

            case eControlTypeId.BooleanCombo:
                return new BooleanCombo(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.BooleanRadioButtons:
                return new BooleanRadioButtons(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.CheckBox:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderCheckBox(value?.ToString());
            case eControlTypeId.CheckBoxList:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderCheckBoxList(value?.ToString());
            case eControlTypeId.ComboBox:
                return new ComboBox(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.MultipleSelectableCombo:
                return new MultipleSelectableCombo(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();

            case eControlTypeId.DatePicker:
            case eControlTypeId.DateTime:
                return new DatePicker(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();

            case eControlTypeId.File:
                if (data.IsBulk) return new CandoStringBuilder();
                return new FileHtmlControl(cmmnFileManager, formLogicHelper, fieldInfo, data, logger, configuration, _sbvrRenderer).Render();
            case eControlTypeId.AdvancedUpload:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderAdvancedUpload(cmmnFileManager, logger, configuration);
            case eControlTypeId.Image:
                return new CandoImage(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();

            case eControlTypeId.Form:
                return new FormIframe(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.GridColumn:
                return new GridColumn(formLogicHelper, fieldInfo, data, this, _sbvrRenderer).Render();
            case eControlTypeId.IndexTable:
                if (data.IsBulk) return new CandoStringBuilder();
                return new IndexTable(formLogicHelper, formStructRoutines, fieldInfo, data, this, _sbvrRenderer).Render();
            case eControlTypeId.LinkList:
                return new LinkList(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.MapMultiPointView:
            case eControlTypeId.MapPointInput:
            case eControlTypeId.MapRegionInput:
            case eControlTypeId.MapRoutingView:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer).RenderMap();
            case eControlTypeId.None:
                return new None(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.NumberInput:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderNumberInput(value?.ToString());
            case eControlTypeId.OperationButton:
                data.AddIncludeNeed(PluginInclude.OperationButton);
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderOperationButton();
            case eControlTypeId.RadioButton: // todo It's useless
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderRadioButton(value?.ToString());
            case eControlTypeId.RadioButtons:
                return new RadioButtons(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.Report:
                data.AddIncludeNeed(PluginInclude.ReportControl);
                return new ReportControl(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
            case eControlTypeId.MultilineTextInput:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderMultilineTextInput(value?.ToString());
            case eControlTypeId.SystemPageLink:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderSystemPageLink();
            case eControlTypeId.Terminal:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderTerminal();
            case eControlTypeId.TextInput:
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderTextInput(value?.ToString());
            case eControlTypeId.DurationInput: //todo should be separated from time input!
            case eControlTypeId.TimeInput:
                data.AddIncludeNeed(PluginInclude.InputMask);
                return new CandoHtmlControl(formLogicHelper, fieldInfo, data, _sbvrRenderer)
                    .RenderTimeInput(value?.ToString());
            case eControlTypeId.Toggle:
                if (!data.Options.IsDesignMode)
                    data.AddIncludeNeed(PluginInclude.Toggle);
                return new Toggle(formLogicHelper, fieldInfo, data, _sbvrRenderer).Render();
        }

        return new CandoStringBuilder();
    }
    private static eControlTypeId DetermineControlType(ControlsRendererData data, InputFieldDefinition fieldInfo)
    {
        return data.Options.IsDesignMode && (fieldInfo.ControlType.GetControlGroup()
                                            ?.Any(c => c == ControlGroup.ContainerGroup ||
                                                       c == ControlGroup.ContainerItem) ??
                                        false)
            ? eControlTypeId.FieldSet
            : fieldInfo.ControlType;
    }

    private static object GetValue(IExpressionValue record, InputFieldDefinition fieldInfo, string calendar,
        bool isReadOnly, out bool isNull)
    {
        isNull = false;
        object oValue = null;
        if (record != null)
        {
            if (string.IsNullOrEmpty(fieldInfo.FieldName) || !record.GetField(fieldInfo.FieldName, out oValue))
                isNull = true;
            if (oValue != null)
            {
                if (oValue is string)
                {
                    if (fieldInfo.ControlType == eControlTypeId.DateTime ||
                        fieldInfo.ControlType == eControlTypeId.DatePicker)
                    {
                        string[] dateObj = oValue.ToString().Split('/');
                        if (dateObj.Length >= 3)
                        {
                            try
                            {
                                oValue = new DateTime(Convert.ToInt32(dateObj[0]), Convert.ToInt32(dateObj[1]),
                                    Convert.ToInt32(dateObj[2]), 0, 0, 0, 0);
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                    }
                }
                if (oValue is DateTime dt)
                {
                    if (dt.Year > 1900)
                    {
                        string dStr;
                        if (calendar == "shamsi")
                        {
                            PersianCalendar pc = new();
                            dStr = pc.GetYear(dt).ToString() + '/' + pc.GetMonth(dt).ToString("d2") + '/' +
                                   pc.GetDayOfMonth(dt).ToString("d2");
                        }
                        else
                            dStr = dt.Year.ToString() + '/' + dt.Month.ToString("d2") + '/' + dt.Day.ToString("d2");

                        if (isReadOnly && !(dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0))
                            dStr += $" {dt.Hour}:{dt.Minute}:{dt.Second}";
                        return Encoder.Encode(dStr);
                    }
                    else
                    {
                        isNull = true;
                        return Encoder.Encode("");
                    }
                }
                else
                {
                    return oValue is long || oValue is int || oValue is double
                        ? fieldInfo.ControlType == eControlTypeId.TimeInput
                                            ? Encoder.Encode(GetTimeSpanValue(oValue))
                                            : oValue is double d ? Encoder.Encode(d.ToString(CultureInfo.InvariantCulture)) : oValue
                        : oValue;
                }
            }
            else
            {
                isNull = true;
                return Encoder.Encode("");
            }
        }
        else
        {
            isNull = true;
            return Encoder.Encode("");
        }
    }
    
    internal static string GetTimeSpanValue(object value)
    {
        TimeSpan timespan = new(Convert.ToInt64(value));
        return timespan.ToTimeInputValue();
    }
}
