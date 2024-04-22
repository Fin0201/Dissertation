using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Dissertation.Models;

namespace Dissertation.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        string AdminRoleId = Guid.NewGuid().ToString();
        string MemberRoleId = Guid.NewGuid().ToString();
        string AdminId = Guid.NewGuid().ToString();
        string MemberId = Guid.NewGuid().ToString();

        public DbSet<Item> Items { get; set; }
        public DbSet<Rent> Rents { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageNotification> MessageNotifications { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            SeedAdmin(builder);
            SeedMember(builder);
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
                    Id = MemberRoleId,
                    Name = "Member",
                    NormalizedName = "Member".ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }

        private void SeedAdmin(ModelBuilder builder)
        {
            PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();
            IdentityUser user = new IdentityUser();
            user.Id = AdminId;
            user.UserName = "admin@test.com";
            user.NormalizedUserName = "admin@test.com".ToUpper();
            user.NormalizedEmail = "admin@test.com".ToUpper();
            user.Email = "admin@test.com";
            user.LockoutEnabled = false;
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            user.PasswordHash = hasher.HashPassword(user, "P@ssword1");
            builder.Entity<IdentityUser>().HasData(user);
        }

        private void SeedMember(ModelBuilder builder)
        {
            PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();
            IdentityUser user = new IdentityUser();
            user.Id = MemberId;
            user.UserName = "member@test.com";
            user.NormalizedUserName = "member@test.com".ToUpper();
            user.NormalizedEmail = "member@test.com".ToUpper();
            user.Email = "member@test.com";
            user.LockoutEnabled = false;
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            user.PasswordHash = hasher.HashPassword(user, "P@ssword1");
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

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = MemberRoleId,
                    UserId = AdminId
                });

            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>()
                {
                    RoleId = MemberRoleId,
                    UserId = MemberId
                });
        }
    }
}
