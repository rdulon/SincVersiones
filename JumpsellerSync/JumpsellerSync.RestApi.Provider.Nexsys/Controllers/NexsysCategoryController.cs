using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Intcomex.Controllers
{
    public class NexsysCategoryController : CategoryController
    {
        public NexsysCategoryController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(providerProductService, jsonSerializerOptions)
        { }
    }
}