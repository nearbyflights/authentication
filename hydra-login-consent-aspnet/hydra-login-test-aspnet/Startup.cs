using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace hydra_login_test_aspnet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddCookie("Cookies", options => { options.Cookie.SameSite = SameSiteMode.Lax; })
                .AddOpenIdConnect(options =>
                {
                    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    options.NonceCookie.SameSite = SameSiteMode.Lax;
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "http://localhost:4444";
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.ClientId = "test-client";
                    options.CallbackPath = new PathString("/callback");
                    options.ClientSecret = "secret";
                    options.SaveTokens = true;
                    options.RequireHttpsMetadata = false;
                    options.UsePkce = true;
                    options.Scope.Remove("profile");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
           
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
