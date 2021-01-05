using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OpenDiscussion.Models;

namespace OpenDiscussionPlatform.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public IEnumerable<SelectListItem> AllRoles { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        //public int PhotoId { get; set; }
        //public virtual FileUpload Photo { get; set; }

        public string Description { get; set; }

        public string FullName { get; set; }

        public string City { get; set; }

        public string School { get; set; }

        public string FavFood { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new
            MigrateDatabaseToLatestVersion<ApplicationDbContext,
            OpenDiscussionPlatform.Migrations.Configuration>("DefaultConnection"));
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}