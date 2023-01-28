using AutoMapper;
using Castle.Core.Internal;
using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.BusinessLogic.Core.Dtos.Jumpseller;
using JumpsellerSync.BusinessLogic.Core.Dtos.Main;
using JumpsellerSync.BusinessLogic.Core.Dtos.Provider;
using JumpsellerSync.BusinessLogic.Core.Dtos.Vtex;
using JumpsellerSync.BusinessLogic.Core.Extensions;
using JumpsellerSync.BusinessLogic.Core.Services.Main;
using JumpsellerSync.BusinessLogic.Impl.Extensions;
using JumpsellerSync.BusinessLogic.Impl.Options;
using JumpsellerSync.BusinessLogic.Impl.Util;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories.Main;
using JumpsellerSync.DataAccess.Impl.DbContexts;
using JumpsellerSync.Domain.Impl;
using JumpsellerSync.Domain.Impl.Main;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using static JumpsellerSync.BusinessLogic.Impl.Constants.HttpClientNames;
using static JumpsellerSync.Common.Util.Services.ServiceUtilities;


namespace JumpsellerSync.BusinessLogic.Impl.Services
{
    public class VtexService : IVtexService
    {
        // Process a page of products in about a minute
        private const int PRODUCTS_PAGE_SIZE = 210;
        private readonly IProductRepository productRepository;
        private readonly ILocalProductRepository localProductRepository;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly MainOptions mainOptions;
        private readonly ILogger<VtexService> logger;
        private Hashtable brandsCreated = new Hashtable();


        private readonly string UrlProductCatalog = "/api/catalog/pvt/product/";
        private readonly string UrlProductsCatalog = "/api/catalog_system/pvt/products/GetProductAndSkuIds";
        private readonly string UrlSkuCatalog = "/api/catalog/pvt/stockkeepingunit/";
        private readonly string UrlPrices = "/pricing/prices/";
        private readonly string UrlStock = "/api/logistics/pvt/inventory/skus/";

        public VtexService(
            IProductRepository productRepository,
            ILocalProductRepository localProductRepository,
            IHttpClientFactory httpClientFactory,
            IOptions<MainOptions> mainOptions,
            ILogger<VtexService> logger)
        {
            this.productRepository = productRepository;
            this.localProductRepository = localProductRepository;
            this.httpClientFactory = httpClientFactory;
            this.mainOptions = mainOptions.Value;
            this.logger = logger;
        }

        // Dependency as property so autofac is able to handle circular dependency 
        public IProductService ProductService { get; set; }

        public async Task SynchronizeProductsAsync()
        {
            var header = "VtexService.SynchronizeProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");

            var syncCount = await productRepository.CountProductsToSynchronizeAsync();
            var productsToSync = await productRepository
                    .ReadProductsToSynchronizeAsync(0, syncCount)
                    .ToListAsync();
            var actualSynced = 0;
            
            foreach (var product in productsToSync)
            {
                if (!product.SKU.IsNullOrEmpty())
                {
                    if (product.SKU == "3m 82-A2N Copy")
                    {
                        logger.LogInformation($"{header} Found: " + product.SKU);
                    }

                    if (!product.Brand.Name.IsNullOrEmpty() && !product.Name.IsNullOrEmpty())
                    {
                        ProductToSyncDto productSynced = await SyncProduct(new ProductToSyncDto()
                        {
                            Brand = new BrandDto()
                            {
                                Active = true,
                                Id = product.BrandId,
                                Name = product.Brand.Name
                            },
                            BrandId = product.BrandId,
                            Description = product.Description,
                            Height = product.Height,
                            Name = product.Name,
                            ImageUrls = product.ImageUrls,
                            IsDigital = product.IsDigital,
                            IsLocalProduct = (product.LocalProduct != null),
                            JumpsellerId = product.JumpsellerId,
                            Length = product.Length,
                            Margin = product.Margin,
                            Price = product.Price,
                            SKU = product.SKU,
                            Stock = product.Stock,
                            Weight = product.Weight,
                            Width = product.Width,
                            SynchronizedToJumpseller = product.SynchronizedToJumpseller,
                            SynchronizingProviderIds = product.SynchronizingProviderIds
                        });

                        if (productSynced.SynchronizedToJumpseller)
                        {
                            product.JumpsellerId = productSynced.JumpsellerId.Value;
                            product.SynchronizedToJumpseller = productSynced.SynchronizedToJumpseller;

                            if (product.Categories == null)
                            {
                                product.Categories = new HashSet<Category>();
                            }

                            productRepository.DbContext.Update(product);
                            actualSynced++;
                        }
                        else
                        {
                            logger.LogInformation($"{header} Failed to sync product: " + product.SKU);
                        }
                    }
                    else
                    {
                        logger.LogInformation($"{header} Failed to sync product (Name or Brand name are invalid): " + product.SKU);
                    }
                }
            }

            productRepository.DbContext.SaveChanges();

            logger.LogInformation(
                $"{header} Synchronization to Vtex finished ({actualSynced}/{syncCount}).");
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
                    await SyncLocalProductsToVtexAsync(header, productsMap);

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
                await SyncLocalProductsToVtexAsync(header, productsMap);

                logger.LogInformation($"{header} End.");
            }
            catch (Exception e)
            { logger.LogException(e, header); }
        }

        public async Task<ServiceResult> SynchronizeProductAsync(string productId)
        {
            var header = "VtexService.SynchronizeProduct".AsLogHeader();
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

                ProductToSyncDto productToSync = new ProductToSyncDto()
                {
                    Brand = new BrandDto() {
                        Active = true,
                        Id = product.BrandId,
                        Name = product.Brand.Name
                    },
                    BrandId = product.BrandId,
                    Description = product.Description,
                    Height = product.Height,
                    Name = product.Name,
                    ImageUrls = product.ImageUrls,
                    IsDigital = product.IsDigital,
                    IsLocalProduct = (product.LocalProduct != null),
                    JumpsellerId = product.JumpsellerId,
                    Length = product.Length,
                    Margin = product.Margin,
                    Price = product.Price,
                    SKU = product.SKU,
                    Stock = product.Stock,
                    Weight = product.Weight,
                    Width = product.Width,
                    SynchronizedToJumpseller = product.SynchronizedToJumpseller,
                    SynchronizingProviderIds = product.SynchronizingProviderIds
                };

                productToSync = await SyncProduct(productToSync);

                if (productToSync.SynchronizedToJumpseller)
                {
                    product.JumpsellerId = productToSync.JumpsellerId;
                    product.SynchronizedToJumpseller = productToSync.SynchronizedToJumpseller;

                    await productRepository.UpdateAsync(product);

                    logger.LogInformation($"{header} End.");
                    return ServiceResult.Succeed();
                }
            }

            logger.LogInformation(
                $"{header} Product \"{productId}\" no sincronizado.");
            return ServiceResult.Error(
                "No es posible guardar/actualizar este producto en la tienda de " +
                "Vtex. Inténtelo otra vez en unos minutos.");
        }

        public async Task<ServiceResult<LoadProductsResultDto>> LoadProductsAsync()
        {
            var header = $"JumpsellerService.LoadProducts".AsLogHeader();
            logger.LogInformation($"{header} Init.");
            var startTime = DateTime.Now;
            var result = ServiceResult.Succeed(new LoadProductsResultDto());
            bool allSyncsOk = true;

            try
            {
                JsonElement productsData = (JsonElement)await CallServiceAsync(
                    UrlProductsCatalog + "?categoryId=1",
                    HttpMethod.Get
                );

                /*var productIds = productsData.EnumerateArray();

                if (!productIds.IsNullOrEmpty())
                {
                    var dbSkus = await productRepository
                            .ReadAsync(p => p.JumpsellerId != null && productIds.Contains(p.JumpsellerId.Value), p => p.Id, 0, -1, p => p.JumpsellerId.Value)
                            .ToHashSetAsync();
                    var newVtexProducts = productIds.Where(p => !dbSkus.Contains(p));

                    var productIdsa = newVtexProducts.Count();
                }*/

                result.Data.Message = allSyncsOk
                        ? "Los productos fueron cargados desde Vtex correctamente."
                        : "No todos los productos fueron cargados. Vuelva a intentarlo " +
                          "otra vez. Si este mensaje persiste contacte el administrador.";
                result.Data.LoadTime = DateTime.Now - startTime;
                logger.LogInformation($"{header} End.");
                return result;

                /*var clientResult = await CreateApiClientAsync(header);
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

                    
                }*/
            }
            catch (Exception e)
            {
                logger.LogException(e, header);
            }

            return ServiceResult.Error<LoadProductsResultDto>(
                "No ha sido posible cargar todos los productos desde Jumpseller.");
        }

        private async Task<ProductToSyncDto> SyncProduct(ProductToSyncDto product)
        {
            // Set product not synchronized
            product.SynchronizedToJumpseller = false;

            var brandId = await UpdateBrandAsync(product.Brand);

            if (!brandId.IsNullOrEmpty())
            {
                // Create or update product
                VtexResponse responseProduct = await CreateProductAsync(product, brandId);

                // If product was created or updated, then continue
                if (responseProduct.id != null)
                {
                    product.JumpsellerId = responseProduct.id;

                    VtexResponse skuUpdateResponse = await CreateSkuAsync(product);

                    // If product was created or updated, then continue
                    if (skuUpdateResponse.id != null)
                    {
                        var skuId = skuUpdateResponse.id.ToString();

                        await CreateImages(product, skuId);

                        HttpResponseMessage priceResponse = await SetPrice(product, skuId);

                        if (priceResponse.IsSuccessStatusCode)
                        {
                            HttpResponseMessage stockResponse = await SetStock(product, skuId);

                            if (stockResponse.IsSuccessStatusCode)
                            {
                                product.SynchronizedToJumpseller = true;
                            }
                            else
                            {
                                logger.LogInformation($"SetStock: " + stockResponse.StatusCode + " - " + stockResponse.Content.ReadAsStringAsync());
                            }
                        }
                        else
                        {
                            logger.LogInformation($"SetPrice: " + priceResponse.StatusCode + " - " + priceResponse.Content.ReadAsStringAsync());
                        }
                    }
                    else
                    {
                        logger.LogInformation($"CreateSkuAsync: " + skuUpdateResponse.response.StatusCode + " - " + skuUpdateResponse.response.Content.ReadAsStringAsync());
                    }
                }
                else
                {
                    logger.LogInformation($"CreateProductAsync: " + responseProduct.response.StatusCode + " - " + responseProduct.response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                logger.LogInformation($"UpdateBrandAsync: invalid brand ("+ product.BrandId +")");
            }

            return product;
        }

        private async Task<VtexResponse> CreateProductAsync(ProductToSyncDto product, string brandId)
        {
            VtexResponse vtexResponse = new VtexResponse();
            var productData = await ReadProductByRefAsync(product.SKU);
            bool productExists = (productData != null);
            string productIdVtex = "";

            if (productExists)
            {
                vtexResponse.id = productData.Value.GetProperty("Id").GetInt32();
                return vtexResponse;
            }

            vtexResponse.response = (HttpResponseMessage) await CallServiceAsync(
                UrlProductCatalog + productIdVtex,
                (productExists ? HttpMethod.Put : HttpMethod.Post),
                new ProductDto
                {
                    BrandId = brandId,
                    CategoryId = "1",
                    DepartmentId = "1",
                    Description = product.Description,
                    IsVisible = true,
                    IsActive = false,
                    LinkId = product.SKU,
                    Name = product.Name,
                    RefId = product.SKU,
                    ShowWithoutStock = false,
                    Title = product.Name
                }
            );

            if (vtexResponse.response.IsSuccessStatusCode)
            {
                var content = await vtexResponse.response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(content);

                vtexResponse.id = data.GetValue("Id").Value<int>();
            }

            return vtexResponse;
        }

        public async Task<JsonElement?> ReadProductByRefAsync(string sku)
        {
            var response = await CallServiceAsync("/api/catalog_system/pvt/products/productgetbyrefid/" + sku, HttpMethod.Get);

            if (response == null) { return null; }

            return (JsonElement)response;
        }

        public async Task<JsonElement?> ReadSkuByRefAsync(string sku)
        {
            var response = await CallServiceAsync(UrlSkuCatalog + "?refId=" + sku, HttpMethod.Get);

            if (response.GetType() == typeof(HttpResponseMessage) && ((HttpResponseMessage)response).StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            return (JsonElement)response;
        }

        private async Task<VtexResponse> CreateSkuAsync(ProductToSyncDto product)
        {
            VtexResponse vtexResponse = new VtexResponse();
            var skuData = await ReadSkuByRefAsync(product.SKU);
            bool skuExists = (skuData != null);
            string skuIdVtex = "";

            if (skuExists)
            {
                vtexResponse.id = skuData.Value.GetProperty("Id").GetInt32();
                return vtexResponse;
            }

            vtexResponse.response = (HttpResponseMessage)await CallServiceAsync(UrlSkuCatalog + skuIdVtex, (skuExists ? HttpMethod.Put : HttpMethod.Post), new SkuDto
            {
                ProductId = product.JumpsellerId.Value,
                Name = product.Name,
                RefId = product.SKU,
                IsActive = false,
                ActivateIfPossible = true,
                PackagedHeight = product.Height,
                PackagedLength = product.Length,
                PackagedWidth = product.Width,
                PackagedWeightKg = product.Weight,
                ManufacturerCode = product.SKU,
                MeasurementUnit = "un"
            });

            if (vtexResponse.response.IsSuccessStatusCode)
            {
                var content = await vtexResponse.response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(content);

                vtexResponse.id = data.GetValue("Id").Value<int>();
            }

            return vtexResponse;
        }

        private async Task CreateImages(ProductToSyncDto product, string skuId)
        {
            if (product.ImageUrls != null && product.ImageUrls.Count > 0)
            {
                // Remove all previous images
                var response = (HttpResponseMessage)await CallServiceAsync(UrlSkuCatalog + skuId + "/file", HttpMethod.Delete);

                if (response.IsSuccessStatusCode)
                {
                    foreach (var imageUrl in product.ImageUrls)
                    {
                        response = (HttpResponseMessage)await CallServiceAsync(UrlSkuCatalog + skuId + "/file", HttpMethod.Post, new SkuImageDto
                        {
                            Name = product.Name,
                            Label = product.Name,
                            IsMain = true,
                            Url = imageUrl
                        });

                        if (!response.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private async Task<HttpResponseMessage> SetPrice(ProductToSyncDto product, string skuId)
        {
            var price = Math.Round(product.Price * 1.19D * (1D + (product.Margin / 100D)), 2);
            product.Price = Math.Max(product.Price, price);
            HttpResponseMessage response = (HttpResponseMessage)await CallServiceAsync(UrlPrices + skuId, HttpMethod.Put, new SkuPriceDto
            {
                markup = 0,
                listPrice = price,
                basePrice = price
            }, true);

            return response;
        }

        private async Task<HttpResponseMessage> SetStock(ProductToSyncDto product, string skuId)
        {
            HttpResponseMessage response = (HttpResponseMessage)await CallServiceAsync(UrlStock + skuId + "/warehouses/" + mainOptions.VtexWarehouseId, HttpMethod.Put, new SkuStockDto
            {
                unlimitedQuantity = false,
                dateUtcOnBalanceSystem = null,
                quantity = Int32.Parse(product.Stock.ToString())
            });

            return response;
        }

        private async Task<ServiceResult> SyncLocalProductsToVtexAsync(
            string header, Dictionary<string, LocalProduct> productsMap)
        {
            LocalProduct product = productsMap.First().Value;
            ProductToSyncDto productToSync = new ProductToSyncDto()
            {
                Brand = new BrandDto()
                {
                    Active = true,
                    Id = product.BrandId,
                    Name = product.Brand.Name
                },
                BrandId = product.BrandId,
                Description = "",
                Height = 0,
                Name = product.Name,
                ImageUrls = null,
                IsDigital = false,
                IsLocalProduct = true,
                JumpsellerId = product.JumpsellerId,
                Length = 0,
                Margin = 0,
                Price = product.Price,
                SKU = product.SKU,
                Stock = product.Stock,
                Weight = 0,
                Width = 0,
                SynchronizedToJumpseller = false,
                SynchronizingProviderIds = null
            };

            productToSync = await SyncProduct(productToSync);

            if (productToSync.SynchronizedToJumpseller)
            {
                product.JumpsellerId = productToSync.JumpsellerId.Value;

                localProductRepository.DbContext.Update(product);

                logger.LogInformation($"{header} End.");
                return ServiceResult.Succeed();
            }

            logger.LogInformation(
                $"{header} Local Product \"{product.Id}\" no sincronizado.");
            return ServiceResult.Error(
                "No es posible guardar/actualizar este producto local en la tienda de " +
                "Vtex. Inténtelo otra vez en unos minutos.");
        }

        private async Task<string> UpdateBrandAsync(BrandDto brand)
        {
            // If brand was created before, then return brandId
            var brandId = Math.Abs(brand.Id.GetHashCode()).ToString();
            
            if (brandsCreated.ContainsKey(brandId))
            {
                return brandId;
            }

            // If not, then create
            var brands = (JsonElement) await CallServiceAsync("/api/catalog_system/pvt/brand/list", HttpMethod.Get);

            foreach (JsonElement brandData in brands.EnumerateArray())
            {
                if (brandData.GetProperty("id").GetInt32().ToString() == brandId || brandData.GetProperty("name").GetString() == brand.Name)
                {
                    return brandData.GetProperty("id").GetInt32().ToString();
                }
            }

            if (brand.Name.IsNullOrEmpty())
            {
                return null;
            }

            HttpResponseMessage response = (HttpResponseMessage) await CallServiceAsync("/api/catalog/pvt/brand", HttpMethod.Post, new BrandDto {
                Id = brandId,
                Name = brand.Name,
                Active = true
            });

            if (response.StatusCode == HttpStatusCode.Conflict || response.IsSuccessStatusCode)
            {
                brandsCreated.Add(brandId, brand);
                return brandId;
            } else
            {

            }

            return null;
        }

        private async Task<object> CallServiceAsync(string endpoint, HttpMethod method, object data = null, bool isApiEndpoint = false)
        {
            var client = httpClientFactory.CreateClient(VTEX_API_HTTP_CLIENT);
            client.DefaultRequestHeaders.Add($"X-VTEX-API-AppKey", mainOptions.VtexSecret);
            client.DefaultRequestHeaders.Add($"X-VTEX-API-AppToken", mainOptions.VtexToken);

            var url = "https://" + (isApiEndpoint ? "api.vtex.com/" + mainOptions.VtexApiAccountName : mainOptions.VtexAccountName + ".vtexcommercestable.com.br") + endpoint;

            try
            {
                return method switch
                {
                    HttpMethod m when m == HttpMethod.Get => await client.GetFromJsonAsync<object>(url),
                    HttpMethod m when m == HttpMethod.Post => await client.PostAsJsonAsync(url, data),
                    HttpMethod m when m == HttpMethod.Put => await client.PutAsJsonAsync(url, data),
                    HttpMethod m when m == HttpMethod.Delete => await client.DeleteAsync(url),
                    _ => new HttpResponseMessage(HttpStatusCode.BadRequest),
                };
            } catch (Exception ex)
            {
                if (ex.Message.Contains("404 (Not Found)"))
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                } else
                {
                    throw ex;
                }
            }
        }
    }
}

