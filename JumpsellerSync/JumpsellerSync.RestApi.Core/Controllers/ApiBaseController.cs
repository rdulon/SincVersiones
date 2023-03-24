using JumpsellerSync.BusinessLogic.Core;

using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace JumpsellerSync.RestApi.Core.Controllers
{
    public abstract class ApiBaseController : ControllerBase
    {
        protected readonly JsonSerializerOptions jsonSerializerOptions;

        public ApiBaseController(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        protected ActionResult Json(object dto, bool notFoundWhenNull = false)
            => dto == null && notFoundWhenNull
                ? (ActionResult)NotFound()
                : new JsonResult(dto, jsonSerializerOptions);

        protected virtual ActionResult FromServiceResult<TData>(ServiceResult<TData> serviceResult)
            => serviceResult.Status switch
            {
                ServiceResultStatus.Succeed => Json(serviceResult.Data),
                ServiceResultStatus.NotFound => NotFound(new { serviceResult.Errors }),
                ServiceResultStatus.UnAuthorized => Unauthorized(),
                ServiceResultStatus.BadInput => BadRequest(new { serviceResult.Errors }),
                var err when
                    err == ServiceResultStatus.Error ||
                    err == ServiceResultStatus.Exception => new StatusCodeResult(500),
                _ => BadRequest()
            };

        protected virtual ActionResult FromServiceResult(ServiceResult serviceResult)
            => serviceResult.Status switch
            {
                ServiceResultStatus.Succeed => Json(new { }),
                ServiceResultStatus.NotFound => NotFound(new { serviceResult.Errors }),
                ServiceResultStatus.UnAuthorized => Unauthorized(),
                ServiceResultStatus.BadInput => BadRequest(new { serviceResult.Errors }),
                var err when
                    err == ServiceResultStatus.Error ||
                    err == ServiceResultStatus.Exception => new StatusCodeResult(500),
                _ => BadRequest()
            };
    }
}