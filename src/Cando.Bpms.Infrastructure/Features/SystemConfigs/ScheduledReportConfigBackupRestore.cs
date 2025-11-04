using Neo.Bpms.Domain.Entities.Cmmn.UI.ConfiguredItems.ScheduledReport;
using Neo.Bpms.Domain.Repository.Entities;
namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public class ScheduledReportConfigBackupRestore(IBpmsSubjectSettingRepository repository)
{
    private static string SubjectTitle => $"Scheduled{ConfigType.Report}Config";
    private static string SubjectId(ConfiguredScheduledReport cr) => SubjectId(cr.ReportConfigId);
    private static string SubjectId(string reportConfigId) => $"{reportConfigId}";

    public async Task Save(ConfiguredScheduledReport config, CancellationToken cancellationToken=default)
    {
        config.ConfigId ??= Guid.NewGuid().ToString();
        await repository.SaveAsync(SubjectTitle, SubjectId(config), config.ConfigId, config, cancellationToken);
    }

    public async Task RemoveConfig(ConfiguredScheduledReport config, CancellationToken cancellationToken=default)
    {
        await RemoveConfig(config.ReportConfigId, config.ConfigId, cancellationToken);
    }

    public async Task RemoveConfig(string reportConfigId, string configId, CancellationToken cancellationToken = default)
    {
        await repository.RemoveAsync(SubjectTitle, SubjectId(reportConfigId), configId, cancellationToken);
    }

    public async Task<List<ConfiguredScheduledReport>> Configurations(string reportConfigId, CancellationToken cancellationToken = default)
    {
        var list = await repository.GetAllConfigsAsync<ConfiguredScheduledReport>(SubjectTitle, SubjectId(reportConfigId), (config, s) => config, cancellationToken);
        return list;
    }

    public async Task<List<ConfiguredScheduledReport>> Configurations(DateTime currentDate, CancellationToken cancellationToken = default)
    {
        var list = await repository.GetAllConfigsAsync<ConfiguredScheduledReport>(currentDate, (config, ss)=>config, cancellationToken);
        return [.. list.Where(x=> !x.IsDisable 
            && (x.StartDate==null || x.StartDate>= currentDate)
            && (x.EndDate == null || x.EndDate <= currentDate))];
    }
}
