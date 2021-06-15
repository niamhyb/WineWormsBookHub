using System;
using DomainModel.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DomainModel.Areas.Identity.IdentityHostingStartup))]
namespace DomainModel.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            //comment out these lines as they're already in the builder
            //builder.ConfigureServices((context, services) => {
            //    services.AddDbContext<DomainModelContext>(options =>
            //        options.UseSqlServer(
            //            context.Configuration.GetConnectionString("DomainModelContextConnection")));

            //    services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //        .AddEntityFrameworkStores<DomainModelContext>();
            //});
        }
    }
}