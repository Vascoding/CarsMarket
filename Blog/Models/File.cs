using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class File
    {
        private ICollection<Article> articles;

        private ICollection<Category> categories;

        public int FileId { get; set; }

        
        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }
        
        
        public byte[] Content { get; set; }

        
        public FileType FileType { get; set; }
        
        
        public virtual ICollection<Article> Article { get; set; }


        public virtual ICollection<Category> Category { get; set; }


    }
}