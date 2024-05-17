using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin;

namespace AcSys.ShiftManager.App.Middleware
{
    /// <summary>
    /// If access token is not present in Authorization header, this middleware will look for access_token in query parameters,
    /// and if present, it will add it to the authorization header so request can be authenticated.
    /// // http://stackoverflow.com/questions/21925367/passing-and-verifying-the-owin-bearer-token-in-query-string-in-webapi
    /// // http://stackoverflow.com/questions/21940450/net-web-api-2-owin-bearer-token-authentication-direct-call/24134176#24134176
    /// </summary>
    public class QueryParamTokenMiddleware : OwinMiddleware
    {
        public QueryParamTokenMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            //Read token from query params if present and add it as Authorization header.
            ProcessQueryParamsForToken(context);

            await Next.Invoke(context);
        }

        public static void ProcessQueryParamsForToken(IOwinContext context)
        {
            if (context.Request.QueryString.HasValue)
            {
                if (string.IsNullOrWhiteSpace(context.Request.Headers.Get("Authorization")))
                {
                    var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.Value);

                    string token = queryString.Get("access_token");
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        context.Request.Headers.Add("Authorization", new[]
                        {
                            string.Format("Bearer {0}", token)
                        });
                    }
                }
            }
        }
    }
}