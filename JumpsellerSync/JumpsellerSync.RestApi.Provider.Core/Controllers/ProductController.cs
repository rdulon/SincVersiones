using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Core.Controllers;
using JumpsellerSync.RestApi.Provider.Core.Models;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.Provider.Core.Controllers
{
    [ApiController]
    [Route("api/product")]
    public abstract class ProductController : ApiBaseController
    {
        private readonly IProviderProductService providerProductService;
        private readonly IMapper mapper;

        public ProductController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions,
            IMapper mapper)
            : base(jsonSerializerOptions)
        {
            this.providerProductService = providerProductService;
            this.mapper = mapper;
        }

        [HttpPut("{productId}")]
        public async Task<ActionResult> SynchronizeProductAsync(
            string productId, [FromBody] string redcetusId)
        {
            var syncResult = await providerProductService.SynchronizeProductAsync(
                productId, redcetusId);
            return FromServiceResult(syncResult);
        }

        [HttpPut("sku")]
        public async Task<ActionResult> SynchronizeProductsBySkuAsync(
            [FromBody] SynchronizeSkuDto info)
        {
            var result = await providerProductService.SynchronizeProductsBySkuAsync(info);
            return result.IsSucceed()
                ? Json(result.Data)
                : FromServiceResult(result);
        }


        [HttpGet("{productId}")]
        public async Task<ActionResult> ReadProductAsync(string productId)
        {
            var productResult = await providerProductService.ReadProductAsync(productId);
            return productResult.IsSucceed()
                ? Json(productResult.Data, true)
                : FromServiceResult(productResult);
        }

        [HttpGet("unsync")]
        public async Task<ActionResult> ReadUnsyncedProductsAsync(
            [FromQuery] LoadProductsPageViewModel q)
        {
            var page =
                await providerProductService.ReadUnsyncedProductsAsync(
                    mapper.Map<SearchProviderUnsyncedProductsDto>(q));

            return Json(page);
        }

        [HttpGet("unsync/suggestions")]
        public async Task<ActionResult> SkuOrNameUnsyncedProductsSuggestionsAsync(
            [FromQuery] ReadUnsyncedProductSuggestionsDto q)
        {
            var result =
                await providerProductService.ReadUnsyncedProductSuggestionsAsync(q);
            return result.IsSucceed()
                ? Json(new PageResultDto<ProductSuggestionDto>
                {
                    Items = result.Data,
                    Limit = q.SuggestionsLimit,
                    Page = 1
                })
                : FromServiceResult(result);
        }

        [HttpDelete("unsync/{redcetusId}")]
        public async Task<ActionResult> UnsynchronizeProductAsync(string redcetusId)
        {
            var result = await providerProductService.UnsynchronizeProductAsync(redcetusId);
            return FromServiceResult(result);
        }
    }
}
