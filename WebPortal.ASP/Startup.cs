using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Security.Claims;
using System.Web.Helpers;
using WebPortal.ASP.Security;
[assembly: OwinStartup(typeof(WebPortal.ASP.Startup))]
namespace WebPortal.ASP
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Links anti-forgery tokens to the user's unique ID to prevent CSRF attacks and fixes the "claim not present".
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    AuthenticationType = AuthenticationConstants.AuthenticationType,
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    CookieName = "WebPortal.Auth",
                    LoginPath = new PathString("/Login/Index"),
                    ExpireTimeSpan = TimeSpan.FromMinutes(5),
                    SlidingExpiration = true,
                    CookieHttpOnly = true,
                    CookieSecure = CookieSecureOption.SameAsRequest,
                    Provider = new CookieAuthenticationProvider
                    {
                        //Prevents Hijacking Through matching -> user agent
                        OnValidateIdentity = async context =>
                        {
                            var agentClaim = context.Identity.FindFirst(AuthenticationConstants.UserAgentClaimType);

                            if (agentClaim == null)
                            {
                                context.RejectIdentity();
                                return;
                            }
                            string currentAgent = context.Request.Headers.Get("User-Agent") ?? "";
                            if (agentClaim.Value != currentAgent)
                            {
                                context.RejectIdentity();
                            }
                        }
                    }
                });
        }
    }
}