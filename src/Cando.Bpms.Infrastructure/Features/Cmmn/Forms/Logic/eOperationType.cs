namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.Logic;

[Flags]
public enum eOperationType
{
    None = 0,

    SetValue = 11,
    ToggleClass,
    SetClass,
    RemoveClass,
    ChangeCSSAttribute,
    SetLocalParameterValue,
    SetProperty,

    ShowWarning = 21,
    ShowError,
    ShowPrompt,
    ShowModal,
    ShowWindow,
    ShowAutoHideWindow,

    AddRow = 31,
    ChangeList,

    CallController = 41,

    ServerOperation = 0x100, //ServerOperationCode = ServerOperation+OperationType
}
