using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class ArticleViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public string AuthorId { get; set; }

        public int CategoryId { get; set; }

        public ICollection<Category> Categories { get; set; }
        
        public string Tags { get; set; }

        public byte[] Image { get; set; }

        
        public ICollection<File> Files { get; set; }

        public ICollection<Cars> Carses { get; set; }


        public double Price { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(30)]
        public string City { get; set; }

        public int HorsePower { get; set; }

        
        
    }
}