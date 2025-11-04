namespace Neo.Bpms.Domain.Entities.Base.Exceptions;

public class ExceptionInformation : BaseModelClass
{
    public ExceptionInformation()
    {
    }

    public ExceptionInformation(ExceptionArea area, int Id, string name)
        : base(null, (int)area + "." + Id, name)
    {
    }

    public ExceptionInformation(ExceptionArea area, string Id)
        : base(null, (int)area + "." + Id, null)
    {
    }

    public ExceptionInformation(DataEngineException Id, string name)
        : this(ExceptionArea.DataEngine, (int)Id, name)
    {
    }

    public ExceptionInformation(DataEngineException Id)
        : this(Id, null)
    {
    }


    public bool textIsFormula;
    public string EnErrorText { get; set; }
    public string EnErrorFormula { get; set; }

    public string ErrorText
    {
        get => Name;
        set
        {
            textIsFormula = false;
            Name = value;
        }
    }

    public string ErrorFormula
    {
        get => Name;
        set
        {
            textIsFormula = true;
            Name = value;
        }
    }

    public override string ToString()
    {
        return ErrorText;
    }
}
