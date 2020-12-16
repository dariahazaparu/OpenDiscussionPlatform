using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenDiscussionPlatform.Models
{
    public class FileUpload
    {
        [Key]
        public int FileId { get; set; } // ID Primary Key

        public string FilePath { get; set; } // Calea unde se va salva fisierul
        public string FileName { get; set; } // Numele fisierului
        public string Extension { get; set; } // Extensia 
    }
}