using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;


namespace TheatreBlogAssessment.Models
{
    public class EditUserDetailsViewModel
    {
        private ApplicationUserManager userManager;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Street")]
        public string Street { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }


        //navigational
        public List<Comment> Comments { get; set; }
        public List<Post> Posts { get; set; }

        [NotMapped]
        public string CurrentRole
        {
            get
            {
                if (userManager == null)
                {
                    userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

                }

                return userManager.GetRoles(Id).Single();
            }
        }
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
