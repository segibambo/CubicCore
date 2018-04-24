using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Cubic.Data;
using Cubic.Services;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Cubic.Data.IdentityModel;
using Autofac;
using Cubic.Data.LibraryContainer.LoggingService;
using Cubic.Repository.AutoFacModule;
using Autofac.Extensions.DependencyInjection;
using Cubic.Data.ViewModel;
using Hangfire;

namespace Cubic
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //configure which container to use
            services.AddAutoMapper();

            services.AddHangfire(config =>config.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));


            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //DBContext
            services.AddDbContext<APPContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")).
                EnableSensitiveDataLogging());


           // services.AddTransient<APPDataSeed>()

            //identity configuration
            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                // Signin settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                // User settings
                options.User.RequireUniqueEmail = true;
            })

                .AddEntityFrameworkStores<APPContext>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddDefaultTokenProviders();
            //  services.AddTransient<IEmailSender, EmailService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.ConfigureApplicationCookie(options =>
            {
               // options.Cookie.Domain = "CubicCookieMiddleware";
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath isn't set, ASP.NET Core defaults 
                // the path to /Account/Login.
                options.LoginPath = "/Account/Login";
                // If the AccessDeniedPath isn't set, ASP.NET Core defaults 
                // the path to /Account/AccessDenied.
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            //services.AddAuthentication("CubicSecurityScheme").AddCookie("CubicSecurityScheme", options =>
            //{
            //    options.AccessDeniedPath = new PathString("/account/index");
            //    options.LoginPath = new PathString("/account/login");
            //});

            // register dependencies
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule<LoggerModule>();
            builder.RegisterModule<RepositoryModule>();
            ApplicationContainer = builder.Build();

            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //    AuthenticationScheme = "MyCookieMiddlewareInstance",
            //    CookieName = "MyCookieMiddlewareInstance",
            //    LoginPath = new PathString("/Home/Login/"),
            //    AccessDeniedPath = new PathString("/Home/AccessDenied/"),
            //    AutomaticAuthenticate = true,
            //    AutomaticChallenge = true
            //});
            //enabling hangfire
            app.UseHangfireDashboard();
            app.UseHangfireServer();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                       name: "areas",
                       template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                     );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=index}/{id?}");
            });
        }
    }
}
