using JumpsellerSync.Domain.Impl.Tecnoglobal;

namespace JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal
{
    public interface ITecnoglobalBrandRepository
        : ITecnoglobalRepository,
          IBaseRepository<TecnoglobalBrand>,
          IUpsertRepository<TecnoglobalBrand>
    { }
}
