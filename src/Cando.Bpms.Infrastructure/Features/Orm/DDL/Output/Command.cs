namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager
{
    protected IEnumerable<ElasticObject> Select(string sql, string errorCode = "", Entity entity = null)
    {
        if (Provider.GetDataSource(entity ?? _firstEntityInThisProvider, ConnectionParameters, null) is not DataSource dataSource)
            yield break;
        dataSource.SetQueryCommand(sql);
        if (!dataSource.openForRead())
            yield break;
        foreach (var record in dataSource.GetRecords())
            yield return record;

        if (dataSource.Exceptions != null && dataSource.Exceptions.Count > 0)
        {
            AddLog(errorCode, Log.Error, sql);
            AddLog(errorCode, Log.Error,
                string.Join("\r\n",
                    dataSource.Exceptions.Select(e =>
                    {
                        if (e is EngineException ee)
                            return ee.info?.ErrorText ?? e.ToString();
                        return e.Message ?? e.ToString();
                    })));
        }

        dataSource.Close();
    }

    protected IEnumerable<T> Select<T>(string sql, string errorCode = "", Entity entity = null)
        where T : new()
    {
        if (Provider.GetDataSource(entity ?? _firstEntityInThisProvider, ConnectionParameters, null) is not DataSource dataSource)
            yield break;
        dataSource.SetQueryCommand(sql);
        if (!dataSource.openForRead())
            yield break;
        foreach (var record in dataSource.GetRecords<T>())
            yield return record;

        if (dataSource.Exceptions != null && dataSource.Exceptions.Count > 0)
        {
            AddLog(errorCode, Log.Error, sql);
            AddLog(errorCode, Log.Error,
                string.Join("\r\n",
                    dataSource.Exceptions.Select(e =>
                    {
                        if (e is EngineException ee)
                            return ee.info?.ErrorText ?? e.ToString();
                        return e.Message ?? e.ToString();
                    })));
        }

        dataSource.Close();
    }

    // ReSharper disable once UnusedParameter.Local
    private bool DoSqlCommand(Entity entity, string ddl, string result, string errorCode)
    {
        if (string.IsNullOrEmpty(result))
        {
            if (Provider.GetDataSource(entity ?? _firstEntityInThisProvider, ConnectionParameters, null) is DataSource dataSource)
            {
                if (dataSource.Command(ddl))
                {
                    dataSource.Close();
                    return true;
                }

                AddLog(errorCode, Log.Error, ddl);
                if (dataSource.Exceptions != null && dataSource.Exceptions.Count > 0)
                    AddLog(errorCode, Log.Error,
                        string.Join("\r\n", dataSource.Exceptions.Select(e =>
                        {
                            if (e is EngineException ee)
                                return ee.info?.ErrorText ?? e.ToString();
                            return e.Message ?? e.ToString();
                        })));
                dataSource.Close();
            }
        }
        else
        {
            var pRs = Select(ddl, errorCode)?.FirstOrDefault();
            if (pRs != null)
            {
                pRs.GetField(result, out var cv);
                if (cv != null)
                {
                    return Convert.ToInt32(cv.ToString()) != 0;
                }
            }
        }

        return false;
    }

    private bool DoSqlCommandAndAddToList(Entity entity, string ddl, string errorCode)
    {
        AddToCommandList(entity, ddl, true);
        return DoSqlCommand(entity, ddl, "", errorCode);
    }
}