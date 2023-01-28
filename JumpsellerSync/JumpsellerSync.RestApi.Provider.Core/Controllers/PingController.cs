using JumpsellerSync.RestApi.Core.Controllers;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Provider.Core.Controllers
{
    [ApiController]
    [Route("api/ping")]
    public abstract class PingController : ApiBaseController
    {
        public PingController(JsonSerializerOptions jsonSerializerOptions)
            : base(jsonSerializerOptions)
        { }

        [HttpGet]
        public ActionResult Ping()
        {
            return Json("Pong");
        }
    }
}
