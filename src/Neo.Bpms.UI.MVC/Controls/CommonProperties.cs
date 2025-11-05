namespace Neo.Bpms.UI.MVC.Controls;

public class CommonProperties
{
    public CommonProperties(List<UIComponentProperty> properties, bool isReadOnly = false)
    {
        if (properties != null)
        {
            foreach (UIComponentProperty property in properties)
            {
                switch (property.PropertyId)
                {
                    case eControlPropertyId.ContextualStyle:
                        Style = property.Value.ToEnum(ContextualStyle.Primary);
                        break;
                    case eControlPropertyId.BackgroundColor:
                        BackgroundColor = property.Value.ToString();
                        break;
                    case eControlPropertyId.Color:
                        Color = property.Value.ToString();
                        break;
                    case eControlPropertyId.DefaultValue:
                        DefaultValue = property.Value.ToString();
                        break;
                    case eControlPropertyId.Direction:
                        Direction = property.Value.ToString();
                        break;
                    case eControlPropertyId.DoubleWidth:
                        IsDoubleWidth = property.GetValueAsBoolean();
                        break;
                    case eControlPropertyId.FullWidth:
                        IsFullWidth = property.GetValueAsBoolean();
                        break;
                    case eControlPropertyId.GridColumns:
                        _modelGridColumnClass += $"col-{property.Value} ";
                        break;
                    case eControlPropertyId.GridColumnsSmall:
                        _modelGridColumnClass += $"col-sm-{property.Value} ";
                        break;
                    case eControlPropertyId.GridColumnsMedium:
                        _modelGridColumnClass += $"col-md-{property.Value} ";
                        break;
                    case eControlPropertyId.GridColumnsLarge:
                        _modelGridColumnClass += $"col-lg-{property.Value} ";
                        break;
                    case eControlPropertyId.GridColumnsExtraLarge:
                        _modelGridColumnClass += $"col-xl-{property.Value} ";
                        break;
                    case eControlPropertyId.HeightInPixels:
                        // this is wrong! but Report and Form iframe controls need it temporarily.
                        _cssRules +=
                            $"height: {property.Value}px; min-height: {property.Value}px; max-height: {property.Value}px;";
                        break;
                    case eControlPropertyId.IsMultiple:
                        IsMultiple = true;
                        break;
                    case eControlPropertyId.ReadOnly:
                        IsReadOnly = property.GetValueAsBoolean();
                        IsReadOnlyByProperty = IsReadOnly;
                        break;
                    case eControlPropertyId.Required:
                        IsRequired = property.GetValueAsBoolean();
                        break;
                    case eControlPropertyId.ShowHide:
                        ShouldShow = property.GetValueAsBoolean();
                        break;
                    case eControlPropertyId.Tooltip:
                        Tooltip = property.Value.ToString();
                        break;
                }
            }
        }

        if (!(properties?.Exists(p => p.PropertyId == eControlPropertyId.ReadOnly) ?? false) && isReadOnly)
        {
            IsReadOnly = true;
        }
    }

    public string Tooltip { get; }
    public string DefaultValue { get; }
    public bool IsRequired { get; }
    public bool ShouldShow { get; } = true;
    public string ShowHideRelatedClass => ShouldShow ? "" : "ShowHide";
    public bool IsReadOnlyByProperty { get; }
    public bool IsReadOnly { get; set; }
    public bool IsMultiple { get; set; }
    public string ReadOnlyRelatedAttribute => IsReadOnly ? "disabled" : "";
    public string Direction { get; } = CultureHelper.IsRightToLeft() ? "rtl" : "ltr";

    #region ContextualStyle

    public ContextualStyle? Style { get; }
    public string? BackgroundColor { get; }
    public string? Color { get; }

    public string ButtonStyleClass(ContextualStyle @default)
    {
        return $"btn-{(Style ?? @default).ToString().ToLower()}";
    }

    public string BgAndTextStyleClass =>
        Style == null
            ? ""
            : $"bg-{Style.ToString().ToLower()} {(Style.In(ContextualStyle.Light, ContextualStyle.Active) ? "" : "text-white")}";

    public string ListGroupItemStyleClass =>
        Style == null ? string.Empty : $"list-group-item-{Style.ToString().ToLower()}";

    #endregion

    #region Grid

    public bool IsDoubleWidth { get; }
    public bool IsFullWidth { get; }
    private readonly string _modelGridColumnClass;

    public string NarrowColumnClasses => !string.IsNullOrEmpty(_modelGridColumnClass) ? _modelGridColumnClass :
        IsFullWidth ? "col-md-12 " :
        IsDoubleWidth ? "col-md-6 " : "col-md-3 col-sm-6 ";

    public string WideColumnClasses =>
        !string.IsNullOrEmpty(_modelGridColumnClass) ? _modelGridColumnClass : "col-12 ";

    public string CustomColumnClasses(string @default)
    {
        return !string.IsNullOrEmpty(_modelGridColumnClass) ? _modelGridColumnClass : @default;
    }

    #endregion

    #region Css

    private readonly string _cssRules;

    public string GetStyle() => $"style=\"{_cssRules}\"";

    #endregion
}
