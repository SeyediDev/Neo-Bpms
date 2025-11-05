using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.JoinQuery;

public class JoinQueryData
{
    public JoinQueryData(Association association, string mainQueryId, List<string> displayFields)
    {
        Association = association;
        MainQueryId = mainQueryId;
        DisplayFields = displayFields;
        _refers = new ConcurrentDictionary<string, ReferList>();
    }

    public Association Association { get; }
    public string MainQueryId { get; set; }
    public List<string> DisplayFields { get; }

    public static string GetKey(Association association) =>
        $"{association.DestNamespaceId}-{association.DestEntityId}-{(association.Maps != null ? string.Join("-", association.Maps?.Select(m => m.SourceField)) : "-")}";

    private readonly ConcurrentDictionary<string, ReferList> _refers;

    public ConcurrentDictionary<string, ReferList> Refers
    {
        get
        {
            lock (string.Intern(MainQueryId))
            {
                return _refers;
            }
        }
    }

    public void Add(string key, string fieldId, ElasticObject record)
    {
        lock (string.Intern(MainQueryId))
        {
            if (!_refers.TryGetValue(key, out ReferList list))
                _refers.TryAdd(key, list = []);
            list.Add(new ReferData(fieldId)
            {
                Data = record
            });
        }
    }

    public void SafeDo(Action action)
    {
        lock (string.Intern(MainQueryId))
        {
            action();
        }
    }
}
