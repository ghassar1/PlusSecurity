using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AcSys.Core.Data.Model.Base;
using AcSys.ShiftManager.Data.Users;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/Employees")]
    public class EmployeesController : ApiControllerBase
    {
        //static NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        IUsersService Service = null;

        public EmployeesController(IUsersService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }

        // GET: api/Employees
        [Authorize(Roles =
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(Task<List<UserDto>>))]
        public async Task<IHttpActionResult> Get([FromUri]FindUsersQuery query)
        {
            query.IncludeRoles = new string[] { AppConstants.RoleNames.Employee };
            query.Status = EntityStatus.Active;
            var users = await Service.GetUsers(query);
            return Ok(users);
        }

        // GET: api/Employees/All
        [Authorize(Roles =
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpGet]
        [Route("All")]
        [ResponseType(typeof(Task<List<UserDto>>))]
        public async Task<IHttpActionResult> GetAll([FromUri]FindUsersQuery query)
        {
            query.IncludeRoles = new string[] { AppConstants.RoleNames.Employee };
            var users = await Service.GetUsers(query);
            return Ok(users);
        }

        // GET: api/Employees/5
        [HttpGet]
        [Route("{id}", Name = "GetEmployee")]
        [ResponseType(typeof(UserDto))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            return Ok(await Service.GetUser(id));
        }

        // POST: api/Employees
        [Authorize(Roles =
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpPost]
        [Route("", Name = "PostEmployee")]
        //[ResponseType(typeof(UserDto))]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Post(UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid id = await Service.CreateUser(dto);
            //return CreatedAtRoute("GetEmployee", new { id = dto.Id }, id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Employees/5
        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        //[Bind(Include = "DateOfBirth, FirstName, LastName, Mobile, PhoneNumber")]
        public async Task<IHttpActionResult> Put(Guid id, UserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.UpdateUser(id, dto);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Employees/5
        [Authorize(Roles =
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpDelete]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteUser(Guid id)
        {
            await Service.DeleteUser(id);
            return Ok();
        }

        // Put: api/Employees/5/Activate
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

        // Put: api/Employees/5/Deactivate
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
    }
}
