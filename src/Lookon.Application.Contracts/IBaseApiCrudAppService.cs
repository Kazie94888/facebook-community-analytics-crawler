using Volo.Abp.Application.Services;

namespace LookOn
{
    public interface IBaseApiCrudAppService<TEntityDto, in TKey, in TGetInput, in TCreateUpdateDto> :
        ICrudAppService<TEntityDto,
            TKey,
            TGetInput,
            TCreateUpdateDto>
    {
        
    }
}