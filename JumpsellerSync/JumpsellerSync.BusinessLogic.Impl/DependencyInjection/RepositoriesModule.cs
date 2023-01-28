using JumpsellerSync.Common.Util.DependencyInjection;
using JumpsellerSync.DataAccess.Impl.Repositories;

using Microsoft.Extensions.Configuration;

namespace JumpsellerSync.BusinessLogic.Impl.DependencyInjection
{
    internal sealed class RepositoriesModule : BaseRepositoriesModule
    {
        public RepositoriesModule(IConfiguration config)
            : base(config, typeof(BaseRepository<,>))
        { }

    }
}
