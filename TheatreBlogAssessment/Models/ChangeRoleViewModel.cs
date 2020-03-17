using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheatreBlogAssessment.Models
{
    //view model which contains information used to change a users role
    public class ChangeRoleViewModel
    {
        //Properties
        public string UserName { get; set; }
        public string OldRole { get; set; }
        public ICollection<SelectListItem> Roles { get; set; }

        [Required, Display(Name = "Role")]
        public string Role { get; set; }
    }
}