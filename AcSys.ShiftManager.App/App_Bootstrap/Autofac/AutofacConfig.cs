using System;
using System.Data.Entity;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AcSys.Core.Data.Repository;
using AcSys.Core.Email;
using AcSys.ShiftManager.Data.EF.Context;
using AcSys.ShiftManager.Data.EF.Identity;
using AcSys.ShiftManager.Data.EF.Repos.ActivityLogs;
using AcSys.ShiftManager.Data.EF.Repos.Users;
using AcSys.ShiftManager.Data.EF.UnitOfWork;
using AcSys.ShiftManager.Data.UnitOfWork;
using AcSys.ShiftManager.Model;
using AcSys.ShiftManager.Service.Users;
using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace AcSys.ShiftManager.App
{
    public static class AutofacConfig
    {
        public static void Configure(HttpConfiguration config, IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            RegisterDependencies(builder, config, app);

            // Create and assign a dependency resolver for Web API to use.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            //Set the MVC DependencyResolver
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // The Autofac middleware should be the first middleware added to the IAppBuilder.
            // If you "UseAutofacMiddleware" then all of the middleware in the container
            // will be injected into the pipeline right after the Autofac lifetime scope
            // is created/injected.
            //
            // Alternatively, you can control when container-based
            // middleware is used by using "UseAutofacLifetimeScopeInjector" along with
            // "UseMiddlewareFromContainer". As long as the lifetime scope injector
            // comes first, everything is good.
            app.UseAutofacMiddleware(container);

            // Again, the alternative to "UseAutofacMiddleware" is something like this:
            // app.UseAutofacLifetimeScopeInjector(container);
            // app.UseMiddlewareFromContainer<FirstMiddleware>();
            // app.UseMiddlewareFromContainer<SecondMiddleware>();

            // Make sure the Autofac lifetime scope is passed to Web API.
            app.UseAutofacWebApi(config);
        }

        private static void RegisterDependencies(ContainerBuilder builder, HttpConfiguration config, IAppBuilder app)
        {
            // Register Web API controllers in executing assembly.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // Register MVC controllers in executing assembly.
            //builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL - Register the filter provider if you have custom filters that need DI.
            // Also hook the filters up to controllers.
            builder.RegisterWebApiFilterProvider(config);

            //builder.RegisterType<CustomActionFilter>()
            //    .AsWebApiActionFilterFor<TestController>()
            //    .InstancePerRequest();

            // Register a logger service to be used by the controller and middleware.
            //builder.Register(c => new Logger()).As<ILogger>().InstancePerRequest();

            builder.RegisterModule<NLogModule>();

            // Autofac will add middleware to IAppBuilder in the order registered.
            // The middleware will execute in the order added to IAppBuilder.
            //builder.RegisterType<FirstMiddleware>().InstancePerRequest();
            //builder.RegisterType<SecondMiddleware>().InstancePerRequest();

            //builder.RegisterType<NLogger>().As<ILogger>().InstancePerRequest();

            //builder.RegisterType<ApplicationDbContext>().As<DbContext>().InstancePerRequest();
            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();

            builder.RegisterType<RoleRepository>().As<IRoleStore<Role, Guid>>().InstancePerRequest();
            builder.RegisterType<ApplicationRoleManager>().AsSelf().InstancePerRequest();

            //builder.RegisterType<ApplicationUserStore>().As<IUserStore<User, Guid>>().InstancePerRequest();
            builder.RegisterType<UserRepository>().As<IUserStore<User, Guid>>().InstancePerRequest();

            //builder.RegisterType<ApplicationUserManager>().As<UserManager<User, Guid>>().InstancePerRequest();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest();

            //app.SetDataProtectionProvider(new DpapiDataProtectionProvider("AcSys.ShiftManager"));
            //builder.Register<IDataProtectionProvider>(c => app.GetDataProtectionProvider()).AsSelf().InstancePerRequest();
            builder.Register<IDataProtectionProvider>(c => new DpapiDataProtectionProvider("AcSys.ShiftManager")).AsSelf().InstancePerRequest();

            builder.Register(c => app.GetDataProtectionProvider().Create("EmailConfirmation")).As<IDataProtector>().InstancePerRequest();
            builder.RegisterType<DataProtectorTokenProvider<User, Guid>>().As<IUserTokenProvider<User, Guid>>().InstancePerRequest();

            //builder.RegisterType<ApplicationSignInManager>().As<SignInManager<User, Guid>>().InstancePerRequest();
            //builder.RegisterType<ApplicationSignInManager>().InstancePerRequest();
            builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerRequest();

            //builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).InstancePerRequest();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).AsSelf().InstancePerRequest();

            builder.RegisterType<IdentityEmailService>().As<IIdentityMessageService>().InstancePerRequest();
            //builder.RegisterType<IdentitySmsService>().As<IIdentityMessageService>().InstancePerRequest();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerRequest();

            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerRequest();

            //builder.RegisterType<GenericRepository<ApplicationDbContext, User>>().As<IGenericRepository<User>>().InstancePerRequest();
            //builder.RegisterType<GenericRepository<ApplicationDbContext, Role>>().As<IGenericRepository<Role>>().InstancePerRequest();
            //builder.RegisterType<GenericRepository<ApplicationDbContext, EmployeeGroup>>().As<IGenericRepository<EmployeeGroup>>().InstancePerRequest();

            //builder.RegisterType<GenericRepository<ApplicationDbContext, Message>>().As<IGenericRepository<Message>>().InstancePerRequest();
            //builder.RegisterType<GenericRepository<ApplicationDbContext, MessageView>>().As<IGenericRepository<MessageView>>().InstancePerRequest();
            //builder.RegisterType<GenericRepository<ApplicationDbContext, Role>>().As<IGenericRepository<Role>>().InstancePerRequest();
            //builder.RegisterType<GenericRepository<ApplicationDbContext, NotificationView>>().As<IGenericRepository<NotificationView>>().InstancePerRequest();

            //builder.RegisterType<GenericRepository<ApplicationDbContext, ActivityLog>>().As<IGenericRepository<ActivityLog>>().InstancePerRequest();

            //builder.RegisterType<GenericRepository<ApplicationDbContext, Shift>>().As<IGenericRepository<Shift>>().InstancePerRequest();

            // Register repositories
            builder.RegisterAssemblyTypes(typeof(ActivityLogRepository).Assembly)
                .Where(t => t.Name.EndsWith("Repository", StringComparison.InvariantCulture))
                //.AsClosedTypesOf(typeof(GenericRepository<,>))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            // Register repositories
            builder.RegisterAssemblyTypes(typeof(IUsersService).Assembly)
                .Where(t => t.Name.EndsWith("Service", StringComparison.InvariantCulture))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            //builder.RegisterType<>().As<>().InstancePerRequest();
        }
    }
}
