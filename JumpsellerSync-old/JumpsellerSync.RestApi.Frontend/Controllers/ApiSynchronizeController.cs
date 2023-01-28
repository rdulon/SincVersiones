using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.RestApi.Core.Controllers;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.FrontEnd.Controllers
{
    [Route("api/sync")]
    [ApiController]
    public class ApiSynchronizeController : ApiBaseController
    {
        private readonly ISynchronizeProvidersService providersService;

        public ApiSynchronizeController(
            ISynchronizeProvidersService providersService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        {
            this.providersService = providersService;
        }

        [HttpPost("products")]
        public async Task<ActionResult> Sync(
            [FromQuery] string syncSessionId,
            [FromBody] SynchronizationInfoDto syncInfo)
        {
            var result =
                await providersService.SynchronizeProviderProductsAsync(syncSessionId, syncInfo);
            return FromServiceResult(result);
        }

        [HttpPost("provider/{providerId}")]
        public async Task<ActionResult> SyncProviderAsync(string providerId)
        {
            var result =
                await providersService.SynchronizeProviderAsync(providerId);
            return FromServiceResult(result);
        }
    }
}