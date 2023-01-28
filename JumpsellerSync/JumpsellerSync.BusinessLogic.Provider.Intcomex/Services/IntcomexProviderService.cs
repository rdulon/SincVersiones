using AutoMapper;

using Hangfire;

using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services;
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Dtos;
using JumpsellerSync.BusinessLogic.Provider.Intcomex.Options;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories.Intcomex;
using JumpsellerSync.Domain.Impl;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Services
{
    public class IntcomexProviderService
        : ProviderService<IntcomexProduct, IntcomexBrand, IntcomexConfiguration, IntcomexProviderService>
    {
        public const string INTCOMEX_HTTP_CLIENT = "IntcomexHttpClientId";

        private static readonly JsonSerializerOptions intcomexJSONSerializerOptions =
            new JsonSerializerOptions
            { PropertyNamingPolicy = new AsIsNamingPolicy() };
        private static readonly string[] productListSkipProperties =
            CreateSkipPropertiesArray<IntcomexProduct>(nameof(IntcomexProduct.Price),
                                                       nameof(IntcomexProduct.Stock),
                                                       nameof(IntcomexProduct.Brand),
                                                       nameof(IntcomexProduct.Category));

        private readonly ICurrencyConverterService currencyConverterService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IIntcomexProductRepository intcomexProductRepository;
        private readonly IIntcomexConfigRepository configRepository;
        private readonly IIntcomexBrandRepository brandRepository;
        private readonly IntcomexOptions options;

        public IntcomexProviderService(
            IBackgroundJobClient jobClient,
            ISynchronizeService.Factory synchronizeFactory,
            ICurrencyConverterService currencyConverterService,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IIntcomexProductRepository productRepository,
            IIntcomexConfigRepository configRepository,
            IIntcomexBrandRepository brandRepository,
            IOptions<IntcomexOptions> options,
            ILogger<IntcomexProviderService> logger)
            : base(synchronizeFactory, productRepository, jobClient, mapper, logger)
        {
            this.currencyConverterService = currencyConverterService;
            this.httpClientFactory = httpClientFactory;
            intcomexProductRepository = productRepository;
            this.configRepository = configRepository;
            this.brandRepository = brandRepository;
            this.options = options.Value;
        }

        protected override async Task<SynchronizationInfoDto> GetSyncInfoAsync(string logHeader)
        {
            var syncedProducts = Enumerable.Empty<SynchronizeProductDto>();
            var updated = await UpdateProductsAsync(logHeader);

            logger.LogInformation($"{logHeader} Provider synchronization finished.");
            if (updated)
            {
                productRepository.DbContext.DetachAll();
                var products = (await productRepository
                        .ReadSynchronizingToJumpsellerAsync()
                        .ToListAsync())
                    .Where(p => p.BrandId != "mik" && p.BrandId != "ubq")
                    .ToList();
                syncedProducts = mapper.Map<IEnumerable<SynchronizeProductDto>>(products);

                LogSyncedProducts(logHeader, products);
            }

            logger.LogInformation(
                $"{logHeader} Synchronizing with main info of {syncedProducts.Count()} products.");

            return new SynchronizationInfoDto
            {
                SyncComplete = true,
                Products = syncedProducts
            };
        }

        protected override async Task<bool> UpdateProductsAsync(string logHeader)
        {
            await UpdateProductCatalogAsync(logHeader);
            return await UpdateProductsListAsync(logHeader);
        }

        private async Task<bool> UpdateProductsListAsync(string logHeader)
        {
            logger.LogInformation($"{logHeader} Update products stock and price.");

            var conversionResult = await currencyConverterService.ConvertAsync("USD", "CLP");
            if (!conversionResult.IsSucceed())
            {
                logger.LogInformation(
                  $"{logHeader} Conversion from USD to CLP not available. Skip update.");
                return false;
            }
            var conversionFactor = conversionResult.Data.Value;
            logger.LogInformation($"{logHeader} Using conversion factor: {conversionFactor}");

            var skusBatchs = (await intcomexProductRepository
                    .ReadAsync(p => true, p => p.Id, 0, -1, p => p.IntcomexSku)
                    .ToListAsync())
                .ToBatchs(100)
                .Select(b => string.Join(',', b))
                .Where(skuList => !string.IsNullOrEmpty(skuList));

            var refreshClientTimestamp = TimeSpan.FromMinutes(4.5D);
            var processingStart = DateTime.Now;
            var client = httpClientFactory.CreateClient(INTCOMEX_HTTP_CLIENT);

            intcomexProductRepository.DbContext.DetachAll();
            var updated = false;
            foreach (var skuList in skusBatchs)
            {
                try
                {
                    var batchStart = DateTime.Now;
                    var q = new NameValueCollection
                    { { "skusList", skuList } };
                    var prodListUri = CreateApiRequestUri(
                        client.BaseAddress, options.Api.Endpoints.GetProducts, q);
                    var prodListResponse = await client.GetAsync(prodListUri);
                    prodListResponse.EnsureSuccessStatusCode();
                    var products = JsonSerializer.Deserialize<IEnumerable<IntcomexProductDto>>(
                            await prodListResponse.Content.ReadAsStringAsync(),
                            intcomexJSONSerializerOptions)
                        .ToDictionary(p => p.Sku);
                    if (products.Count > 0)
                    {
                        var dbProducts = mapper
                            .Map<IEnumerable<IntcomexProduct>>(products.Values)
                            .ForEach(p => p.Price = Math.Round(
                                products[p.IntcomexSku].Price.UnitPrice * conversionFactor,
                                2));
                        await Upsert(intcomexProductRepository,
                                     dbProducts,
                                     logHeader,
                                     productListSkipProperties);
                        updated = true;

                        List<string> productCodes = new List<string>();

                        foreach (var product in dbProducts)
                        {
                            productCodes.Add(product.ProductCode.TrimEnd('.'));
                        }

                        await ClearProducts(productCodes);
                    }

                    var batchLapse = DateTime.Now - batchStart;
                    if (batchLapse < TimeSpan.FromSeconds(0.5D))
                    { Thread.Sleep(TimeSpan.FromSeconds(0.5D) - batchLapse); }

                    if (DateTime.Now - processingStart >= refreshClientTimestamp)
                    {
                        client = httpClientFactory.CreateClient(INTCOMEX_HTTP_CLIENT);
                        processingStart = DateTime.Now;
                    }
                }
                catch (Exception e)
                { logger.LogException(e, logHeader); }
            }

            logger.LogInformation($"{logHeader} Stock and price update finished.");
            return updated;
        }

        private async Task UpdateProductCatalogAsync(string logHeader)
        {
            var lastUpdate = await configRepository.GetLastCatalogUpdateAsync();
            if (lastUpdate.OperationSucceed && lastUpdate.Data.Date != DateTime.Today)
            {
                logger.LogInformation($"{logHeader} Products catalog needs update.");

                try
                {
                    var client = httpClientFactory.CreateClient(INTCOMEX_HTTP_CLIENT);
                    var catalogUri = CreateApiRequestUri(
                        client.BaseAddress, options.Api.Endpoints.GetCatalog);
                    var catalogResponse = await client.GetAsync(catalogUri);
                    catalogResponse.EnsureSuccessStatusCode();
                    var catalog = JsonSerializer
                        .Deserialize<IEnumerable<IntcomexProductDto>>(
                            await catalogResponse.Content.ReadAsStringAsync(),
                            intcomexJSONSerializerOptions);
                    var prodCatalog = RemoveDuplicateProducts(
                        mapper.Map<IEnumerable<IntcomexProduct>>(catalog), logHeader);

                    await Upsert(brandRepository,
                                 prodCatalog.Select(p => p.Brand).Distinct(
                                     new DomainModelEqualityComparer<IntcomexBrand>()),
                                 logHeader);

                    await intcomexProductRepository.MarkProductsAsDirtyAsync();

                    await Upsert(intcomexProductRepository,
                                 prodCatalog,
                                 logHeader,
                                 nameof(IntcomexProduct.RedcetusProductId),
                                 nameof(IntcomexProduct.Price),
                                 nameof(IntcomexProduct.Stock));

                    await intcomexProductRepository.RemoveDirtyProductsAsync();

                    await configRepository.SetLastCatalogUpdateAsync(DateTime.Now);
                }
                catch (Exception e)
                {
                    logger.LogException(e, logHeader);
                    return;
                }
                logger.LogInformation($"{logHeader} Products catalog updated.");
            }

            logger.LogInformation($"{logHeader} Update product catalog finished.");
        }

        private IEnumerable<IntcomexProduct> RemoveDuplicateProducts(
            IEnumerable<IntcomexProduct> catalog, string header)
        {
            var products = new Dictionary<string, IntcomexProduct>();
            foreach (var product in catalog)
            {
                // Mark this product as non-dirty
                product.Dirty = false;
                if (products.TryGetValue(product.ProductCode, out var intcomexProduct))
                {
                    if (product.Price > intcomexProduct.Price)
                    {
                        intcomexProduct.Price = product.Price;
                        intcomexProduct.IntcomexSku = product.IntcomexSku;
                    }
                    else if (intcomexProduct.Price <= product.Price && //
                        intcomexProduct.Price >= product.Price &&      // intcomexProduct.Price == product.Price
                        product.Stock > intcomexProduct.Stock)
                    {
                        intcomexProduct.Stock = product.Stock;
                        intcomexProduct.IntcomexSku = product.IntcomexSku;
                    }

                    logger.LogInformation(
                        $"{header} Product \"{product.ProductCode}\" duplicated. " +
                        $"Updating Price: {intcomexProduct.Price} and Stock: {intcomexProduct.Stock}.");
                }
                else { products[product.ProductCode] = product; }
            }

            return products.Values;
        }

        private Uri CreateApiRequestUri(
            Uri baseUri, EndpointOption endpoint, NameValueCollection q = null)
        {
            q ??= new NameValueCollection();
            var parsedQ = HttpUtility.ParseQueryString(endpoint.Query ?? "");

            foreach (var name in q.AllKeys)
            {
                foreach (var value in q.GetValues(name))
                {
                    parsedQ.Add(name, value);
                }
            }

            return baseUri.Augment(endpoint.Path, parsedQ);
        }
    }
}

