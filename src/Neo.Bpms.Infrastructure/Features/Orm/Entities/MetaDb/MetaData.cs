using Neo.Bpms.Domain.Entities.Base;
using Neo.Bpms.Domain.Modeling.Entities.Cmmn;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.QueryUtilities;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.MetaDb;
public abstract class MetaData : BaseMetaData
{
    protected string Filter { get; set; }
    protected PublicEntityStateId StateId { get; set; }

    public bool Init(bool bActive)
    {
        StateId = bActive ? PublicEntityStateId.Active : PublicEntityStateId.Backup;
        Filter = $"IsNull(StateId,{(long)PublicEntityStateId.Backup})=={(long)StateId}";
        Init();
        return !HasNotExistsTable;
    }

    protected abstract void Init();

    protected Dictionary<string, T> ToDictionaryByUnique<T>(string keyFieldId)
        where T : class, IStateBasedEntity, new()
    {
        try
        {
            if (!CheckEntity<T>())
            {
                return [];
            }

            Dictionary<string, T> ret = QueryUtility<T>.ToDictionaryByUnique(Filter, keyFieldId);
            InitData(ret?.Values);
            return ret;
        }
        catch (Exception e)
        {
            AddError(typeof(T).Name, $"In Load {typeof(T).Name}", e);
            return [];
        }
    }

    public Dictionary<long, Dictionary<string, T>> ToDictionaryOfDictionary<T>(string keyFieldId1, string keyFieldId2,
        string filter = null)
        where T : class, IStateBasedEntity, new()
    {
        try
        {
            if (!CheckEntity<T>())
            {
                return [];
            }

            Dictionary<long, Dictionary<string, T>> ret = QueryUtility<T>
                .ToDictionaryOfDictionary(Filter + (string.IsNullOrEmpty(filter) ? "" : $" And ({filter})"),
                    keyFieldId1, keyFieldId2);
            InitData(ret?.Values.SelectMany(i => i?.Values));
            return ret;
        }
        catch (Exception e)
        {
            AddError(typeof(T).Name, $"In Load {typeof(T).Name}", e);
            return [];
        }
    }

    private bool CheckEntity<T>() where T : class, IStateBasedEntity, new()
    {
        Entity entity = ProjectDefinition.Project.GetEntity<T>();
        if (entity == null)
        {
            AddError($"In Load {typeof(T).Name}", $"Can not find entity {typeof(T).Name}");
            HasNotExistsTable = true;
            return false;
        }

        QueryUtility q = QueryUtility<DatabaseTables>.Where($"{nameof(DatabaseTables.Name)}=='{EntityDbNameManager.GetDbTableName(entity)}'");
        if (q.FirstOrDefault<DatabaseTables>() == null)
        {
            AddError($"In Load {typeof(T).Name}", $"Can not find table {typeof(T).Name}");
            HasNotExistsTable = true;
            return false;
        }

        return true;
    }

    private void InitData<T>(IEnumerable<T> list) where T : class, IStateBasedEntity, new()
    {
        if (StateId == PublicEntityStateId.Active && list != null)
        {
            foreach (T item in list)
            {
                item.StateId = (long)PublicEntityStateId.WaitForBackup;
            }
        }
    }
}
