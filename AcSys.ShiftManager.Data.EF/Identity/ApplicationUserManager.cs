using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AcSys.ShiftManager.Model;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;

namespace AcSys.ShiftManager.Data.EF.Identity
{
    public class ApplicationUserManager : UserManager<User, Guid>//AcSysUserManager<User, Role, UserRole, UserClaim, UserLogin>
    {
        public ApplicationUserManager(
            IUserStore<User, Guid> store, 
            IIdentityMessageService identityMessageService, 
            IDataProtectionProvider provider,
            IUserTokenProvider<User, Guid> userTokenProvider)
            : base(store)   //, identityMessageService, provider, userTokenProvider)
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<User, Guid>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User, Guid>
            {
                MessageFormat = "Your security code is {0}"
            });
            this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User, Guid>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            this.EmailService = identityMessageService;
            //this.SmsService = new IdentitySmsService();

            //var dataProtectionProvider = options.DataProtectionProvider;
            //if (dataProtectionProvider != null)
            //{
            //    this.UserTokenProvider =
            //        new DataProtectorTokenProvider<User, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
            //}

            //IDataProtectionProvider provider = new DpapiDataProtectionProvider("AcSys.Core");
            //this.UserTokenProvider = new DataProtectorTokenProvider<User, Guid>(provider.Create("EmailConfirmation"));
            this.UserTokenProvider = userTokenProvider;

            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<User, Guid>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                //RequireDigit = true,
                //RequireLowercase = true,
                //RequireUppercase = true,
            };
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(User user)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await this.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        //public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
        //    IOwinContext context)
        //{
        //    var manager = new ApplicationUserManager(new UserStore<User, Role, Guid, UserLogin, UserRole, UserClaim>(context.Get<ApplicationDbContext>()));
        //    // Configure validation logic for usernames
        //    manager.UserValidator = new UserValidator<User, Guid>(manager)
        //    {
        //        AllowOnlyAlphanumericUserNames = false,
        //        RequireUniqueEmail = true
        //    };

        //    // Configure validation logic for passwords
        //    manager.PasswordValidator = new PasswordValidator
        //    {
        //        RequiredLength = 6,
        //        RequireNonLetterOrDigit = true,
        //        RequireDigit = true,
        //        RequireLowercase = true,
        //        RequireUppercase = true,
        //    };

        //    // Configure user lockout defaults
        //    manager.UserLockoutEnabledByDefault = true;
        //    manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
        //    manager.MaxFailedAccessAttemptsBeforeLockout = 5;

        //    // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
        //    // You can write your own provider and plug it in here.
        //    manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<User, Guid>
        //    {
        //        MessageFormat = "Your security code is {0}"
        //    });
        //    manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<User, Guid>
        //    {
        //        Subject = "Security Code",
        //        BodyFormat = "Your security code is {0}"
        //    });
        //    manager.EmailService = new IdentityEmailService(new EmailService());
        //    manager.SmsService = new IdentitySmsService();
        //    var dataProtectionProvider = options.DataProtectionProvider;
        //    if (dataProtectionProvider != null)
        //    {
        //        manager.UserTokenProvider =
        //            new DataProtectorTokenProvider<User, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
        //    }
        //    return manager;
        //}
    }
}
