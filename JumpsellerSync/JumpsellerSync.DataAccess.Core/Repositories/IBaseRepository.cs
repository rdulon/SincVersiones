using JumpsellerSync.Domain.Impl;

namespace JumpsellerSync.DataAccess.Core.Repositories
{
    public interface IBaseRepository<TModel>
        : ICreateRepository<TModel>,
          IReadRepository<TModel>,
          IUpdateRepository<TModel>,
          IDeleteRepository<TModel>
        where TModel : DomainModel
    { }
}
