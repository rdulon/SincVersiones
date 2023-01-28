using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Linkstore.Controllers
{
    public class LinkstoreProductController : ProductController
    {
        public LinkstoreProductController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions,
            IMapper mapper)
            : base(providerProductService, jsonSerializerOptions, mapper)
        { }
    }
}