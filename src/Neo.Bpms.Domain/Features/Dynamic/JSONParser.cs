namespace Neo.Bpms.Domain.Features.Dynamic;

/// <summary>
/// Parses a JSON to Elastic Object
/// </summary>
class JSONParser
{
    char[] charArray;
    int maxlen;
    /// <summary>
    /// if parser returns null, index returns the posiition of error
    /// </summary>
    int index;
    public string error = null;
    internal ElasticObject parse(string json, string name)
    {
        charArray = json != null ? json.ToCharArray() : new char[1];
        index = 0;
        maxlen = json?.Length ?? 0;
        object obj = lookForValue(name);
        if (obj is string)
        {
            obj = lookForValue(name);
        }
        return obj as ElasticObject;
    }

    private object lookForValue(string name)
    {
        //			ElasticObject result = new ElasticObject();
        for (; index < maxlen; index++)
        {
            char c = charArray[index];
            if (char.IsWhiteSpace(c)) continue;
            if (c == '[')
            {
                List<ElasticObject> col = [];
                for (index++; index < maxlen; index++)
                {
                    object e = lookForValue(name);
                    if (e is not ElasticObject) return null;
                    col.Add(e as ElasticObject);

                    for (; index < maxlen && char.IsWhiteSpace(charArray[index]); index++) ;
                    if (index >= maxlen) return null;
                    if (charArray[index] == ']')
                    {
                        index++;
                        break;
                    }
                    if (charArray[index] != ',')
                        return null;
                }
                return col;
            }
            if (c == '{')
            {
                ElasticObject elastic = new(name);
                for (; index < maxlen;)
                {
                    string name1 = lookForName();
                    if (name1 == null) return null;

                    for (; index < maxlen && char.IsWhiteSpace(charArray[index]); index++) ;

                    object e = lookForValue(name1);
                    if (e is ElasticObject)
                        elastic.SetField(name1, e as ElasticObject);
                    else if (e is List<ElasticObject>)
                    {
                        elastic.SetField(name1, e ?? "");
                        /*foreach (var item in e as List<ElasticObject>)
							{
								elastic.AddElement(item);
							}*/
                    }
                    else
                        elastic.SetField(name1, e ?? "");

                    while (index < maxlen && char.IsWhiteSpace(charArray[index])) index++;
                    if (index < maxlen)
                    {
                        if (charArray[index] == '}')
                        {
                            index++;
                            break;
                        }
                        if (charArray[index] == ']')
                        {
                            break;
                        }
                        if (charArray[index] != ',')
                        {
                            error = ", or } expected.";
                            index++;
                            return null;
                        }
                    }
                }
                return elastic;
            }
            /*else if (c == '\\')
				{
					if (charArray[index + 1] == '\"')
					{
						index++;
						continue;
					}
            }*/
            else if (c == '\"')
            {
                index++;
                string s = "";
                for (; index < maxlen; index++)
                {
                    if (charArray[index] == '\"')
                    {
                        index++;
                        return s;
                    }
                    if (charArray[index] == '\\')
                    {
                        index++;
                        if (charArray[index] == '\\')
                            s += '\\';
                        else if (charArray[index] == '\"')
                            s += '\"';
                        if (charArray[index] == '/')
                            s += '/';
                        if (charArray[index] == 'b')
                        {
                            if (s.Length > 0)
                                s.Remove(s.Length - 1);
                        }
                        if (charArray[index] == 'f')
                        {
                            s += '\f';
                        }
                        if (charArray[index] == 'r')
                        {
                            s += '\r';
                        }
                        if (charArray[index] == 'n')
                        {
                            s += '\n';
                        }
                        if (charArray[index] == 't')
                        {
                            s += '\t';
                        }
                        if (charArray[index] == 'u')
                        {
                            if (char.TryParse(new string(charArray, index, 5), out char c1))
                                s += c1;
                            index += 5;
                        }
                    }
                    else
                        s += charArray[index];
                }
                error = "\" for end of value expected.";
                return null;
            }
            else if (c == '\'')
            {
                index++;
                int idx2 = index;
                for (; index < maxlen; index++)
                {
                    if (charArray[index] == '\'')
                    {
                        string value = new(charArray, idx2, index - idx2 - 1);
                        index++;
                        return value;
                    }
                }
                error = "\" for end of value expected.";
                return null;
            }
            else if (char.IsDigit(c) || c == '-')
            {
                int idx2 = index;
                for (index++; index < maxlen && char.IsDigit(charArray[index]); index++) ;
                if (charArray[index] == '.')
                {
                    for (index++; index < maxlen && char.IsDigit(charArray[index]); index++) ;
                    if (charArray[index] == 'e' || charArray[index] == 'E')
                    {
                        for (index++; index < maxlen && char.IsDigit(charArray[index]); index++) ;
                    }
                    return double.Parse(new string(charArray, idx2, index - idx2));
                }
                if (charArray[index] == 'e' && charArray[index] == 'E')
                {
                    for (index++; index < maxlen && char.IsDigit(charArray[index]); index++) ;
                    return double.Parse(new string(charArray, idx2, index - idx2));
                }
                return int.Parse(new string(charArray, idx2, index - idx2));
            }
            else if (c == 't')
            {
                if (charArray[index + 1] == 'r' && charArray[index + 2] == 'u' && charArray[index + 3] == 'e')
                {
                    index += 4;
                    return true;
                }
            }
            else if (c == 'f')
            {
                if (charArray[index + 1] == 'a' && charArray[index + 2] == 'l' && charArray[index + 3] == 's' && charArray[index + 4] == 'e')
                {
                    index += 5;
                    return false;
                }
            }
            else if (c == 'n')
            {
                if (charArray[index + 1] == 'u' && charArray[index + 2] == 'l' && charArray[index + 3] == 'l')
                {
                    index += 4;
                    return null;
                }
            }
            error = "invalid character.";
            return null;
        }
        error = "no value.";
        return null;
    }

    private string lookForName()
    {
        for (; index < maxlen; index++)
        {
            char c = charArray[index];
            if (char.IsWhiteSpace(c)) continue;
            if (c == '\"')
            {
                for (int idx2 = index + 1; idx2 < maxlen; idx2++)
                {
                    if (charArray[idx2] == '\"')
                    {
                        string name = new(charArray, index + 1, idx2 - index - 1);
                        for (index = idx2 + 1; index < maxlen && char.IsWhiteSpace(charArray[index]); index++) ;
                        if (charArray[index] != ':')
                        {
                            error = ": expected.";
                            return null;
                        }
                        index++;
                        return name;
                    }
                }
                error = "\" for end of name expected.";
                return null;
            }
            else if (char.IsLetter(c) || c == '_')
            {
                for (int idx2 = index + 1; idx2 < maxlen; idx2++)
                {
                    if (!char.IsLetterOrDigit(charArray[idx2]))
                    {
                        string name = new(charArray, index, idx2 - index);
                        for (index = idx2; index < maxlen && char.IsWhiteSpace(charArray[index]); index++) ;
                        if (charArray[index] != ':')
                        {
                            error = ": expected.";
                            return null;
                        }
                        index++;
                        return name;
                    }
                }
                error = "end of name expected.";
                return null;
            }
        }
        error = "name expected.";
        return null;
    }
}
