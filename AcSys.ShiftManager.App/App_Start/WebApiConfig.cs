using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using AcSys.ShiftManager.App.Filters;
using Elmah.Contrib.WebApi;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace AcSys.ShiftManager.App
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.Filters.Add(new ExceptionFilter());

            //config.Filters.Add(new ElmahExceptionFilter());
            //config.Filters.Add(new ElmahHandleErrorApiAttribute());

            // https://github.com/rdingwall/elmah-contrib-webapi
            // http://stackoverflow.com/a/28796566/3423802
            config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
