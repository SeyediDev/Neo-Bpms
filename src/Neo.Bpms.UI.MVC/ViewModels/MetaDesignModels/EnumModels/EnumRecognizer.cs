namespace Neo.Bpms.UI.MVC.ViewModels.MetaDesignModels.EnumModels;

public class EnumRecognizer
{
    public EnumRecognizer()
    {

    }

    public EnumRecognizer(Enumeration @enum)
    {
        id = @enum.Id;
        name = @enum.Name;
        enName = @enum.EnName;
        namespaceId = @enum.NamespaceId;
    }
    public string id { get; set; }
    public string name { get; set; }
    public string enName { get; set; }
    public string namespaceId { get; set; }
}
