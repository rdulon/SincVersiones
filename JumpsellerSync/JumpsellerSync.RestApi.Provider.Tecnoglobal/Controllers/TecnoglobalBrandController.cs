
using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Tecnoglobal.Controllers
{
    public class TecnoglobalBrandController : BrandController
    {
        public TecnoglobalBrandController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions,
            IMapper mapper)
            : base(providerProductService, jsonSerializerOptions, mapper)
        { }
    }
}