using System.Data.Common;
using Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;
using Microsoft.Data.SqlClient;
using Neo.Bpms.Domain.Models.Base.Exceptions;
using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.SqlServer;

public class SqlServerDatabaseConnection(IDataProvider provider, ILogger logger,
    SqlServerDatabaseConnectionDefinition definition, string tableName) 
    : AdoDotNetDatabaseConnection(provider, definition)
{
    public string TableName { get; set; } = tableName;

    public new SqlServerDatabaseConnectionDefinition Definition =>
        base.Definition as SqlServerDatabaseConnectionDefinition;

    public override DbConnection GetNewConnection()
    {
        SqlConnection db = new(Definition.GetConnectionString(TableName));
        return db;
    }

    public override string Id => (dbConnection as SqlConnection)?.ClientConnectionId.ToString();

    public void SqlCon_InfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
        Exceptions ??= [];
        Exceptions.Add(CreateSqlException(logger, e.Message, e.Errors, e.Source));
        logger.LogInformation("SqlCon_InfoMessage {message} {source}", e.Message, e.Source);
    }

    public static DataLayerException CreateSqlException(ILogger logger, string sqlMessage, SqlErrorCollection errors, string source)
    {
        Dictionary<int, string> data = [];
        DataEngineException id = DataEngineException.None;
        string message = sqlMessage;
        foreach (object error in errors)
        {
            if (error is not SqlError)
            {
                continue;
            }

            SqlError sqlError = error as SqlError;
            if (!data.ContainsKey(sqlError.Number))
            {
                data.Add(sqlError.Number, sqlError.Message);
            }

            switch (sqlError.Number)
            {
                case 2601:
                    id = DataEngineException.UniqueIndex;
                    message = ParseUniqueIndexMessage(sqlError.Message);
                    break;
                case 3621:
                    break;
            }
            logger.LogInformation("SqlError {number} {message} {sqlError}", sqlError.Number, sqlError.Message, sqlError);
        }

        DataLayerException exc = new(id, message, sqlMessage)
        {
            Source = source,
        };
        foreach (KeyValuePair<int, string> d in data)
        {
            exc.Data.Add(d.Key, d.Value);
        }

        return exc;
    }

    private static string ParseUniqueIndexMessage(string sqlMessage)
    {
        const string specific = "The duplicate key value is ";
        return sqlMessage[(sqlMessage.IndexOf(specific, 0, StringComparison.Ordinal) + specific.Length)..];
    }
}
