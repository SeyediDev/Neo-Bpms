using Neo.Bpms.Domain.Features.Cmmn.ObjectStorage.Dto;
using Neo.Bpms.Domain.Models.Cmmn.UI.Components;

namespace Neo.Bpms.UI.MVC.Controls;

public class TableHelper
{
    /// <summary>
    /// create Elements for Table's cell
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cellType"></param>
    /// <param name="cellInfo"></param>
    /// <param name="calendar"></param>
    /// <returns></returns>
    public static HtmlString CreateCellElement(object value,
        ColumnFieldDefinition cellInfo, string calendar = "shamsi")
    {
        string str = CreateCellElem(value, cellInfo, calendar);
        return new HtmlString(str);
    }

    public static string CreateCellElem(object value, InputFieldDefinition cellInfo, string calendar = "shamsi")
    {
        object str = value ?? "";
        switch (cellInfo.FieldType)
        {
            case TVariableTypes.Double:
            case TVariableTypes.Decimal:
                try
                {
                    if (value is DateTime)
                        goto case TVariableTypes.DateTime;
                    if (decimal.TryParse(value?.ToString(), out decimal v))
                    {
                        // Check if it's essentially a whole number
                        if (v == Math.Floor(v))
                        {
                            str = $"{v:n0}";
                        }
                        else
                        {
                            // Format with enough decimal places, then remove trailing zeros
                            var culture = System.Globalization.CultureInfo.CurrentCulture;
                            str = v.ToString("N10", culture);
                            // Remove trailing zeros after decimal point (culture-aware)
                            var decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
                            if (str.ToString().Contains(decimalSeparator))
                            {
                                str = str.ToString().TrimEnd('0').TrimEnd(decimalSeparator.ToCharArray());
                            }
                        }
                    }
                    else
                    {
                        str = value?.ToString();
                    }
                }
                catch
                {
                    str = value?.ToString();
                }

                break;
            case TVariableTypes.Int:
            case TVariableTypes.Long:
            case TVariableTypes.Short:
                try
                {
                    if (long.TryParse(value?.ToString(), out long intVal))
                    {
                        str = $"{intVal:n0}";
                    }
                    else
                    {
                        str = value?.ToString();
                    }
                }
                catch
                {
                    str = value?.ToString();
                }
                break;
            case TVariableTypes.BOOL:
                str = CreateBoolElement(cellInfo, value);
                break;
            case TVariableTypes.DayHourMinute:
            case TVariableTypes.DurHourMinute:
                if (!string.IsNullOrEmpty(str.ToString().Trim()))
                {
                    str = FormDataRoutines.GetTimeSpanDisplayValue(str);
                }

                break;
            case TVariableTypes.Date:
            case TVariableTypes.DateStr:
            case TVariableTypes.DateTime:
                str = CreateDateTimeElement(calendar, str, cellInfo);
                break;
            case TVariableTypes.File:
                List<DocumentView> documents = (List<DocumentView>)str;
                if (documents == null || documents.Count==0) return "@";
                if (documents.Count == 1)
                {
                    //TODO TEMP
                    str = $@"<span id={documents[0].Id}>{documents[0].Title}</i>";
                }
                else
                {
                    //TODO TEMP
                    foreach (var document in documents)
                    {
                        str += $@"<span id={documents[0].Id}>{documents[0].Title}</i>";
                    }
                }
                break;
            default:
                str = ControlsRenderer.Encoder.Encode(str.ToString()!);
                break;
        }

        //str = str?.ToString().Replace("_", " _")??"";
        return str?.ToString();
    }

    public static object CreateDateTimeElement(string calendar, object str, InputFieldDefinition cellInfo)
    {
        if (!string.IsNullOrEmpty(str.ToString().Trim()))
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(str);
            }
            catch
            {
                // ignored
            }

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
                {
                    dStr = dt.Year.ToString() + '/' + dt.Month.ToString("d2") + '/' + dt.Day.ToString("d2");
                }

                if (dt.Hour != 0 || dt.Minute != 0 || dt.Second != 0)
                {
                    dStr += " " + dt.Hour + ":" + dt.Minute;
                    if (cellInfo?.PropertyValue(eControlPropertyId.TimeDisplayResolution)?.Equals("s") ?? false
                    ) //todo s is :/
                        dStr += $":{dt.Second}";
                }

                str = dStr;
            }
            else
            {
                str = "";
            }
        }

        return str;
    }

    private static string CreateBoolElement(InputFieldDefinition col, object value)
    {
        static string GetIcon(bool? b)
        {
            return b == null
                ? null
                : $@"<i class=""fa fa-{(b.Value ? "check" : "times")}""></i>";
        }

        string GetContextualStyle(bool? b)
        {
            if (b != null)
            {
                string style = col.PropertyValue(
                    b.Value ? eControlPropertyId.TrueStyle : eControlPropertyId.FalseStyle);
                if (!string.IsNullOrEmpty(style))
                    return $"text-{style.ToLower()}";
            }

            return null;
        }

        static string GetTitle(string textualRepresentation, bool? b)
        {
            return textualRepresentation is not null
                ? textualRepresentation
                : b switch
                {
                    true => ViewTexts.TrueTitle,
                    false => ViewTexts.FalseTitle,
                    null => null
                };
        }

        string GetTextualRepresentation(bool? b)
        {
            return !col.HasProperty(eControlPropertyId.FalseTitle)
                ? null
                : b switch
                {
                    true => col.PropertyValue(eControlPropertyId.TrueTitle) ?? col.Alias,
                    false => col.PropertyValue(eControlPropertyId.FalseTitle) ?? "",
                    null => col.PropertyValue(eControlPropertyId.NullTitle) ?? "-"
                };
        }

        bool? actualValue = value is bool ? (bool?)value : value == null ? null : ConvUtill.ToBoolean(value);
        string textualRepresentation = GetTextualRepresentation(actualValue);
        return
            $@"<span title=""{GetTitle(textualRepresentation, actualValue)}"" class=""{GetContextualStyle(actualValue)}"">{textualRepresentation}{GetIcon(actualValue)}</span>";
    }
}
