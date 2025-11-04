using Neo.Bpms.Domain.Repository;
using Neo.Bpms.Domain.Repository.Entities;
using Neo.Domain.Entities.Common;

namespace Neo.Bpms.Infrastructure.Features.SystemConfigs;

public class BpmsSubjectSettingRepository(
    IBpmsCommandRepository<SubjectSetting, long> commandRepo,
    IBpmsQueryRepository<SubjectSetting, long> queryRepo)
    : IBpmsSubjectSettingRepository
{
    public async Task<List<SubjectSetting>> GetAllAsync(string subjectTitle, string subjectId, CancellationToken cancellationToken = default)
    {
        return [.. await queryRepo.GetAllAsync(cancellationToken, x =>
            x.SubjectTitle == subjectTitle &&
            x.SubjectId == subjectId)];
    }

    public async Task<List<TConfig>> GetAllConfigsAsync<TConfig>(string subjectTitle, string subjectId,
        Extraction<TConfig> extraction, CancellationToken cancellationToken = default)
        where TConfig : IConfig
    {
        List<SubjectSetting> list = [.. await queryRepo.GetAllAsync(cancellationToken, x =>
            x.SubjectTitle == subjectTitle &&
            x.SubjectId == subjectId)];
        List<TConfig> configs = list.Select(x => Extract(extraction, x)).ToList();
        return configs;
    }

    public async Task<List<TConfig>> GetAllConfigsAsync<TConfig>(DateTime currentDate,
        Extraction<TConfig> extraction, CancellationToken cancellationToken = default)
        where TConfig : IConfig
    {
        IEnumerable<SubjectSetting> list = await queryRepo.GetAllAsync(cancellationToken,
                x => x.CreateDate >= currentDate && x.IsDeleted == false);
        return [.. list.Select(x => Extract(extraction, x))];
    }

    public async Task<SubjectSetting> GetAsync(string subjectTitle, string subjectId, string key, CancellationToken cancellationToken = default)
    {
        return await queryRepo.FirstOrDefaultAsync(x =>
            x.SubjectTitle == subjectTitle &&
            x.SubjectId == subjectId &&
            x.Key == key
            , cancellationToken);
    }

    public async Task<SubjectSetting> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        return await queryRepo.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TConfig> GetAsync<TConfig>(long id, Extraction<TConfig> extraction, CancellationToken cancellationToken = default)
        where TConfig : IConfig
    {
        SubjectSetting item = await queryRepo.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is not null ? Extract(extraction, item) : default;
    }

    public async Task<TConfig> GetAsync<TConfig>(string subjectTitle, string subjectId, string key,
        Extraction<TConfig> extraction, CancellationToken cancellationToken = default)
        where TConfig : IConfig
    {
        SubjectSetting item = await queryRepo.FirstOrDefaultAsync(x =>
            x.SubjectTitle == subjectTitle &&
            x.SubjectId == subjectId &&
            x.Key == key
            , cancellationToken);
        return item is not null ? Extract(extraction, item) : default;
    }

    public async Task<SubjectSetting> GetByKeyAsync(string subjectTitle, string key, CancellationToken cancellationToken = default)
    {
        return await queryRepo.FirstOrDefaultAsync(x =>
            x.SubjectTitle == subjectTitle &&
            x.Key == key
            , cancellationToken);
    }

    public async Task<TConfig> GetByKeyAsync<TConfig>(string subjectTitle, string key,
        Extraction<TConfig> extraction, CancellationToken cancellationToken = default)
        where TConfig : IConfig
    {
        SubjectSetting item = await queryRepo.FirstOrDefaultAsync(x =>
            x.SubjectTitle == subjectTitle &&
            x.Key == key, cancellationToken);
        return item is not null ? Extract(extraction, item) : default;
    }

    public async Task SaveAsync<TConfig>(string subjectTitle, string subjectId, string key, TConfig config, CancellationToken cancellationToken = default)
        where TConfig : IConfig
    {
        if (config.Id <= 0)
        {
            SubjectSetting subjectSetting = await GetAsync(subjectTitle, subjectId, key, cancellationToken);
            if (subjectSetting is null)
            {
                subjectSetting = new SubjectSetting
                {
                    SubjectTitle = subjectTitle,
                    SubjectId = subjectId,
                    Key = key,
                };
                await AddAsync(subjectSetting, cancellationToken);
            }
            config.Id = subjectSetting.Id;
        }
        await UpdateAsync(config.Id, config.ToJson(), cancellationToken);
    }

    public async Task AddAsync(SubjectSetting subjectSetting, CancellationToken cancellationToken = default)
    {
        commandRepo.Add(subjectSetting);
        _ = await commandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(long id, CancellationToken cancellationToken = default)
    {
        int result = await commandRepo.ExecuteUpdateAsync(x => x.Id == id,
            x => x.SetProperty(p => p.ExpireDate, DateTime.Now)
                  .SetProperty(p => p.IsDeleted, true), cancellationToken);
        _ = await commandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(string subjectTitle, string subjectId, string key, CancellationToken cancellationToken = default)
    {
        int result = await commandRepo.ExecuteUpdateAsync(x =>
            x.SubjectTitle == subjectTitle &&
            x.SubjectId == subjectId &&
            x.Key == key,
            x => x.SetProperty(p => p.ExpireDate, DateTime.Now)
                  .SetProperty(p => p.IsDeleted, true), cancellationToken);
        _ = await commandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(long id, string value, CancellationToken cancellationToken = default)
    {
        int result = await commandRepo.ExecuteUpdateAsync(x => x.Id == id,
            x => x.SetProperty(p => p.Value, value), cancellationToken);
        _ = await commandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync<TConfig>(long id, TConfig config, CancellationToken cancellationToken = default)
        where TConfig : IConfig
    {
        string value = config.ToJson();
        int result = await commandRepo.ExecuteUpdateAsync(x => x.Id == id,
            x => x.SetProperty(p => p.Value, value), cancellationToken);
        _ = await commandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(string subjectTitle, string subjectId, string key, string value, CancellationToken cancellationToken = default)
    {
        int result = await commandRepo.ExecuteUpdateAsync(x =>
            x.SubjectTitle == subjectTitle &&
            x.SubjectId == subjectId &&
            x.Key == key,
            x => x.SetProperty(p => p.Value, value), cancellationToken);
        _ = await commandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static TConfig Extract<TConfig>(Extraction<TConfig> extraction, SubjectSetting x)
        where TConfig : IConfig
    {
        TConfig config = x.Value.FromJson<TConfig>();
        config.Id = x.Id;
        _ = extraction(config, x);
        return config;
    }
}
