using JumpsellerSync.Domain.Impl.Main;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Main
{
    public interface ILocalProductRepository
        : IReadRepository<LocalProduct>,
          IDeleteRepository<LocalProduct>,
          IUpsertRepository<LocalProduct>
    {
        IAsyncEnumerable<Product> SearchSynchronizedProductsAsync(string skuOrName, string brandId, int limit);

        Task<int> CountAsync();
    }
}
