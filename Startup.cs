using DomainModel.Data;
using DomainModel.Models;
using DomainModel.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using WebPWrecover.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DomainModel
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllersWithViews();
        //    //extra code for identities
        //    services.AddRazorPages();
        //    //extra line of code
        //    services.AddDbContext<DomainModelContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DomainModelContext")));
        //    services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
        //        .AddDefaultUI()
        //        .AddEntityFrameworkStores<DomainModelContext>()
        //        .AddDefaultTokenProviders();

        //}

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<DomainModelContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DomainModelContext")));
            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            //services.AddDefaultIdentity<ApplicationUser>((options => options.SignIn.RequireConfirmedAccount = true)).AddRoles<IdentityRole>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<DomainModelContext>()
                .AddDefaultTokenProviders();
            //services.AddMvc().AddNewtonsoftJson();
            //services.AddMvc();
            //services.Configure<AuthMessageSenderOptions>(Configuration);

            //extra code for SendGrid API
            //services.AddDefaultIdentity<IdentityUser>(
            //              options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<DomainModelContext>();

            // requires
            // using Microsoft.AspNetCore.Identity.UI.Services;
            // using WebPWrecover.Services;
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //extra code for identities
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //extra code
                endpoints.MapRazorPages();
            });
        }
    }
}
