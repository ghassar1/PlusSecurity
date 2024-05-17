using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using AcSys.ShiftManager.Service.Base;
using Autofac.Extras.NLog;

namespace AcSys.ShiftManager.App.Controllers
{
    public class ApiControllerBase : ApiController
    {
        IApplicationService _service = null;

        //private static ILogger logger = LogManager.GetCurrentClassLogger();
        //private static ILogger logger = LogManager.GetLogger("logfile");
        //private static ILogger logger = LogManager.GetLogger("console");
        //private static ILogger logger = LogManager.GetLogger("database");
        //private static ILogger logger = LogManager.GetLogger("*");
        //private static ILogger logger = new LogFactory(LogManager.Configuration).GetCurrentClassLogger();
        //private static ILogger Logger = LogManager.GetCurrentClassLogger();
        protected ILogger Logger { get; set; }

        public ApiControllerBase(IApplicationService service, ILogger logger)
        {
            _service = service;
            Logger = logger;
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            if (HttpContext.Current != null)
            {
                Thread.CurrentPrincipal = HttpContext.Current.User as IPrincipal;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
