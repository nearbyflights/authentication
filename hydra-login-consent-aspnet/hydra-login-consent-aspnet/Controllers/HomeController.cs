using Microsoft.AspNetCore.Mvc;

namespace hydra_login_consent_aspnet.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
