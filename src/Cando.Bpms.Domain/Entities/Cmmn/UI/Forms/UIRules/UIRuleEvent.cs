namespace Neo.Bpms.Domain.Entities.Cmmn.UI.Forms.UIRules;

/// <summary>
/// A rule trigger Event
/// </summary>
public class UIRuleEvent
{
    //public enum TControllerEventKeyType
    //{
    //	ctrlkey_Evry = 1,
    //	ctrlkey_ENTER = 2,
    //	ctrlkey_Up = 3,
    //	ctrlkey_DOWN = 4,
    //	ctrlkey_LEFT = 5,
    //	ctrlkey_RIGHT = 6,
    //	ctrlkey_TAB = 7,
    //	ctrlkey_SPACE = 8,
    //}
    public enum eEventType
    {
        invalid = 0,
        onPageLoad = 1,
        onPageUnLoad = 2,
        onPageScroll = 3,
        onPageResize = 4,
        onSubmitForm = 5,

        onClick = 11,
        onDblClick = 12,
        onUserChange = 13,
        onFocus = 14,
        onBlur = 15,
        onLoad = 16,
        onChange = 17,

        onKeyDown = 21,
        onKeyPress = 22,
        onKeyUp = 23,

        onMouseDown = 31,
        onMouseUp = 32,
        onMouseMove = 33,
        onMouseOut = 34,
        onMouseOver = 35,
        onDrag = 36,
        onDrop = 37,

        onTableRowChange = 40,
        onTableRowAdd = 41,
        onTableRowDelete = 42,

    }
    public eEventType type;
    public string exporterFieldId, RepeatScopeId;
    //			private int RepeatScopeId;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIRuleEvent"/> class.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="fieldId">The field identifier.</param>
    /// <param name="RepeatScopeId">The repeat scope identifier.</param>
    public UIRuleEvent(eEventType type, string fieldId = null, string RepeatScopeId = null)
    {
        this.type = type;
        exporterFieldId = fieldId;
        this.RepeatScopeId = RepeatScopeId;
    }

    public UIRuleEvent() { }
}