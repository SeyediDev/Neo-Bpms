namespace Neo.Bpms.Infrastructure.Features.MetaLoader;

public class EntityMetaData : MetaData
{
    protected override void Init()
    {
        Namespaces = ToDictionaryByUnique<MetaModelNamespace>("MetaNamespaceId");
        Entities = ToDictionaryOfDictionary<MetaModelEntity>("MetaNamespaceId", "MetaEntityId");
        Fields = ToDictionaryOfDictionary<MetaModelField>("MetaEntityId", "MetaFieldId");
        Forms = ToDictionaryOfDictionary<MetaModelPage>("MetaEntityId", "PageId", $"PageTypeId=={MetaModelPageTypeId.Form:D}");
        Reports = ToDictionaryOfDictionary<MetaModelPage>("MetaEntityId", "PageId", $"PageTypeId=={MetaModelPageTypeId.Report:D}");
        Dashboards = ToDictionaryOfDictionary<MetaModelPage>("MetaEntityId", "PageId", $"PageTypeId=={MetaModelPageTypeId.Dashboard:D}");
    }

    public Dictionary<string, MetaModelNamespace> Namespaces { get; set; }
    public Dictionary<long, Dictionary<string, MetaModelEntity>> Entities { get; set; }
    public Dictionary<long, Dictionary<string, MetaModelField>> Fields { get; set; }
    public Dictionary<long, Dictionary<string, MetaModelPage>> Forms { get; set; }
    public Dictionary<long, Dictionary<string, MetaModelPage>> Reports { get; set; }
    public Dictionary<long, Dictionary<string, MetaModelPage>> Dashboards { get; set; }
}
