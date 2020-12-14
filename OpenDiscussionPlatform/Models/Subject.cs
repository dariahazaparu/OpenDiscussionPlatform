using OpenDiscussionPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenDiscussion.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "The title is required!")]
        [StringLength(50, ErrorMessage = "The title must not be longer than 50 characters!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The content is required!")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "The topic is required!")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public IEnumerable<SelectListItem> Categ { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}