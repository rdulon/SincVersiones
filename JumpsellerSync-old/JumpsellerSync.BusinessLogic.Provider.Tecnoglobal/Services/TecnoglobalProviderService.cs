using AutoMapper;

using Hangfire;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Dtos;
using JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Options;
using JumpsellerSync.Common.Util.Extensions;
using JumpsellerSync.DataAccess.Core.Repositories;
using JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal;
using JumpsellerSync.Domain.Impl;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Services
{
    public class TecnoglobalProviderService
        : ProviderService<TecnoglobalProduct, TecnoglobalBrand, TecnoglobalConfiguration, TecnoglobalProviderService>
    {
        public const string TECNOGLOBAL_HTTP_CLIENT = "TecnoglobalHttpClientId";

        private static readonly JsonSerializerOptions tecnoglobalJSONSerializerOptions =
            new JsonSerializerOptions
            { PropertyNamingPolicy = new CamelCaseNamingPolicy() };

        private readonly IHttpClientFactory httpClientFactory;
        private readonly ITecnoglobalCategoryRepository categoryRepository;
        private readonly ITecnoglobalSubcategoryRepository subcategoryRepository;
        private readonly ITecnoglobalBrandRepository brandRepository;
        private readonly ApiOptions apiOptions;

        public TecnoglobalProviderService(
            IBackgroundJobClient jobClient,
            ISynchronizeService.Factory synchronizeFactory,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            ITecnoglobalProductRepository productRepository,
            ITecnoglobalCategoryRepository categoryRepository,
            ITecnoglobalSubcategoryRepository subcategoryRepository,
            ITecnoglobalBrandRepository brandRepository,
            IOptions<TecnoglobalOptions> options,
            ILogger<TecnoglobalProviderService> logger)
            : base(synchronizeFactory, productRepository, jobClient, mapper, logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.categoryRepository = categoryRepository;
            this.subcategoryRepository = subcategoryRepository;
            this.brandRepository = brandRepository;
            apiOptions = options.Value.Api;
        }

        protected override async Task<bool> UpdateProductsAsync(string logHeader)
        {
            try
            {
                var client = httpClientFactory.CreateClient(TECNOGLOBAL_HTTP_CLIENT);
                var reqUri = client.BaseAddress.Augment(
                    apiOptions.Endpoints.GetProducts.Path,
                    HttpUtility.ParseQueryString(
                        apiOptions.Endpoints.GetProducts.Query ?? ""));
                var response = await client.GetAsync(reqUri);
                response.EnsureSuccessStatusCode();
                var tgResponse = JsonSerializer.Deserialize<TecnoglobalResponseDto>(
                    await response.Content.ReadAsStringAsync(), tecnoglobalJSONSerializerOptions);
                if (tgResponse.Error)
                {
                    logger.LogInformation(
                      $"{logHeader} Request to Tecnoglobal unsuccessful. Message: \"{tgResponse.Message}\"");
                }
                else
                {
                    logger.LogInformation($"{logHeader} {tgResponse.Message}");
                    var products = mapper.Map<IEnumerable<TecnoglobalProduct>>(tgResponse.Products);

                    var brands = products.Select(p => p.Brand).Distinct(
                        new DomainModelEqualityComparer<TecnoglobalBrand>());
                    var categories = products.Select(p => p.Category).Distinct(
                        new DomainModelEqualityComparer<TecnoglobalCategory>());
                    var subCategories = products.Select(p => p.Subcategory).Distinct(
                        new DomainModelEqualityComparer<TecnoglobalSubcategory>());

                    await Upsert(subcategoryRepository, subCategories, logHeader);
                    await Upsert(categoryRepository, categories, logHeader);
                    await Upsert(brandRepository, brands, logHeader);
                    await Upsert((IUpsertRepository<TecnoglobalProduct>)productRepository,
                        products, logHeader, nameof(TecnoglobalProduct.RedcetusProductId));

                    logger.LogInformation(
                        $"{logHeader} Products, categories, sub categories and brands up-to-date.");
                    return true;
                }
            }
            catch (Exception e)
            { logger.LogException(e, logHeader); }

            return false;
        }
    }
}