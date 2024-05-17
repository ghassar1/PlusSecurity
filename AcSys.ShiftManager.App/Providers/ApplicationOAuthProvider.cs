using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Model.Helpers;
using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using NLog;

namespace AcSys.ShiftManager.App.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        static ILogger Logger = LogManager.GetCurrentClassLogger();
        
        readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
                throw new ArgumentNullException(nameof(publicClientId));

            _publicClientId = publicClientId;
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
                else if (context.ClientId == "web")
                {
                    var expectedUri = new Uri(context.Request.Uri, "/");
                    context.Validated(expectedUri.AbsoluteUri);
                }
            }

            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //TODO: check following three commented lines and set allowed origin in owin context.
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            if (allowedOrigin == null) allowedOrigin = "*";
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            ////context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "http://localhost:29186" });
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" });
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "content-type", "accept" });

            //var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            //var userManager = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ApplicationUserManager)) as ApplicationUserManager;

            // OwinContext is still under construction so the request scope is not available at this point.
            // http://stackoverflow.com/questions/29591545/resolve-autofac-service-within-instanceperlifetimescope-on-owin-startup
            //ILifetimeScope lifetimeScope = context.OwinContext.Environment.FirstOrDefault(f => f.Key == "autofac:OwinLifetimeScope").Value as ILifetimeScope;
            ILifetimeScope lifetimeScope = context.OwinContext.GetAutofacLifetimeScope();
            if (lifetimeScope == null) throw new Exception("RequestScope cannot be null...");

            var userManager = lifetimeScope.Resolve<ApplicationUserManager>();

            var dbContext = lifetimeScope.Resolve<ApplicationDbContext>();
            if (dbContext == null) throw new Exception("dbContext cannot be null...");

            //User user = await userManager.FindAsync(context.UserName, context.Password);
            User user = await userManager.FindAsync(context.UserName, context.Password);

            //User user = await userManager.FindByNameAsync(context.UserName);
            //string sPasswordHash = new PasswordHasher().HashPassword(context.Password);
            //if (user == null || user.PasswordHash != sPasswordHash)
            //{
            //}

            //var userManager = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserService)) as UserService;
            //UserDto user = await userManager.AuthenticateUser(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            else if (user.IsNotActive())
            {
                context.SetError("account_deactivated", "Your account has been deactivated. Please contact the system administrator.");
                return;
            }

            ClaimsIdentity userIdentity = await userManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
            userIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            //userIdentity.AddClaim(new Claim(ClaimTypes.Sid, user.UserName));
            //userIdentity.AddClaim(new Claim(ClaimTypes.Hash, context.Password));
            userIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            //userIdentity.AddClaim(new Claim(ClaimTypes.UserData, user.Email));
            userIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            userIdentity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
            userIdentity.AddClaim(new Claim(ClaimTypes.Version, "1.0"));
            //userIdentity.AddClaim(new Claim("test", "testdata"));

            Role role = user.GetRole();
            if (role != null)
                userIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));

            AuthenticationProperties properties = CreateProperties(user.Email); //context.UserName
            AuthenticationTicket ticket = new AuthenticationTicket(userIdentity, properties);
            context.Validated(ticket);
            //context.Request.Context.Authentication.SignIn();
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);

            //Logger = lifetimeScope.Resolve<Autofac.Extras.NLog.ILogger>();
            //Logger = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(Autofac.Extras.NLog.ILogger)) as Autofac.Extras.NLog.ILogger;
            Logger.Info("{0} logged in.", user.Email);

            user.AddLog(Enums.SubjectType.None, Enums.ActivityType.Login, "{0} logged in".FormatWith(user.ToString()));

            await dbContext.SaveChangesAsync();
        }

        //public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        //{
        //    foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
        //    {
        //        context.AdditionalResponseParameters.Add(property.Key, property.Value);
        //    }
        //    return Task.FromResult<object>(null);
        //}

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}
