using JumpsellerSync.BusinessLogic.Core.Dtos;

using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services
{
    public interface IReadService<TDetailsDto> : IService
        where TDetailsDto : class
    {
        Task<ServiceResult<TDetailsDto>> ReadAsync(string keyDto);

        Task<PageResultDto<TDetailsDto>> ReadAsync(string filter, int page, int limit);
    }
}
