using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Linkstore.Controllers
{
    public class LinkstoreBrandController : BrandController
    {
        public LinkstoreBrandController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions,
            IMapper mapper)
            : base(providerProductService, jsonSerializerOptions, mapper)
        { }
    }
}