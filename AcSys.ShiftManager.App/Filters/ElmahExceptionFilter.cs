using System.Web;
using System.Web.Http.Filters;
using Elmah;

namespace AcSys.ShiftManager.App.Filters
{
    public class ElmahExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            ErrorLog log = ErrorLog.GetDefault(HttpContext.Current);
            log.Log(new Error(actionExecutedContext.Exception));
        }
    }
}
