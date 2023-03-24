using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.RestApi.Core.Controllers;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.FrontEnd.Controllers
{
    [ApiController, Route("jumpseller")]
    public class JumpsellerController : BaseController
    {
        private readonly IJumpsellerService jumpsellerService;

        public JumpsellerController(IJumpsellerService jumpsellerService)
        {
            this.jumpsellerService = jumpsellerService;
        }

        [HttpGet("callback")]
        public async Task<ActionResult> AuthCallbackAsync([FromQuery] string code)
        {
            var result = await jumpsellerService.ExchangeCodeAsync(code);
            return result.IsSucceed()
                ? View("Success", default)
                : View("Error", default);
        }

        [HttpGet("entry")]
        public ActionResult EntryAsync([FromQuery] int? store_id)
        {
            var result = jumpsellerService.GetAuthorizeUrlAsync(store_id);
            return result.IsSucceed()
                ? (ActionResult)Redirect(result.Data)
                : View("Error", default);
        }
    }
}