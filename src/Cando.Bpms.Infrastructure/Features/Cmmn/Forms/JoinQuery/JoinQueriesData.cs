using Neo.Bpms.Domain.Entities.Cmmn.Relationship;

namespace Neo.Bpms.Infrastructure.Features.Cmmn.Forms.JoinQuery;

public class JoinQueriesData : ConcurrentDictionary<string, JoinQueryData>
{
    public JoinQueriesData(string mainQueryId)
    {
        MainQueryId = Guid.NewGuid().ToString();
    }
    public JoinQueriesData()
    {
        MainQueryId = Guid.NewGuid().ToString();
    }

    public string MainQueryId { get; set; }

    public bool TryGetValue(Association association, out JoinQueryData joinQueryData)
    {
        lock (string.Intern(MainQueryId))
        {
            string key = JoinQueryData.GetKey(association);
            return TryGetValue(key, out joinQueryData);
        }
    }

    public bool TryAdd(Association association, List<string> displayFields)
    {
        string key = JoinQueryData.GetKey(association);
        return !ContainsKey(key) && TryAdd(key, new JoinQueryData(association, MainQueryId, displayFields));
    }

    public ICollection<JoinQueryData> List
    {
        get
        {
            lock (string.Intern(MainQueryId))
            {
                return Values;
            }
        }
    }
}
