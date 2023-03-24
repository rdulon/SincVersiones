using JumpsellerSync.BusinessLogic.Provider.Intcomex.Services;
using JumpsellerSync.Common.Util.DependencyInjection;

namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.DependencyInjection
{
    internal sealed class ServicesModule : BaseServicesModule
    {
        public ServicesModule()
            : base(typeof(IntcomexProviderService))
        { }
    }
}
