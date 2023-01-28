using JumpsellerSync.Domain.Impl;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JumpsellerSync.DataAccess.Core.Repositories
{
    public interface IReadRepository<TModel> : IRepository
        where TModel : DomainModel
    {
        Task<DataAccessResult<TModel>> ReadAsync(object[] keys);

        IAsyncEnumerable<TProjection> ReadAsync<TOrderBy, TProjection>(
           Expression<Func<TModel, bool>> conditions,
           Expression<Func<TModel, TOrderBy>> orderBy,
           int offset, int limit,
           Expression<Func<TModel, TProjection>> projection);

        IAsyncEnumerable<TModel> ReadAsync<TOrderBy>(
            Expression<Func<TModel, bool>> conditions,
            Expression<Func<TModel, TOrderBy>> orderBy,
            int offset, int limit);

        IAsyncEnumerable<TModel> ReadAsync(
            Expression<Func<TModel, bool>> conditions, int offset, int limit);

        Task<int> GetPagesAsync(Expression<Func<TModel, bool>> conditions, int limit);
    }
}
