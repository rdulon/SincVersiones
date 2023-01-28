
using JumpsellerSync.RestApi.Provider.Core.Controllers;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Tecnoglobal.Controllers
{
    public sealed class TecnoglobalPingController : PingController
    {
        public TecnoglobalPingController(JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        { }
    }
}