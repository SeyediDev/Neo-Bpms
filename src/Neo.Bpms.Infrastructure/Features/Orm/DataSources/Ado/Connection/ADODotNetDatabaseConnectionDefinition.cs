using Neo.Bpms.Domain.Models.Cmmn.Data.Base;

namespace Neo.Bpms.Infrastructure.Features.Orm.DataSources.Ado.Connection;
public abstract class AdoDotNetDatabaseConnectionDefinition : ConnectionDefinition
{
    public AdoDotNetDatabaseConnectionDefinition(IConfiguration configuration, string providerName)
    {
        var prefix = providerName.Equals("default") ? "" : providerName ?? "";
        var config = configuration;
        ConnectionString = config.GetConnectionString(providerName);
        DatabaseName = config[prefix + "Database"];

        Server_DataSource = config[prefix + "Server"];
        if (string.IsNullOrWhiteSpace(Server_DataSource))
            Server_DataSource = "localhost";
        FailoverPartner = config[prefix + "FailoverPartner"];
        var s = config[prefix + "IntegratedSecurity"];
        if (!bool.TryParse(s, out var integratedSecurity)) integratedSecurity = false;
        IntegratedSecurity = integratedSecurity;
        UserName = config[prefix + "UserId"];
        Password = config[prefix + "PW"];
        s = config[prefix + "encrypt"];
        if (!bool.TryParse(s, out var encrypt)) encrypt = false;
        Encrypt = encrypt;
        s = config[prefix + "MARS"];
        if (!bool.TryParse(s, out var mars)) mars = false;
        MultipleActiveResultSets = mars;
        ExtendedProperties = config[prefix + "extendedProperties"];
        Driver_Provider = config[prefix + "driver"];
        if (string.IsNullOrWhiteSpace(Driver_Provider))
            Driver_Provider = config[prefix + "provider"];
        s = config[prefix + "maxConnections"];
        if (!int.TryParse(s, out var maxConnections)) maxConnections = 20;
        MaxConnections = maxConnections;
    }
    public override bool MetaDataCouldBeExtracted() { return true; }

    public string ConnectionString { get; set; }
    public int MaxConnections;
    public string Server_DataSource;
    public string FailoverPartner;
    public bool IntegratedSecurity;
    public string UserName;
    public string Password;
    public string Driver_Provider;
    public string ExtendedProperties;
    public bool Encrypt;
    public bool MultipleActiveResultSets;
}
