using Neo.Bpms.Domain.Models.Cmmn.Data;
using Neo.Bpms.Domain.Models.Cmmn.Data.DatabaseModels;
using Neo.Bpms.Domain.Models.Cmmn.Data.DDL;
using Neo.Bpms.Domain.Models.Cmmn.Data.DML;
using Neo.Bpms.Domain.Models.Cmmn.Data.Provider;
using Neo.Bpms.Domain.Models.Cmmn.Data.Query;
using Neo.Bpms.Domain.Models.Cmmn.Entities;

namespace Neo.Bpms.Engine.DDL;

public abstract partial class DDLManager : IDDLManager
{
    protected MigrationOptions Options;
    private bool _pleaseStop;
    private readonly DatabaseModel _model;

    public readonly IDataProvider Provider;

    public IConfiguration _configuration { get; }

    protected readonly IDDLGenerator DDLGenerator;
    protected readonly IDMLGenerator DMLGenerator;
    protected readonly IQueryGenerator QueryGenerator;
    private string ProviderName => Provider.Name;
    protected string DatabaseName 
    {
        get
        {
            return ProviderName == "default" ? _configuration["Database"] : _configuration[$"{ProviderName}Database"];
        }
    }

    public List<string> Commands { get; private set; }
    private SortedDictionary<string, Entity> _changedEntities;
    private Func<string, string, string, bool> _messageFunction;
    protected readonly LocalParameters ConnectionParameters;

    protected DDLManager(IDataProvider provider, IConfiguration configuration, LocalParameters connectionParameters)
    {
        Provider = provider;
        _configuration = configuration;
        ConnectionParameters = connectionParameters;
        DDLGenerator = Provider.GetDDLGenerator();
        DMLGenerator = Provider.GetDMLGenerator();
        QueryGenerator = Provider.GetQueryGenerator();
        _model = new(DatabaseName);
    }
}
