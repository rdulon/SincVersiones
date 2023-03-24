using JumpsellerSync.DataAccess.Core.Repositories.Provider;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

namespace JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal
{
    public interface ITecnoglobalConfigRepository
        : ITecnoglobalRepository,
          IProviderConfigRepository<TecnoglobalConfiguration>
    { }
}
