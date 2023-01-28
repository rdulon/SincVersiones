using JumpsellerSync.Domain.Impl.Main;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Main
{
    public interface IProductRepository
        : IBaseRepository<Product>,
          IUpsertRepository<Product>
    {
        IAsyncEnumerable<Product> ReadProductsToSynchronizeAsync(
            int offset, int limit);

        Task<DataAccessResult<Product>> ReadBySkuAsync(string sku);

        Task<int> CountProductsToSynchronizeAsync();

        IAsyncEnumerable<Product> ReadSkuOrNameSuggestionsAsync(string skuOrName, int suggestionsLimit);

        Task<IAsyncEnumerable<Product>> ReadAsync(
            string skuOrName, IEnumerable<string> brandIds, int offset, int limit, string provider = null);

        Task<int> GetProductPagesAsync(string skuOrName, IEnumerable<string> brandIds, int limit, string provider = null);
    }
}
