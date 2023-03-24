using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Nexsys;

namespace JumpsellerSync.DataAccess.Core.Repositories.Nexsys
{
    public interface INexsysProductRepository
        : INexsysRepository,
          IBaseRepository<NexsysProduct>,
          IProviderProductRepository<NexsysProduct>,
          IUpsertRepository<NexsysProduct>
    { }
}
