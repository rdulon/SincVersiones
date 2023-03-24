using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Intcomex.Controllers
{
    public sealed class IntcomexPingController : PingController
    {
        public IntcomexPingController(JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        { }
    }
}