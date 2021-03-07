using System.ComponentModel.DataAnnotations;

namespace hydra_login_consent_aspnet.Models
{
    public class SignupViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
