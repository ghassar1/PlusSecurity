using System;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using AcSys.Core.Email;
using AcSys.Core.Extensions;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Common;
using Autofac.Extras.NLog;
using Microsoft.AspNet.Identity;

namespace AcSys.ShiftManager.Service.Base
{
    public abstract class ApplicationServiceBase : IApplicationService
    {
        public IUnitOfWork UnitOfWork { get; set; }

        //ApplicationUserManager _userManager;
        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager;// ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        public ApplicationRoleManager RoleManager { get; set; }
        public ApplicationUserManager UserManager { get; set; }

        //static ILogger logger = LogManager.GetCurrentClassLogger();
        //static ILogger logger = LogManager.GetLogger("logfile");
        //static ILogger logger = LogManager.GetLogger("console");
        //static ILogger logger = LogManager.GetLogger("database");
        //static ILogger logger = LogManager.GetLogger("*");
        //static ILogger logger = new LogFactory(LogManager.Configuration).GetCurrentClassLogger();
        //static ILogger Logger = LogManager.GetCurrentClassLogger();
        protected ILogger Logger { get; set; }

        public IEmailService EmailService { get; set; }

        public ApplicationServiceBase(
            IUnitOfWork unitOfWork,
            ApplicationRoleManager roleManager,
            ApplicationUserManager userManager,
            ILogger logger,
            IEmailService emailService
            )
        {
            UnitOfWork = unitOfWork;
            RoleManager = roleManager;
            UserManager = userManager;
            Logger = logger;
            EmailService = emailService;
        }

        protected IPrincipal User
        {
            get
            {
                //IPrincipal user = HttpContext.Current.User;// as IPrincipal;
                //if (user == null)
                //    return null;//GuestUser; // Just some default user object
                //return user;
                return Thread.CurrentPrincipal;
            }
        }

        protected Guid UserId
        {
            get
            {
                if (!User.Identity.IsAuthenticated) Unauthorized();

                string userId = User.Identity.Name;
                if (string.IsNullOrWhiteSpace(userId)) Unauthorized();

                return Guid.Parse(userId);
            }
        }

        protected string UserName
        {
            get
            {
                if (!User.Identity.IsAuthenticated) Unauthorized();

                string userName = User.Identity.GetUserName();
                return userName;
            }
        }

        User _loggedInUser = null;
        protected User LoggedInUser
        {
            get
            {
                if (_loggedInUser == null)
                {
                    // Find by username instead. Id changes when database is recreated during development.
                    //User user = UserManager.FindById(UserId);
                    _loggedInUser = UserManager.FindByName(UserName);
                    if (_loggedInUser == null) Unauthorized();
                }
                return _loggedInUser;
            }
        }

        protected bool LoggedInUserIsNot(User user)
        {
            return !LoggedInUserIs(user);
        }

        protected bool LoggedInUserIs(User user)
        {
            return LoggedInUser.Id == user.Id;
        }

        protected void LoggedInUserShouldBe(User user)
        {
            if (LoggedInUserIsNot(user)) Unauthorized();
        }

        protected void LoggedInUserShouldNotBe(User user)
        {
            if (LoggedInUserIs(user)) Unauthorized();
        }

        protected Role LoggedInUserRole
        {
            get
            {
                var userRole = LoggedInUser.GetRole();
                return userRole;
            }
        }

        protected void LoggedInUserShouldHaveAnyRoleIn(params string[] roles)
        {
            if (!LoggedInUserHasAnyRoleIn(roles))
                Unauthorized();
        }

        protected bool LoggedInUserHasAnyRoleIn(params string[] roles)
        {
            if (roles == null || roles.Length < 1)
                throw new ApplicationException("Roles not specified.");

            return roles.Any(role => this.User.IsInRole(role));
        }

        protected bool LoggedInUserHasNoRoleIn(params string[] roles)
        {
            return !LoggedInUserHasAnyRoleIn(roles);
        }

        protected void LoggedInUserShouldHaveRoles(params string[] roles)
        {
            if (!LoggedInUserHasRoles(roles))
                Unauthorized();
        }

        protected bool LoggedInUserHasRoles(params string[] roles)
        {
            if (roles == null || roles.Length < 1)
                throw new ApplicationException("Roles not specified.");

            return roles.All(role => this.User.IsInRole(role));
        }

        protected void Unauthorized(string message = "")
        {
            throw message.IsNullOrWhiteSpace() ? new UnauthorizedAccessException()
                : new UnauthorizedAccessException(message);
        }

        protected void Forbidden(string message = "")
        {
            throw message.IsNullOrWhiteSpace() ? new ForbiddenException()
                : new ForbiddenException(message);
        }

        protected void NotFound(string message = "")
        {
            throw message.IsNullOrWhiteSpace() ? new NotFoundException()
                : new NotFoundException(message);
        }

        protected void BadRequest(string message = "")
        {
            throw message.IsNullOrWhiteSpace() ? new BadRequestException()
                : new BadRequestException(message);
        }

        #region Validation Checks

        protected void CheckIfPropertyHasValue(string propertyName, string value)
        {
            if (value.IsNullOrWhiteSpace())
                BadRequest("{0} must be specified.".FormatWith(propertyName));
        }

        protected void CheckIfPropertyHasValue(string propertyName, DateTime? value)
        {
            if (value.IsNullOrEmpty())
                BadRequest("{0} must be specified.".FormatWith(propertyName));
        }

        #endregion

        #region IDisposable Support
        bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ApplicationServiceBase() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        protected static string GetBaseURL()
        {
            string activationURLBase = ConfigurationManager.AppSettings["BaseUrl"];
            if (!activationURLBase.EndsWith("/", StringComparison.CurrentCulture))
                activationURLBase += "/";
            return activationURLBase;
        }
    }
}
