using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Intcomex.Controllers
{
    public sealed class IntcomexSynchronizeController : SynchronizeController
    {
        public IntcomexSynchronizeController(
            IProviderService providerService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(providerService, jsonSerializerOptions)
        { }
    }
}