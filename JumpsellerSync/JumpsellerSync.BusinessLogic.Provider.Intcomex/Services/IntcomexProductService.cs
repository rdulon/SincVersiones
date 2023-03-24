using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.DataAccess.Core.Repositories.Intcomex;
using JumpsellerSync.Domain.Impl.Intcomex;

using Microsoft.Extensions.Logging;

using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Provider.Intcomex.Services
{
    public class IntcomexProductService
        : ProviderProductService<IntcomexProduct,
                                 IntcomexBrand,
                                 IntcomexCategory,
                                 IntcomexProductService>,
          IProviderProductService
    {
        private static readonly string[] skuSkipUpdateProperties =
            CreateSkipPropertiesArray<IntcomexProduct>(
                nameof(IntcomexProduct.Id), nameof(IntcomexProduct.RedcetusProductId),
                nameof(IntcomexProduct.Brand), nameof(IntcomexProduct.Category));

        public IntcomexProductService(
            IIntcomexProductRepository intcomexProductRepository,
            IIntcomexBrandRepository intcomexBrandRepository,
            IIntcomexCategoryRepository intcomexCategoryRepository,
            IMapper mapper,
            ILogger<IntcomexProductService> logger)
            : base(
                intcomexProductRepository,
                intcomexBrandRepository,
                intcomexCategoryRepository,
                mapper,
                logger)
        { }

        protected override string[] SkuSkipUpdateProperties => skuSkipUpdateProperties;
    }
}
