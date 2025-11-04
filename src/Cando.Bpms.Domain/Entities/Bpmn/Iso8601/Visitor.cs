using System.Text;

namespace Neo.Bpms.Domain.Entities.Bpmn.Iso8601;

internal class Visitor
{
    protected char[] tokens;
    protected List<char> currentDigits = [];
    protected bool inTimeSection = false;
    protected bool HandleDateDesignator(ref double target)
    {
        return HandleDesignator(false, ref target);
    }

    protected bool HandleTimeDesignator(ref double target)
    {
        return HandleDesignator(true, ref target);
    }

    protected bool HandleDesignator(bool timeToken, ref double target)
    {
        if (inTimeSection != timeToken || currentDigits.Count == 0)
        {
            return false;
        }

        if (!double.TryParse(CharListToString(currentDigits), out double result))
        {
            return false;
        }

        target = result;
        currentDigits.Clear();

        return true;
    }

    protected static string CharListToString(IList<char> chars)
    {
        return chars.Aggregate(new StringBuilder(), (builder, c) => builder.Append(c), builder => builder.ToString());
    }
}