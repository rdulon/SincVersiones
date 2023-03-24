using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Core.Services.Main
{
    public interface IReconcileProductsService : IService
    {
        Task ReconcileSessionsAsync();

        Task ReconcileSessionAsync(string sessionId);
    }
}
