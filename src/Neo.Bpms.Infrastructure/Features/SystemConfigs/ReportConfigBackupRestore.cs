using Neo.Bpms.Domain.Entities.Cmmn.UI;
using Neo.Bpms.Domain.Repository.Entities;
using Neo.Domain.Entities.Common;
using MongoDB.Driver;
using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public class ReportConfigBackupRestore(IBpmsSubjectSettingRepository repository)
    : ItemConfigBackupRestore<ConfiguredReport>
{
    protected override string SubjectTitle => $"{ConfigType.Report}Config";
    private static string SubjectId(ConfiguredReport cr) => SubjectId(cr.Report);
    private static string SubjectId(Report report) => SubjectId(report.NamespaceId, report.EntityId, report.Id);
    private static string SubjectId(string namespaceId, string entityId, string reportId) => $"{namespaceId}.{entityId}.{reportId}";

    public async Task Save(ConfiguredReport config, CancellationToken cancellationToken = default)
    {
        if (config.IsMeta) return;
        config.ConfigId ??= Guid.NewGuid().ToString();
        await repository.SaveAsync(SubjectTitle, SubjectId(config), config.ConfigId, config, cancellationToken);
    }

    public async Task RemoveConfig(ConfiguredReport config, CancellationToken cancellationToken=default)
    {
        if (config.IsMeta) return;
        if (config.Id>0)
            await repository.RemoveAsync(config.Id, cancellationToken);
        else
            await repository.RemoveAsync(SubjectTitle, SubjectId(config), config.ConfigId, cancellationToken);
    }

    public async Task RemoveConfig(string namespaceId, string entityId, string reportId, string configId, CancellationToken cancellationToken = default)
    {
        await repository.RemoveAsync(SubjectTitle, SubjectId(namespaceId, entityId, reportId), configId, cancellationToken);
    }

    public async Task<ConfiguredReport> GetConfig(string configId, CancellationToken cancellationToken = default)
    {
        return await repository.GetByKeyAsync<ConfiguredReport>(SubjectTitle, configId, Extract, cancellationToken);
    }

    public async Task<ConfiguredReport> GetConfig(Report report, string configId, CancellationToken cancellationToken = default)
    {
        return GetMetaConfig(report, configId)??
            await repository.GetAsync<ConfiguredReport>(
                SubjectTitle, SubjectId(report), configId, (config, setting) => Extract(config, report, setting.Key), cancellationToken);
    }
    
    public async Task<List<ConfiguredReport>> Configurations(Report report, CancellationToken cancellationToken = default)
    {
        List<ConfiguredReport> configs = report.MetaConfigures?.Values.ToList() ?? [];
        configs.AddRange(await repository.GetAllConfigsAsync<ConfiguredReport>(
            SubjectTitle, SubjectId(report), (config, setting) => Extract(config, report, setting.Key), cancellationToken));
        return configs;
    }

    private static ConfiguredReport Extract(ConfiguredReport config, SubjectSetting subjectSetting)
    {
        var split = subjectSetting.SubjectId.Split(".");
        var report = ProjectDefinition.Project.GetUiEntity(split[0], split[1])?.GetReport(split[2]);
        return Extract(config, report, subjectSetting.Key);
    }

    private static ConfiguredReport Extract(ConfiguredReport config, Report report, string configId)
    {
        config.ConfigId = configId;
        config.Report = report;
        NormalizeConfig(config);
        return config;
    }

    private static void NormalizeConfig(ConfiguredReport config)
    {
        foreach (var subReport in config?.SubReports.Values ?? [])
        {
            subReport.ParentConfiguredReport = config;
            NormalizeConfig(subReport.ConfiguredReport);
        }
    }

    private static ConfiguredReport GetMetaConfig(Report report, string configId)
    {
        return report.MetaConfigures?.TryGetValue(configId, out var config) ?? false && config != null
            ? config : null;
    }
}
