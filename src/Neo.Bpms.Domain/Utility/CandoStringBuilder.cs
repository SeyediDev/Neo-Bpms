using System.Text;

namespace Neo.Bpms.Domain.Utility;

public class CandoStringBuilder
{

    public CandoStringBuilder()
    {
        _stringBuilder = new StringBuilder();
    }
    public CandoStringBuilder(string initialValue)
    {
        _stringBuilder = new StringBuilder(initialValue);
    }

    private readonly StringBuilder _stringBuilder;
    public StringBuilder StringBuilder=> _stringBuilder;

    public CandoStringBuilder Append(string s)
    {
        _stringBuilder.Append(s);
        return this;
    }

    public CandoStringBuilder Append(CandoStringBuilder s)
    {
        _stringBuilder.Append(s.StringBuilder);
        return this;
    }

    public CandoStringBuilder Append(StringBuilder s)
    {
        _stringBuilder.Append(s);
        return this;
    }

    public static CandoStringBuilder operator +(CandoStringBuilder tsb, string s)
    {
        return tsb.Append(s);
    }

    public static CandoStringBuilder operator +(CandoStringBuilder tsb, CandoStringBuilder s)
    {
        return tsb.Append(s.StringBuilder);
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }

    public long Length => _stringBuilder.Length;
}
