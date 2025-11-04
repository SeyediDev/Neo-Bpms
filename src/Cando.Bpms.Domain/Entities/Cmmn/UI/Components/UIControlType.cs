namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Components;

public enum eControlTypeId
{
    [ControlGroup(ControlGroup.None, ControlGroup.Input, ControlGroup.Number, ControlGroup.Text, ControlGroup.DateTime,
        ControlGroup.Association)]
    None = 0,

    //        [ControlGroup(ControlGroup.Input, ControlGroup.Association)]
    RadioButton = 1,

    [ControlGroup(ControlGroup.Input, ControlGroup.Number)]
    NumberInput = 4,

    [ControlGroup(ControlGroup.Input, ControlGroup.Text)]
    TextInput = 5,

    [ControlGroup(ControlGroup.Input, ControlGroup.Text)]
    MultilineTextInput = 7,

    [ControlGroup(ControlGroup.Input, ControlGroup.DateTime)]
    DatePicker = 8,

    [ControlGroup(ControlGroup.Input, ControlGroup.DateTime)]
    DateTime = 9,

    [ControlGroup(ControlGroup.Input, ControlGroup.Boolean)]
    CheckBox = 15,

    //        [ControlGroup(ControlGroup.Input, ControlGroup.MultiAssociation)]
    //        [ControlGroup(ControlGroup.Input, ControlGroup.Table)]
    CheckBoxList = 16,

    [ControlGroup(ControlGroup.Input, ControlGroup.Association)]
    ComboBox = 17,

    //        [ControlGroup(ControlGroup.Input, ControlGroup.Time)]
    DurationInput = 20,

    [ControlGroup(ControlGroup.Input, ControlGroup.Time)]
    TimeInput = 21,

    [ControlGroup(ControlGroup.Input, ControlGroup.File)]
    File = 24,

    /// <summary>
    /// Link button for forms with dynamic evaluation of LinkParameters
    /// </summary>
    [ControlGroup(ControlGroup.Link)]
    SystemPageLink = 29,

    [ControlGroup(ControlGroup.Input, ControlGroup.Text)]
    ColorPicker = 30, // todo

    /// <summary>
    /// Specific pages link in Index Forms
    /// </summary>
    [ControlGroup(ControlGroup.Link)]
    SpecificLinkColumn = 33,

    [ControlGroup(ControlGroup.Input, ControlGroup.File)]
    AdvancedUpload = 34,

    [ControlGroup(ControlGroup.Input, ControlGroup.Table)]
    MultipleSelectableCombo = 35,
    [ControlGroup(ControlGroup.Table)]
    IndexTable = 36,

    [ControlGroup(ControlGroup.Input, ControlGroup.Boolean)] //todo!
    BooleanCombo = 37,

    [ControlGroup(ControlGroup.Input, ControlGroup.Association)]
    RadioButtons = 38,

    [ControlGroup(ControlGroup.Input, ControlGroup.Boolean)]
    Toggle = 39,

    [ControlGroup(ControlGroup.Input, ControlGroup.Boolean)] //todo!
    BooleanRadioButtons,


    //        [ControlGroup(ControlGroup.ContainerGroup)]
    MultiPage = 101, // todo ?

    [ControlGroup(ControlGroup.ContainerGroup)]
    MultiTab = 102,

    [ControlGroup(ControlGroup.ContainerGroup, ControlGroup.ContainerItem)]
    FieldSet = 104,

    [ControlGroup(ControlGroup.ContainerItem)]
    MultiTabItem = 130,

    [ControlGroup(ControlGroup.ContainerItem)]
    AccordionItem = 131,

    [ControlGroup(ControlGroup.ContainerGroup, ControlGroup.ContainerItem, ControlGroup.Card)] // todo
    Card,

    [ControlGroup(ControlGroup.ContainerGroup, ControlGroup.ContainerItem)] // todo
    GridColumn,

    MapPointInput = 162,
    MapRegionInput = 163,
    MapRoutingView = 164,
    MapMultiPointView = 165,

    [ControlGroup(ControlGroup.ContainerGroup)]
    Accordion = 201,

    Report = 217,
    Form,

    [ControlGroup(ControlGroup.Card)]
    LinkList,
    [ControlGroup(ControlGroup.Link)]
    LinkListItem,

    [ControlGroup(ControlGroup.Link)]
    Link,

    /// <remarks>
    /// Needs ServiceInterfaceProtocol and OperationName properties
    /// </remarks>
    OperationButton,

    Terminal,

    [ControlGroup(ControlGroup.StaticFile)]
    Image,

    /// <summary>
    /// Can be useful when something is needed in the data object
    /// </summary>
    DontRender = 100000,
    /// <remarks>
    /// Needs BusinessControlId property
    /// </remarks>
    [ControlGroup(ControlGroup.None)]
    BusinessDefinedControl = 100001,

    [ControlGroup(ControlGroup.ContainerGroup)]
    MainPage = 220,

    [ControlGroup(ControlGroup.ContainerGroup)]
    AppHeader = 230,

    [ControlGroup(ControlGroup.ContainerGroup)]
    AppToolbar = 240,

    [ControlGroup(ControlGroup.ContainerGroup)]
    AppCard = 250,

    [ControlGroup(ControlGroup.ContainerGroup)]
    AppFormSelector = 260,
}

public class UIControlType(eControlTypeId id, string name) : BaseModelClass(null, (int)id, name)
{
    public List<UIControlProperty> AllowedProperties = [];
    public List<UIControlProperty> LabelAllowedProperties = [];

    public eControlTypeId TypeId
    {
        get { return (eControlTypeId)Convert.ToInt32(Id); }
    }
}