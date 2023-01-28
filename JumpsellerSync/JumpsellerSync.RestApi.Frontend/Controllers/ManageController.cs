using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.RestApi.Core.Controllers;

using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.FrontEnd.Controllers
{
    public class ManageController : BaseController
    {
        private readonly IVtexService vtexService;

        public ManageController(IVtexService vtexService)
        {
            this.vtexService = vtexService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadJumpsellerProducts()
        {
            if (IsAjaxRequest)
            {
                var result = await vtexService.LoadProductsAsync();
                return PartialView("LoadResult", result.Data);
            }
            return BadRequest();
        }
    }
}
