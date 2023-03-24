
using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.RestApi.Core.Controllers;
using JumpsellerSync.RestApi.FrontEnd.Models;

using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.FrontEnd.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProviderService providerService;
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public ProductController(
            IProviderService providerService,
            IProductService productService,
            IMapper mapper)
        {
            this.providerService = providerService;
            this.productService = productService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var providers = await providerService.ReadActiveProvidersAsync();

            return View(providers);
        }

        [HttpGet]
        public async Task<IActionResult> PrefetchBrands()
        {
            if (IsAjaxRequest)
            {
                var brands = await productService.PrefetchBrandsAsync();
                return Json(brands.Data);
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> SkuOrNameProductSuggestions(
            [FromQuery] ReadProductSuggestionsViewModel q)
        {
            if (IsAjaxRequest)
            {
                var suggestionsResult = q.SyncedProducts
                    ? await productService.ReadProductSuggestionsAsync(
                        mapper.Map<ReadProductSuggestionsDto>(q))
                    : await providerService.ReadProductSuggestionsAsync(
                        mapper.Map<ReadProviderProductSuggestionsDto>(q));

                return Json(suggestionsResult);
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> UnsyncedProducts(
            [FromQuery] LoadUnsyncedProductsPageViewModel q)
        {
            if (IsAjaxRequest && ModelState.IsValid)
            {
                var page =
                    await providerService.ReadProductsAsync(mapper.Map<SearchUnsyncedProductsDto>(q));
                ViewBag.TotalPages = page.TotalPages;

                return page.Items.Count() == 0
                    ? (IActionResult)Content("")
                    : PartialView(page.Items);
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> SyncedProducts([FromQuery] LoadSyncedProductsPageViewModel q)
        {
            if (IsAjaxRequest)
            {
                var page = await productService.ReadAsync(mapper.Map<SearhProductsDto>(q));
                ViewBag.TotalPages = page.TotalPages;

                return page.Items.Count() == 0
                    ? (IActionResult)Content("")
                    : PartialView(page.Items);

            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> SyncProduct(
            [FromBody] CreateProductDto data)
        {
            if (ModelState.IsValid)
            {
                var result = await productService.CreateAsync(data);
                return FromServiceResult(result);
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var productResult = await productService.ReadAsync(id);
            return productResult.IsSucceed()
                ? View(productResult.Data)
                : FromServiceResult(productResult);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ProductDetailsDto product)
        {
            if (!ModelState.IsValid)
            { return View(product); }
            var updateResult = await productService.UpdateMarginAsync(product);
            return updateResult.IsSucceed()
                ? RedirectToAction("Index")
                : FromServiceResult(updateResult);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var productResult = await productService.ReadAsync(id);
            return productResult.IsSucceed()
                ? View(productResult.Data)
                : FromServiceResult(productResult);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete([FromForm] string id)
        {
            var unsyncAsync = await productService.DeleteAsync(id);
            return unsyncAsync.IsSucceed()
                ? RedirectToAction("Index")
                : FromServiceResult(unsyncAsync);
        }
    }
}