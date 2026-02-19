using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Core.Entities.Product
{
    public class Product:BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public virtual List<Photo> Photos { get; set; } 
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; }

        // For reviews and ratings
        public double AverageRating { get; set; } = 0.0;
        public int TotalReviews { get; set; } = 0; // reviews count for calculating average rating
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
