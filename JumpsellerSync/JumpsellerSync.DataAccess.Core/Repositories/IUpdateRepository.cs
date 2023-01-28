using JumpsellerSync.Domain.Impl;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories
{
    public interface IUpdateRepository<TModel> : IRepository
        where TModel : DomainModel
    {
        Task<DataAccessResult<TModel>> UpdateAsync(
            TModel domainModel, bool deferSave = false, params string[] skipProperitesUpdate);

        Task<DataAccessResult<int>> UpdateAsync(
            IEnumerable<TModel> domainModel, bool deferSave = false, params string[] skipProperitesUpdate);
    }
}
