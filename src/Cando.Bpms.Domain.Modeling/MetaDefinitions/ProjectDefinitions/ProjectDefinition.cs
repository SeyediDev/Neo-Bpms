using Neo.Bpms.Domain.Entities.Cmmn.Fields;
using Neo.Bpms.Domain.Entities.Cmmn.UI;

namespace Neo.Bpms.Domain.Modeling.MetaDefinitions.ProjectDefinitions;

public abstract class ProjectDefinition
{
    public static ProjectContext Project { get; set; }
    public static ConcurrentDictionary<long, Entity> Entities { get; } =
        new ConcurrentDictionary<long, Entity>();
    public static ConcurrentDictionary<long, Form> Forms { get; } =
        new ConcurrentDictionary<long, Form>();
    public static ConcurrentDictionary<long, Report> Reports { get; } =
        new ConcurrentDictionary<long, Report>();
    public static ConcurrentDictionary<long, Dashboard> Dashboards { get; } =
        new ConcurrentDictionary<long, Dashboard>();
    public static ConcurrentDictionary<long, EntityField> Fields { get; } =
        new ConcurrentDictionary<long, EntityField>();

    public static Form GerForm(string formId, string formEntityId, string formNamespaceId = null)
    {
        if (Forms is null || Forms.IsEmpty)
        {
            return null;
        }

        List<Form> forms = Forms.Values.Where(f => f.Id == formId && f.EntityId == formEntityId).ToList();
        if (forms.Count == 0)
        {
            return null;
        }

        if (forms.Count == 1)
        {
            return forms.FirstOrDefault();
        }

        if (string.IsNullOrEmpty(formNamespaceId))
        {
            return null;
        }

        forms = [.. forms.Where(f => f.NamespaceId == formNamespaceId)];
        if (forms.Count == 0)
        {
            return null;
        }

        return forms.Count == 1 ? forms.FirstOrDefault() : throw new Exception();
    }
    public static bool SaveLoadedMeta { get; set; }
    public static bool DontSyncMetaData { get; set; }
}
