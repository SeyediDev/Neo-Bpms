using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.ApplyUtilities;

public class BatchDataManipulationRunner : IBatchDataManipulationRunner
{
    private readonly Entity _entity;
    private LocalParameters _connectionValues;
    private IDataSource _dataSource;
    private IAuditTrail _auditTrail;

    public BatchDataManipulationRunner(Entity entity, LocalParameters connectionValues, IAuditTrail auditTrail)
    {
        _connectionValues = connectionValues;
        _entity = entity;
        if (_entity == null)
        {

        }
        _auditTrail = auditTrail;
    }

    public void SetConnectionDefinition(LocalParameters connectionValues)
    {
        _connectionValues = connectionValues;
    }

    public int RunCommand( /*bool withTransaction,*/ string sqlCommand)
    {
        /*				var command = "";
							if (withTransaction)
							{
								command += $@"BEGIN TRY 
				BEGIN TRANSACTION;
					{sqlCommand}
				COMMIT;
				END TRY 
				BEGIN CATCH 
					ROLLBACK;  
				END CATCH 
				";
							}
							else
								command = sqlCommand;
								*/
        _dataSource ??= DataSourceProviderManager.GetProvider(_entity)
                .GetDataSource(_entity, _connectionValues, _auditTrail);
        _dataSource.Command(sqlCommand);
        return _dataSource.RecordsAffected;
    }

    public void Release()
    {
        _dataSource?.Release();
        _dataSource?.Close();
        _dataSource = null;
    }
}
