using AcSys.Core.Data.Model.Base;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;
using Elmah.Contrib.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiControllerBase
    {
        //static NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        IUsersService Service = null;

        public UsersController(IUsersService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
            // wrong
            //Logger = LogManager.GetCurrentClassLogger();

            //Logger = logger;

            //Logger.Trace("Sample trace message");
            //Logger.Debug("Sample debug message");
            //Logger.Info("Sample informational message");
            //Logger.Warn("Sample warning message");
            //Logger.Error("Sample error message");
            //Logger.Fatal("Sample fatal error message");

            //// alternatively you can call the Log() method and pass log level as the parameter.
            //Logger.Log(LogLevel.Info, "Sample informational message");

            //throw new ApplicationException("Me");
        }

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

        // GET: api/Users/Test
        [HttpGet]
        [Route("Test")]
        [AllowAnonymous]
        [ResponseType(typeof(string))]
        public IHttpActionResult Test()
        {
            Logger.Trace("Sample trace message");
            Logger.Debug("Sample debug message");
            Logger.Info("Sample informational message");
            Logger.Warn("Sample warning message");
            Logger.Error("Sample error message");
            Logger.Fatal("Sample fatal error message");

            // alternatively you can call the Log() method and pass log level as the parameter.
            Logger.Log(NLog.LogLevel.Info, "Sample informational message");

            //return Ok(DateTime.Now.ToLongTimeString());
            return Ok(Logger.Name);
        }

        List<ElmahError> GetElmahErrors()
        {
            var elmahContext = new ElmahContext("Elmah.SqlServer"); //MainConnection

            string elmahAppName = Elmah.ErrorLog.GetDefault(HttpContext.Current).ApplicationName;
            List<ElmahError> errors = elmahContext.ElmahErrors.Where(o => o.Application == elmahAppName).ToList();
            return errors;
        }

        // GET: api/Users
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(Task<List<UserDto>>))]
        public async Task<IHttpActionResult> Get([FromUri]FindUsersQuery query)
        {
            query.ExcludeRoles = new string[] { AppConstants.RoleNames.Employee };
            query.Status = EntityStatus.Active;
            var users = await Service.GetUsers(query);
            return Ok(users);
        }

        // GET: api/Users/All
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpGet]
        [Route("All")]
        [ResponseType(typeof(Task<List<UserDto>>))]
        public async Task<IHttpActionResult> GetAll([FromUri]FindUsersQuery query)
        {
            query.ExcludeRoles = new string[] { AppConstants.RoleNames.Employee };
            var users = await Service.GetUsers(query);
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet]
        [Route("{id}", Name = "GetUser")]
        [ResponseType(typeof(UserDto))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            return Ok(await Service.GetUser(id));
        }

        // POST: api/Users
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpPost]
        [Route("", Name = "PostUser")]
        //[ResponseType(typeof(UserDto))]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Post(UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid userId = await Service.CreateUser(dto);
            //return CreatedAtRoute("GetUser", new { id = dto.Id }, userId);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Users/5
        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        //[Bind(Include = "DateOfBirth, FirstName, LastName, Mobile, PhoneNumber")]
        public async Task<IHttpActionResult> Put(Guid id, UserDto userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.UpdateUser(id, userDto);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Users/5
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpDelete]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            await Service.DeleteUser(id);
            return Ok();
        }

        // Put: api/Users/5/Activate
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpPut]
        [Route("{id}/Activate")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Activate(Guid id)
        {
            await Service.ActivateUser(id);
            return Ok();
        }

        // Put: api/Users/5/Deactivate
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpPut]
        [Route("{id}/Deactivate")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Deactivate(Guid id)
        {
            await Service.DeactivateUser(id);
            return Ok();
        }

        // GET api/Users/Me
        [HttpGet]
        [Route("me")]
        //[Authorize(Roles = AppConstants.RoleNames.SuperAdmin)]
        [ResponseType(typeof(UserDto))]
        public UserDto Me()
        {
            return Service.Me();
        }

        [HttpGet]
        [Route("Notifications")]
        [ResponseType(typeof(Task<List<UserNotificationDto>>))]
        public async Task<IHttpActionResult> GetUserNotifications()
        {
            var notifications = await Service.GetUserNotifications();
            return Ok(notifications);
        }
    }
}
