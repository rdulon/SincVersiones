using JumpsellerSync.Domain.Impl;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories
{
    public interface IUpsertRepository<TModel> : IRepository
        where TModel : DomainModel
    {
        Task<DataAccessResult<int>> UpsertAsync(
            IEnumerable<TModel> models, bool deferSave = false, params string[] skipProperitesUpdate);
    }
}
