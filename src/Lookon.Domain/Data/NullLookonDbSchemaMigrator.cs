using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace LookOn.Data;

/* This is used if database provider does't define
 * ILookOnDbSchemaMigrator implementation.
 */
public class NullLookOnDbSchemaMigrator : ILookOnDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
