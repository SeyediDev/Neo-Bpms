namespace Neo.Bpms.Domain.Models.Base;

public class ErrorInformationList : List<ErrorInformation>
{
    public void AddFatal(string text, string @for, string code, string generalText)
    {
        Add(text, @for, code, generalText, eErrorLevel.FatalError);
    }

    public void AddError(string text, string @for, string code, string generalText)
    {
        Add(text, @for, code, generalText, eErrorLevel.Error);
    }

    public void AddWarning(string text, string @for, string code, string generalText, eWarningLevel warningLevel)
    {
        Add(text, @for, code, generalText, (eErrorLevel)warningLevel);
    }

    public void AddInfo(string text, string @for, string code, string generalText)
    {
        Add(text, @for, code, generalText, eErrorLevel.Info);
    }

    private void Add(string text, string @for, string code, string generalText, eErrorLevel type)
    {
        Add(new ErrorInformation
        {
            Text = text,
            For = @for,
            Type = type,
            Code = code,
            GeneralText = generalText,
        });
    }
}

public class ErrorInformation
{
    public eErrorLevel Type;
    public string For;
    public string Text;
    public string Code;
    public string GeneralText;
}

public enum eErrorLevel
{
    FatalError,
    Error,
    WarningLevel0,
    WarningLevel1,
    WarningLevel2,
    Info
}

public enum eWarningLevel
{
    WarningLevel0 = eErrorLevel.WarningLevel0,
    WarningLevel1 = eErrorLevel.WarningLevel1,
    WarningLevel2 = eErrorLevel.WarningLevel2
}