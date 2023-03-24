using JumpsellerSync.Domain.Impl.Tecnoglobal;

namespace JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal
{
    public interface ITecnoglobalSubcategoryRepository
        : ITecnoglobalRepository,
          IBaseRepository<TecnoglobalSubcategory>,
          IUpsertRepository<TecnoglobalSubcategory>
    { }
}
