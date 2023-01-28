using JumpsellerSync.Domain.Impl.Linkstore;

namespace JumpsellerSync.DataAccess.Core.Repositories.Linkstore
{
    public interface ILinkstoreSubcategoryRepository
        : ILinkstoreRepository,
          IBaseRepository<LinkstoreSubcategory>,
          IUpsertRepository<LinkstoreSubcategory>
    { }
}
