using JumpsellerSync.BusinessLogic.Provider.Linkstore.Services;
using JumpsellerSync.Common.Util.DependencyInjection;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.DependencyInjection
{
    internal sealed class ServicesModule : BaseServicesModule
    {
        public ServicesModule()
            : base(typeof(LinkstoreProviderService))
        { }
    }
}
