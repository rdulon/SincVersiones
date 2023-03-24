using JumpsellerSync.Domain.Impl.Main;

using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Main
{
    public interface IMainConfigRepository : IRepository
    {
        Task<DataAccessResult<JumpsellerConfiguration>> ReadJumpsellerAuthInfoAsync();

        Task<DataAccessResult> SaveJumpsellerAuthorizationInfoAsync(JumpsellerConfiguration info);

        Task<DataAccessResult> ResetJumpsellerAuthInfoAsync();
    }
}
