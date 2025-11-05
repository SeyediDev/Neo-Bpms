using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Neo.Bpms.Domain.Extensions;

public class XMLMap
{
    class Table
    {
        public string TableName = null;
        public string ColumnName = null;
    }

    static string XMLPath = "xmlpath";
    static string XML_hide_no_info = "xml_hide_no_info";

    public static bool FillHTMLByXML(string FilesPath, string HtmlFileName, string XMLString, string ResultFileName,
        ref string ErrorString)
    {
        try
        {
            StringBuilder Result = new();
            XDocument htmldoc = XDocument.Load(FilesPath + HtmlFileName);
            XmlDocument mXmlDocument = new();
            try
            {
                mXmlDocument.LoadXml(XMLString);
            }
            catch
            {
                throw new Exception("XML وارد شده دارای مشکل می‌باشد");
            }

            FillHTML(mXmlDocument.ChildNodes, htmldoc);

            //XmlWriter www;
            htmldoc.Save(FilesPath + "re\\" + ResultFileName);
            //.rea(www);
            ///XmlWriter.Create(
            //www.Close();
            //XmlReader xmlReader = htmldoc.CreateReader();

            //Result.Append(xmlReader.ReadOuterXml());
            //Result.Append(htmldoc.ToString());
            //StreamWriter sw = new StreamWriter(FilesPath + "re\\"+ResultFileName, false, Encoding.UTF8);
            //sw.Write(Result.ToString());
            //sw.Close();
        }
        catch (Exception ex)
        {
            ErrorString = ex.Message;
            return false;
        }

        return true;
    }

    private static void FillHTML(XmlNodeList mXmlNodeList, XDocument htmldoc)
    {
        XElement mBody = findBody(htmldoc.Nodes());
        if (mBody == null) return;
        foreach (XmlNode mXmlNode in mXmlNodeList)
        {
            FillElement(mXmlNode, mBody);
        }
    }

    private static void FillElement(XmlNode ParentXmlNode, XElement ParentXElement)
    {
        if (!ParentXElement.HasElements) return;
        for (int i = 0; i < ParentXElement.Elements().Count(); i++)
        {
            XElement mXElement = ParentXElement.Elements().ElementAt(i);
            int MaxLength = GetMaxLength(mXElement);
            string mXMLPath = GetXMLPath(mXElement);
            bool bHideNoInfo = HideNoInfo(mXElement);

            if (string.IsNullOrEmpty(mXMLPath))
            {
                FillElement(ParentXmlNode, mXElement);
                continue;
            }

            XmlNode mXmlNode = ParentXmlNode.SelectSingleNode(mXMLPath);
            string XMLElementType = GetXMLElementType(mXmlNode);
            if (XMLElementType == "qury")
            {
                XmlNodeList mXmlNodeList = ParentXmlNode.SelectNodes(mXMLPath);
                int ChildNodesLen = mXmlNodeList.Count;
                if (ChildNodesLen == 0)
                {
                    mXElement.Remove();
                    i--;
                }
                else
                {
                    for (int l = 1; l < ChildNodesLen; l++)
                    {
                        if (MaxLength != -100)
                            if (l + 1 > MaxLength)
                                break;
                        XElement newmXElement = new(mXElement);
                        XmlNode mSubXmlNode = mXmlNodeList[l]; //.sel.SelectSingleNode(mXMLPath);
                        FillElement(mSubXmlNode, newmXElement);
                        mXElement.Parent.Add(newmXElement);
                    }

                    XmlNode mFirstSubXmlNode = mXmlNodeList[0];
                    FillElement(mFirstSubXmlNode, mXElement);
                }

                i += ChildNodesLen - 1;
            }
            else if (XMLElementType == "struct")
            {
                if (mXmlNode != null)
                    FillElement(mXmlNode, mXElement);
            }
            else if (bHideNoInfo)
            {
                mXElement.Remove();
                i--;
            }
            else
            {
                if (mXmlNode != null)
                    mXElement.Value = XMLUtill.XML_GetInnerText(mXmlNode);
                //else FillElement(mXmlNode, mXElement);
            }
        }
    }

    private static string GetXMLElementType(XmlNode mXmlNode)
    {
        string outval = "txt";
        try
        {
            if (mXmlNode.Attributes["_x"] != null)
                outval = mXmlNode.Attributes["_x"].Value;
        }
        catch
        {
        }

        return outval;
    }

    private static bool HasChild(XmlNode mXmlNode)
    {
        if (!mXmlNode.HasChildNodes) return false;
        XmlNode mSubXmlNode = mXmlNode.ChildNodes[0];
        return mSubXmlNode.HasChildNodes;
        //else if(mSubXmlNode.HasChildNodes && mSubXmlNode.ChildNodes[0].InnerText != mSubXmlNode.InnerText)

    }

    private static string GetXMLPath(XElement mXElement)
    {
        if (!mXElement.HasAttributes) return null;
        foreach (XAttribute mXAttribute in mXElement.Attributes())
        {
            if (mXAttribute.Name.LocalName.ToLower() == XMLPath)
                return mXAttribute.Value;
        }

        return null;
    }

    private static bool HideNoInfo(XElement mXElement)
    {
        if (!mXElement.HasAttributes) return false;
        foreach (XAttribute mXAttribute in mXElement.Attributes())
        {
            if (mXAttribute.Name.LocalName.ToLower() == XML_hide_no_info)
                return true;
        }

        return false;
    }

    private static int GetMaxLength(XElement mXElement)
    {
        if (!mXElement.HasAttributes) return -100;
        foreach (XAttribute mXAttribute in mXElement.Attributes())
        {
            if (mXAttribute.Name.LocalName.ToLower() == "maxlength")
                return NumberUtill.ToInt(mXAttribute.Value);
        }

        return -100;
    }

    private static XElement findBody(IEnumerable<XNode> XNodes)
    {
        foreach (XNode mXNode in XNodes)
        {
            if (mXNode.NodeType != XmlNodeType.Element) continue;
            XElement mXElement = mXNode as XElement;
            if (mXElement.Name.LocalName.ToUpper() == "BODY")
                return mXElement;
            XElement mBodyXNode = findBody(mXElement.Nodes());
            if (mBodyXNode != null) return mBodyXNode;
        }

        return null;

    }
}
