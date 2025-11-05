namespace Neo.Bpms.UI.MVC.Controllers.Public;

public class FilterManager(FilterConfigBackupRestore filterConfigBackupRestore)
{
    public async Task<(ElasticObject record, ConfiguredFilter configuredFilter)> GetConfiguredFilterValues
        (long? filterId, List<ConfiguredFilter> configuredFilters, IdentityUser user, PersistenceObject po, ElasticObject values)
    {
        ConfiguredFilter configuredFilter = null;
        if (filterId > 0)
        {
            configuredFilter = await filterConfigBackupRestore.GetConfig(filterId.Value);
        }

        if (po == null && configuredFilter == null)
        {
            configuredFilter = ItemConfigBackupRestore<ConfiguredFilter>.GetDefaultItem(configuredFilters, user);
        }

        ElasticObject fv = configuredFilter == null
            ? po?.FilterValues
            : GetConfiguredFilterValues(configuredFilter);
        return (fv != null ? fv.Merge(values) : values, configuredFilter);
    }

    private static ElasticObject GetConfiguredFilterValues(ConfiguredFilter configuredFilter)
    {
        ElasticObject filterValues = new();
        if (configuredFilter.Values != null)
        {
            foreach (ConfiguredFilterValue val in configuredFilter.Values)
            {
                _ = filterValues.SetField(val.FieldId, val.Value);
            }
        }

        return filterValues;
    }
}
