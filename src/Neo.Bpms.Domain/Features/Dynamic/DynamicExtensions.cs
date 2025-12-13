using System.Text;
using System.Xml.Linq;

namespace Neo.Bpms.Domain.Features.Dynamic;

/// <summary>
/// Extension methods for our ElasticObject. 
/// See http://amazedsaint.blogspot.com/2010/02/introducing-elasticobject-for-net-40.html for details
/// </summary>
public static class DynamicExtensions
{
    public static LocalParameters Set(this LocalParameters lc, ElasticObject elasticObject)
    {
        if (elasticObject?.Attributes == null)
        {
            return lc;
        }

        foreach (KeyValuePair<string, ElasticObject> item in elasticObject.Attributes)
        {
            _ = lc.AddOrUpdate(item.Key, item.Value);
        }

        return lc;
    }

    public static ElasticObject ToElastic(this LocalParameters lc)
    {
        ElasticObject elasticObject = new();
        elasticObject.Merge(lc);
        return elasticObject;
    }
    public static ElasticObject DeepToElastic(this LocalParameters lc)
    {
        ElasticObject elasticObject = new();
        elasticObject.DeepMerge(lc);
        return elasticObject;
    }
    public static ElasticObject ToElastic(this Dictionary<string, string> dic)
    {
        ElasticObject elasticObject = new();
        foreach (KeyValuePair<string, string> keyValuePair in dic)
        {
            _ = elasticObject.SetField(keyValuePair.Key, keyValuePair.Value);
        }
        return elasticObject;
    }

    /// <summary>
    /// Converts an XElement to the expando
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static ElasticObject ToElastic(this XElement e)
    {
        return ElasticFromXElement(e);
    }

    /// <summary>
    /// Converts an expando to XElement
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static XElement ToXElement(this ElasticObject e)
    {
        return XElementFromElastic(e);
    }

    public static ElasticObject FromJSON(this string json)
    {
        return ElasticFromJson(json);
    }

    /// <summary>
    /// Converts an expando to XElement
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static string ToJSON(this ElasticObject e)
    {
        return JsonFromElastic(0, e);
    }

    /// <summary>
    /// Build an expando from an XElement
    /// </summary>
    /// <param name="el"></param>
    /// <returns></returns>
    private static ElasticObject ElasticFromXElement(XElement el)
    {
        ElasticObject exp = new();
        if (!string.IsNullOrEmpty(el.Value))
        {
            exp.InternalValue = el.Value;
        }

        exp.InternalName = el.Name.LocalName;
        exp.Namespace = el.Name.Namespace;
        foreach (XAttribute a in el.Attributes())
        {
            _ = exp.CreateOrGetAttribute(a.Name.LocalName, a.Value);
        }

        XNode textNode = el.Nodes().FirstOrDefault();
        if (textNode is XText)
        {
            exp.InternalValue = textNode.ToString();
        }

        foreach (XElement c in el.Elements())
        {
            ElasticObject child = ElasticFromXElement(c);
            child.InternalParent = exp;
            exp.AddElement(child);
        }

        return exp;
    }

    /// <summary>
    /// Returns an XElement from an ElasticObject
    /// </summary>
    /// <param name="elastic"></param>
    /// <returns></returns>
    private static XElement XElementFromElastic(ElasticObject elastic)
    {
        XElement exp = null;
        if (elastic?.Namespace != null)
        {
            if (elastic.Namespace is string)
            {
                XNamespace ns = elastic.Namespace.ToString();
                exp = new XElement(ns + elastic.InternalName);
            }
            else if (elastic.Namespace is XNamespace)
            {
                exp = new XElement((XNamespace)elastic.Namespace + elastic.InternalName);
            }
        }

        exp ??= new XElement(elastic?.InternalName ?? "");
        if (elastic == null)
        {
            return exp;
        }

        foreach (KeyValuePair<string, ElasticObject> a in elastic.Attributes ?? Enumerable.Empty<KeyValuePair<string, ElasticObject>>())
        {
            if (a.Value.InternalValue == null)
            {
                continue;
            }

            if (a.Value.InternalValue is XAttribute)
            {
                exp.Add(a.Value.InternalValue);
            }
            else
            {
                XName key = a.Key;
                if (a.Value.InternalValue is XNamespace && a.Key != "xmlns")
                {
                    key = XNamespace.Xmlns + a.Key;
                }
                else
                {
                    object n = a.Value.Namespace;
                    if (n != null)
                    {
                        if (n is string)
                        {
                            XNamespace ns = n as string;
                            key = ns + a.Key;
                        }

                        if (n is XNamespace)
                        {
                            key = (n as XNamespace) + a.Key;
                        }
                    }
                }

                exp.Add(new XAttribute(key, a.Value.InternalValue));
            }
        }

        if (elastic.InternalValue is string)
        {
            if (null != elastic.InternalValue)
            {
                exp.Add(new XText(elastic.InternalValue as string));
            }
        }
        else if (elastic.InternalValue is XCData or XComment or
                 XContainer or
                 XElement or XNode or
                 XObject or
                 XStreamingElement or XText or
                 XStreamingElement or
                 XDocument)
        {
            exp.Add(elastic.InternalValue);
        }
        else if (elastic.InternalValue != null)
        {
            exp.Add(new XText(elastic.InternalValue.ToString()));
        }

        foreach (ElasticObject c in elastic.Elements)
        {
            XElement child = XElementFromElastic(c);
            exp.Add(child);
        }

        return exp;
    }

    /// <summary>
    /// return Elastics from json.
    /// </summary>
    /// <param name="json">The json.</param>
    /// <returns></returns>
    private static ElasticObject ElasticFromJson(string json)
    {
        JSONParser parser = new();
        return parser.parse(json, "") ?? new ElasticObject();
    }

    /// <summary>
    /// return Json string from elastic.
    /// </summary>
    /// <param name="indent">The indent.</param>
    /// <param name="elastic">The elastic.</param>
    /// <returns></returns>
    private static string JsonFromElastic(int indent, ElasticObject elastic)
    {
        StringBuilder s = new();
        if (elastic.InternalValue != null)
        {
            if (elastic.InternalValue is ElasticObject)
            {
                _ = s.Append(JsonFromElastic(indent, elastic.InternalValue as ElasticObject));
            }
            else
            {
                s.Append("\"" + elastic.InternalName + "\": " + elastic.InternalValue.ToString());
                //_ = s.Append(elastic.InternalValue);
            }
        }
        else
        {
            bool firstline = true;
            if (!elastic.Attributes.Any() && elastic.ElementCollections.Count == 1)
            {
                KeyValuePair<string, List<ElasticObject>> item = elastic.ElementCollections.FirstOrDefault(); //if (item.Key == elastic.InternalName) { 
                _ = s.Append('[');
                indent++;
                bool firstline2 = true;
                foreach (ElasticObject c in item.Value)
                {
                    if (!firstline2)
                    {
                        _ = s.Append(',');
                    }
                    //s.Append('\t', indent);
                    firstline2 = false;
                    _ = s.Append(JsonFromElastic(indent, c));
                }

                _ = s.Append(']');
            }
            else
            {
                _ = s.Append('{');
                indent++;
                foreach (KeyValuePair<string, ElasticObject> a in elastic.Attributes)
                {
                    if (a.Value.InternalValue == null)
                    {
                        continue;
                    }

                    if (!firstline)
                    {
                        _ = s.Append(',');
                    }
                    //s.Append('\t', indent);
                    firstline = false;
                    if (a.Value.InternalValue is ElasticObject)
                    {
                        _ = s.Append(JsonFromElastic(indent, a.Value.InternalValue as ElasticObject));
                    }
                    else if (a.Value.InternalValue is string)
                    {
                        _ = s.Append("\"" + a.Key + "\": \"" + a.Value.InternalValue + "\"");
                    }
                    else
                    {
                        _ = s.Append("\"" + a.Key + "\": " + a.Value.InternalValue);
                    }
                }

                foreach (KeyValuePair<string, List<ElasticObject>> item in elastic.ElementCollections)
                {
                    if (!firstline)
                    {
                        _ = s.Append(',');
                    }
                    //s.Append('\t', indent);
                    firstline = false;
                    _ = s.Append(item.Key + ": [");
                    indent++;
                    bool firstline2 = true;
                    foreach (ElasticObject c in item.Value)
                    {
                        if (!firstline2)
                        {
                            _ = s.Append(',');
                        }
                        //s.Append('\t', indent);
                        firstline2 = false;
                        _ = s.Append(JsonFromElastic(indent, c));
                    }

                    indent++;
                    _ = s.Append(']');
                }

                _ = s.Append('}');
            }
        }

        return s.ToString();
    }
}
