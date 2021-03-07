using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace hydra_login_test_aspnet.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Secure()
        {
            ViewData["access_token"] = await this.HttpContext.GetTokenAsync("access_token");
            return View();
        }
    }
}
