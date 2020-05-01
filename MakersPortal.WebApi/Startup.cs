using System;
using System.IO;
using AutoMapper;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure;
using MakersPortal.Infrastructure.Options;
using MakersPortal.Infrastructure.Services;
using MakersPortal.WebApi.Authentication;
using MakersPortal.WebApi.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace MakersPortal.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly KeyVaultClient _keyVaultClient;

        public Startup(IHostEnvironment env, IConfiguration configuration)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MakersPortalDbContext>();
            services.ConfigureOptions<ConfigureDbContextOptionBuilder>();

            services.AddAutoMapper(typeof(Startup));

            #region Dependency Injection

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(provider => Configuration);
            services.AddSingleton<IKeyVaultClient>(_keyVaultClient);
            services.AddSingleton<IKeyManager, KeyManager>();
            services.AddSingleton<IUserService, UserService>();
            
            services.Configure<IdentityProvidersOptions>(Configuration);
            services.Configure<ConnectionStringsOption>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<KeysOptions>(Configuration.GetSection("Keys"));
            services.Configure<JwtIssuerOptions>(Configuration.GetSection("JwtIssuer"));

            #endregion

            #region Authentication and authorization

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MakersPortalDbContext>()
                .AddDefaultTokenProviders();
            services.ConfigureOptions<ConfigureIdentityOptions>();

            var authenticationBuilder = services.AddAuthentication();
            authenticationBuilder 
                .AddJwtBearer()
                .AddExternalIdentityProviders(Configuration.Get<IdentityProvidersOptions>().IdentityProviders);
            services.ConfigureOptions<ConfigureJwtBearerOptions>();
            services.ConfigureOptions<ConfigureAuthenticationOptions>();

            services.AddAuthorization();
            services.ConfigureOptions<ConfigureAuthorizationOptions>();

            #endregion

            services.ConfigureOptions<ConfigureForwardedHeadersOptions>();

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

            app.UseEndpoints(builder => builder.MapControllers());
        }
    }
}