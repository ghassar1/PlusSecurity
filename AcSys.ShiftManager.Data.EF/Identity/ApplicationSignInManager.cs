using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AcSys.ShiftManager.Model;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace AcSys.ShiftManager.Data.EF.Identity
{
    // Configure the application sign-in manager which is used in this application.  
    public class ApplicationSignInManager : SignInManager<User, Guid>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager)
        {

        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            //return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
            return (this.UserManager as ApplicationUserManager).GenerateUserIdentityAsync(user);
        }

        //public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        //{
        //    return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        //}
    }
}
