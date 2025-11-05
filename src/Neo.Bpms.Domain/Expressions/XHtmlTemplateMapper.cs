using System.Xml;
using System.Xml.Linq;

namespace Neo.Bpms.Domain.Expressions;

public class XHtmlTemplateMapper(Stream template, object data)
{
    public string ErrorString;
    private readonly object _data = data;
    private readonly Stream _template = template;
    private XDocument _html;
    public override string ToString() => _html?.ToString() ?? "";

    public bool Render()
    {
        if (!LoadXmlTemplate())
            return false;
        if (_data == null)
            return true;
        XElement body = FindBody(_html.Root, _html.Nodes(), out XElement parent);
        Dictionary<string, MemberInfo> members = FetchMembers(_data);
        FillElement(parent, body, _data, members, 0);

        return true;
    }

    private bool LoadXmlTemplate()
    {
        try
        {
            _html = XDocument.Load(_template);
        }
        catch (Exception e)
        {
            ErrorString = e.ToString();
            return false;
        }

        return true;
    }

    private static void FillElement(XContainer parentElement, XElement xElement, object data,
        IReadOnlyDictionary<string, MemberInfo> members, int rowIndex)
    {
        if (GetAttribute("_field", xElement, data, members, rowIndex, out object value))
        {
            string fieldValue = value?.ToString();
            if (!string.IsNullOrEmpty(fieldValue))
                xElement.Value = fieldValue;
        }
        else if (GetAttribute("_fieldValue", xElement, data, members, rowIndex, out value))
        {
            string fieldValue = value?.ToString() ?? "";
            XAttribute attribute = GetAttribute("value", xElement);
            if (attribute != null)
                attribute.Value = fieldValue;
            else
                xElement.Add(new XAttribute("value", fieldValue));
        }
        else if (GetAttribute("_list", xElement, data, members, rowIndex, out object list))
        {
            XAttribute att = GetAttribute("_list", xElement);
            att.Value = "";
            IList<object> records = ReflectionTools.GetEnumerableValue(list);
            IReadOnlyDictionary<string, MemberInfo> subElements = null;
            int recordCount = 0;
            if (records != null)
            {
                foreach (object record in records)
                    FillElementsOfRecord(parentElement, xElement, ref subElements, record, recordCount++);
            }

            if (GetAttribute("_minRecordCount", xElement, data, members, rowIndex, out object oMinRecordCount))
            {
                int minRecordCount = Convert.ToInt32(oMinRecordCount);
                for (; recordCount < minRecordCount;)
                    FillElementsOfRecord(parentElement, xElement, ref subElements, new object(), recordCount++);
            }
        }
        else
        {
            IReadOnlyDictionary<string, MemberInfo> subElements = members;
            object subData = data;
            if (GetAttribute("_struct", xElement, data, members, rowIndex, out object structValue))
            {
                subData = structValue;
                subElements = FetchMembers(subData);
            }

            foreach (XElement element in xElement.Elements())
                FillElement(xElement, element, subData, subElements, rowIndex);
        }
    }

    private static void FillElementsOfRecord(XContainer parentElement, XElement xElement,
        ref IReadOnlyDictionary<string, MemberInfo> subElements, object record, int rowIndex)
    {
        XElement recordElement = xElement;
        if (subElements == null)
            subElements = FetchMembers(record);
        else
        {
            recordElement = XElement.Parse(xElement.ToString());
            parentElement.Add(recordElement);
        }

        foreach (XElement element in recordElement.Elements())
            FillElement(recordElement, element, record, subElements, rowIndex + 1);
    }

    private static bool GetAttribute(string name, XElement xElement, object data,
        IReadOnlyDictionary<string, MemberInfo> members, int rowIndex, out object value)
    {
        value = null;
        if (!xElement.HasAttributes)
            return false;
        XAttribute attribute = GetAttribute(name, xElement);
        if (attribute == null)
            return false;
        if (attribute.Value == "_RowIndex")
        {
            value = rowIndex;
            return true;
        }
        if (data is not IExpressionValue e)
        {
            if (!members.TryGetValue(attribute.Value, out MemberInfo member))
                return false;
            value = ReflectionField.GetValue(data, member);
        }
        else
            e.GetField(attribute.Value, out value);

        return true;
    }

    private static XAttribute GetAttribute(string name, XElement xElement)
    {
        name = name.ToLower();
        return xElement.Attributes().FirstOrDefault(a => a.Name.LocalName.ToLower() == name);
    }

    private static Dictionary<string, MemberInfo> FetchMembers(object data)
    {
        return ReflectionField.Members(data.GetType()).ToDictionary(i => i.Name);
    }

    private static XElement FindBody(XElement parent, IEnumerable<XNode> nodes, out XElement bodyParent)
    {
        foreach (XNode node in nodes)
        {
            if (node.NodeType != XmlNodeType.Element)
                continue;
            XElement xElement = node as XElement;
            if (xElement == null)
                continue;
            if (xElement.Name.LocalName.ToUpper() == "BODY")
            {
                bodyParent = parent;
                return xElement;
            }

            XElement body = FindBody(parent, xElement.Nodes(), out bodyParent);
            if (body != null)
                return body;
        }

        bodyParent = null;
        return null;
    }
}