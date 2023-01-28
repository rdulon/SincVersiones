using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Core.Controllers;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.Provider.Core.Controllers
{
    [ApiController]
    [Route("api/sync")]
    public abstract class SynchronizeController : ApiBaseController
    {
        public SynchronizeController(
            IProviderService providerService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        {
            Provider = providerService;
        }

        protected IProviderService Provider { get; }

        [HttpPost]
        public virtual async Task<ActionResult> Sync([FromBody] StartSynchronizationDto syncInfo)
        {
            if (ModelState.IsValid)
            {
                await Provider.SynchronizeProductsAsync(syncInfo);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut("{productId}/{redcetusProductId}")]
        public virtual async Task<ActionResult> SyncWithRedcetus(string productId, string redcetusProductId)
        {
            var result = await Provider.SynchronizeWithRedcetusAsync(productId, redcetusProductId);

            return result.IsSucceed()
                ? Json(result.Data, true)
                : FromServiceResult(result);
        }
    }
}
