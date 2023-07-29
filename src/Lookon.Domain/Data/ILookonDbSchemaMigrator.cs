using System.Threading.Tasks;

namespace LookOn.Data;

public interface ILookOnDbSchemaMigrator
{
    Task MigrateAsync();
}
