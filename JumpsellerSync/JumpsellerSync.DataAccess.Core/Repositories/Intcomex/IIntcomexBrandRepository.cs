using JumpsellerSync.Domain.Impl.Intcomex;

namespace JumpsellerSync.DataAccess.Core.Repositories.Intcomex
{
    public interface IIntcomexBrandRepository
        : IIntcomexRepository,
          IBaseRepository<IntcomexBrand>,
          IUpsertRepository<IntcomexBrand>
    { }
}
