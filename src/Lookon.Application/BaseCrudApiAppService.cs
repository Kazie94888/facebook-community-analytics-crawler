using LookOn.Configs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace LookOn;

public class BaseCrudApiAppService<TEntity, TEntityDto, TKey, TGetInput, TCreateUpdateDto> :
    CrudAppService<TEntity, TEntityDto, TKey, TGetInput, TCreateUpdateDto>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : IEntityDto<TKey>
{
    public GlobalConfig GlobalConfig { get; set; }

    protected BaseCrudApiAppService(IRepository<TEntity, TKey> repository) : base(repository)
    {
    }
}