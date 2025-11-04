using System.Text;
using Neo.Bpms.Domain.Entities.Cmmn.Data;
using Neo.Bpms.Domain.Entities.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Engine.DDL;
using Neo.Bpms.Infrastructure.Features.Orm.Entities.EntityConnections;

namespace Neo.Bpms.Infrastructure.Features.Orm.DDL;

public sealed class MigrationSingleton(ILogger<MigrationSingleton> logger, IConfiguration configuration)
{
    private volatile bool _isBusy;

    public bool IsBusy
    {
        get { return _isBusy; }
        private set { _isBusy = value; }
    }

    private readonly StringBuilder _messages = new();
    private readonly object _messagesLock = new();
    public List<string> Commands { get; private set; }
    public List<DDLManager.Log> Errors { get; private set; }


    public string PopMessages()
    {
        string result;
        lock (_messagesLock)
        {
            result = _messages.ToString();
            _messages.Clear();
        }

        return result;
    }

    public void StartMigration(MigrationOptions options)
    {
        if (IsBusy)
            return;
        IsBusy = true;
        try
        {
            SyncDatabase(options);
        }
        catch (Exception e)
        {
            Errors.Add(new DDLManager.Log
            {
                code = "10.45.1",
                comment = e.Message
            });
            logger.LogError(e, e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
    public void SetEnumerationItems(MigrationOptions options)
    {
        if (IsBusy)
            return;
        IsBusy = true;
        try
        {
            setEnumerationItems(options);
        }
        catch (Exception e)
        {
            Errors.Add(new DDLManager.Log
            {
                code = "10.45.1",
                comment = e.Message
            });
            logger.LogError(e, e.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public DatabaseModel ReadDataDictionary(string providerName, MigrationOptions options)
    {
        _pleaseStop = false;
        Errors = [];
        Commands = [];
        DatabaseModel databaseModel = null;
        foreach (var ddlManger in DdlMangers(options))
        {
            if (_pleaseStop) break;
            if (ddlManger.Provider.Name != providerName)
                continue;
            _mgr = ddlManger;
            try
            {
                databaseModel = _mgr.ReadDataDictionary(options, AddMessage);
            }
            catch (Exception e)
            {
                Errors.Add(new DDLManager.Log
                {
                    code = "10.45.0",
                    comment = e.Message
                });
                logger.LogError(e, e.Message);
            }

            Errors.AddRange(_mgr.Logs);
            Commands.AddRange(_mgr.Commands);
        }

        return databaseModel;
    }

    private IEnumerable<DDLManager> DdlMangers(MigrationOptions options)
    {
        var providers = FetchProvidersNames(options);
        foreach (var providerName in providers ?? [])
        {
            if (_pleaseStop) break;
            var provider = DataSourceProviderManager.ProviderContainer.GetProvider(providerName);
            if (provider == null)
            {
                var log = new DDLManager.Log
                {
                    code = "10.45.4",
                    comment = $"Provider {providerName} not implemented."
                };
                Errors.Add(log);
                logger.LogError(log.comment);
                continue;
            }

            if (provider.DontHaveDataDictionary || provider.DontSync)
                continue;
            yield return (DDLManager)provider.GetDdlManager(provider, configuration, []);
        }
    }

    private DDLManager _mgr; //todo IDdlManager
    private bool _pleaseStop;

    private void SyncDatabase(MigrationOptions options)
    {
        _pleaseStop = false;
        Errors = [];
        Commands = [];
        foreach (var ddlManger in DdlMangers(options))
        {
            if (_pleaseStop) break;
            _mgr = ddlManger;
            try
            {
                _mgr.SyncDatabasesWithModel(options, AddMessage);
                AddMessage(ddlManger.Provider.Name,
                    $"Migration {ddlManger.Provider.Name} {(_pleaseStop ? "Stopped" : "Finished")}.", null);
            }
            catch (Exception e)
            {
                Errors.Add(new DDLManager.Log
                {
                    code = "10.45.0.2",
                    comment = e.Message
                });
                logger.LogError(e, e.Message);
            }

            Errors.AddRange(_mgr.Logs);
            Commands.AddRange(_mgr.Commands);
        }
    }
    private void setEnumerationItems(MigrationOptions options)
    {
        _pleaseStop = false;
        Errors = [];
        Commands = [];
        foreach (var ddlManger in DdlMangers(options))
        {
            if (_pleaseStop) break;
            _mgr = ddlManger;
            try
            {
                _mgr.SetEnumerationItems(options, AddMessage);
                AddMessage(ddlManger.Provider.Name,
                    $"SetEnumerationItems {ddlManger.Provider.Name} {(_pleaseStop ? "Stopped" : "Finished")}.", null);
            }
            catch (Exception e)
            {
                Errors.Add(new DDLManager.Log
                {
                    code = "10.45.0.1",
                    comment = e.Message
                });
                logger.LogError(e, e.Message);
            }

            Errors.AddRange(_mgr.Logs);
            Commands.AddRange(_mgr.Commands);
        }
    }

    private static IEnumerable<string> FetchProvidersNames(MigrationOptions options)
    {
        IEnumerable<string> providers = null;
        if (string.IsNullOrEmpty(options.SpecificNamespace) && string.IsNullOrEmpty(options.SpecificEntity))
        {
            providers = ProjectDefinition.Project.Namespaces?.Values.SelectMany(
                    m => m.GetEntities().Select(e => e.Value.ProviderName ?? "")).GroupBy(g => g)
                .Select(i => i.Key).ToList();
        }
        else if (!string.IsNullOrEmpty(options.SpecificNamespace))
        {
            ModelNamespace modelNamespace = null;
            ProjectDefinition.Project.Namespaces?.TryGetValue(options.SpecificNamespace, out modelNamespace);
            if (modelNamespace == null)
                throw new Exception($"cannot find Namespace with name {options.SpecificNamespace}");
            if (!string.IsNullOrEmpty(options.SpecificEntity))
            {
                modelNamespace.GetEntities().TryGetValue(options.SpecificEntity, out var specificEntity);
                if (specificEntity == null)
                    throw new Exception(
                        $"cannot find Entity with name {options.SpecificEntity} in namespace {options.SpecificNamespace}");
                providers = new List<string> { specificEntity.ProviderName ?? "" };
            }
            else
            {
                providers = modelNamespace.GetEntities()
                    .Select(e => e.Value.ProviderName ?? "")
                    .GroupBy(g => g)
                    .Select(i => i.Key).ToList();
            }
        }

        return providers;
    }

    private bool AddMessage(string key, string message, string container)
    {
        lock (_messagesLock)
        {
            _messages.Append("<div dir=\"ltr\">");
            _messages.Append(key);
            _messages.Append(" ");
            _messages.Append(message);
            _messages.Append("</div>");
        }

        return true;
    }

    public void Stop()
    {
        _pleaseStop = true;
        _mgr?.Stop();
    }
}
