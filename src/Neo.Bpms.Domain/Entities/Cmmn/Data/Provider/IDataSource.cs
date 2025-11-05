global using Neo.Bpms.Domain.Features.Dynamic;

namespace Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;

public interface IDataSource
{
    CancellationToken CancellationToken { get; set; }
    int RecordsAffected { get; set; }

    bool Close();
    void Release();
    List<Exception> Exceptions { get; set; }
    bool Command(string sqlCommand, List<object> parameters = null);
    bool Select(string querySql);
    IEnumerable<ElasticObject> GetRecords();
}
