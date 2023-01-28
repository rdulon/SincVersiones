using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Linkstore.Controllers
{
    public class LinkstoreCategoryController : CategoryController
    {
        public LinkstoreCategoryController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(providerProductService, jsonSerializerOptions)
        { }
    }
}