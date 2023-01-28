using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Intcomex;

using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Intcomex
{
    public interface IIntcomexProductRepository
        : IIntcomexRepository,
          IBaseRepository<IntcomexProduct>,
          IProviderProductRepository<IntcomexProduct>,
          IUpsertRepository<IntcomexProduct>
    {
        Task MarkProductsAsDirtyAsync();

        Task RemoveDirtyProductsAsync();
    }
}
