using System;
using System.Collections.Generic;
using MakersPortal.Core.Dtos.Configuration;
using MakersPortal.Core.Models;
using MakersPortal.Core.Services;
using MakersPortal.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace MakersPortal.WebApi
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MakersPortalDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("mssql")));

            #region Dependency Injection

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton(Configuration);
            //services.TryAddScoped<IUserService, UserService>();
            /*    services.TryAddSingleton<UserStore<ApplicationUser>>();
                services.TryAddSingleton<UserManager<ApplicationUser>>();
                services.TryAddSingleton<MakersPortalDbContext>(); Configuration.GetSection("IdentityProviders").Get<IEnumerable<IdentityProviderDto>>()*/

            #endregion

            #region Identity Providers

            services.AddIdentity<ApplicationUser, IdentityRole>(options => { options.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<MakersPortalDbContext>()
                .AddDefaultTokenProviders();

            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            IEnumerable<IdentityProviderDto> identityProviders =
                Configuration.GetSection("IdentityProviders").Get<IdentityProviderDto[]>();
            /*
            foreach (IdentityProviderDto identityProvider in identityProviders)
            {
                authenticationBuilder.AddJwtBearer(identityProvider.Name, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = identityProvider.Issuer
                    };

                    options.Audience = identityProvider.Audience;
                    options.Authority = identityProvider.Issuer;

                    options.SaveToken = true;

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = TokenValidationHandler
                    };
                });
            }

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(identityProviders.Select(p => p.Name).ToArray())
                    .Build();

                options.AddPolicy("protectedScope", policy => { policy.RequireClaim("profile", "email", "openid"); });
            });
            */

            #endregion

            #region Automapper

            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
/*
        private static Task TokenValidationHandler(TokenValidatedContext context)
        {
            /*
            Console.WriteLine(">>>>>>>>>>TEST-----");
            UserManager<ApplicationUser> userManager =
                context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            var claims = context.Principal.Claims;

            foreach (var d in claims)
            {
                Console.WriteLine("{0} >>>> {1}", d.Type, d.Type == ClaimTypes.Surname);
            }

            // throw new BadRequestResult();
            Console.WriteLine("{0} <<<< {1} <<<< {2}", claims.First(p => p.Type == ClaimTypes.GivenName).Value,
                claims.First(p => p.Type == ClaimTypes.Surname).Value,
                claims.First(p => p.Type == ClaimTypes.Email).Value);

            ApplicationUser user = Task.Run(() =>
            {
                return userManager.FindByLoginAsync(context.Options.Authority,
                    claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value);
            }).Result;

            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    GivenName = claims.First(p => p.Type == ClaimTypes.GivenName).Value,
                    Surname = claims.First(p => p.Type == ClaimTypes.Surname).Value,
                    Email = claims.First(p => p.Type == ClaimTypes.Email).Value,
                    //EmailConfirmed = bool.Parse(claims.First(p => p.Type == "email_verified").Value)
                    UserName = claims.First(p => p.Type == ClaimTypes.Email).Value
                };

                var result = Task.Run(() => { return userManager.CreateAsync(newUser); }).Result;

                if (!result.Succeeded)
                    throw new Exception("Unable to create the user");
                //return Task.FromException(new Exception("Diocane1"));
                result = Task.Run(() =>
                {
                    return userManager.AddLoginAsync(newUser,
                        new ExternalLoginInfo(context.Principal, context.Principal.Identity.Name,
                            claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value,
                            context.Principal.Identity.Name));
                }).Result;

                if (!result.Succeeded)
                    throw new Exception("Unable to bind the Identity Provider to the user");

                Console.WriteLine("-------------------- UserID: {0}", user.Id);
            }

            return Task.CompletedTask;
        }*/
    }
}