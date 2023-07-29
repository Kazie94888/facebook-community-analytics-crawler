using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace LookOn.SystemConfigs;

public class SystemConfigManager : LookOnManager
{
    private readonly IRepository<SystemConfig> _repo;

    public SystemConfigManager(IRepository<SystemConfig> repo)
    {
        _repo = repo;
    }

    public async Task<SystemConfig> GetOrInit()
    {
        var config = await _repo.FirstOrDefaultAsync();
        if (config != null) return config;

        var newConfig = new SystemConfig();

        await _repo.InsertAsync(newConfig);
        return newConfig;
    }
}