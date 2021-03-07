using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ory.Hydra.Client.Api;
using Ory.Hydra.Client.Client;
using Ory.Hydra.Client.Model;
using System.Threading.Tasks;

namespace hydra_login_consent_aspnet.Controllers
{
    public class ConsentController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<ConsentController> logger;

        public ConsentController(IConfiguration configuration, ILogger<ConsentController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery(Name = "consent_challenge")]string consentChallenge)
        {
            if (string.IsNullOrEmpty(consentChallenge))
            {
                var message = "Expected a consent challenge to be set but received none.";
                logger.LogError(message);

                return BadRequest(message);
            }

            var hydraSettings = this.configuration.GetSection(nameof(Hydra)).Get<Hydra>();
            var configuration = new Configuration { BasePath = hydraSettings.HydraAdminUrl };
            var adminApi = new AdminApi(configuration);

            var response = await adminApi.GetConsentRequestAsync(consentChallenge);
            
            // we don't want to show the consent UI to the user, skip it
            var completed = await adminApi.AcceptConsentRequestAsync(
                consentChallenge,
                new HydraAcceptConsentRequest 
                { 
                    GrantScope = response.RequestedScope, 
                    GrantAccessTokenAudience = response.RequestedAccessTokenAudience ,
                    Session = new HydraConsentRequestSession { IdToken = new SessionTest() }
                });
            
            return Redirect(completed.RedirectTo);
        }
    }

    public class SessionTest
    {
        public string Test { get; set; } = "Test2";
    }
}
