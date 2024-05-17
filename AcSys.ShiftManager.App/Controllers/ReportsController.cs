using System;
using System.Linq;
using System.Web.Http;
using AcSys.ShiftManager.Service.Shifts;
using AcSys.ShiftManager.Service.Users;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    [Authorize]
    [RoutePrefix("api/Reports")]
    public class ReportsController : ApiControllerBase
    {
        IShiftsService Service = null;

        public ReportsController(IShiftsService service, ILogger logger)
            : base(service, logger)
        {
            Service = service;
        }


    }
}
