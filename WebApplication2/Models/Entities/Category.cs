using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindSalesAnalysis.Models.Entities
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [Column("CategoryID")]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(15)]
        [Column("CategoryName")]
        public string CategoryName { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("Picture")]
        public byte[] Picture { get; set; }

        // Propiedad de navegación para productos
        public virtual ICollection<Product> Products { get; set; }
    }
}