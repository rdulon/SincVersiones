using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Linkstore.Controllers
{
    public sealed class LinkstoreSynchronizeController : SynchronizeController
    {
        public LinkstoreSynchronizeController(
            IProviderService providerService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(providerService, jsonSerializerOptions)
        { }
    }
}