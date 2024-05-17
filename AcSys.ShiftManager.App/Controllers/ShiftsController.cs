using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AcSys.ShiftManager.Data.Shifts;
using AcSys.ShiftManager.Model.Helpers;
using AcSys.ShiftManager.Service.Shifts;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/Shifts")]
    public class ShiftsController : ApiControllerBase
    {
        IShiftsService Service = null;

        public ShiftsController(IShiftsService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }

        // GET: api/Shifts
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager + ", " +
            AppConstants.RoleNames.Employee)]
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(Task<RotaDto>))]
        public async Task<IHttpActionResult> Get([FromUri]FindShiftsQuery query)
        {
            return Ok(await Service.Get(query));
        }

        // GET: api/Shifts/5
        [HttpGet]
        [Route("{id}", Name = "GetShift")]
        [ResponseType(typeof(ShiftDto))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            return Ok(await Service.Get(id));
        }

        // POST: api/Shifts
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager + ", " +
            AppConstants.RoleNames.Employee)]
        [HttpPost]
        [Route("", Name = "PostShift")]
        [ResponseType(typeof(List<Guid>))]
        public async Task<IHttpActionResult> Post(CreateShiftDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            List<Guid> ids = await Service.Create(dto);
            //return CreatedAtRoute("GetShift", new { id = dto.Id }, shiftId);
            //return StatusCode(HttpStatusCode.NoContent);
            return Ok(ids);
        }

        // PUT: api/Shifts/5
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin + ", " +
            AppConstants.RoleNames.HRManager + ", " +
            AppConstants.RoleNames.RecManager)]
        [HttpPut]
        [Route("{id}")]
        [ResponseType(typeof(void))]
        //[Bind(Include = "DateOfBirth, FirstName, LastName, Mobile, PhoneNumber")]
        public async Task<IHttpActionResult> Put(Guid id, UpdateShiftDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.Update(id, dto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Shifts/5
        [Authorize(Roles = AppConstants.RoleNames.HRManager)]
        [HttpPut]
        [Route("{id}/Assign")]
        [ResponseType(typeof(void))]
        //[Bind(Include = "DateOfBirth, FirstName, LastName, Mobile, PhoneNumber")]
        public async Task<IHttpActionResult> Assign(Guid id, AssignShiftDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.Assign(id, dto);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Shifts/5/Take
        [Authorize(Roles = AppConstants.RoleNames.Employee)]
        [HttpPut]
        [Route("{id}/Take")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Take(Guid id)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            //TODO: check for conflicts with other shifts.
            await Service.Take(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Shifts/5/Leave
        [Authorize(Roles = AppConstants.RoleNames.Employee)]
        [HttpPut]
        [Route("{id}/Leave")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Leave(Guid id)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            //TODO: check if shift can be left i.e. enough time in starting (1 day or so).
            await Service.Leave(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Shifts/5/ClockIn
        [Authorize(Roles = AppConstants.RoleNames.Employee)]
        [HttpPut]
        [Route("{id}/ClockIn")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ClockIn(Guid id)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.ClockIn(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Shifts/5/ClockOut
        [Authorize(Roles = AppConstants.RoleNames.Employee)]
        [HttpPut]
        [Route("{id}/ClockOut")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> ClockOut(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await Service.ClockOut(id);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Shifts/5
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
            await Service.Delete(id);
            return Ok();
        }

        // GET: api/Shifts/Reports/Attendance
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin)]
        [HttpGet]
        [Route("Reports/Attendance")]
        [ResponseType(typeof(Task<List<AttendanceReportRowDto>>))]
        public async Task<IHttpActionResult> GetAttendanceData([FromUri]AttendanceReportShiftsQuery query)
        {
            return Ok(await Service.GetAttendanceData(query));
        }

        // GET: api/Shifts/Reports/Attendance/Summary
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin)]
        [HttpGet]
        [Route("Reports/Attendance/Summary")]
        [ResponseType(typeof(Task<AttendanceSummaryReportDto>))]
        public async Task<IHttpActionResult> GetAttendanceSummaryData([FromUri]AttendanceReportShiftsQuery query)
        {
            return Ok(await Service.GetAttendanceSummaryData(query));
        }

        // GET: api/Shifts/Dashboard
        [Authorize(Roles =
            AppConstants.RoleNames.SuperAdmin + ", " +
            AppConstants.RoleNames.Admin)]
        [HttpGet]
        [Route("Dashboard")]
        [ResponseType(typeof(Task<DashboardDto>))]
        public async Task<IHttpActionResult> GetDashboardData()
        {
            return Ok(await Service.GetDashboardData());
        }
    }
}
