using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Linkstore.Controllers
{
    public sealed class LinkstorePingController : PingController
    {
        public LinkstorePingController(JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        {

        }
    }
}