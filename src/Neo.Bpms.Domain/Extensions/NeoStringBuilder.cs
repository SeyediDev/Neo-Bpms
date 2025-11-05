using System.Text;

namespace Neo.Bpms.Domain.Extensions;

public class NeoStringBuilder
{

    public NeoStringBuilder()
    {
        _stringBuilder = new StringBuilder();
    }
    public NeoStringBuilder(string initialValue)
    {
        _stringBuilder = new StringBuilder(initialValue);
    }

    private readonly StringBuilder _stringBuilder;
    public StringBuilder StringBuilder=> _stringBuilder;

    public NeoStringBuilder Append(string s)
    {
        _stringBuilder.Append(s);
        return this;
    }

    public NeoStringBuilder Append(NeoStringBuilder s)
    {
        _stringBuilder.Append(s.StringBuilder);
        return this;
    }

    public NeoStringBuilder Append(StringBuilder s)
    {
        _stringBuilder.Append(s);
        return this;
    }

    public static NeoStringBuilder operator +(NeoStringBuilder tsb, string s)
    {
        return tsb.Append(s);
    }

    public static NeoStringBuilder operator +(NeoStringBuilder tsb, NeoStringBuilder s)
    {
        return tsb.Append(s.StringBuilder);
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }

    public long Length => _stringBuilder.Length;
}
