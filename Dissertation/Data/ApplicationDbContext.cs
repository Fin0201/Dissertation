using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        string AdminRoleId = Guid.NewGuid().ToString();
        string AdminId = Guid.NewGuid().ToString();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            SeedAdmin(builder);
            SeedUserRoles(builder);

        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Customer",
                    NormalizedName = "Customer".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }

        private void SeedAdmin(ModelBuilder builder)
        {
            PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();
            IdentityUser user = new IdentityUser();
            user.Id = AdminId;
            user.UserName = "admin@admin.com";
            user.NormalizedUserName = "admin@admin.com".ToUpper();
            user.NormalizedEmail = "admin@admin.com".ToUpper();
            user.Email = "admin@admin.com";
            user.LockoutEnabled = false;
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            user.PasswordHash = hasher.HashPassword(user, "Admin123!");
            builder.Entity<IdentityUser>().HasData(user);

        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = AdminRoleId,
                    UserId = AdminId
                });
        }
    }
}
