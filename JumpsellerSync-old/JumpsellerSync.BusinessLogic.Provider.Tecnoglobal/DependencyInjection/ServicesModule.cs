using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Services;
using JumpsellerSync.Common.Util.DependencyInjection;

namespace JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.DependencyInjection
{
    internal sealed class ServicesModule : BaseServicesModule
    {
        public ServicesModule()
            : base(typeof(TecnoglobalProviderService))
        { }
    }
}
