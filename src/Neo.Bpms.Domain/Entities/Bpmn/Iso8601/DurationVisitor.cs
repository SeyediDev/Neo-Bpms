using Neo.Bpms.Domain.Entities.Bpmn.Iso8601;

namespace Neo.Bpms.Domain.Model.Iso8601;

internal sealed partial class DurationVisitor : Visitor
{
    private double years = 0;
    private double month = 0;
    private double weeks = 0;
    private double days = 0;
    private double hours = 0;
    private double minutes = 0;
    private double seconds = 0;

    private DurationVisitor(string duration)
    {
        tokens = [.. duration];
    }

    private void Visit()
    {
        if (tokens.Length == 0)
        {
            IsValid = true;
            return;
        }

        if (tokens.Length < 2 || tokens[0] != 'P')
        {
            IsValid = false;
            return;
        }

        foreach (char token in tokens.Skip(1))
        {
            if (!IsValid)
            {
                return;
            }

            switch (token)
            {
                case 'Y':
                    IsValid = HandleDateDesignator(ref years);
                    continue;
                case 'T':
                    inTimeSection = true;
                    continue;
                case 'W':
                    IsValid = HandleDateDesignator(ref weeks);
                    continue;
                case 'D':
                    IsValid = HandleDateDesignator(ref days);
                    continue;
                case 'H':
                    IsValid = HandleTimeDesignator(ref hours);
                    continue;
                case 'M':
                    IsValid = inTimeSection ? HandleTimeDesignator(ref minutes) : HandleTimeDesignator(ref month);
                    continue;
                case 'S':
                    IsValid = HandleTimeDesignator(ref seconds);
                    continue;
            }

            currentDigits.Add(token);
        }

        IsValid &= currentDigits.Count == 0;
    }

}
