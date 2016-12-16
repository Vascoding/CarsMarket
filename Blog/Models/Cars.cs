using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Cars
    {
        private ICollection<Category> categories;

        public Cars()
        {
            this.categories = new HashSet<Category>();
        }

        public int CarsId { get; set; }
            
        [Required]
        [Index(IsUnique = true)]
        [StringLength(20)]
        public string Name { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}