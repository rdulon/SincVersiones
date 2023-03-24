using JumpsellerSync.Common.Util.DependencyInjection;
using JumpsellerSync.DataAccess.Impl.Repositories;

using Microsoft.Extensions.Configuration;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.DependencyInjection
{
    internal class RepositoriesModule : BaseRepositoriesModule
    {
        public RepositoriesModule(
            IConfiguration configuration)
            : base(configuration, typeof(BaseRepository<,>), "Linkstore")
        { }
    }
}
