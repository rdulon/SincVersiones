using JumpsellerSync.DataAccess.Core.QueryModels.Main;
using JumpsellerSync.Domain.Impl.Main;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories.Main
{
    public interface ISynchronizationSessionRepository
        : ICreateRepository<SynchronizationSession>,
          IUpdateRepository<SynchronizationSession>,
          IReadRepository<SynchronizationSession>
    {
        Task<bool> IsSessionSynchronizedAsync(string sessionId);

        IAsyncEnumerable<ReconcileProductInformation> GetReconcileInformationAsync(string sessionId);

        Task<DataAccessResult<int>> MarkSessionProcessedAsync(string sessionId, bool deferSave = false);

        IAsyncEnumerable<BaseProvider> ReadSessionProvidersAsync(string sessionId);

        IAsyncEnumerable<string> GetSynchronizedSessionsAsync();
    }
}
