using AutoMapper;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.BusinessLogic.Filter;
using JumpsellerSync.BusinessLogic.Impl.Extensions;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static JumpsellerSync.BusinessLogic.Impl.Constants.HttpClientNames;
using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class ProviderService
        : BaseService<ProviderDto,
                      ProviderDto,
                      ProviderDto,
                      BaseProvider,
                      ProviderService>,
          IProviderService
    {
        private readonly IProductRepository productRepository;
        private readonly IBrandRepository brandRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        private static readonly string[] updateProductBrandSkipProperties =
            CreateSkipPropertiesArray<Product>(
                nameof(Product.Id), nameof(Product.BrandId),
                nameof(Product.Brand), nameof(Product.Categories),
                nameof(Product.LocalProduct));

        public ProviderService(
            IMapper mapper,
            ILogger<ProviderService> logger,
            IProviderRepository providerRepository,
            IProductRepository productRepository,
            IBrandRepository brandRepository,
            IDeleteRepository<BaseProvider> deleteRepository,
            DomainFilter<BaseProvider>.Factory domainFilterFactory,
            IHttpClientFactory httpClientFactory,
            JsonSerializerOptions jsonSerializerOptions)
            : base(
                  mapper, logger, providerRepository, providerRepository,
                  providerRepository, deleteRepository, domainFilterFactory)
        {
            this.productRepository = productRepository;
            this.brandRepository = brandRepository;
            this.httpClientFactory = httpClientFactory;
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        public async Task<IEnumerable<ProviderBrandDetailsDto>> ReadProviderBrandsAsync(
            string providerId)
        {
            var header = "ProviderService.ReadProviderBrands".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var path = "/api/brand";
            var query = "page=1&limit=-1";

            var result = await GetFromProviderAsync<ProviderBrandDetailsDto>(
                providerId, path, query, header);
            return result.Items;
        }

        public async Task<IEnumerable<ProviderDto>> ReadProvidersAsync()
        {
            var providers = await readRepository.ReadAsync(
                provider => true,
                provider => provider.Priority,
                0, -1).ToListAsync();

            return mapper.Map<IEnumerable<ProviderDto>>(providers);
        }

        public async Task<IEnumerable<ProviderDto>> ReadActiveProvidersAsync()
        {
            var providers = await readRepository.ReadAsync(
                provider => provider.Active,
                provider => provider.Priority,
                0, -1).ToListAsync();

            return mapper.Map<IEnumerable<ProviderDto>>(providers);
        }

        public async Task<ServiceResult<ProviderProductDetailsDto>> ReadProductAsync(
            string providerId, string providerProductId)
        {
            var header = "ProviderService.ReadProduct".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var providerResult = await ReadAsync(providerId);
            if (providerResult.IsSucceed())
            {
                var provider = providerResult.Data;
                var client = httpClientFactory.CreateClient(PROVIDER_API_HTTP_CLIENT);
                var requestUri = new UriBuilder(provider.Url)
                {
                    Path = $"/api/product/{providerProductId}"
                }.Uri;

                try
                {
                    var response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();

                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed(
                        JsonSerializer.Deserialize<ProviderProductDetailsDto>(
                            await response.Content.ReadAsStringAsync(), jsonSerializerOptions));
                }
                catch (Exception e)
                { logger.LogException(e, header); }
            }

            return ServiceResult.Error<ProviderProductDetailsDto>(
                "No se ha podido obtener el producto del proveedor.");

        }

        public Task<PageResultDto<ProviderProductDetailsDto>> ReadProductsAsync(SearchUnsyncedProductsDto search)
        {
            var header = "ProviderService.ReadUnsyncedProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var skuOrName = search.SkuOrName ?? "";
            var path = string.IsNullOrEmpty(search.BrandId)
                            ? "/api/product/unsync"
                            : $"/api/brand/{search.BrandId}/unsync";
            var withStock = $"{search.WithStock}".ToLower();
            var query = $"page={search.Page}&limit={search.Limit}&" +
                $"skuOrName={skuOrName}&withStock={withStock}";

            return GetFromProviderAsync<ProviderProductDetailsDto>(
                search.ProviderId, path, query, header);
        }

        public async Task<ServiceResult> SynchronizeProductAsync(
            string providerId, string providerProductId, string productId)
        {
            var header = "ProviderService.SynchronizeProduct".AsLogHeader();
            logger.LogInformation($"{header} Init.");


            var providerResult = await ReadAsync(providerId);
            if (providerResult.IsSucceed())
            {
                var provider = providerResult.Data;
                var client = httpClientFactory.CreateClient(PROVIDER_API_HTTP_CLIENT);
                var requestUri = new UriBuilder(provider.Url)
                {
                    Path = $"/api/product/{providerProductId}"
                }.Uri;
                var content = new StringContent(
                    JsonSerializer.Serialize(productId),
                    Encoding.UTF8,
                    MediaTypeNames.Application.Json);

                try
                {
                    var response = await client.PutAsync(requestUri, content);
                    response.EnsureSuccessStatusCode();

                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed();
                }
                catch (Exception e)
                { logger.LogException(e, header); }
            }

            return ServiceResult.Error(
                "No se ha podido sincronizar el producto del proveedor.");
        }

        public async Task<ServiceResult> UnsynchronizeProductAsync(string productId)
        {
            var header = "ProviderService.UnsynchronizeProduct".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var productResult = await productRepository.ReadAsync(new[] { productId });
            if (productResult.OperationSucceed)
            {
                var product = productResult.Data;
                if (product == null)
                {
                    logger.LogInformation($"{header} Product \"{productId}\" not found.");
                    return ServiceResult.NotFound();
                }

                var client = httpClientFactory.CreateClient(PROVIDER_API_HTTP_CLIENT);

                try
                {
                    var unsyncTasks = (await readRepository
                        .ReadAsync(p => p.Active, 0, -1)
                        .ToListAsync())
                        .Select(
                            p => new UriBuilder(p.Url)
                            {
                                Path = $"/api/product/unsync/{productId}"
                            }.Uri)
                        .Select(requestUri => UnsyncProductSafeAsync(client, requestUri, header))
                        .ToList();
                    await Task.WhenAll(unsyncTasks);

                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed();
                }
                catch (Exception e)
                { logger.LogException(e, header); }
            }

            return ServiceResult.Error(
                "No se ha podido dejar de sincronizar el producto.");
        }

        public async Task<ServiceResult> SynchronizeProductsBySkuAsync(
            IEnumerable<SynchronizeProductSkuDto> skuInfo)
        {
            var header = "ProviderService.SyncProductsBySku".AsLogHeader();
            logger.LogInformation($"{header} Init");

            var providers = await readRepository.ReadAsync(
                p => p.Active, 0, -1).ToListAsync();
            var allSynced = true;

            var client = httpClientFactory.CreateClient(PROVIDER_API_HTTP_CLIENT);
            var bodyObj = new SynchronizeSkuDto { SkuInfo = skuInfo };
            var body = new StringContent(
                JsonSerializer.Serialize(bodyObj, jsonSerializerOptions),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            foreach (var provider in providers)
            {
                try
                {
                    var requestUri = new UriBuilder(provider.Url)
                    { Path = $"/api/product/sku" }.Uri;

                    var syncResponse = await client.PutAsync(requestUri, body);
                    syncResponse.EnsureSuccessStatusCode();
                    var brandInfo = JsonSerializer.Deserialize<IEnumerable<ProviderBrandInfo>>(
                        await syncResponse.Content.ReadAsStringAsync(), jsonSerializerOptions);
                    await UpdateProductsBrandAsync(brandInfo);

                    logger.LogInformation($"{header} Provider {provider.Name} synchronized by SKU.");
                }
                catch (Exception e)
                {
                    logger.LogException(e, header);
                    allSynced = false;
                }
            }

            logger.LogInformation($"{header} End.");
            return allSynced
                 ? ServiceResult.Succeed()
                 : ServiceResult.Error();
        }

        public async Task<IEnumerable<ProductSuggestionDto>> ReadProductSuggestionsAsync(
            ReadProviderProductSuggestionsDto input)
        {
            var header = "ProviderService.ReadProductSuggestions".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var skuOrName = input.SkuOrName ?? "";
            var path = string.IsNullOrEmpty(input.BrandId)
                            ? "/api/product/unsync/suggestions"
                            : $"/api/brand/{input.BrandId}/unsync/suggestions";
            var withStock = $"{input.WithStock}".ToLower();
            var query = $"skuOrName={skuOrName}&" +
                $"suggestionsLimit={input.SuggestionsLimit}&withStock={withStock}";

            var result = await GetFromProviderAsync<ProductSuggestionDto>(
                input.ProviderId, path, query, header);
            return result.Items;
        }

        public override Task<ServiceResult<ProviderDto>> CreateAsync(ProviderDto createDto)
        {
            createDto.Id = null;
            return base.CreateAsync(createDto);
        }

        public override async Task<ServiceResult<ProviderDto>> UpdateAsync(ProviderDto provider)
        {
            var readResult = await readRepository.ReadAsync(new[] { provider?.Id });
            if (readResult.OperationSucceed && readResult.Data != null)
            {
                var dbProvider = readResult.Data;
                var model = mapper.Map<BaseProvider>(provider);
                model.NextSynchronization = dbProvider.NextSynchronization;

                using var tx = createRepository.DbContext.BeginTransaction();
                await deleteRepository.DeleteAsync(dbProvider);
                var result = await createRepository.CreateAsync(model);
                await tx.CommitAsync();

                return result.ToServiceResult<BaseProvider, ProviderDto>(mapper);
            }
            return readResult.ToServiceResult<BaseProvider, ProviderDto>(mapper);
        }

        private async Task UpdateProductsBrandAsync(IEnumerable<ProviderBrandInfo> brandInfo)
        {
            var providerBrandNames = brandInfo.ToDictionary(
                                    info => info.Brand.Description.ToUpper());
            var brandsDb = await brandRepository.ReadAsync(
                    brand => providerBrandNames.Keys.Contains(brand.NormalizedName), 0, -1)
                .ToDictionaryAsync(brand => brand.NormalizedName);
            var newBrands = providerBrandNames
                .Where(kv => !brandsDb.ContainsKey(kv.Key))
                .Select(kv => mapper.Map<Brand>(kv.Value.Brand))
                .ToDictionary(brand => brand.NormalizedName);
            var allBrands = brandsDb.Concat(newBrands).ToDictionary(kv => kv.Key);

            brandRepository.DbContext.DetachAll();
            var tx = brandRepository.DbContext.BeginTransaction();
            var createResult = await brandRepository.CreateAsync(newBrands.Values);
            var products = providerBrandNames.SelectMany(
                kv => kv.Value.RedcetusIds.Select(id => new Product
                {
                    Id = id,
                    BrandId = allBrands[kv.Key].Value.Id
                }));
            var updateProductsResult = await productRepository.UpdateAsync(
                products, skipProperitesUpdate: updateProductBrandSkipProperties);
            if (createResult.OperationSucceed && updateProductsResult.OperationSucceed)
            { await tx.CommitAsync(); }
            else
            { await tx.RollbackAsync(); }
        }

        private async Task<PageResultDto<TDto>> GetFromProviderAsync<TDto>(
           string providerId, string path, string query, string header)
            where TDto : class
        {
            logger.LogInformation($"{header} Init.");

            try
            {
                var providerResult = await ReadAsync(providerId);
                if (providerResult.IsSucceed())
                {
                    var provider = providerResult.Data;
                    logger.LogInformation(
                        $"{header} Provider found and is{(provider.Active ? "" : " not")} active");
                    if (provider.Active)
                    {
                        var client = httpClientFactory.CreateClient(PROVIDER_API_HTTP_CLIENT);
                        var requestUri = new UriBuilder(provider.Url)
                        {
                            Path = path,
                            Query = query
                        }.Uri;
                        var response = await client.GetAsync(requestUri);
                        response.EnsureSuccessStatusCode();
                        var result = JsonSerializer
                            .Deserialize<PageResultDto<TDto>>(
                                await response.Content.ReadAsStringAsync(), jsonSerializerOptions);
                        logger.LogInformation($"{header} End.");

                        return result;
                    }
                    throw new Exception($"Provider \"{providerId}\" is not active.");
                }
            }
            catch (Exception e)
            { logger.LogException(e, header); }

            return new PageResultDto<TDto>
            {
                Items = Enumerable.Empty<TDto>(),
                Limit = 10,
                Page = 1
            };
        }

        private async Task UnsyncProductSafeAsync(HttpClient client, Uri requestUri, string logHeader)
        {
            try
            { await client.DeleteAsync(requestUri); }
            catch (Exception e)
            { logger.LogException(e, logHeader); }

        }
    }
}
