using MakersPortal.Core.Models;
using MakersPortal.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MakersPortal.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MakersPortalDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("mssql")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MakersPortalDbContext>();

            #region Identity Providers
            
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                // .AddAzureADBearer(options => Configuration.Bind("AzureAd", options))
                .AddGoogle(options => Configuration.Bind("Google"));
                // ToDo: Apple Authenticaton
            
            #endregion
            
            #region Dependency Injection

            #endregion
            
            #region Automapper
            
            #endregion
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                
            } else {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}