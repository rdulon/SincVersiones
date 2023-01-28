using AutoMapper;

using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos.Jumpseller;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.BusinessLogic.Impl.Extensions;
using JumpsellerSync.BusinessLogic.Impl.Options;
using JumpsellerSync.BusinessLogic.Impl.Util;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.Domain.Impl;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using static JumpsellerSync.BusinessLogic.Impl.Constants.HttpClientNames;
using static JumpsellerSync.Common.Util.Services.ServiceUtilities;


namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class JumpsellerService : IJumpsellerService
    {
        private static readonly JumpsellerJsonNamingPolicy jumpsellerJsonNamingPolicy
            = new JumpsellerJsonNamingPolicy();
        private static readonly JsonSerializerOptions jsonJumpsellerSerializerOptions
            = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = false,
                ReadCommentHandling = JsonCommentHandling.Skip,
                PropertyNamingPolicy = jumpsellerJsonNamingPolicy
            };
        // Process a page of products in about a minute
        private const int PRODUCTS_PAGE_SIZE = 210;
        private static readonly string[] jsUpdateSkipProductProperties
            = CreateSkipPropertiesArray<Product>(
                nameof(Product.Id), nameof(Product.JumpsellerId),
                nameof(Product.SynchronizedToJumpseller),
                nameof(Product.Categories), nameof(Product.Brand),
                nameof(Product.LocalProduct));
        private static readonly string jsProductsFields = string.Join(
                ',',
                typeof(JumpsellerProductDto)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(pi => pi.CanWrite && pi.CanRead)
                    .Select(pi => pi.Name)
                    .Select(name => jumpsellerJsonNamingPolicy.ConvertName(name)));

        private readonly IProviderService providerService;
        private readonly IProductRepository productRepository;
        private readonly ILocalProductRepository localProductRepository;
        private readonly IBrandRepository brandRepository;
        private readonly IMainConfigRepository configRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMapper mapper;
        private readonly JumpsellerOptions jumpsellerOptions;
        private readonly MainOptions mainOptions;
        private readonly ILogger<JumpsellerService> logger;
        private readonly string productsEndpoint;

        public JumpsellerService(
            IProviderService providerService,
            IProductRepository productRepository,
            ILocalProductRepository localProductRepository,
            IBrandRepository brandRepository,
            IMainConfigRepository configRepository,
            IHttpClientFactory httpClientFactory,
            IOptions<JumpsellerOptions> jumpsellerOptions,
            IOptions<MainOptions> mainOptions,
            IMapper mapper,
            ILogger<JumpsellerService> logger)
        {
            this.providerService = providerService;
            this.productRepository = productRepository;
            this.localProductRepository = localProductRepository;
            this.brandRepository = brandRepository;
            this.configRepository = configRepository;
            this.httpClientFactory = httpClientFactory;
            this.mapper = mapper;
            this.jumpsellerOptions = jumpsellerOptions.Value;
            this.mainOptions = mainOptions.Value;
            this.logger = logger;
            productsEndpoint = this.jumpsellerOptions.Api.Endpoints.Products;
        }

        // Dependency as property so autofac is able to handle circular dependency 
        public IProductService ProductService { get; set; }

        public async Task<ServiceResult> ExchangeCodeAsync(string code)
        {
            var header = "JumpsellerService.ExchangeCode".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var serviceResult = ServiceResult.Error();

            var result = await RetrieveAndSaveTokensAsync(
                new
                {
                    jumpsellerOptions.Auth.ClientId,
                    jumpsellerOptions.Auth.ClientSecret,
                    GrantType = "authorization_code",
                    Code = code,
                    RedirectUri = AuthCallbackUrl
                },
                header);

            var authInfoResult = await configRepository.ReadJumpsellerAuthInfoAsync();
            if (authInfoResult.OperationSucceed)
            {
                authInfoResult.Data.ApplicationAuthorized = result.IsSucceed();

                var saveResult =
                    await configRepository.SaveJumpsellerAuthorizationInfoAsync(authInfoResult.Data);
                if (saveResult.OperationSucceed)
                { serviceResult = ServiceResult.Succeed(); }
            }

            logger.LogInformation(
                $"{header} Exchange code ended. " +
                $"Success: {result.IsSucceed()}");
            return serviceResult;
        }

        public async Task SynchronizeProductsAsync()
        {
            var header = "JumpsellerService.SynchronizeProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var syncCount = await productRepository.CountProductsToSynchronizeAsync();
            var pages = (syncCount / PRODUCTS_PAGE_SIZE) + (syncCount % PRODUCTS_PAGE_SIZE == 0 ? 0 : 1);
            var curPage = 1;
            var actualSynced = 0;

            while (curPage <= pages)
            {
                var productsToSync = await productRepository
                    .ReadProductsToSynchronizeAsync(0, PRODUCTS_PAGE_SIZE)
                    .ToListAsync();
                logger.LogInformation(
                    $"{header} Products (Page: {curPage++}) to synchronize loaded. Count: {productsToSync.Count}");
                LogProductsInfo(header, productsToSync);

                var createClientResult = await CreateApiClientAsync(header);
                if (createClientResult.IsSucceed())
                {
                    logger.LogInformation($"{header} Http client created.");
                    var client = createClientResult.Data;
                    var requestMap = CreateRequestMap(productsToSync, client.BaseAddress);
                    logger.LogInformation($"{header} Jumpseller request batch created. Executing...");

                    var processedProducts = new List<Product>(PRODUCTS_PAGE_SIZE);
                    await BatchRequestAsync<JumpsellerProductWrapperDto>(
                        client,
                        requestMap,
                        (id, dto) =>
                        {
                            var p = mapper.Map<Product>(dto);
                            p.Id = id;
                            processedProducts.Add(p);
                            return Task.CompletedTask;
                        },
                        header,
                        error: async (id, request, response) =>
                        {
                            if (request.Method == HttpMethod.Put &&
                                response.StatusCode == HttpStatusCode.NotFound)
                            {
                                logger.LogInformation(
                                    $"{header} Product \"{id}\" doesn't exist in Jumpseller anymore.");

                                var product = new Product { Id = id };

                                var unsyncResult = await providerService.UnsynchronizeProductAsync(product.Id);
                                logger.LogInformation(
                                    $"{header} Product \"{id}\" unsynced? {unsyncResult.YesNo()}");
                                var deleteResult = await productRepository.DeleteAsync(product);
                                logger.LogInformation(
                                    $"{header} Product \"{id}\" deleted? {deleteResult.YesNo()}");
                            }
                        });
                    logger.LogInformation($"{header} Batch requests finished.");
                    productRepository.DbContext.DetachAll();
                    using var tx = productRepository.DbContext.BeginTransaction();
                    var updateResult = await productRepository.UpdateAsync(
                        processedProducts, skipProperitesUpdate: jsUpdateSkipProductProperties);
                    if (updateResult.OperationSucceed)
                    {
                        await tx.CommitAsync();
                        logger.LogInformation($"{header} Products updated.");
                        actualSynced += updateResult.Data;
                    }
                    else
                    { await tx.RollbackAsync(); }
                }
            }
            logger.LogInformation(
                $"{header} Synchronization to Jumpseller finished ({actualSynced}/{syncCount}).");
            logger.LogInformation($"{header} End.");
        }

        public async Task SynchronizeLocalProductsAsync()
        {
            var header = "JumpsellerService.SyncLocalProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                var productsToSyncCount = await localProductRepository.CountAsync();
                var pages = (productsToSyncCount / PRODUCTS_PAGE_SIZE) +
                            (productsToSyncCount % PRODUCTS_PAGE_SIZE == 0 ? 0 : 1);
                var curPage = 1;

                while (curPage <= pages)
                {
                    logger.LogInformation($"{header} Processing page {curPage}.");
                    var offset = curPage.GetDbOffset(PRODUCTS_PAGE_SIZE);
                    var productsMap = (await localProductRepository
                                        .ReadAsync(p => true, p => p.Id, offset, PRODUCTS_PAGE_SIZE)
                                        .ToListAsync())
                                    .ToDictionary(p => p.Id);
                    await SyncLocalProductsToJumpsellerAsync(header, productsMap);

                    logger.LogInformation($"{header} Page {curPage} processed.");
                    curPage++;
                }
                logger.LogInformation($"{header} End.");
            }
            catch (Exception e)
            { logger.LogException(e, header); }
        }

        public async Task SynchronizeLocalProductsAsync(IEnumerable<string> localProductIds)
        {
            var header = "JumpsellerService.SyncLocalProductIds".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            try
            {
                localProductIds ??= new string[0];
                var productsMap = (await localProductRepository
                                        .ReadAsync(p => localProductIds.Contains(p.Id), 0, -1)
                                        .ToListAsync())
                                    .ToDictionary(p => p.Id);
                await SyncLocalProductsToJumpsellerAsync(header, productsMap);

                logger.LogInformation($"{header} End.");
            }
            catch (Exception e)
            { logger.LogException(e, header); }
        }

        public ServiceResult<string> GetAuthorizeUrlAsync(int? storeId)
        {
            var header = "JumpsellerService.EntryPoint".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var client = httpClientFactory.CreateClient(JUMPSELLER_AUTH_HTTP_CLIENT);
            var clientId = Uri.EscapeDataString(jumpsellerOptions.Auth.ClientId);
            var redirectUri = Uri.EscapeDataString(AuthCallbackUrl);
            var scopes = Uri.EscapeDataString(string.Join(' ', jumpsellerOptions.Auth.RequestScopes));

            var addrBuilder = new UriBuilder(client.BaseAddress)
            {
                Query = string.Join(
                    '&',
                    new[] {
                            $"client_id={clientId}", $"redirect_uri={redirectUri}",
                            "response_type=code", $"scope={scopes}"
                    }),
                Path = jumpsellerOptions.Auth.Endpoints.Authorize
            };

            return ServiceResult.Succeed(addrBuilder.Uri.ToString());
        }

        public async Task<ServiceResult> SynchronizeProductAsync(string productId)
        {
            var header = "JumpsellerService.SynchronizeProduct".AsLogHeader();
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

                var updateDataResult = await UpdateProductFromJumpsellerAsync(product, header);
                if (!updateDataResult.IsSucceed())
                { return updateDataResult; }

                var clientResult = await CreateApiClientAsync(header);
                if (clientResult.IsSucceed())
                {
                    var client = clientResult.Data;
                    var requestMap = CreateRequestMap(new List<Product> { product }, client.BaseAddress);

                    var synced = false;
                    await BatchRequestAsync<JumpsellerProductWrapperDto>(
                        client, requestMap,
                        async (id, dto) =>
                        {
                            productRepository.DbContext.DetachAll();
                            using var tx = productRepository.DbContext.BeginTransaction();
                            mapper.Map(dto, product, typeof(JumpsellerProductWrapperDto), typeof(Product));
                            var updateResult = await productRepository.UpdateAsync(
                                product, skipProperitesUpdate: jsUpdateSkipProductProperties);
                            if (updateResult.OperationSucceed)
                            {
                                await tx.CommitAsync();
                                synced = true;
                            }
                            else
                            { await tx.RollbackAsync(); }
                        },
                        header);

                    if (synced)
                    {
                        logger.LogInformation($"{header} End.");
                        return ServiceResult.Succeed();
                    }
                }
            }

            logger.LogInformation(
                $"{header} Product \"{productId}\" no sincronizado.");
            return ServiceResult.Error(
                "No es posible guardar/actualizar este producto en la tienda de " +
                "Jumpseller. Inténtelo otra vez en unos minutos.");
        }

        public async Task<ServiceResult<JumpsellerProductWrapperDto>> ReadProductAsync(
            int jumpsellerProductId)
        {
            var header = "JumpsellerService.ReadProduct".AsLogHeader();
            logger.LogInformation($"{header} Init");

            var clientResult = await CreateApiClientAsync(header);
            if (clientResult.IsSucceed())
            {
                var client = clientResult.Data;
                var requestUri = CreateApiRequestUri(
                    client.BaseAddress, $"{productsEndpoint}/{jumpsellerProductId}");
                try
                {
                    var response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();

                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed(
                        JsonSerializer.Deserialize<JumpsellerProductWrapperDto>(
                            await response.Content.ReadAsStringAsync(), jsonJumpsellerSerializerOptions));
                }
                catch (Exception e)
                { logger.LogException(e, header); }
            }

            logger.LogInformation(
                $"{header} Jumpseller product \"{jumpsellerProductId}\" not read.");
            return ServiceResult.Error<JumpsellerProductWrapperDto>(
                "No se pudo leer el producto de la tienda de Jumpseller. " +
                "Inténtelo otra vez en unos minutos.");
        }

        public async Task<ServiceResult<JumpsellerProductWrapperDto>> ReadProductBySkuAsync(string sku)
        {
            var header = "JumpsellerService.ReadProductBySku".AsLogHeader();
            logger.LogInformation($"{header} Init");

            var clientResult = await CreateApiClientAsync(header);
            if (clientResult.IsSucceed())
            {
                var client = clientResult.Data;
                var q = new NameValueCollection
                {
                    ["query"] = sku,
                    ["fields"] = jsProductsFields
                };
                var requestUri = CreateApiRequestUri(
                    client.BaseAddress, $"{productsEndpoint}/search", q);

                try
                {
                    var response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode();

                    logger.LogInformation($"{header} End.");
                    var products =
                        JsonSerializer.Deserialize<List<JumpsellerProductWrapperDto>>(
                            await response.Content.ReadAsStringAsync(), jsonJumpsellerSerializerOptions);
                    var product = products?.FirstOrDefault(p => p.Product.Sku == sku);

                    return product == null
                        ? ServiceResult.NotFound<JumpsellerProductWrapperDto>()
                        : ServiceResult.Succeed(product);
                }
                catch (Exception e)
                { logger.LogException(e, header); }
            }

            logger.LogInformation(
                $"{header} Jumpseller product with SKU \"{sku}\" not read.");
            return ServiceResult.Error<JumpsellerProductWrapperDto>(
                "No se pudo leer el producto de la tienda de Jumpseller. " +
                "Inténtelo otra vez en unos minutos.");
        }

        public async Task<ServiceResult> DeleteProductAsync(int jumpsellerProductId)
        {
            var header = "JumpsellerService.DeleteProduct".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var clientResult = await CreateApiClientAsync(header);
            if (clientResult.IsSucceed())
            {
                var client = clientResult.Data;
                var endpoint = CreateSimpleApiEndpoint(
                    productsEndpoint, jumpsellerProductId);
                var requestUri = CreateApiRequestUri(
                    client.BaseAddress, endpoint);
                try
                {
                    var response = await client.DeleteAsync(requestUri);
                    response.EnsureSuccessStatusCode();

                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed();
                }
                catch (Exception e)
                { logger.LogException(e, header); }
            }

            logger.LogInformation(
                $"{header} Jumpseller product \"{jumpsellerProductId}\" not deleted.");
            return ServiceResult.Error(
                "No es posible eliminar el producto de la tienda de Jumpseller. " +
                "Inténtelo otra vez en unos minutos.");
        }

        public async Task<ServiceResult<LoadProductsResultDto>> LoadProductsAsync()
        {
            var header = $"JumpsellerService.LoadProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var startTime = DateTime.Now;
            var result = ServiceResult.Succeed(new LoadProductsResultDto());

            try
            {
                var clientResult = await CreateApiClientAsync(header);
                if (clientResult.IsSucceed())
                {
                    var client = clientResult.Data;
                    var countUri = CreateApiRequestUri(
                        client.BaseAddress, $"{productsEndpoint}/count");
                    var productsCountResponse = await client.GetAsync(countUri);
                    productsCountResponse.EnsureSuccessStatusCode();
                    var count = JsonSerializer.Deserialize<JumpsellerProductsCountDto>(
                        await productsCountResponse.Content.ReadAsStringAsync(),
                        jsonJumpsellerSerializerOptions).Count;
                    logger.LogInformation($"{header} Loading {count} products from Jumpseller.");

                    var page = 1;
                    var allSyncsOk = true;

                    while (count > 0)
                    {
                        var batchStart = DateTime.Now;
                        var q = new NameValueCollection
                        {
                            ["limit"] = jumpsellerOptions.Api.ReadPageSize.ToString(),
                            ["page"] = page++.ToString()
                        };
                        var getProductsUri = CreateApiRequestUri(client.BaseAddress, productsEndpoint, q);
                        var productsResponse = await client.GetAsync(getProductsUri);
                        productsResponse.EnsureSuccessStatusCode();
                        var products = JsonSerializer.Deserialize<List<JumpsellerProductWrapperDto>>(
                            await productsResponse.Content.ReadAsStringAsync(), jsonJumpsellerSerializerOptions);
                        count -= products.Count;
                        logger.LogInformation($"{header} Read {products.Count} products from Jumpseller.");

                        products = products.Where(p => !string.IsNullOrEmpty(p.Product.Sku)).ToList();
                        var skus = products.Select(p => p.Product.Sku).ToHashSet();
                        var dbSkus = await productRepository
                            .ReadAsync(p => skus.Contains(p.SKU), p => p.Id, 0, -1, p => p.SKU)
                            .ToHashSetAsync();
                        var newJumpsellerProducts = products.Where(p => !dbSkus.Contains(p.Product.Sku));
                        using var tx = productRepository.DbContext.BeginTransaction();

                        var newProducts = mapper.Map<IEnumerable<Product>>(newJumpsellerProducts);
                        var brands = newProducts.Select(p => p.Brand).Distinct(
                            new DomainModelEqualityComparer<Brand>());
                        await brandRepository.UpsertAsync(brands);
                        var upsertResult = await productRepository.UpsertAsync(
                            newProducts);

                        if (upsertResult.OperationSucceed)
                        {
                            await tx.CommitAsync();
                            result.Data.Amount += upsertResult.Data;
                            skus.ExceptWith(dbSkus); // Set diff
                            if (skus.Count > 0)
                            {
                                var syncInfo = await productRepository.ReadAsync(
                                       p => skus.Contains(p.SKU),
                                       p => p.Id, 0, -1,
                                       p => new SynchronizeProductSkuDto { RedcetusId = p.Id, Sku = p.SKU })
                                   .ToListAsync();
                                var loadProviderResult =
                                    await providerService.SynchronizeProductsBySkuAsync(syncInfo);
                                allSyncsOk = allSyncsOk && loadProviderResult.IsSucceed();
                            }
                        }
                        else { await tx.RollbackAsync(); }

                        var elapsed = DateTime.Now - batchStart;
                        if (elapsed < TimeSpan.FromSeconds(1))
                        { Thread.Sleep(TimeSpan.FromSeconds(1) - elapsed); }
                    }

                    result.Data.Message = allSyncsOk
                        ? "Los productos fueron cargados desde Jumpseller correctamente."
                        : "No todos los productos fueron cargados. Vuelva a intentarlo " +
                          "otra vez. Si este mensaje persiste contacte el administrador.";
                    result.Data.LoadTime = DateTime.Now - startTime;
                    logger.LogInformation($"{header} End.");
                    return result;
                }
            }
            catch (Exception e)
            {
                logger.LogException(e, header);
            }

            return ServiceResult.Error<LoadProductsResultDto>(
                "No ha sido posible cargar todos los productos desde Jumpseller.");
        }

        private async Task<ServiceResult> UpdateProductFromJumpsellerAsync(Product product, string header)
        {
            var jsProductResult = await ReadProductBySkuAsync(product.SKU);
            if (jsProductResult.IsSucceed() && jsProductResult.Data?.Product != null)
            {
                var jsProduct = jsProductResult.Data.Product;
                var deleteResult = await DeleteProductByJumpsellerIdAsync(jsProduct.Id.Value);
                if (!deleteResult.IsSucceed())
                { return deleteResult; }

                jsProduct.Price *= 10000D / (119D * (product.Margin + 100));
                product.Price = Math.Max(product.Price, jsProduct.Price);
                product.Stock += jsProduct.Stock;
                product.JumpsellerId = jsProduct.Id;
                product.SynchronizedToJumpseller = true;

                return ServiceResult.Succeed();
            }

            return jsProductResult.Status == ServiceResultStatus.NotFound
                ? ServiceResult.Succeed()
                : jsProductResult;
        }

        private async Task<ServiceResult> DeleteProductByJumpsellerIdAsync(int id)
        {
            var products = await productRepository
                    .ReadAsync(p => p.JumpsellerId == id, 0, -1)
                    .ToListAsync();

            return products.Count == 1
                ? await ProductService.DeleteAsync(products[0].Id)
                : ServiceResult.Succeed();
        }

        private Dictionary<string, HttpRequestMessage> CreateRequestMap(
            List<Product> productsToSync, Uri baseAddress)
        {
            return productsToSync
                .Select(p => new { p.Id, P = mapper.Map<JumpsellerProductWrapperDto>(p) })
                .Select(i => new
                {
                    i.Id,
                    i.P,
                    U = i.P.Product.Id != null
                        ? CreateApiRequestUri(baseAddress, $"{productsEndpoint}/{i.P.Product.Id}")
                        : CreateApiRequestUri(baseAddress, productsEndpoint),
                    M = i.P.Product.Id != null ? HttpMethod.Put : HttpMethod.Post
                })
                .Select(i => new
                {
                    i.Id,
                    R = new HttpRequestMessage(i.M, i.U)
                    {
                        Content = CreateJsonStringContent(
                            i.P.Product.Id != null
                                ? new { Product = new { i.P.Product.Price, i.P.Product.Stock } }
                                : (object)i.P)
                    }
                })
                .ToDictionary(i => i.Id, i => i.R);
        }

        private async Task BatchRequestAsync<TResponse>(
            HttpClient client,
            Dictionary<string, HttpRequestMessage> requestMap,
            Func<string, TResponse, Task> processResponse,
            string header,
            Func<string, HttpRequestMessage, HttpResponseMessage, Task> error = null)
        {
            var oneMin = TimeSpan.FromMinutes(1.2);
            var oneSec = TimeSpan.FromSeconds(1.5);
            var minStart = DateTime.Now;
            var secStart = DateTime.Now;

            foreach (var (id, request) in requestMap)
            {
                HttpResponseMessage httpResponse = default;
                try
                {
                    httpResponse = await client.SendAsync(request);
                    httpResponse.EnsureSuccessStatusCode();
                    var response = JsonSerializer.Deserialize<TResponse>(
                        await httpResponse.Content.ReadAsStringAsync(), jsonJumpsellerSerializerOptions);
                    if (processResponse != null)
                    { await processResponse(id, response); }

                    if (PerSecondRequestsRemaining(httpResponse.Headers) == 0)
                    { secStart = ResetPerRequestLimit(secStart, oneSec); }
                    if (PerMinuteRequestsRemaining(httpResponse.Headers) == 0)
                    { minStart = ResetPerRequestLimit(minStart, oneMin); }
                }
                catch (Exception e)
                {
                    logger.LogException(e, header);
                    if (httpResponse.Headers.TryGetValues("Jumpseller-BannedByRateLimit-Reset",
                                                          out var values))
                    {
                        logger.LogInformation($"{header} Ip banned from making new requests.");
                        if (DateTime.TryParse(values.FirstOrDefault(), out var dt) &&
                            dt > DateTime.Now)
                        {
                            logger.LogInformation(
                                $"{header} Sleeping until {dt:yyyy-MM-dd HH:mm} to resume.");
                            Thread.Sleep(dt - DateTime.Now);
                        }
                    }
                    else if (!httpResponse.IsSuccessStatusCode && error != null)
                    { await error(id, request, httpResponse); }
                    else { break; }
                }
                finally
                { httpResponse?.Dispose(); }
            }
        }

        private static DateTime ResetPerRequestLimit(DateTime start, TimeSpan lapse)
        {
            var consumed = DateTime.Now - start;
            if (consumed < lapse)
            { Thread.Sleep(lapse - consumed); }
            return DateTime.Now;
        }

        private static int PerSecondRequestsRemaining(HttpHeaders headers)
            => PerRequestRemaining(headers, "Jumpseller-PerSecondRateLimit-Remaining");

        private static int PerMinuteRequestsRemaining(HttpHeaders headers)
            => PerRequestRemaining(headers, "Jumpseller-PerMinuteRateLimit-Remaining");

        private static int PerRequestRemaining(HttpHeaders header, string headerName)
        {
            if (header.TryGetValues(headerName, out var values))
            {
                if (int.TryParse(values.FirstOrDefault(), out var value))
                { return value; }
            }
            return default;
        }

        private string AuthCallbackUrl
            => new UriBuilder(mainOptions.HostedUrl)
            { Path = jumpsellerOptions.Auth.Endpoints.AuthorizeCallback }.Uri.ToString();

        private static StringContent CreateJsonStringContent(object body)
           => new StringContent(
                   JsonSerializer.Serialize(body, jsonJumpsellerSerializerOptions),
                   Encoding.UTF8,
                   MediaTypeNames.Application.Json);

        private async Task<ServiceResult<HttpClient>> CreateApiClientAsync(string header)
        {
            logger.LogInformation($"{header} Create Jumpseller API Http client.");
            var authInfoResult = await configRepository.ReadJumpsellerAuthInfoAsync();
            var authInfo = authInfoResult.Data;
            if (authInfoResult.OperationSucceed && authInfo.ApplicationAuthorized)
            {
                var now = DateTime.UtcNow;
                var shouldCreateClient = true;
                if (now > authInfo.TokenExpiresAt)
                { shouldCreateClient = await RefreshTokensAsync(header); }

                if (shouldCreateClient)
                {
                    authInfoResult = await configRepository.ReadJumpsellerAuthInfoAsync();
                    if (authInfoResult.OperationSucceed)
                    {
                        authInfo = authInfoResult.Data;
                        var client = httpClientFactory.CreateClient(JUMPSELLER_API_HTTP_CLIENT);
                        client.DefaultRequestHeaders.Authorization
                            = new AuthenticationHeaderValue(authInfo.AccessTokenType, authInfo.AccessToken);

                        logger.LogInformation($"{header} Jumpseller API Http client created.");
                        return ServiceResult.Succeed(client);
                    }
                }
            }

            logger.LogInformation($"{header} Jumpseller API Http client couldn't be created.");
            return ServiceResult.Error<HttpClient>();
        }

        private async Task<bool> RefreshTokensAsync(string header)
        {
            logger.LogInformation($"{header} Refreshing tokens.");
            var refreshed = false;
            var authInfoResult = await configRepository.ReadJumpsellerAuthInfoAsync();
            if (authInfoResult.OperationSucceed)
            {
                var result = await RetrieveAndSaveTokensAsync(
                    new
                    {
                        jumpsellerOptions.Auth.ClientId,
                        jumpsellerOptions.Auth.ClientSecret,
                        GrantType = "refresh_token",
                        authInfoResult.Data.RefreshToken
                    },
                    header);

                refreshed = result.IsSucceed();
            }

            logger.LogInformation($"{header} Refresh tokens ended. Success: {refreshed}");
            return refreshed;
        }

        private async Task<ServiceResult> RetrieveAndSaveTokensAsync(object body, string header)
        {
            logger.LogInformation($"{header} Start retrieving tokens.");
            var client = httpClientFactory.CreateClient(JUMPSELLER_AUTH_HTTP_CLIENT);
            var content = CreateJsonStringContent(body);

            var authInfoResult = await configRepository.ReadJumpsellerAuthInfoAsync();
            if (authInfoResult.OperationSucceed)
            {
                try
                {
                    var authInfo = authInfoResult.Data;
                    var uri = new UriBuilder(client.BaseAddress)
                    { Path = jumpsellerOptions.Auth.Endpoints.Token }.Uri;

                    var response = await client.PostAsync(uri, content);
                    response.EnsureSuccessStatusCode();
                    logger.LogInformation($"{header} Tokens successfully retrieved from Jumpseller.");

                    var tokens = JsonSerializer.Deserialize<AuthTokensDto>(
                        await response.Content.ReadAsStringAsync(), jsonJumpsellerSerializerOptions);
                    mapper.Map(tokens, authInfo, typeof(AuthTokensDto), typeof(JumpsellerConfiguration));
                    var saveResult = await configRepository.SaveJumpsellerAuthorizationInfoAsync(authInfo);
                    if (saveResult.OperationSucceed)
                    {
                        logger.LogInformation($"{header} Tokens information persisted.");
                        return ServiceResult.Succeed();
                    }
                }
                catch (Exception e)
                { logger.LogException(e, header); }
            }

            logger.LogInformation($"{header} Retrieve tokens ended.");
            return ServiceResult.Error();
        }

        private Uri CreateApiRequestUri(
            Uri baseUri, string endpoint, NameValueCollection query = default)
        {
            if (!string.IsNullOrEmpty(endpoint) &&
                !endpoint.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            { endpoint += ".json"; }

            return baseUri.Augment(endpoint, query);
        }

        private string CreateSimpleApiEndpoint(string baseEndpoint, object id = default)
        {
            var endpoint = baseEndpoint;

            if (id != null)
            {
                endpoint = endpoint.TrimEnd('/');
                endpoint += $"/{id}.json";
            }

            return endpoint;
        }

        private async Task SyncLocalProductsToJumpsellerAsync(
            string header, Dictionary<string, LocalProduct> productsMap)
        {
            await UpdateLocalInfoFromJumpsellerAsync(productsMap, header);
            await UpdateJumpsellerFromLocalProductsAsync(productsMap, header);
        }

        private async Task UpdateLocalInfoFromJumpsellerAsync(
            Dictionary<string, LocalProduct> productsMap, string header)
        {
            logger.LogInformation(
                $"{header} Starting to retrieve info of {productsMap.Count} products from Jumpseller.");
            var totalGet = 0;
            var failedKeys = new HashSet<string>();

            var clienResult = await CreateApiClientAsync(header);
            if (clienResult.IsSucceed())
            {
                var client = clienResult.Data;
                var requestsMap = productsMap
                    .Select(kv => new { kv.Key, EndPoint = $"{productsEndpoint}/{kv.Value.JumpsellerId}" })
                    .Select(info => new { info.Key, Uri = CreateApiRequestUri(client.BaseAddress, info.EndPoint) })
                    .Select(info => new { info.Key, Req = new HttpRequestMessage(HttpMethod.Get, info.Uri) })
                    .ToDictionary(info => info.Key, info => info.Req);

                await BatchRequestAsync<JumpsellerProductWrapperDto>(
                    client, requestsMap,
                    (key, dto) =>
                    {
                        totalGet++;
                        var lp = productsMap[key];
                        lp.Price = Math.Round(Math.Max(lp.Price, dto.Product.Price), 2);
                        lp.Stock += dto.Product.Stock;

                        return Task.CompletedTask;
                    }, header, (key, req, resp) =>
                    {
                        logger.LogInformation(
                            $"{header} Get information for local product \"{key}\" has failed.");
                        failedKeys.Add(key);
                        return Task.CompletedTask;
                    });

                foreach (var failedKey in failedKeys)
                { productsMap.Remove(failedKey); }
            }
            else
            { logger.LogInformation($"{header} Can't connect to jumpseller."); }

            logger.LogInformation(
                $"{header} Done retrieving info from Jumpseller ({totalGet}/{productsMap.Count}).");
        }

        private async Task UpdateJumpsellerFromLocalProductsAsync(
            Dictionary<string, LocalProduct> productsMap, string header)
        {
            logger.LogInformation(
                $"{header} Starting to update {productsMap.Count} jumpseller products.");
            var totalUpdated = 0;

            var clienResult = await CreateApiClientAsync(header);
            if (clienResult.IsSucceed())
            {
                var client = clienResult.Data;
                var requestsMap = CreateSyncLocalProductsRequestMap(productsMap, client);

                await BatchRequestAsync<JumpsellerProductWrapperDto>(
                    client, requestsMap,
                    (key, dto) =>
                    {
                        totalUpdated++;
                        return Task.CompletedTask;
                    }, header, (key, req, resp) =>
                    {
                        logger.LogInformation(
                            $"{header} Update from local product \"{key}\" has failed.");
                        return Task.CompletedTask;
                    });
            }
            else
            { logger.LogInformation($"{header} Can't connect to jumpseller."); }

            logger.LogInformation(
                $"{header} Done updating from local products ({totalUpdated}/{productsMap.Count}).");
        }

        private Dictionary<string, HttpRequestMessage> CreateSyncLocalProductsRequestMap(
            Dictionary<string, LocalProduct> productsMap, HttpClient client)
        {
            return productsMap
                .Select(kv => new
                {
                    kv.Key,
                    EndPoint = $"{productsEndpoint}/{kv.Value.JumpsellerId}",
                    P = kv.Value
                })
                .Select(info => new
                {
                    info.Key,
                    Uri = CreateApiRequestUri(client.BaseAddress, info.EndPoint),
                    info.P
                })
                .Select(info => new
                {
                    info.Key,
                    Req = new HttpRequestMessage(HttpMethod.Put, info.Uri)
                    {
                        Content = CreateJsonStringContent(
                            new { Product = new { Price = Math.Round(info.P.Price), info.P.Stock } })
                    }
                })
                .ToDictionary(info => info.Key, info => info.Req);
        }

        private void LogProductsInfo(string header, List<Product> productsToSync)
        {
            var productsInfo = new StringBuilder();
            productsInfo.AppendLine("********** Products Info **********");
            for (var i = 0; i < productsToSync.Count; i++)
            {
                var p = productsToSync[i];
                productsInfo.AppendLine(
                    $"{i + 1,3}-Product: {p.SKU}. Price: {p.Price}, Stock: {p.Stock}. " +
                    $"==ACTION: {(p.JumpsellerId == null ? "POST" : $"PUT ({p.JumpsellerId})")}==");
            }

            logger.LogInformation($"{header} {productsInfo}");
        }
    }
}

