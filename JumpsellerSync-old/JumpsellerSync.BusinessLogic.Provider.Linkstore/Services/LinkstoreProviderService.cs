using AutoMapper;

using Hangfire;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.BusinessLogic.Provider.Linkstore.Options;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories;
using JumpsellerSync.DataAccess.Core.Repositories.Linkstore;
using JumpsellerSync.Domain.Impl;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using static JumpsellerSync.BusinessLogic.Provider.Linkstore.Constants.HttpClientConstants;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.Services
{
    public class LinkstoreProviderService
        : ProviderService<LinkstoreProduct, LinkstoreBrand, LinkstoreConfiguration, LinkstoreProviderService>
    {
        private readonly LinkstoreOptions options;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILinkstoreCategoryRepository categoryRepository;
        private readonly ILinkstoreSubcategoryRepository subcategoryRepository;
        private readonly ILinkstoreBrandRepository brandRepository;

        public LinkstoreProviderService(
            IBackgroundJobClient jobClient,
            ISynchronizeService.Factory synchronizeFactory,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            ILinkstoreProductRepository productRepository,
            ILinkstoreCategoryRepository categoryRepository,
            ILinkstoreSubcategoryRepository subcategoryRepository,
            ILinkstoreBrandRepository brandRepository,
            ILogger<LinkstoreProviderService> logger,
            IOptions<LinkstoreOptions> options)
            : base(synchronizeFactory, productRepository, jobClient, mapper, logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.categoryRepository = categoryRepository;
            this.subcategoryRepository = subcategoryRepository;
            this.brandRepository = brandRepository;
            this.options = options.Value;
        }

        protected override async Task<bool> UpdateProductsAsync(string logHeader)
        {
            await SyncCategoriesAsync(logHeader);
            await SyncSubcategoriesAsync(logHeader);
            await SyncBrandsAsync(logHeader);
            return await SyncProductsAsync(logHeader);
        }

        private Task<bool> SyncProductsAsync(string logHeader)
            => SyncAsync(
                options.Products, (IUpsertRepository<LinkstoreProduct>)productRepository, "products",
                logHeader, nameof(LinkstoreProduct.RedcetusProductId), nameof(LinkstoreProduct.ProductCode));

        private Task<bool> SyncBrandsAsync(string logHeader)
            => SyncAsync(options.Brands, brandRepository, "brands", logHeader);

        private Task<bool> SyncSubcategoriesAsync(string logHeader)
            => SyncAsync(options.Subcategories, subcategoryRepository, "subcategories", logHeader);

        private Task<bool> SyncCategoriesAsync(string logHeader)
            => SyncAsync(options.Categories, categoryRepository, "categories", logHeader);


        private async Task<bool> SyncAsync<TModel>(
            EndPointOptions endPointOptions, IUpsertRepository<TModel> repo,
            string what, string logHeader, params string[] skipUpdateProps)
            where TModel : DomainModel
        {
            using var tx = repo.DbContext.BeginTransaction();
            try
            {
                logger.LogInformation($"{logHeader} Init {what} sync.");
                var client = httpClientFactory.CreateClient(LINKSTORE_HTTP_CLIENT);
                var request = CreateLinkstoreRequest(
                    client.BaseAddress, endPointOptions.Path, endPointOptions.Query);
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                logger.LogInformation($"{logHeader} Request successful. Processing {what}...");
                var linkstoreWhat = JsonConvert.DeserializeObject<IEnumerable<JArray>>(
                    await response.Content.ReadAsStringAsync());
                var dbWhat = mapper.Map<IEnumerable<JArray>,
                                        IEnumerable<TModel>>(linkstoreWhat);

                await repo.UpsertAsync(dbWhat, skipProperitesUpdate: skipUpdateProps);
                await tx.CommitAsync();

                logger.LogInformation($"{logHeader} End {what} sync.");
                return true;
            }
            catch (Exception e)
            {
                logger.LogException(e, $"{logHeader} Sync {what} failed");
                if (tx != null)
                { await tx.RollbackAsync(); }

                return false;
            }
        }

        private HttpRequestMessage CreateLinkstoreRequest(Uri baseUri, string path, string query)
            => new HttpRequestMessage(
                HttpMethod.Get,
                new UriBuilder(baseUri)
                {
                    Path = path,
                    Query = query
                }.Uri);
    }
}
