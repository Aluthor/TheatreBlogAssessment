using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheatreBlogAssessment.Models
{
    public class Comment
    {
        
        
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post{ get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Key]
        public int CommentId { get; set; }
        [Required]
        public string Content { get; set; }

        public bool IsAproved { get; set; }
    }
}