using JumpsellerSync.Domain.Impl.Main;

using System.Collections.Generic;

namespace JumpsellerSync.DataAccess.Core.Repositories.Main
{
    public interface IProviderRepository
        : IBaseRepository<BaseProvider>
    {
        IAsyncEnumerable<BaseProvider> ReadProvidersToSynchronize();
    }
}
