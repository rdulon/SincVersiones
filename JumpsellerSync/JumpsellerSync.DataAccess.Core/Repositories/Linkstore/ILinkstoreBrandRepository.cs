using JumpsellerSync.Domain.Impl.Linkstore;

namespace JumpsellerSync.DataAccess.Core.Repositories.Linkstore
{
    public interface ILinkstoreBrandRepository
        : ILinkstoreRepository,
          IBaseRepository<LinkstoreBrand>,
          IUpsertRepository<LinkstoreBrand>
    { }
}
