
using JumpsellerSync.Domain.Impl;

using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories
{
    public interface IDeleteRepository<TModel> : IRepository
        where TModel : DomainModel
    {
        Task<DataAccessResult> DeleteAsync(TModel model, bool deferSave = false);
    }
}
