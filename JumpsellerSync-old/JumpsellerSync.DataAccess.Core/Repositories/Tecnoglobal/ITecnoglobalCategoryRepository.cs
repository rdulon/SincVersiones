using JumpsellerSync.Domain.Impl.Tecnoglobal;

namespace JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal
{
    public interface ITecnoglobalCategoryRepository
        : ITecnoglobalRepository,
          IBaseRepository<TecnoglobalCategory>,
          IUpsertRepository<TecnoglobalCategory>
    { }
}
