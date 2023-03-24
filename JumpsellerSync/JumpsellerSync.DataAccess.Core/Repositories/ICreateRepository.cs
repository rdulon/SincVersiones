using JumpsellerSync.Domain.Impl;

using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories
{
    public interface ICreateRepository<TModel> : IRepository
        where TModel : DomainModel
    {
        Task<DataAccessResult<TModel>> CreateAsync(TModel domainModel, bool deferSave = false);
    }
}
