using Neo.Bpms.Domain.Entities.Base.Exceptions;
using Neo.Bpms.Domain.Entities.Cmmn;
using Neo.Bpms.Domain.Expressions.Parsers;

namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.FormModels;

public class ValidationViewModel
{
    public ValidationViewModel()
    {

    }

    public ValidationViewModel(Validation validation)
    {
        fieldId = validation.FieldId;
        condition = validation.Condition?.ExpressionString;
        statement = validation.Statement?.ExpressionString;
        exceptionMessage = validation.GetExceptionText();
    }
    public string fieldId { get; set; }
    public string condition { get; set; }
    public string statement { get; set; }
    public string exceptionMessage { get; set; }

    public Validation ToValidation()
    {
        return new Validation
        {
            FieldId = fieldId,
            Condition = Parser.ParseTree(condition),
            Statement = Parser.ParseTree(statement),
            Exception = new ExceptionInformation
            {
                ErrorText = exceptionMessage
            }
        };
    }
}
