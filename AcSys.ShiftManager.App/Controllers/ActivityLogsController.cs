using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AcSys.ShiftManager.Data.ActivityLogs;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.ActivityLogs;
using AcSys.ShiftManager.Service.Results;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [RoutePrefix("api/ActivityLogs")]
    [Authorize]
    public class ActivityLogsController : ApiControllerBase
    {
        public IActivityLogsService Service { get; set; }

        public ActivityLogsController(IActivityLogsService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }

        // GET: api/ActivityLogs
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(Task<IListResult<ActivityLog, ActivityLogListItemDto>>))]
        public async Task<IHttpActionResult> GetActivityLogs([FromUri]FindLogsQuery query)
        {
            IListResult<ActivityLog, ActivityLogListItemDto> result = await Service.Get(query);
            return Ok(result);
        }

        //GET: api/ActivityLogs/5
        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(ActivityLogDetailsDto))]
        public async Task<IHttpActionResult> GetActivityLog(Guid id)
        {
            ActivityLogDetailsDto activityLog = await Service.Get(id);
            return Ok(activityLog);
        }

        // DELETE: api/ActivityLogs/5
        [ResponseType(typeof(void))]
        [Route("{id}")]
        public async Task<IHttpActionResult> DeleteActivityLog(Guid id)
        {
            await Service.Delete(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/ActivityLogs
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteActivityLog([FromBody]List<Guid> id)
        {
            await Service.Delete(id);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}