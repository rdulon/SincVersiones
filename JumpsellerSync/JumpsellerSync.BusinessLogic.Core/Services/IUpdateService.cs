using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services
{
    public interface IUpdateService<TUpdateDto, TDetailsDto> : IService
        where TUpdateDto : class
        where TDetailsDto : class
    {
        Task<ServiceResult<TDetailsDto>> UpdateAsync(TUpdateDto updatelDto);
    }
}
