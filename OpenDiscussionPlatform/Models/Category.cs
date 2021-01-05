using OpenDiscussionPlatform.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenDiscussion.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "The name is required!")]
        [StringLength(25, ErrorMessage = "The name must not be longer than 25 characters!")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "The picture is required!")]
        public int CategoryPicture { get; set; }
        public virtual FileUpload File { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }
    }
}