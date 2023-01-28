using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Intcomex.Controllers
{
    public class IntcomexBrandController : BrandController
    {
        public IntcomexBrandController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions,
            IMapper mapper)
            : base(providerProductService, jsonSerializerOptions, mapper)
        { }
    }
}