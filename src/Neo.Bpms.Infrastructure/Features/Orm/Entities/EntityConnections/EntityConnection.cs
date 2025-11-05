using Neo.Bpms.Domain.Entities.Cmmn.Data.Provider;

namespace Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

public abstract class EntityConnection(Entity entity, AuditTrail auditTrail, string name) : IExpressionReference
{
    public CancellationToken CancellationToken { get; set; } = new CancellationToken(false);
    public string Name { get; } = name;
    public Entity Entity { get; set; } = entity;
    public EntityConnection Parent { get; set; }
    public string CommandTxt { get; set; }
    public string ErrorText { get; set; }

    public IDataProvider Provider { get; } = DataSourceProviderManager.GetProvider(entity);
    protected DataSource DataSource => DataSourceInstance as DataSource;

    protected IDataSource DataSourceInstance => _dataSource ??= GetDataSource();

    public string DatabaseName => DataSource?.DataSrcDefinition?.connection?.DatabaseName;

    protected IDataSource _dataSource;
    public static ILogger Logger => DependencyInjectionHolder.Instance.Logger;
    public AuditTrail AuditTrail { get; set; } = auditTrail;
    public ExceptionInfos Errors { get; set; }
    public readonly LocalParameters ConnectionValues = [];

    public void SetConnectionValue(string key, object value)
    {
        _ = ConnectionValues.AddOrUpdate(key, value);
        ReleaseConnection();
    }

    public virtual void Release()
    {
        AppendErrors();
        if (string.IsNullOrEmpty(ErrorText) && Errors != null && Errors.Any())
        {
            ErrorText += string.Join("\n,", Errors.Select(e => e.Exception?.Message));
        }

        ReleaseConnection();
    }

    protected void AppendErrors(IList<ExceptionInfo> newErrors)
    {
        if (newErrors != null && newErrors.Any())
        {
            Errors ??= [];
            Errors.AddRange(newErrors);
        }
    }

    protected virtual void ReleaseConnection()
    {
        CloseDataSource();
    }

    protected void CloseDataSource()
    {
        ReleaseDataSource();
        try
        {
            _dataSource?.Close();
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
        }

        _dataSource = null;
    }

    protected void ReleaseDataSource()
    {
        try
        {
            _dataSource?.Release();
        }
        catch (Exception e)
        {
            Logger.LogError(e, e.Message);
        }
    }

    private void AppendErrors()
    {
        if (_dataSource?.Exceptions == null)
        {
            return;
        }

        AppendErrors([.. _dataSource.Exceptions.Select(exc =>
                new ExceptionInfo
                {
                    ForField = null,
                    Exception = exc
                })]);
    }

    private IDataSource GetDataSource()
    {
        var dataSource = Provider.GetDataSource(Entity, ConnectionValues, AuditTrail);
        dataSource.CancellationToken = CancellationToken;
        return dataSource;
    }
}
