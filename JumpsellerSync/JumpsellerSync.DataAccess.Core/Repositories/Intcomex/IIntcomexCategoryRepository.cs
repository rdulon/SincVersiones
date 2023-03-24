using JumpsellerSync.Domain.Impl.Intcomex;

namespace JumpsellerSync.DataAccess.Core.Repositories.Intcomex
{
    public interface IIntcomexCategoryRepository
        : IIntcomexRepository,
          IBaseRepository<IntcomexCategory>,
          IUpsertRepository<IntcomexCategory>
    { }
}
