using Neo.Bpms.Domain.Models.Cmmn.UI.ConfiguredItems;
using Neo.Bpms.Domain.Repository.Entities;

namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public class FolderConfigBackupRestore(IBpmsSubjectSettingRepository repository): EntityItemConfigBackupRestore<ConfiguredFolder>(repository)
{
    protected override string SubjectTitle => $"{ConfigType.Folder}Config";
}
