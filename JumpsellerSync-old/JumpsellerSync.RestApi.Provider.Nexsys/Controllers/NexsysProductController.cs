using AutoMapper;

using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Intcomex.Controllers
{
    public class NexsysProductController : ProductController
    {
        public NexsysProductController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions,
            IMapper mapper)
            : base(providerProductService, jsonSerializerOptions, mapper)
        { }
    }
}