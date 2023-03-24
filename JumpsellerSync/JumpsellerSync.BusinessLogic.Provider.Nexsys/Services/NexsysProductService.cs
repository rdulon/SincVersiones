using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.BusinessLogic.Provider.Impl.Services;
using JumpsellerSync.DataAccess.Core.Repositories.Nexsys;
using JumpsellerSync.Domain.Impl.Nexsys;

using Microsoft.Extensions.Logging;

using static JumpsellerSync.Common.Util.Services.ServiceUtilities;

namespace JumpsellerSync.BusinessLogic.Provider.Nexsys.Services
{
    public class NexsysProductService
        : ProviderProductService<NexsysProduct,
                                 NexsysBrand,
                                 NexsysCategory,
                                 NexsysProductService>,
          IProviderProductService
    {
        private static readonly string[] skuSkipUpdateProperties =
            CreateSkipPropertiesArray<NexsysProduct>(
                nameof(NexsysProduct.Id), nameof(NexsysProduct.RedcetusProductId),
                nameof(NexsysProduct.Brand), nameof(NexsysProduct.Category));

        public NexsysProductService(
            INexsysProductRepository nexsysProductRepository,
            INexsysBrandRepository nexsysBrandRepository,
            INexsysCategoryRepository nexsysCategoryRepository,
            IMapper mapper,
            ILogger<NexsysProductService> logger)
            : base(
                nexsysProductRepository,
                nexsysBrandRepository,
                nexsysCategoryRepository,
                mapper,
                logger)
        { }

        protected override string[] SkuSkipUpdateProperties => skuSkipUpdateProperties;
    }
}
