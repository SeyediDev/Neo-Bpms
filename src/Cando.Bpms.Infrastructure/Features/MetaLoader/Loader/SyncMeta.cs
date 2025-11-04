namespace Neo.Bpms.Infrastructure.Features.MetaLoader.Loader;

public interface IMetaDataLoader
{
    void LoadAndSyncMetaData(bool syncMetaData);
}
public class MetaDataLoader(ILogger<MetaDataLoader> logger) : IMetaDataLoader
{
    public void LoadAndSyncMetaData(bool syncMetaData)
    {
        if (ProjectDefinition.DontSyncMetaData || !syncMetaData)
        {
            return;
        }

        try
        {
            EntityMetaDataManager entityMetaDataManager = new();
            entityMetaDataManager.SaveMetadataToDatabase();
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "In Save Metadata To Database");
        }
    }
}
