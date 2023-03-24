using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.RestApi.Core.Controllers;
using JumpsellerSync.RestApi.FrontEnd.Models;

using Microsoft.AspNetCore.Mvc;

using System.Linq;
using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.FrontEnd.Controllers
{
    public class LocalProductController : BaseController
    {
        private readonly ILocalProductService localProductService;
        private readonly IProductService productService;
        private readonly IMapper mapper;

        public LocalProductController(
            ILocalProductService localProductService,
            IProductService productService,
            IMapper mapper)
        {
            this.localProductService = localProductService;
            this.productService = productService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var brandsResult = await productService.PrefetchBrandsAsync();

            return brandsResult.IsSucceed()
                ? View(brandsResult.Data)
                : FromServiceResult(brandsResult);
        }

        [HttpGet]
        public async Task<IActionResult> Products([FromQuery] ReadPageDto q)
        {
            if (IsAjaxRequest)
            {
                var page = await localProductService.ReadAsync(default, q.Page, q.Limit);

                ViewBag.TotalPages = page.TotalPages;
                return page.Items.Count() == 0
                    ? (IActionResult)Content("")
                    : PartialView(page.Items);
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> SyncedProducts([FromQuery] SearchSyncedProductsViewModel q)
        {
            if (IsAjaxRequest)
            {
                var products = await localProductService.SearchSyncedProductsAsync(
                    q?.BrandId, q?.SkuOrName, q?.Limit ?? 10);

                return products.Count() == 0
                    ? (IActionResult)Content("")
                    : PartialView(products);

            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocalProductsViewModel body)
        {
            if (ModelState.IsValid)
            {
                var result = await localProductService.CreateAsync(body.Products);
                return FromServiceResult(result);
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var readLocalProductResult = await localProductService.ReadAsync(id);

            return readLocalProductResult.IsSucceed()
                ? View(readLocalProductResult.Data)
                : FromServiceResult(readLocalProductResult);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDelete([FromForm] string id)
        {
            var deleteResult = await localProductService.DeleteAsync(id);
            return deleteResult.IsSucceed()
                ? RedirectToAction(nameof(Index))
                : FromServiceResult(deleteResult);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateLocalProductsViewModel body)
        {
            if (ModelState.IsValid)
            {
                var updateResult = await localProductService.UpdateAsync(body.Products);
                return FromServiceResult(updateResult);
            }

            return BadRequest();
        }
    }
}
