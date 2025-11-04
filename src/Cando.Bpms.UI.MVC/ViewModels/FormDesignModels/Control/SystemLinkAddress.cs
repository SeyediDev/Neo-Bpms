namespace Neo.Bpms.UI.MVC.ViewModels.FormDesignModels.Control;

public class SystemLinkAddress : FormAddress
{
    public SystemLinkAddress()
    {

    }
    public SystemLinkAddress(string commaSeparatedForm)
    {
        string[] array = commaSeparatedForm.Split(',');
        if (array.Length > 0)
            NamespaceId = array[0];
        if (array.Length > 1)
            EntityId = array[1];
        if (array.Length > 2)
            FormId = array[2];
    }

    public bool IsValid => NamespaceId != null && EntityId != null && FormId != null;
    public string ToCommaSeparatedForm()
    {
        return $"{NamespaceId},{EntityId},{FormId}";
    }
}