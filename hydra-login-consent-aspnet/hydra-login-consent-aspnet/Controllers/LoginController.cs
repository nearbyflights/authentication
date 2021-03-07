using hydra_login_consent_aspnet.Models;
using hydra_login_consent_aspnet.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ory.Hydra.Client.Api;
using Ory.Hydra.Client.Client;
using Ory.Hydra.Client.Model;
using System.Threading.Tasks;

namespace hydra_login_consent_aspnet.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<LoginController> logger;
        private readonly IRedisConnectionFactory redisConnectionFactory; 

        public LoginController(IConfiguration configuration, ILogger<LoginController> logger, IRedisConnectionFactory redisConnectionFactory)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.redisConnectionFactory = redisConnectionFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery(Name = "login_challenge")] string loginChallenge)
        {
            if (Request.Headers.ContainsKey("User-Agent"))
            {
                logger.LogInformation(Request.Headers["User-Agent"].ToString());
            }

            if (string.IsNullOrEmpty(loginChallenge))
            {
                var message = "Expected a login challenge to be set but received none.";
                logger.LogError(message);

                return BadRequest(message);
            }

            var hydraSettings = this.configuration.GetSection(nameof(Hydra)).Get<Hydra>();
            var configuration = new Configuration { BasePath = hydraSettings.HydraAdminUrl };
            var adminApi = new AdminApi(configuration);

            var response = await adminApi.GetLoginRequestAsync(loginChallenge);

            if (response.Skip)
            {
                var completed = await adminApi.AcceptLoginRequestAsync(
                    loginChallenge,
                    new HydraAcceptLoginRequest { Subject = response.Subject }
                );

                return Redirect(completed.RedirectTo);
            }

            return View(new LoginViewModel { Challenge = loginChallenge });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel viewModel)
        {
            var database = redisConnectionFactory.GetDatabase();

            if (database == null)
            {
                var message = "Connectivity error in the backend. Please try again later.";
                logger.LogError(message);

                ModelState.AddModelError("", message);

                return View(viewModel);
            }

            var value = await database.StringGetAsync($"email:{viewModel.Email}");
            if (value.IsNull || !BCrypt.Net.BCrypt.Verify(viewModel.Password, value))
            {
                var message = "The username/password combination is not correct.";
                logger.LogWarning(message);

                ModelState.AddModelError("", message);

                return View(viewModel);
            }

            var hydraSettings = this.configuration.GetSection(nameof(Hydra)).Get<Hydra>();
            var configuration = new Configuration { BasePath = hydraSettings.HydraAdminUrl };
            var adminApi = new AdminApi(configuration);

            var completed = await adminApi.AcceptLoginRequestAsync(
                viewModel.Challenge,
                new HydraAcceptLoginRequest(subject: viewModel.Email, remember: viewModel.RememberMe, rememberFor: 3600));

            logger.LogInformation($"User {viewModel.Email} logged successfully!");

            logger.LogInformation($"Redirect to: ${completed.RedirectTo}");

            return Redirect(completed.RedirectTo);
        }

        public IActionResult Test()
        {
            return View("Index");
        }
    }
}
