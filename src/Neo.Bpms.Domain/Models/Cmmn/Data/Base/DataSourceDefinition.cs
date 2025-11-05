namespace Neo.Bpms.Domain.Models.Cmmn.Data.Base;

public abstract class DataSourceDefinition
{
    protected DataSourceDefinition(ConnectionDefinition connection, string name, Entity entity)
    {
        this.name = name;
        this.connection = connection;
        Entity = entity;
        if (entity == null)
        {
            bExtractMetaData = true;
        }
    }
    public Entity Entity;
    public string name;
    public ConnectionDefinition connection;
    public bool bExtractMetaData = true;
}