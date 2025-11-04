namespace Neo.Bpms.UI.MVC.ViewModels;

public class FormLink
{
    public string name;
    public string namespaceId;
    public string entityId;
    public string Ids;

    public string action;
    public string formId;
    public string formSubjectId;

    public string process;
    public string task;
    public long? ai;

    public string getId()
    {
        return getId(namespaceId, entityId, formId, Ids, action);
    }
    public static string getId(string namespaceId, string entityId, string formId, string ids, string action)
    {
        return namespaceId + "$" + entityId + "$" + formId + "$" + ids + "$" + action;
    }
}
