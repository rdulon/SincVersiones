using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services
{
    public interface IDeleteService : IService
    {
        Task<ServiceResult> DeleteAsync(string keyDto);
    }
}
