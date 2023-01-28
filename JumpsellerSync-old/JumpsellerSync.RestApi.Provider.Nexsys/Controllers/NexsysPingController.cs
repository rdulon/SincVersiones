using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Intcomex.Controllers
{
    public sealed class NexsysPingController : PingController
    {
        public NexsysPingController(JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        { }
    }
}