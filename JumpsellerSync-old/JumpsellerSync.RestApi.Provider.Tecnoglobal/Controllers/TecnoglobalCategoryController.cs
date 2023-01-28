
using JumpsellerSync.BusinessLogic.Core.Services.Provider;
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Tecnoglobal.Controllers
{
    public class TecnoglobalCategoryController : CategoryController
    {
        public TecnoglobalCategoryController(
            IProviderProductService providerProductService,
            JsonSerializerOptions jsonSerializerOptions)
            : base(providerProductService, jsonSerializerOptions)
        { }
    }
}