using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Intcomex;

using System;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Intcomex
{
    public interface IIntcomexConfigRepository
        : IIntcomexRepository,
          IProviderConfigRepository<IntcomexConfiguration>
    {
        Task<DataAccessResult<DateTime>> GetLastCatalogUpdateAsync();

        Task<DataAccessResult> SetLastCatalogUpdateAsync(DateTime dt);
    }
}
