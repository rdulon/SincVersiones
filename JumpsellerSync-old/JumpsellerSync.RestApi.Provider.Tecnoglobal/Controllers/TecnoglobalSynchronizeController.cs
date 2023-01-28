
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Tecnoglobal.Controllers
{
    public sealed class TecnoglobalSynchronizeController : SynchronizeController
    {
        public TecnoglobalSynchronizeController(
            IProviderService providerService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(providerService, jsonSerializerOptions)
        { }
    }
}