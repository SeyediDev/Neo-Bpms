using Neo.Bpms.Domain.Models.Base.Exceptions;

namespace Neo.Bpms.Infrastructure.Base;

public static class ExceptionInfoExtensions
{
    public static string ToString(this ExceptionInformation exp, object obj)
    {
        if (!exp.textIsFormula)
        {
            return exp.ErrorText;
        }

        ExpressionNode Expression = Parser.Parse(exp.ErrorFormula);
        return Expression != null ? Expression.Eval(obj, []).ToString() : exp.ErrorText;
    }
}
