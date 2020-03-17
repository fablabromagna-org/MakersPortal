using MakersPortal.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MakersPortal.Infrastructure
{
    public class MakersPortalDbContext : IdentityDbContext<ApplicationUser>
    {
        public MakersPortalDbContext(DbContextOptions<MakersPortalDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
    }
}