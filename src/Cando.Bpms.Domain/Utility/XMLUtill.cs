using System.Data;
using System.Xml;

namespace Neo.Bpms.Domain.Utility;

public class XMLUtill
{
    public static DataSet XMLToDataSet(string strXML, string SchimaDataSeperator)
    {
        string strSchima = strXML[..strXML.IndexOf(SchimaDataSeperator)];
        string strData = strXML[(strXML.IndexOf(SchimaDataSeperator) + SchimaDataSeperator.Length)..];

        TextReader textReaderSchima = new StringReader(strSchima);
        TextReader textReaderData = new StringReader(strData);
        DataSet ds = new();
        ds.ReadXmlSchema(textReaderSchima);
        ds.ReadXml(textReaderData);
        return ds;
    }

    public static string DataSetToXML(DataSet ds, string SchimaDataSeperator)
    {
        if (string.IsNullOrEmpty(ds.DataSetName))
            ds.DataSetName = "UnknownDataSetName";

        StringWriter TextwriterData = new();
        StringWriter TextwriterSchima = new();

        ds.WriteXmlSchema(TextwriterSchima);
        ds.WriteXml(TextwriterData);
        return TextwriterSchima.ToString() + SchimaDataSeperator + TextwriterData.ToString();
    }

    public static DataTable XMLToDataTable(string strXML, string SchimaDataSeperator)
    {
        string StrSchima = strXML[..strXML.IndexOf(SchimaDataSeperator)];
        string StrData = strXML[(strXML.IndexOf(SchimaDataSeperator) + SchimaDataSeperator.Length)..];

        TextReader TextReaderSchima = new StringReader(StrSchima);
        TextReader TextReaderData = new StringReader(StrData);
        DataTable dt = new();
        dt.ReadXmlSchema(TextReaderSchima);
        dt.ReadXml(TextReaderData);
        return dt;
    }

    public static string DataTableToXML(DataTable dt, string SchimaDataSeperator)
    {
        //"<&LID:SEP&>" 
        if (string.IsNullOrEmpty(dt.TableName))
            dt.TableName = "UnknownTableName";

        StringWriter TextwriterData = new();
        StringWriter TextwriterSchima = new();
        dt.WriteXmlSchema(TextwriterSchima, true);
        dt.WriteXml(TextwriterData, true);
        return TextwriterSchima + SchimaDataSeperator + TextwriterData;
    }

    public static string XML_GetInnerText(XmlNode node)
    {
        try
        {
            return node.InnerText;
        }
        catch
        {
            return "";
        }
    }

    public static string XML_GetInnerText(XmlNode node, string SingleNodeName, string defVal)
    {
        if (string.IsNullOrEmpty(SingleNodeName))
        {
            try
            {
                return node.InnerText;
            }
            catch
            {
                return defVal;
            }
        }
        else
        {
            try
            {
                return node.SelectSingleNode(SingleNodeName).InnerText;
            }
            catch
            {
                return defVal;
            }
        }
    }

    public static int XML_GetInnerText(XmlNode node, string SingleNodeName, int defVal)
    {
        if (string.IsNullOrEmpty(SingleNodeName))
        {
            try
            {
                return NumberUtill.ToInt(node.InnerText);
            }
            catch
            {
                return defVal;
            }
        }
        else
        {
            try
            {
                return NumberUtill.ToInt(node.SelectSingleNode(SingleNodeName).InnerText);
            }
            catch
            {
                return defVal;
            }
        }
    }

    public static int ReadAttribute(XmlNode node, string AttributeName, int defval)
    {
        try
        {
            return NumberUtill.ToInt(node.Attributes[AttributeName].Value);
        }
        catch
        {
            return defval;
        }
    }

    public static string ReadAttribute(XmlNode node, string AttributeName, string defval)
    {
        try
        {
            return node.Attributes[AttributeName].Value;
        }
        catch
        {
            return defval;
        }
    }

    public static string GetXmlFromObject<T>(T o)
    {
        StringWriter sw = new();
        using (XmlTextWriter tw = new(sw))
        {
            try
            {
                XmlSerializer serializer = new(o.GetType());
                serializer.Serialize(tw, o);
            }
            catch
            {
                throw;
            }
            finally
            {
                sw.Close();
                tw.Close();
            }
        }

        return sw.ToString();
    }

    public static T GetObjectFromXmlFile<T>(string fileAddress) where T : class
    {
        if (!File.Exists(fileAddress))
            return null;
        FileInfo fi = new(fileAddress);
        if (fi.Length == 0)
            return null;
        using (StreamReader sr = new(fileAddress))
        {
            string xmlText = sr.ReadToEnd();
            XmlReader xmlReader = XmlReader.Create(new StringReader(xmlText));
            return GetObjectFromXml<T>(xmlReader);
        }
    }

    public static T GetObjectFromXml<T>(XmlReader xmlReader) where T : class
    {
        object obj = null;
        try
        {
            Type serializerType = typeof(T);
            bool canDeserialize = true;
            if (!string.IsNullOrEmpty(xmlReader.Name) && serializerType.Name != xmlReader.Name)
            {
                Type subType = Assembly.GetAssembly(serializerType)
                    .GetTypes().FirstOrDefault(myType =>
                        myType.IsClass && myType.IsSubclassOf(serializerType) && myType.Name == xmlReader.Name);
                if (subType == null)
                {
                    //Logger.LogCritical("type {0} not in assemly of type {1} ({2})", xmlReader.Name, serializerType.Name,
                    //	serializerType.Assembly.FullName);
                    canDeserialize = false;
                }
                else
                    serializerType = subType;
            }

            if (canDeserialize)
            {
                XmlSerializer serializer = new(serializerType);
                obj = serializer.Deserialize(xmlReader);
            }
        }
        catch //(Exception e)
        {
            //Logger.LogCritical(e,e.Message);
        }
        finally
        {
            xmlReader?.Close();
        }

        return obj is T t ? t : default;
    }
}
