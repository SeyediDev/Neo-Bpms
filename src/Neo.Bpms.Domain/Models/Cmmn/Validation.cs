namespace Neo.Bpms.Domain.Models.Cmmn;

public class Validation
{
    public string FieldId { get; set; }
    public ExpressionTree Condition;
    public ExpressionTree Statement;
    public ExceptionInformation Exception;
    public Validation() { }

    public string GetExceptionText() // todo place?
    {
        return Exception?.ErrorText;
    }
}
