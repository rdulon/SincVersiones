using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Linkstore;

namespace JumpsellerSync.DataAccess.Core.Repositories.Linkstore
{
    public interface ILinkstoreProductRepository
        : ILinkstoreRepository,
          IBaseRepository<LinkstoreProduct>,
          IProviderProductRepository<LinkstoreProduct>,
          IUpsertRepository<LinkstoreProduct>
    { }
}
