using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Repository.Entities;

namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public class FilterConfigBackupRestore(IBpmsSubjectSettingRepository repository) :
    EntityItemConfigBackupRestore<ConfiguredFilter>(repository)
{
    protected override string SubjectTitle => $"{ConfigType.Filter}Config";
}
