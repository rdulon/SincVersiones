using AutoMapper;

using Hangfire;

using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services;
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.BusinessLogic.Provider.Nexsys.Dtos;
using JumpsellerSync.BusinessLogic.Provider.Nexsys.Options;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories.Nexsys;
using JumpsellerSync.Domain.Impl;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.Services
{
    public class NexsysProviderService
        : ProviderService<NexsysProduct, NexsysBrand, NexsysConfiguration, NexsysProviderService>
    {
        private readonly INexsysProductRepository nexsysProductRepository;
        private readonly INexsysCategoryRepository categoryRepository;
        private readonly INexsysBrandRepository brandRepository;
        private readonly ICurrencyConverterService currencyConverterService;
        private readonly NexsysServiceSoapClient serviceSoapClient;
        private readonly webServiceClient webServiceClient;
        private readonly string brandsFilename;

        public NexsysProviderService(
            IBackgroundJobClient jobClient,
            ISynchronizeService.Factory synchronizeFactory,
            IMapper mapper,
            INexsysProductRepository productRepository,
            INexsysCategoryRepository categoryRepository,
            INexsysBrandRepository brandRepository,
            IOptions<NexsysOptions> options,
            ICurrencyConverterService currencyConverterService,
            NexsysServiceSoapClient serviceSoapClient,
            webServiceClient webServiceClient,
            ILogger<NexsysProviderService> logger)
            : base(synchronizeFactory, productRepository, jobClient, mapper, logger)
        {
            nexsysProductRepository = productRepository;
            this.categoryRepository = categoryRepository;
            this.brandRepository = brandRepository;
            this.currencyConverterService = currencyConverterService;
            this.serviceSoapClient = serviceSoapClient;
            this.webServiceClient = webServiceClient;
            brandsFilename = options.Value.BrandsFilename;

            if (!File.Exists(brandsFilename))
            { throw new ArgumentException(nameof(options.Value.BrandsFilename)); }
        }

        protected override async Task<bool> UpdateProductsAsync(string logHeader)
        {
            try
            {
                await SyncBrandsAsync(logHeader);
                var brands = await brandRepository
                    .ReadAsync(b => true, b => b.Id, 0, -1, b => b.NexsysId)
                    .ToListAsync();

                var conversionResult = await currencyConverterService.ConvertAsync("USD", "CLP");
                if (!conversionResult.IsSucceed())
                {
                    logger.LogInformation(
                      $"{logHeader} Conversion from USD to CLP not available. Skip update.");
                    return false;
                }
                var conversionFactor = conversionResult.Data.Value;
                List<string> productCodes = new List<string>();
                logger.LogInformation($"{logHeader} Using conversion factor: {conversionFactor}");

                foreach (var brand in brands)
                {
                    productRepository.DbContext.DetachAll();

                    try
                    {
                        logger.LogInformation(
                                        $"{logHeader} Pulling info for mark \"{brand}\".");

                        var response = await serviceSoapClient
                            .StoreProductByMarksAsync(new[] { brand }, webServiceClient);

                        var products = GetValidProducts(
                            mapper.Map<IEnumerable<NexsysProduct>>(response.@return),
                            conversionFactor,
                            brand,
                            logHeader);

                        var categories = FlatCategories(products);
                        await Upsert(categoryRepository, categories, logHeader);

                        await Upsert(nexsysProductRepository,
                                     products,
                                     logHeader,
                                     new[] { nameof(NexsysProduct.RedcetusProductId) });

                        if (products != null)
                        {
                            foreach (var product in products)
                            {
                                productCodes.Add(product.ProductCode.TrimEnd('.'));
                            }
                        }

                        logger.LogInformation($"{logHeader} Info for mark \"{brand}\" is now up-to-date.");
                    }
                    catch (Exception e)
                    {
                        logger.LogException(
                          e,
                          $"{logHeader} Updating info for mark \"{brand}\" ended up with error");
                    }
                }

                await ClearProducts(productCodes);

                logger.LogInformation($"{logHeader} Nexsys update finished.");
                return true;

            }
            catch (Exception e)
            { logger.LogException(e, logHeader); }

            return false;
        }

        private IEnumerable<NexsysProduct> GetValidProducts(
            IEnumerable<NexsysProduct> products, double conversionFactor, string curMark, string logHeader)
        {
            var duplicates = products
                .GroupBy(p => p.ProductCode)
                .Select(g => new { SKU = g.Key, Count = g.Count() })
                .Where(r => r.Count > 1)
                .Select(r => r.SKU)
                .ToHashSet();

            logger.LogInformation($"{logHeader} Found {duplicates.Count} duplicate SKU for mark \"{curMark}\"");

            var nonDuplicate = products.Where(p => !duplicates.Contains(p.ProductCode)).ToList();
            nonDuplicate.ForEach(product => product.Price = Math.Round(product.Price * conversionFactor, 2));
            return nonDuplicate;
        }

        private IEnumerable<NexsysCategory> FlatCategories(IEnumerable<NexsysProduct> products)
        {
            var categories = new HashSet<NexsysCategory>(
                new DomainModelEqualityComparer<NexsysCategory>());

            foreach (var product in products)
            { categories.AddRange(FlatCategories(product)); }

            return categories;
        }

        private IEnumerable<NexsysCategory> FlatCategories(NexsysProduct product)
        {
            var cur = product?.Category;
            while (cur != null)
            {
                yield return cur;
                cur = cur.SubCategory;
            }
        }

        private async Task SyncBrandsAsync(string logHeader)
        {
            try
            {
                logger.LogInformation($"{logHeader} Loading brands from file.");
                if (!File.Exists(brandsFilename))
                { return; }

                using var brandsFile = new StreamReader(brandsFilename);
                var brands = mapper.Map<IEnumerable<NexsysBrand>>(
                    JsonSerializer.Deserialize<IEnumerable<BrandDto>>(brandsFile.ReadToEnd()));
                await Upsert(brandRepository, brands, logHeader);
                logger.LogInformation($"{logger} Finished loading brands from file.");
            }
            catch (Exception e)
            { logger.LogException(e, logHeader); }
        }
    }
}
