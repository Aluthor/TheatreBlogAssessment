using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheatreBlogAssessment.Models
{
    public class Post
    {
        //properties
        [Key]
        public int PostId { get; set; }
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        public string Title { get; set; }

        [Display(Name = "Date Posted")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DatePosted { get; set; }
        
        //navigational properties
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }


        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        
        //One post has a list of products
        public List<Comment> Comments { get; set; }
    }
}