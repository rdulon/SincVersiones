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
    [Route("api/brand")]
    public abstract class BrandController : ApiBaseController
    {
        protected readonly IProviderProductService providerProductService;
        protected readonly IMapper mapper;

        public BrandController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions,
            IMapper mapper)
            : base(jsonSerializerOptions)
        {
            this.providerProductService = providerProductService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> ReadBrandsAsync(
            [FromQuery] ReadPageDto q)
        {
            var brands = await providerProductService
                .ReadBrandsAsync(q.Page, q.Limit);

            return Json(new PageResultDto<ProviderBrandDetailsDto>
            {
                Items = brands,
                Page = q.Page,
                Limit = q.Limit
            });
        }

        [HttpGet("{brandId}")]
        public async Task<ActionResult> ReadBrandAsync(string brandId)
        {
            var brandResult = await providerProductService.ReadBrandAsync(brandId);
            return brandResult.IsSucceed()
                 ? Json(brandResult.Data, true)
                 : FromServiceResult(brandResult);
        }

        [HttpGet("{brandId}/unsync")]
        public async Task<ActionResult> ReadUnsyncedProductsAsync(
            string brandId, [FromQuery] LoadProductsPageViewModel q)
        {
            var search = mapper.Map<SearchProviderUnsyncedProductsDto>(q);
            search.BrandId = brandId;
            var page =
                await providerProductService.ReadUnsyncedProductsAsync(search);

            return Json(page);
        }

        [HttpGet("{brandId}/unsync/suggestions")]
        public async Task<ActionResult> SkuOrNameUnsyncedProductsSuggestionsAsync(
          string brandId, [FromQuery] ReadUnsyncedProductSuggestionsDto q)
        {
            q.BrandId = brandId;
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
    }
}
