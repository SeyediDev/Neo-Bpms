using Neo.Domain.Entities.Common;

namespace Neo.Bpms.Domain.Repository.Entities;

public interface IConfig 
{
    public long Id { get; set; }
}
public delegate TConfig Extraction<TConfig>(TConfig config, SubjectSetting subjectSetting) where TConfig : IConfig;
public interface IBpmsSubjectSettingRepository
{
    Task<List<SubjectSetting>> GetAllAsync(string subjectTitle, string subjectId, CancellationToken cancellationToken = default);
    Task<List<TConfig>> GetAllConfigsAsync<TConfig>(string subjectTitle, string subjectId, Extraction<TConfig> extraction, CancellationToken cancellationToken = default) where TConfig : IConfig;
    Task<List<TConfig>> GetAllConfigsAsync<TConfig>(DateTime currentDate, Extraction<TConfig> extraction, CancellationToken cancellationToken = default) where TConfig : IConfig;
    Task<SubjectSetting> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<TConfig> GetAsync<TConfig>(long id, Extraction<TConfig> extraction, CancellationToken cancellationToken = default) where TConfig : IConfig;
    Task<SubjectSetting> GetAsync(string subjectTitle, string subjectId, string key, CancellationToken cancellationToken = default);
    Task<TConfig> GetAsync<TConfig>(string subjectTitle, string subjectId, string key, Extraction<TConfig> extraction, CancellationToken cancellationToken = default) where TConfig : IConfig;
    Task<SubjectSetting> GetByKeyAsync(string subjectTitle, string key, CancellationToken cancellationToken = default);
    Task<TConfig> GetByKeyAsync<TConfig>(string subjectTitle, string key, Extraction<TConfig> extraction, CancellationToken cancellationToken = default) where TConfig : IConfig;

    Task SaveAsync<TConfig>(string subjectTitle, string subjectId, string key, TConfig config, CancellationToken cancellationToken = default) where TConfig : IConfig;
    Task AddAsync(SubjectSetting subjectSetting, CancellationToken cancellationToken = default);
    Task UpdateAsync(long id, string value, CancellationToken cancellationToken = default);
    Task UpdateAsync<TConfig>(long id, TConfig config, CancellationToken cancellationToken = default) where TConfig : IConfig;
    Task UpdateAsync(string subjectTitle, string subjectId, string key, string value, CancellationToken cancellationToken = default);
    Task RemoveAsync(long id, CancellationToken cancellationToken = default);
    Task RemoveAsync(string subjectTitle, string subjectId, string key, CancellationToken cancellationToken = default);
}
