namespace hydra_login_consent_aspnet.Models
{
    public class LoginViewModel
    {
        public string Challenge { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
