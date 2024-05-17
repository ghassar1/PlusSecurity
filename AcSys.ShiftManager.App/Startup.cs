using System.Web.Http;
using AcSys.ShiftManager.App.Middleware;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AcSys.ShiftManager.App.Startup))]

namespace AcSys.ShiftManager.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(typeof(QueryParamTokenMiddleware));

            // In OWIN you create your own HttpConfiguration rather than re-using the GlobalConfiguration.
            //var config = new HttpConfiguration();
            var config = GlobalConfiguration.Configuration;

            AutofacConfig.Configure(config, app);

            //NLogConfig.Configure();

            ConfigureAuth(app);

            //ConfigureStaticFileServerPath(app);
        }

        //// Works only in asp.net core or asp.net mvc 6
        //// Details at: http://stackoverflow.com/a/37475219/3423802
        //public void ConfigureStaticFileServerPath(IAppBuilder app)
        //{
        //    //Configure the file/ static file serving middleware
        //    var physicalFileSystem = new PhysicalFileSystem(@".\client");
        //    var fileServerOptions = new FileServerOptions
        //    {
        //        EnableDefaultFiles = true,
        //        RequestPath = PathString.Empty,
        //        FileSystem = physicalFileSystem
        //    };

        //    fileServerOptions.DefaultFilesOptions.DefaultFileNames = new[] { "index.html" };
        //    fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
        //    fileServerOptions.StaticFileOptions.FileSystem = physicalFileSystem;
        //    app.UseFileServer(fileServerOptions);
        //}
    }
}
