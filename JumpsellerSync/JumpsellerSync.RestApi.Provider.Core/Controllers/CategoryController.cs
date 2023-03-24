using JumpsellerSync.BusinessLogic.Core.Dtos;
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
    [Route("api/category")]
    public abstract class CategoryController : ApiBaseController
    {
        private readonly IProviderProductService providerProductService;

        public CategoryController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        {
            this.providerProductService = providerProductService;
        }

        [HttpGet]
        public async Task<ActionResult> ReadCategoriesAsync(
            [FromQuery] ReadPageDto q)
        {
            var categories = await providerProductService
                .ReadCategoriesAsync(q.Page, q.Limit);

            return Json(new PageResultDto<ProviderCategoryDetailsDto>
            {
                Items = categories,
                Page = q.Page,
                Limit = q.Limit
            });
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult> ReadCategoryAsync(string categoryId)
        {
            var categoryResult = await providerProductService.ReadCategoryAsync(categoryId);
            return categoryResult.IsSucceed()
                 ? Json(categoryResult.Data, true)
                 : FromServiceResult(categoryResult);
        }
    }
}
