using Neo.Bpms.Domain.Entities.Base.Audit;
using Neo.Bpms.Domain.Entities.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Entities.Cmmn.Data.DML;
using Neo.Bpms.Domain.Entities.Cmmn.Data.Query;
using Neo.Bpms.Domain.Entities.Cmmn.Entities;
using Microsoft.Extensions.Configuration;

namespace Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;

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
