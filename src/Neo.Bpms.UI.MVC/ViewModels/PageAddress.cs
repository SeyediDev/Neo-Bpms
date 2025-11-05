using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Neo.Bpms.UI.MVC.ViewModels;

public class PageAddress
{
    public enum PageTypeEnum
    {
        Form,
        Report,
        Dashboard
    }
    public PageAddress(CommonFormStructure structure)
    {
        NamespaceId = structure.NamespaceId;
        EntityId = structure.EntityId;
        PageId = structure.Form_ReportId;
        PageType = DeterminePageType(structure);
        FormSubjectId = structure.FormSubjectId;
    }

    private PageTypeEnum DeterminePageType(CommonFormStructure structure)
    {
        if (structure is ReportStructure)
            return PageTypeEnum.Report;
        return structure is DashboardStructure ? PageTypeEnum.Dashboard : PageTypeEnum.Form;
    }

    public string NamespaceId { get; set; }
    public string EntityId { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public PageTypeEnum PageType { get; set; }
    public string PageId { get; set; }
    public string FormSubjectId { get; set; }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}