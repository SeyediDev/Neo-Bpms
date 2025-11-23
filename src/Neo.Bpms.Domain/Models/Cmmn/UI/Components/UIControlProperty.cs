namespace Neo.Bpms.Domain.Models.Cmmn.UI.Components;

public enum eControlPropertyId
{
    None = 0,

    [Title(nameof(Texts.ShowHide), typeof(Texts))]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.All)]
    ShowHide = 1,
    [Title(nameof(Texts.ReadOnly), typeof(Texts))]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Input)]
    ReadOnly = 2,
    [Title(nameof(Texts.Required), typeof(Texts))]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Input)]
    Required = 4,

    DefaultValue = 9,
    Tooltip = 10,
    Direction = 27,
    // Intentionally not in any group
    LabelName = 30,
    /// <remarks>
    /// Specific property for the SpecificLinkColumn control
    /// </remarks>
    LinkTarget = 45,
    /// <summary>
    /// Specific property for the SystemPageLink control
    /// </summary>
    SystemLinkAddress = 46,
    [Title(nameof(Texts.IsPassword), typeof(Texts))]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Text)]
    IsPassword = 48,
    [Title("Display Fields")]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    DisplayFields = 50,
    /// <value>
    /// font-awesome 4.2 + bpms icons + project's font icons
    /// </value>
    [Title(nameof(Texts.IconClass), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Card)]
    IconClass = 54,
    /// <summary>
    /// Relative path to the image file
    /// </summary>
    [Title(nameof(Texts.IconImage), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Card)]
    IconImage = 55,
    /// <summary>
    /// Relative path to the file
    /// </summary>
    [Title(nameof(Texts.SourceFileAddress), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.StaticFile)]
    SourceFileAddress = 56,
    [Title("TimeDisplayResolution", typeof(Texts))]
    [ValueType(PropertyValueType.String)] // can only have 's' value!
    [ControlGroup(ControlGroup.DateTime)]
    TimeDisplayResolution = 63,
    DataSourceString = 77,
    [Title("فرمولِ فیلتر")]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    FilterFormula = 80,

    DontLoadData = 89,

    [Title(nameof(Texts.Summable), typeof(Texts))]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Column)]
    Summable = 90,

    [Title(nameof(Texts.SuccessWhen), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    SuccessWhen = 91,

    [Title(nameof(Texts.DangerWhen), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    DangerWhen = 92,

    [Title(nameof(Texts.WarningWhen), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    WarningWhen = 93,

    [Title(nameof(Texts.InfoWhen), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    InfoWhen = 94,

    [Title(nameof(Texts.ActiveWhen), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    ActiveWhen = 95,

    TrueTitle = 102,
    FalseTitle = 103,
    AllTitle = 104,
    NullTitle = 105,

    /// <value>
    /// Value should be chosen from the ContextualStyle enum
    /// </value>
    [Title(nameof(Texts.TrueStyle), typeof(Texts))]
    [ValueType(PropertyValueType.ContextualStyle)]
    [ControlGroup(ControlGroup.Boolean)]
    TrueStyle,
    /// <value>
    /// Value should be chosen from the ContextualStyle enum
    /// </value>
    [Title(nameof(Texts.FalseStyle), typeof(Texts))]
    [ValueType(PropertyValueType.ContextualStyle)]
    [ControlGroup(ControlGroup.Boolean)]
    FalseStyle,
    /// <value>
    /// Value should be chosen from the ContextualStyle enum
    /// </value>
    [Title(nameof(Texts.ContextualStyle), typeof(Texts))]
    [ValueType(PropertyValueType.ContextualStyle)]
    [ControlGroup(ControlGroup.Link, ControlGroup.Card)]
    ContextualStyle,

    /// <value>
    /// Value should be chosen from the RGB 
    /// </value>
    [Title(nameof(Texts.BackgroundColor), typeof(Texts))]
    [ValueType(PropertyValueType.Color)]
    [ControlGroup(ControlGroup.Link, ControlGroup.Card, ControlGroup.ContainerItem)]
    BackgroundColor,

    /// <value>
    /// Value should be chosen from the RGB 
    /// </value>
    [Title(nameof(Texts.Color), typeof(Texts))]
    [ValueType(PropertyValueType.Color)]
    [ControlGroup(ControlGroup.Link, ControlGroup.Card, ControlGroup.ContainerItem)]
    Color,

    NoDetailsIcon = 151,
    NoEditIcon,
    //        [Title("موضوع")]
    //        [ValueType(PropertyValueType.String)]
    //        [ControlGroup(ControlGroup.Table)]
    Subject,
    //        [Title("رابطه")]
    //        [ValueType(PropertyValueType.String)]
    //        [ControlGroup(ControlGroup.Table)]
    Association,
    ForFieldId,
    /// <remarks>
    /// It's better to use GridColumn properties instead.
    /// </remarks>
    [Title("عرض دو برابر")]
    [ValueType(PropertyValueType.Boolean)]
    // [ControlGroup(ControlGroup.Input)]
    DoubleWidth,
    /// <summary>
    /// Query string parameter
    /// </summary>
    [Title("پارامتر کوئری‌استرینگ")]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    LinkParameter,

    //	    [Title("فضای نامی")]
    //	    [ValueType(PropertyValueType.String)]
    //	    [ControlGroup(ControlGroup.Table)]
    NamespaceId,
    //        [Title("موجودیت")]
    //        [ValueType(PropertyValueType.String)]
    //        [ControlGroup(ControlGroup.Table)]
    EntityId,
    //	    [Title("کد فرم")]
    //	    [ValueType(PropertyValueType.String)]
    //	    [ControlGroup(ControlGroup.Table)]
    /// <value>
    /// FormId/ReportId/DashboardId
    /// </value>
    EntityItemId,
    /// <value>
    /// "Form"/"Report"/"Dashboard"
    /// </value>
    [Title(nameof(Texts.PageType), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    PageType,

    /// <value>
    /// e.g. "Edit" in Form/Edit
    /// </value>
    [Title(nameof(Texts.PageSubType), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    PageSubType,

    [Title(nameof(Texts.ReportConfigurationId), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    ReportConfigurationId,

    [Title(nameof(Texts.ProcessId), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    ProcessId,

    [Title(nameof(Texts.TaskId), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    TaskId,

    [Title("کلید خارجی موجودیت متناظر")]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Table)]
    MultipleForeignKeyFieldId,

    [Title("دارای فیلتر خاص")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Filter)]
    SpecialFilter,
    [Title("غیر چندگانه")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Filter)]
    IsNotMultiple,

    [Title("چندگانه")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Filter, ControlGroup.File)]
    IsMultiple,

    /// <summary>
    /// Like 'col-' in Twitter Bootstrap
    /// </summary>
    [Title(nameof(Texts.GridColumns), typeof(Texts))]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.Input, ControlGroup.Table, ControlGroup.ContainerGroup)]
    GridColumns,
    /// <summary>
    /// Like 'col-sm-' in Twitter Bootstrap
    /// </summary>
    [Title(nameof(Texts.GridColumnsSmall), typeof(Texts))]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.Input, ControlGroup.Table, ControlGroup.ContainerGroup)]
    GridColumnsSmall,
    /// <summary>
    /// Like 'col-md-' in Twitter Bootstrap
    /// </summary>
    [Title(nameof(Texts.GridColumnsMedium), typeof(Texts))]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.Input, ControlGroup.Table, ControlGroup.ContainerGroup)]
    GridColumnsMedium,
    /// <summary>
    /// Like 'col-lg-' in Twitter Bootstrap
    /// </summary>
    [Title(nameof(Texts.GridColumnsLarge), typeof(Texts))]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.Input, ControlGroup.Table, ControlGroup.ContainerGroup)]
    GridColumnsLarge,
    /// <summary>
    /// Like 'col-xl-' in Twitter Bootstrap
    /// </summary>
    [Title(nameof(Texts.GridColumnsExtraLarge), typeof(Texts))]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.Input, ControlGroup.Table, ControlGroup.ContainerGroup)]
    GridColumnsExtraLarge,

    /// <summary>
    /// For dynamic entity evaluation of select box
    /// </summary>
    [Title(nameof(Texts.AssociatedEntityFormula), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    AssociatedEntityFormula,
    /// <summary>
    /// For dynamic entity evaluation of select box
    /// </summary>
    FormEntityId,

    [Title("عرض کامل")]
    [ValueType(PropertyValueType.Boolean)]
    // [ControlGroup(ControlGroup.Input)]
    FullWidth,

    OrderBy = 1351,
    FieldMapping = 1401,
    [Title(nameof(Texts.HeightInPixels), typeof(Texts))]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.All)]
    HeightInPixels = 3076,

    [Title(nameof(Texts.RefreshEveryXMilliseconds), typeof(Texts))]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.Link)]
    RefreshEveryXMilliseconds,

    [ControlGroup(ControlGroup.None)]
    BusinessControlId = 100001,

    /// <summary>
    /// For internal specific links (e.g. Controller's name in a controller-action)
    /// </summary>
    [Title(nameof(Texts.LinkPathFirstPart), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    LinkPathFirstPart = 103001,
    /// <summary>
    /// For internal specific links (e.g. Action's name in a controller-action)
    /// </summary>
    [Title(nameof(Texts.LinkPathLastPart), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    LinkPathLastPart = 103002,
    /// <summary>
    /// For external links
    /// </summary>
    [Title(nameof(Url))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Link)]
    Url = 103005,
    [Title("آوردن گزینه ها در صورت نیاز")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Association)]
    OnDemand,
    [Title("قابل ویرایش")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Table)]
    Editable,
    MaxRecordCount = 103008,

    [Title("گرفتن گزینه ها از سرور")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.Association)]
    RemoteData,
    [Title("Remote Data Controller")]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    RemoteDataController,
    [Title("Remote Data Action")]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    RemoteDataAction,
    
    [Title("استفاده از فایل سرور مجزا")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.File)]
    UseFileServer,

    [Title("اسکیل سایز تصاویر(کیلوبایت)")]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.File)]
    ScaleImageSizeKb,

    [Title("با لینک دانلود مستقیم")]
    [ValueType(PropertyValueType.Boolean)]
    [ControlGroup(ControlGroup.File)]
    ShowDocumentInPage,

    [Title("نوع مستند")]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.File)]
    DocumentType,

    [Title(nameof(Texts.WidthPercentage), typeof(Texts))]
    [ValueType(PropertyValueType.Double)]
    [ControlGroup(ControlGroup.Column)]
    WidthPercentage,

    [Title("Decimal Digits")]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.Column, ControlGroup.Number)]
    DecimalDigits,

    [Title(nameof(Texts.EnLabelName), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.All)]
    EnLabelName = 300000,
    Calender = 300001,
    [Title(nameof(Texts.Option), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Association)]
    Option,


    SubType = 400000,
    /// <summary>
    /// With a key:value format
    /// </summary>
    [Title(nameof(Texts.CustomProperty), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.None)]
    CustomProperty = 400001,

    [Title(nameof(Texts.ServiceInterfaceProtocol), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Operation)]
    ServiceInterfaceProtocol = 500001,

    [Title(nameof(Texts.OperationName), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Operation)]
    OperationName = 500002,

    [Title(nameof(Texts.ConfirmationMessage), typeof(Texts))]
    [ValueType(PropertyValueType.String)]
    [ControlGroup(ControlGroup.Operation)]
    ConfirmMessage,
    Category,

    /// <summary>
    /// Timeout برای پرس‌وجوهای ویجت در داشبورد (به میلی‌ثانیه)
    /// </summary>
    [Title("Timeout ویجت (میلی‌ثانیه)")]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.None)]
    WidgetTimeoutMs = 600001,

    /// <summary>
    /// Threshold برای لاگ‌گیری پرس‌وجوهای کند (به میلی‌ثانیه)
    /// </summary>
    [Title("Threshold لاگ پرس‌وجوهای کند (میلی‌ثانیه)")]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.None)]
    SlowQueryThresholdMs = 600002,

    /// <summary>
    /// زمان کش برای داده‌های ویجت در داشبورد (به دقیقه)
    /// </summary>
    [Title("زمان کش ویجت (دقیقه)")]
    [ValueType(PropertyValueType.Int)]
    [ControlGroup(ControlGroup.None)]
    WidgetCacheTimeMinutes = 600003
}
public enum eControlPropertyTarget
{
    Control = 0,
    Content = 1,
    Label = 2,
}

//todo refactor into appropriate files begin
public enum ControlGroup
{
    None,
    /// <summary>
    /// Controls don't need to declare this group to be a part of it.
    /// </summary>
    All,

    Input,

    Table,
    //MultiAssociation,
    Association,

    Number,
    Text,
    Boolean,
    File,
    Time,
    DateTime,

    ContainerGroup,
    ContainerItem,
    Card,

    StaticFile,
    Link,

    Filter, // Maybe a different concept
    Column // Maybe a different concept
    ,
    Operation
}
public enum PropertyValueType { String, Int, Double, Color, Boolean, List, ContextualStyle }

//attributes
public class ControlGroupAttribute(params ControlGroup[] controlGroup) : Attribute
{
    public readonly ControlGroup[] ControlGroup = controlGroup;
}

public class ValueTypeAttribute(PropertyValueType type) : Attribute
{
    public readonly PropertyValueType Type = type;
}

//extension methods

public static class UiEnumExtensions
{
    public static ControlGroup[] GetControlGroup(this Enum value)
    {
        ControlGroupAttribute attribute = value.GetAttribute<ControlGroupAttribute>();

        return attribute?.ControlGroup;
    }

    public static PropertyValueType? GetValueType(this Enum value)
    {
        ValueTypeAttribute attribute = value.GetAttribute<ValueTypeAttribute>();

        return attribute?.Type;
    }
}

//todo refactor into appropriate files end


public class UIControlProperty : BaseModelClass
{
    public enum eType
    {
        Integer,
        Number,
        String,
        Selection,
        Color,
    }
    public enum eCategory
    {

    }
    public UIControlProperty(eControlPropertyId id, string name, eCategory category)
        : base(null, "" + (int)id, name)
    {
        this.category = category;
        cssName = id.ToString().Replace("_", "-");
        if (cssName == "float-")
            cssName = "float";
    }

    public List<string> selections = null;
    public string cssName;
    public eCategory category;
    public eControlPropertyId PropertyId { get { return (eControlPropertyId)Convert.ToInt32(Id); } }
    public void addSelection(string selection)
    {
        selections ??= [];
        selections.Add(selection);
    }
}
