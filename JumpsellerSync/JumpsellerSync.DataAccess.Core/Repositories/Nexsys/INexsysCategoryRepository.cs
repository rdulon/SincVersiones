using JumpsellerSync.Domain.Impl.Nexsys;

namespace JumpsellerSync.DataAccess.Core.Repositories.Nexsys
{
    public interface INexsysCategoryRepository
        : INexsysRepository,
          IBaseRepository<NexsysCategory>,
          IUpsertRepository<NexsysCategory>
    { }
}
