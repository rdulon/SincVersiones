using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Nexsys;

namespace JumpsellerSync.DataAccess.Core.Repositories.Nexsys
{
    public interface INexsysConfigRepository
        : INexsysRepository,
          IProviderConfigRepository<NexsysConfiguration>
    { }
}
