
using JumpsellerSync.BusinessLogic.Impl.Services;
using JumpsellerSync.Common.Util.DependencyInjection;

namespace JumpsellerSync.BusinessLogic.Impl.DependencyInjection
{
    internal sealed class ServicesModule : BaseServicesModule
    {
        public ServicesModule()
            : base(typeof(BaseService<,,,,>), typeof(VtexService))
        { }
    }
}
