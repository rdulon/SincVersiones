using JumpsellerSync.BusinessLogic.Core;
using JumpsellerSync.RestApi.Core.ViewModels;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace JumpsellerSync.RestApi.Core.Controllers
{
    public abstract class BaseController : Controller
    {
        protected virtual ActionResult FromServiceResult(ServiceResult serviceResult)
            => serviceResult.Status switch
            {
                ServiceResultStatus.Succeed => Ok(),
                ServiceResultStatus.NotFound => NotFound(),
                ServiceResultStatus.UnAuthorized => Unauthorized(),
                ServiceResultStatus.BadInput => BadRequest(new { serviceResult.Errors }),
                var err when
                    err == ServiceResultStatus.Error ||
                    err == ServiceResultStatus.Exception => new StatusCodeResult(500),
                _ => BadRequest()
            };

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public virtual IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        protected bool IsAjaxRequest
            => HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}