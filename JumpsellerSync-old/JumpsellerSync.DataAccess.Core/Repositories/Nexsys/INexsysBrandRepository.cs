using JumpsellerSync.Domain.Impl.Nexsys;

namespace JumpsellerSync.DataAccess.Core.Repositories.Nexsys
{
    public interface INexsysBrandRepository
        : INexsysRepository,
          IBaseRepository<NexsysBrand>,
          IUpsertRepository<NexsysBrand>
    { }
}
