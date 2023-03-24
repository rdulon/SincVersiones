using JumpsellerSync.Domain.Impl.Provider;

using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Provider
{
    public interface IProviderConfigRepository<TConfiguration> : IRepository
        where TConfiguration : ProviderConfiguration
    {
        TConfiguration DefaultConfig { get; }

        Task<DataAccessResult<TConfiguration>> ReadConfigAsync();

        Task<DataAccessResult> UpdateConfigAsync(TConfiguration config);
    }
}
