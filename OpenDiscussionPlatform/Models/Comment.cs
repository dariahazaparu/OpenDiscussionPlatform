using OpenDiscussionPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenDiscussion.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "The comment cannot be empty!")]
        [DataType(DataType.MultilineText)]
        public string CommContent { get; set; }

        public DateTime Date { get; set; }
        public int SubjectId { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Subject Subject { get; set; }
    }
}