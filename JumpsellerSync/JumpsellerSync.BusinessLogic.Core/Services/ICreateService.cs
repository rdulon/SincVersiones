using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services
{
    public interface ICreateService<TCreateDto, TDetailsDto> : IService
        where TCreateDto : class
        where TDetailsDto : class
    {
        Task<ServiceResult<TDetailsDto>> CreateAsync(TCreateDto createDto);
    }
}
