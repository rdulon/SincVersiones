using JumpsellerSync.Domain.Impl.Linkstore;

namespace JumpsellerSync.DataAccess.Core.Repositories.Linkstore
{
    public interface ILinkstoreCategoryRepository
        : ILinkstoreRepository,
          IBaseRepository<LinkstoreCategory>,
          IUpsertRepository<LinkstoreCategory>
    { }
}
