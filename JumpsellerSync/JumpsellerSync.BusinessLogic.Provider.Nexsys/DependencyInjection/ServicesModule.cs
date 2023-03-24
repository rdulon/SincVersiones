using JumpsellerSync.BusinessLogic.Provider.Nexsys.Services;
using JumpsellerSync.Common.Util.DependencyInjection;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.DependencyInjection
{
    internal sealed class ServicesModule : BaseServicesModule
    {
        public ServicesModule()
            : base(typeof(NexsysProviderService))
        { }
    }
}
