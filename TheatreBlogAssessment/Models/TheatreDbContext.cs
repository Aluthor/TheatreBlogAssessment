using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TheatreBlogAssessment.Models
{
    public class TheatreDbContext : IdentityDbContext<User>
    {
        //Initializes the database with the seeded tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public TheatreDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new DatabaseInitializer());
        }

        public static TheatreDbContext Create()
        {
            return new TheatreDbContext();
        }
    }
}