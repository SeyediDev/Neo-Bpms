namespace Neo.Bpms.Domain.Entities.Attributes.EntityAttributes;

/// <summary>
/// Validation Attribute
/// 
/// entity can have multiple validation attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EAttr_Validation : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="errorText"></param>
    /// <param name="errorFormula"></param>
    /// <param name="enErrorText"></param>
    /// <param name="enErrorFormula"></param>
    public EAttr_Validation(string statement, string errorText, string errorFormula = null,
        string enErrorText = null, string enErrorFormula = null)
    {
        Statement = statement;
        Error = new ExceptionInformationData(ExceptionArea.DataEngine, Guid.NewGuid().ToString())
        {
            ErrorFormula = errorFormula,
            //name = statement,
            ErrorText = errorText,
            Name = statement,
            EnErrorText = errorText,
            EnErrorFormula = errorFormula,
        };
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="EAttr_Validation"/> class.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <param name="error">The error.</param>
    public EAttr_Validation(string statement, ExceptionInformationData error)
    {
        Statement = statement;
        Error = error;
    }

    public string Statement { get; set; }
    public string Condition { get; set; }

    //todo: document in advanced meta design notes
    /// <summary>
    /// if validation must be repeated on any table row, SubEntityId indicates the table id
    /// </summary>
    public string SubEntityId { get; set; }

    public ExceptionInformationData Error { get; set; }

}
