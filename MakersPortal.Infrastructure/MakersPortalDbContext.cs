using MakersPortal.Core.Models;
using MakersPortal.Core.Models.Activities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MakersPortal.Infrastructure
{
    public class MakersPortalDbContext : IdentityDbContext<ApplicationUser>
    {
        public MakersPortalDbContext() : base()
        {
        }

        public MakersPortalDbContext(DbContextOptions<MakersPortalDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public virtual DbSet<MakerSpace> MakerSpaces { get; set; }
        public virtual DbSet<Totem> Totems { get; set; }
        public virtual DbSet<BaseActivity> BaseActivities { get; set; }
        public virtual DbSet<SimpleActivity> SimpleActivities { get; set; }
        public virtual DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region 1-n relationships

            builder.Entity<Totem>()
                .HasOne(p => p.MakerSpace)
                .WithMany(p => p.Totems)
                .HasForeignKey(p => p.MakerSpaceId);

            builder.Entity<BaseActivity>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            builder.Entity<Attendance>()
                .HasOne(p => p.StartTotem)
                .WithMany()
                .HasForeignKey(p => p.StartTotemId);

            builder.Entity<Attendance>()
                .HasOne(p => p.EndTotem)
                .WithMany()
                .HasForeignKey(p => p.EndTotemId);

            #endregion

            base.OnModelCreating(builder);
        }
    }
}