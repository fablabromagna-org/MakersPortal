using System;
using System.IO;
using AutoMapper;
using MakersPortal.Core.Dtos.Configuration;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure;
using MakersPortal.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MakersPortal.WebApi
{
    public class Startup
    {
        private static KeyVaultClient _keyVaultClient;

        public Startup(IHostEnvironment env, IConfiguration configuration)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider
                    .KeyVaultTokenCallback));

            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration) // We need to override the DI configuration
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"conf{Path.DirectorySeparatorChar}appsettings.json", optional: true,
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            var keyVaultEndpoint = Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");

            if (keyVaultEndpoint != null)
            {
                builder.AddAzureKeyVault(keyVaultEndpoint, _keyVaultClient, new DefaultKeyVaultSecretManager());
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MakersPortalDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("mssql")));

            services.AddAutoMapper(typeof(Startup));

            #region Dependency Injection

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(provider => Configuration);
            services.AddSingleton(Configuration.GetSection("JwtIssuer").Get<JwtIssuerDto>());
            services.AddSingleton<IKeyVaultClient>(_keyVaultClient);
            services.AddSingleton<IKeyManager, KeyManager>();

            #endregion

            #region Identity Providers

            services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<MakersPortalDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer();
            
            services.ConfigureOptions<ConfigureJwtBearerOptions>();

            #endregion

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseHttpsRedirection();
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "v1/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}