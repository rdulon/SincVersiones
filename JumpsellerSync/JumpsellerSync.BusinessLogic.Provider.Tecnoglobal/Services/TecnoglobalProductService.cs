using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.DataAccess.Core.Repositories.Tecnoglobal;
using JumpsellerSync.Domain.Impl.Tecnoglobal;

using Microsoft.Extensions.Logging;

using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Provider.Tecnoglobal.Services
{
    public class TecnoglobalProductService
        : ProviderProductService<TecnoglobalProduct,
                                 TecnoglobalBrand,
                                 TecnoglobalCategory,
                                 TecnoglobalProductService>,
          IProviderProductService
    {
        private static readonly string[] skuSkipUpdateProperties =
            CreateSkipPropertiesArray<TecnoglobalProduct>(
                nameof(TecnoglobalProduct.Id), nameof(TecnoglobalProduct.RedcetusProductId),
                nameof(TecnoglobalProduct.Brand), nameof(TecnoglobalProduct.Category),
                nameof(TecnoglobalProduct.Subcategory));

        public TecnoglobalProductService(
            ITecnoglobalProductRepository tecnoglobalProductRepository,
            ITecnoglobalBrandRepository tecnoglobalBrandRepository,
            ITecnoglobalCategoryRepository tecnoglobalCategoryRepository,
            IMapper mapper,
            ILogger<TecnoglobalProductService> logger)
            : base(
                tecnoglobalProductRepository,
                tecnoglobalBrandRepository,
                tecnoglobalCategoryRepository,
                mapper,
                logger)
        { }

        protected override string[] SkuSkipUpdateProperties => skuSkipUpdateProperties;
    }
}
