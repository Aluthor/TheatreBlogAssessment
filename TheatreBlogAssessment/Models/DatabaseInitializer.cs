using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TheatreBlogAssessment.Models
{
    public class DatabaseInitializer : DropCreateDatabaseAlways<TheatreDbContext>
    {
        protected override void Seed(TheatreDbContext context)
        {
            ////Creating Roles/////////
            
            //create a rolemanager
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            //if the admin role doesn't exist
            if (!roleManager.RoleExists("Admin"))
            {
                //create an Admin role
                roleManager.Create(new IdentityRole("Admin"));
            }

            //Staff
            if(!roleManager.RoleExists("Staff"))
            {
                roleManager.Create(new IdentityRole("Staff"));
            }

            //Member
            if(!roleManager.RoleExists("Member"))
            {
                roleManager.Create(new IdentityRole("Member"));
            }

            ////Creating Admin Users/////////
            UserManager<User> userManager = new UserManager<User>(new UserStore<User>(context));
            if(userManager.FindByName("admin@admin.com") == null)
            {
                var admin = new User()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    FirstName = "Admin",
                    LastName = "Member",
                    Street = "Admin Street",
                    City = "Admin City",
                    Postcode = "A23 5M1",
                    EmailConfirmed = true
                };
                userManager.Create(admin, "admin123");
                userManager.AddToRole(admin.Id, "Admin");
            }
            

            ////Creating Staff members//////////
            var staff1 = new User()
            {
                UserName = "staff@staff.com",
                Email = "staff@staff.com",
                FirstName = "Staff",
                LastName = "Member",
                Street = "Staff street",
                City = "Staff City",
                Postcode = "S34 6M1",
                EmailConfirmed = true
            };
            if (userManager.FindByName("staff@staff.com") == null)
            { userManager.Create(staff1, "staff123");
              userManager.AddToRole(staff1.Id, "Staff");}

            var staff2 = new User()
            {
                UserName = "staff2@staff2.com",
                Email = "staff2@staff2.com",
                FirstName = "Staff2",
                LastName = "Member2",
                Street = "Staff street",
                City = "Staff City",
                Postcode = "S34 6M1",
                EmailConfirmed = true
            };
            if (userManager.FindByName("staff2@staff2.com") == null)
            {
                userManager.Create(staff2, "staff456");
                userManager.AddToRole(staff2.Id, "Staff");
            }
            ////Creating member members//////////
            var member1 = new User()
            {
                UserName = "member@member.com",
                Email = "member@member.com",
                FirstName = "member",
                LastName = "Member",
                Street = "member street",
                City = "member City",
                Postcode = "S34 6M1",
                EmailConfirmed = true
            };
            if (userManager.FindByName("member@member.com") == null)
            {
                userManager.Create(member1, "member123");
                userManager.AddToRole(member1.Id, "Member");
            }
            var member2 = new User()
            {
                UserName = "member2@member2.com",
                Email = "member2@member2.com",
                FirstName = "member2",
                LastName = "Member2",
                Street = "member street",
                City = "member City",
                Postcode = "S34 6M1",
                EmailConfirmed = true
            };

            if (userManager.FindByName("member2@member2.com") == null)
            {
                userManager.Create(member2, "member456");
                userManager.AddToRole(member2.Id, "Member");
            }

            context.SaveChanges();

            //*************************************************************************
            //Seeding the Categories table
            //*************************************************************************

            //create a few categories
            var cat1 = new Category() { Name = "Announcements" };
            var cat2 = new Category() { Name = "Reviews" };
            var cat3 = new Category() { Name = "Other" };

            //add each category to the Categories table
            context.Categories.Add(cat1);
            context.Categories.Add(cat2);
            context.Categories.Add(cat3);

            //*************************************************************************
            //Seeding the Posts table
            //*************************************************************************
            var post1 = new Post()
            {
                Title = "MacBeth Review",
                Content = "Blargh blargh tradgey etc etc.",
                DatePosted = new DateTime(2019, 1, 1, 8, 0, 15),
                User = staff1,
                Category = cat2
            };

            var com1 = new Comment()
            {
                Content = "FIRST!1!!",
                Post = post1
            };
            
            context.Posts.Add(post1);
            context.Comments.Add(com1);

            context.SaveChanges();
        }

    }
}