namespace Neo.Bpms.Domain.Expressions.Exceptions;

public class ParserException : Exception
{
    public ParserException(string expression, int position, string message = null) :
        base($"Parser Error:\r\n{expression}\r\nposition: {GetErrorPosition(expression, position)}\n{message}")
    {

    }

    public ParserException(string message) : base(message)
    {
    }

    private static string GetErrorPosition(string expression, int index)
    {
        int line = 1;
        int position = 1;
        for (int i = 0; i < index; i++)
        {
            if (expression[i] is '\r' or '\n')
            {
                if (expression[i] == '\n')
                {
                    line++;
                }

                position = 1;
            }
            else
            {
                position++;
            }
        }

        return "Line " + line + " Position " + position;
    }
}
