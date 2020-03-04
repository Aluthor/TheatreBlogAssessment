using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheatreBlogAssessment.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Display(Name = "Category")]
        public string Name { get; set; }

        //One category has many 
        public List<Post> Posts { get; set; }
    }
}