using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MakersPortal.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MakersPortal.WebApi.Authentication
{
    /*
    public class JwtBearerEventsWithSignUpCheck : JwtBearerEvents
    {
        public new Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = TokenValidationHandler;

        
    }*/
}