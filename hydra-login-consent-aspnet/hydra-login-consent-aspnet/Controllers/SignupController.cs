using hydra_login_consent_aspnet.Models;
using hydra_login_consent_aspnet.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace hydra_login_consent_aspnet.Controllers
{
    public class SignupController : Controller
    {
        private readonly ILogger<SignupController> logger;
        private readonly IRedisConnectionFactory redisConnectionFactory;

        public SignupController(ILogger<SignupController> logger, IRedisConnectionFactory redisConnectionFactory)
        {
            this.logger = logger;
            this.redisConnectionFactory = redisConnectionFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Success()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm]SignupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var database = redisConnectionFactory.GetDatabase();
            if (database == null)
            {
                var message = "Connectivity error in the backend. Please try again later.";
                logger.LogError(message);
                ModelState.AddModelError("", message);

                return View(viewModel);
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);
            var success = await database.StringSetAsync($"email:{viewModel.Email}", hashedPassword, null, StackExchange.Redis.When.NotExists);
            if (!success)
            {
                var message = "This email already exists in the system.";
                logger.LogError(message);
                ModelState.AddModelError("", message);

                return View(viewModel);
            }

            return RedirectToAction("Success", "Signup");
        }
    }
}
