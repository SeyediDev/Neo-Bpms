using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.SqlServer;

public class SqlServerDatabaseConnectionDefinition(IConfiguration configuration, string providerName) : AdoDotNetDatabaseConnectionDefinition(configuration, providerName)
{
    public override void Init()
    {
    }

    public string GetConnectionString(string tableName)
    {
        var timeOut = "120";
        if (!string.IsNullOrEmpty(tableName))
            timeOut = "" + (120 + Math.Abs(tableName.GetHashCode()) % 100);
        var connectionString = ConnectionString ??
            (string.IsNullOrEmpty(DatabaseName) ? "" : $"Database={DatabaseName};") +
            (string.IsNullOrEmpty(Server_DataSource) ? "" : $"Data Source={Server_DataSource};") +
            "Persist Security Info=false;" +
            //				(Encrypt ? "" : (@"Encrypt=true;")) +
            (MultipleActiveResultSets ? "" : @"MultipleActiveResultSets=true;") +
            (string.IsNullOrEmpty(FailoverPartner) ? "" : $"Failover Partner={FailoverPartner};") +
            (IntegratedSecurity
                ? @"Integrated Security=SSPI;"
                : (string.IsNullOrEmpty(UserName) ? "" : $"User ID={UserName};") +
                  (string.IsNullOrEmpty(Password) ? "" : $"Password={Password};")) +
            (string.IsNullOrEmpty(timeOut) ? "" : $"Timeout={timeOut};") +
            "";
        return connectionString;
    }
}
