using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.DataAccess.Core.Repositories.Linkstore;
using JumpsellerSync.Domain.Impl.Linkstore;

using Microsoft.Extensions.Logging;

using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Provider.Linkstore.Services
{
    public class LinkstoreProductService
        : ProviderProductService<LinkstoreProduct,
                                 LinkstoreBrand,
                                 LinkstoreCategory,
                                 LinkstoreProductService>,
          IProviderProductService
    {
        private static readonly string[] skuSkipUpdateProperties =
            CreateSkipPropertiesArray<LinkstoreProduct>(
                nameof(LinkstoreProduct.Id), nameof(LinkstoreProduct.RedcetusProductId),
                nameof(LinkstoreProduct.Brand), nameof(LinkstoreProduct.Category),
                nameof(LinkstoreProduct.SubCategory));

        public LinkstoreProductService(
            ILinkstoreProductRepository linkstoreProductRepository,
            ILinkstoreBrandRepository linkstoreBrandRepository,
            ILinkstoreCategoryRepository linkstoreCategoryRepository,
            IMapper mapper,
            ILogger<LinkstoreProductService> logger)
            : base(
                linkstoreProductRepository,
                linkstoreBrandRepository,
                linkstoreCategoryRepository,
                mapper,
                logger)
        { }

        protected override string[] SkuSkipUpdateProperties => skuSkipUpdateProperties;
    }
}
