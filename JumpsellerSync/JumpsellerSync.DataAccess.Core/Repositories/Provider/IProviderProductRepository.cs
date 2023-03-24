using JumpsellerSync.DataAccess.Core.QueryModels.Provider;
using JumpsellerSync.Domain.Impl.Provider;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Provider
{
    public interface IProviderProductRepository<TProduct>
        : IReadRepository<TProduct>,
          IUpdateRepository<TProduct>
        where TProduct : ProviderProduct
    {
        IAsyncEnumerable<TProduct> ReadSynchronizingToJumpsellerAsync();

        Task<DataAccessResult<TProduct>> FindByRedcetusIdAsync(string redcetusId);

        IAsyncEnumerable<ProviderUnsyncedProductSuggestion> ReadUnsyncedSuggestionsAsync(
            string skuOrName, string brandId, int suggestionsLimit, bool withStock);

        Task<int> GetUnsyncedProductPagesAsync(string skuOrName, string brandId, bool withStock, int limit);

        IAsyncEnumerable<TProduct> ReadUnsyncedProductsAsync(
            string skuOrName, string brandId, int offset, int limit, bool withStock);
    }
}
