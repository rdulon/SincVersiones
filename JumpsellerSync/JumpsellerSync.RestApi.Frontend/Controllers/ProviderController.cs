using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.RestApi.Core.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JumpsellerSync.RestApi.FrontEnd.Controllers
{
    public class ProviderController : BaseController
    {
        private readonly IProviderService providerService;
        private readonly ISynchronizeProvidersService synchronizeProvidersService;

        public ProviderController(
            IProviderService providerService,
            ISynchronizeProvidersService synchronizeProvidersService)
        {
            this.providerService = providerService;
            this.synchronizeProvidersService = synchronizeProvidersService;
        }

        public async Task<IActionResult> Index()
        {
            var providers = await providerService.ReadProvidersAsync();

            return View(providers);
        }

        public async Task<IActionResult> Brands([FromQuery] string providerId)
        {
            var brands = await providerService.ReadProviderBrandsAsync(providerId);

            if (IsAjaxRequest)
            {
                return brands.Count() == 0
                    ? (IActionResult)Content("")
                    : PartialView(brands);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Sync(string id)
        {
            var syncResult =
                await synchronizeProvidersService.SynchronizeProviderAsync(id);

            return FromServiceResult(syncResult);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ProviderTypes = ProviderTypes(ProviderType.HourlyProvider);
            return View(
                new ProviderDto
                {
                    Active = true,
                    ProviderType = ProviderType.HourlyProvider,
                    Priority = 1,
                    Interval = 2,
                    Hours = new List<string> { "07:00", "19:00" },
                    StartTime = "19:00"
                });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProviderDto provider)
        {
            if (ModelState.IsValid)
            {
                var createResult = await providerService.CreateAsync(provider);
                if (createResult.IsSucceed())
                { return RedirectToAction("Index"); }

                ModelState.AddModelError("", "Ha ocurrido un error mientras se creaba el proveedor.");
            }

            ConfigBadProviderInput(provider);
            return View(provider);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var readResult = await providerService.ReadAsync(id);
            if (readResult.IsSucceed())
            {
                var provider = readResult.Data;
                ViewBag.ProviderTypes = ProviderTypes(provider.ProviderType);
                return View(provider);
            }
            return FromServiceResult(readResult);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] ProviderDto provider)
        {
            if (ModelState.IsValid)
            {
                var createResult = await providerService.UpdateAsync(provider);
                if (createResult.IsSucceed())
                { return RedirectToAction("Index"); }

                ModelState.AddModelError("", "Ha ocurrido un error mientras se actualizaba el proveedor.");
            }

            ConfigBadProviderInput(provider);
            return View(provider);
        }

        private void ConfigBadProviderInput(ProviderDto provider)
        {
            AddHoursError();
            if (!Enum.IsDefined(typeof(ProviderType), provider.ProviderType))
            { provider.ProviderType = ProviderType.HourlyProvider; }
            ViewBag.ProviderTypes = ProviderTypes(provider.ProviderType);
        }

        private static List<SelectListItem> ProviderTypes(ProviderType selected)
            => new List<SelectListItem>
            {
                new SelectListItem(
                    "Periódica",
                    ((int)ProviderType.PeriodicallyProvider).ToString(),
                    selected == ProviderType.PeriodicallyProvider),
                new SelectListItem(
                    "Por horas",
                    ((int)ProviderType.HourlyProvider).ToString(),
                    selected == ProviderType.HourlyProvider),
            };

        private void AddHoursError()
        {
            var p = nameof(ProviderDto.Hours);
            if (ModelState
                    .FindKeysWithPrefix(p)
                    .Where(kv => Regex.IsMatch(kv.Key, $@"{p}\[\d+\]"))
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .Count() > 0)
            {
                ModelState.AddModelError(
                    nameof(ProviderDto.Hours),
                    "El formato de las horas es 23:59.");
            }
        }
    }
}