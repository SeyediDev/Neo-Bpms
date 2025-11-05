using Neo.Bpms.Domain.Models.Base.Audit;
using Neo.Bpms.Domain.Models.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Models.Cmmn.Data.DML;
using Neo.Bpms.Domain.Models.Cmmn.Data.Query;

namespace Neo.Bpms.Domain.Models.Cmmn.Data.Provider;

public interface IDataProvider : IDisposable
{
    CancellationToken CancellationToken { get; set; }
    string Name { get; }
    bool DontHaveDataDictionary { get; }
    bool DontSync { get; }

    IDataSource GetDataSource(Entity entity, LocalParameters connectionParameters, IAuditTrail auditTrail);
    IDDLGenerator GetDDLGenerator();
    IDMLGenerator GetDMLGenerator();
    IQueryGenerator GetQueryGenerator();
    IDDLManager GetDdlManager(IDataProvider provider, IConfiguration configuration, LocalParameters connectionParameters);
    void Init();
    void SetConnectionParams(LocalParameters connectionParameters, LocalParameters localParameters);
    void SetRecordConnectionParams(LocalParameters connectionParameters, string recordId);
}
