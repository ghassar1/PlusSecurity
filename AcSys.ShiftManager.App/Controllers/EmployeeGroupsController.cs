using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AcSys.ShiftManager.Service.EmployeeGroups;
using AcSys.ShiftManager.Service.Results;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/EmployeeGroups")]
    public class EmployeeGroupsController : ApiControllerBase
    {
        IEmployeeGroupsService Service = null;

        public EmployeeGroupsController(IEmployeeGroupsService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }

        // GET: api/EmployeeGroups
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(Task<IListResult<EmployeeGroupDto>>))]
        public async Task<IHttpActionResult> Get()
        {
            var dtos = await Service.Get();
            return Ok(dtos);
        }

        // GET: api/EmployeeGroups/5
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(EmployeeGroupDto))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            EmployeeGroupDto dto = await Service.Get(id);
            return Ok(dto);
        }

        // POST: api/EmployeeGroups
        [HttpPost]
        [Route("", Name = "PostEmployeeGroup")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Post(EmployeeGroupDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid id = await Service.Create(dto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/EmployeeGroups/5
        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(Guid id, EmployeeGroupDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.Update(id, dto);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/EmployeeGroups/5
        [HttpDelete]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Delete(Guid id)
        {
            await Service.Delete(id);
            return Ok();
        }
    }
}
