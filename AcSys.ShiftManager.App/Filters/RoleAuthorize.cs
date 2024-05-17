using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AcSys.ShiftManager.App.Filters
{
    public class RoleAuthorize : AuthorizationFilterAttribute
    {
        public RoleAuthorize()
        {
            
        }

        public override bool AllowMultiple { get { return true; } }     //return base.AllowMultiple;
        
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnAuthorizationAsync(actionContext, cancellationToken);
        }
    }
}
