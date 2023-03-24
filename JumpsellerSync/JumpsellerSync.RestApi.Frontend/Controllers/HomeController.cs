using Microsoft.AspNetCore.Mvc;

namespace JumpsellerSync.RestApi.FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}