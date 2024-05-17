using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/Roles")]
    public class RolesController : ApiControllerBase
    {
        IUsersService Service = null;

        public RolesController(IUsersService service,ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }

        // GET: api/Roles
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(Task<List<RoleDto>>))]
        public async Task<IHttpActionResult> GetRoles()
        {
            //throw new ApplicationException("Error getting roles...");

            var roleDtos = await Service.GetRoles();
            return Ok(roleDtos);
        }

        // GET: api/Roles/5
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(RoleDto))]
        public async Task<IHttpActionResult> GetRole(Guid id)
        {
            RoleDto dto = await Service.GetRole(id);
            return Ok(dto);
        }
    }
}
